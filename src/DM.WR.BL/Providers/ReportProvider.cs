using DM.WR.BL.Builders;
using DM.WR.Models.Config;
using DM.WR.BL.Managers;
using DM.WR.Data.Logging;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.ViewModels;
using DM.WR.Models.Xml;
using DM.WR.ServiceClient;
using DM.WR.Models.BackgroundReport;
using System;
using System.Net;
using System.Text;
using DM.WR.ServiceClient.BackgroundReport;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DM.WR.BL.Providers
{
    public class ReportProvider : IReportProvider
    {
        private readonly IGenerateReportRequestBuilder _generateReportRequestBuilder;
        private readonly IOptionsManager _optionsManager;
        private readonly ICriteriaManager _criteriaManager;
        private readonly IReportingBackgroundRepository _reportingBackgroundRepository;
        private readonly ISessionManager _sessionManager;

        private readonly IOptionsBuilder _optionsBuilder;
        private readonly IActuateQueryStringBuilder _actuateQueryStringBuilder;

        private readonly IDbLogger _dbLogger;
        private readonly IActuateServiceClient _actuateServiceClient;

        private readonly UserData _userData;

        public ReportProvider(IGenerateReportRequestBuilder generateReportRequestBuilder, IOptionsManager optionsManager, ICriteriaManager criteriaManager, IActuateQueryStringBuilder actuateQueryStringBuilder, IUserDataManager userDataManager, IDbLogger dbLogger, IOptionsBuilder optionsBuilder, IActuateServiceClient actuateServiceClient, IReportingBackgroundRepository reportingBackgroundRepository, ISessionManager sessionManager)
        {
            _generateReportRequestBuilder = generateReportRequestBuilder;
            _optionsManager = optionsManager;
            _criteriaManager = criteriaManager;
            _actuateQueryStringBuilder = actuateQueryStringBuilder;
            _dbLogger = dbLogger;
            _optionsBuilder = optionsBuilder;
            _actuateServiceClient = actuateServiceClient;
            _sessionManager = sessionManager;
            _reportingBackgroundRepository = reportingBackgroundRepository;

            _userData = userDataManager.GetUserData();
        }

        public ReportViewerModel BuildReportViewerModel()
        {
            var book = _optionsManager.GetOptionBook();
            var currentPage = book.GetCurrentPage();

            StringBuilder query;

            //Grab query string from Session if it's stored there by LibraryPageApi as a result of loading a background report.
            var libraryParams = _sessionManager.Retrieve(SessionKey.ReportLibraryParams);
            if (libraryParams != null)
            {
                _sessionManager.Delete(SessionKey.ReportLibraryParams);
                query = new StringBuilder($"{ConfigSettings.AcGeneratedReportUrl}?{libraryParams}");
            }
            else
            {
                var extraQueryParams = _sessionManager.Retrieve(SessionKey.ExtraQueryParams);
                _sessionManager.Delete(SessionKey.ExtraQueryParams);

                // Decide which query to build and build it!
                query = new StringBuilder(currentPage.IsMultimeasure
                    ? _actuateQueryStringBuilder.BuildMultimeasureQueryString(book, _userData, false)
                    : _actuateQueryStringBuilder.BuildQueryString(currentPage, _userData, false, extraQueryParams: extraQueryParams));
            }

            // Append logging params to the query & log to SDR
            // SCWO-278 - we don't want to log when the report is the result of user clicking a link in the report library
            if (_userData.LoggingFlag && libraryParams == null)
                query = HandleLogging(book, query, false);

            return new ReportViewerModel
            {
                QueryString = query.ToString(),
                HasExportToExcel = currentPage.XmlDisplayOption.exportToExcel,
                HasLastNameSearch = currentPage.XmlDisplayType == XMLReportType.SR || currentPage.XmlDisplayType == XMLReportType.MSR
            };
        }

        public string CreateExcelQueryString(string query)
        {
            var book = _optionsManager.GetOptionBook();

            if (query.Contains("criteriaID")) //if the query came from background report, need to rebuild it
            {
                var currentPage = book.GetCurrentPage();

                query = currentPage.IsMultimeasure
                        ? _actuateQueryStringBuilder.BuildMultimeasureQueryString(book, _userData, false)
                        : _actuateQueryStringBuilder.BuildQueryString(currentPage, _userData, false);
            }

            var result = new StringBuilder(MakeExcelQueryString(query));

            return _userData.LoggingFlag ?
                   HandleLogging(book, result, false).ToString() :
                   result.ToString();
        }

        public string ApplyLastNameSearch(string lastName)
        {
            var book = _optionsManager.GetOptionBook();
            var currentPage = book.GetCurrentPage();

            var query = new StringBuilder(currentPage.IsMultimeasure
                            ? _actuateQueryStringBuilder.BuildMultimeasureQueryString(book, _userData, false, lastName)
                            : _actuateQueryStringBuilder.BuildQueryString(currentPage, _userData, false, lastName));

            return _userData.LoggingFlag ?
                   HandleLogging(book, query, false).ToString() :
                   query.ToString();
        }

        public void UpdateOptions(string queryString)
        {
            var book = _optionsManager.GetOptionBook();
            var currentPage = book.GetCurrentPage();
            var updatedCurrentPage = _optionsBuilder.UpdateReportOptions(currentPage, queryString, _userData, out OptionGroup topChangedGroup, out string extraQueryParams);
            var rebuiltCurrentPage = _optionsBuilder.BuildOptions(updatedCurrentPage, topChangedGroup.Type, _userData, book.CurrentPageIndex, true);

            if (!string.IsNullOrEmpty(extraQueryParams))
                _sessionManager.Store(extraQueryParams, SessionKey.ExtraQueryParams);

            book.UpdateCurrentPage(rebuiltCurrentPage);

            if (rebuiltCurrentPage.IsMultimeasure)
                book = _optionsBuilder.SyncUpPagesForMultimeasure(book);

            _optionsManager.UpdateOptionBook(book);
        }
        #region BackgroundReportCalling
        public async Task<string> SendReportToBackgroundAsync(string reportName)
        {
            if (ConfigSettings.IsTelerikReportFeatureEnabled)
            {
                var criteriaInfo = _criteriaManager.SaveNewCriteria(reportName, null, true);
                //Create a user if it does not exist in Actuate iServer
                _actuateServiceClient.CreateUser(_userData.UserId, _userData.ActuateUserId, ConfigSettings.AcBpVolume, ConfigSettings.AcBpUserName, ConfigSettings.AcBpPassword, out string error);
                var book = _optionsManager.GetOptionBook();
                var currentPage = book.GetCurrentPage();
                var query = new StringBuilder(currentPage.IsMultimeasure ?
                            _actuateQueryStringBuilder.BuildMultimeasureQueryString(book, _userData, true) :
                            _actuateQueryStringBuilder.BuildQueryString(currentPage, _userData, true));

                query.Append(_actuateQueryStringBuilder.BuildBackgroundReportParameters(criteriaInfo.Id, reportName, currentPage, _userData));


                /* 
                 a. Append logging params to the query & log to SDR
                 b. Send Report to background
                 */
                if (_userData.LoggingFlag)
                    query = HandleLogging(book, query, true);

                var reportTemplateAndloggingId = GetReportTemplate(query).Split('+');
                var reportTemplate = reportTemplateAndloggingId[0].ToString();
                var loggingId = reportTemplateAndloggingId[1].ToString();
                var testFamilyGroupCode = reportTemplateAndloggingId[2].ToString();
                var reportStringToArray = ConfigSettings.ReportList.Split(',');
                var reportList = new List<string>(reportStringToArray);
                var SystemName = ConfigSettings.SystemName;
                var HasExportToExcel = currentPage.XmlDisplayOption.exportToExcel;
                var HasLastNameSearch = currentPage.XmlDisplayType == XMLReportType.SR || currentPage.XmlDisplayType == XMLReportType.MSR;
                var rFormat = currentPage.ScoringOptions.RFormat;
                //IowaCogatProfileNarrative
                if (reportList.Contains(reportTemplate))
                {
                    if (testFamilyGroupCode == "IOWA_INTERIM" && reportTemplate == "CatalogExporter" )
                    {
                        GenerateReportRequest generateReportRequest = SendToActuate(reportName, criteriaInfo, query, HasExportToExcel, HasLastNameSearch);
                        await CreatingTaskForBackground(generateReportRequest);
                        return string.Empty;
                    }
                    else if (testFamilyGroupCode == "LOGRAMOS")
                    {
                        GenerateReportRequest generateReportRequest = SendToActuate(reportName, criteriaInfo, query, HasExportToExcel, HasLastNameSearch);
                        await CreatingTaskForBackground(generateReportRequest);
                        return string.Empty;
                    }
                    else
                    {
                        var generateReportRequest = _generateReportRequestBuilder.GetReport(reportTemplate, _userData, currentPage, reportName, loggingId, SystemName,rFormat);
                        generateReportRequest.ActuateJobID = "0";
                        generateReportRequest.isActuate = "0";
                        generateReportRequest.CriteriaID = criteriaInfo.Id.ToString();
                        generateReportRequest.folderPath = ConfigSettings.FolderPath + _userData.UserId + "_" + _userData.CurrentGuid;
                        generateReportRequest.HasExportToExcel = HasExportToExcel;
                        generateReportRequest.HasLastNameSearch = HasLastNameSearch;
                        await CreatingTaskForBackground(generateReportRequest);
                    }
                }
                else
                {
                    GenerateReportRequest generateReportRequest = SendToActuate(reportName, criteriaInfo, query, HasExportToExcel, HasLastNameSearch);
                    await CreatingTaskForBackground(generateReportRequest);
                    return string.Empty;
                }
                return string.Empty;
            }
            else
            {
                var criteriaInfo = _criteriaManager.SaveNewCriteria(reportName, null, true);

                //Create a user if it does not exist in Actuate iServer
                _actuateServiceClient.CreateUser(_userData.UserId, _userData.ActuateUserId, ConfigSettings.AcBpVolume, ConfigSettings.AcBpUserName, ConfigSettings.AcBpPassword, out string error);

                var book = _optionsManager.GetOptionBook();
                var currentPage = book.GetCurrentPage();

                var query = new StringBuilder(currentPage.IsMultimeasure ?
                            _actuateQueryStringBuilder.BuildMultimeasureQueryString(book, _userData, true) :
                            _actuateQueryStringBuilder.BuildQueryString(currentPage, _userData, true));

                query.Append(_actuateQueryStringBuilder.BuildBackgroundReportParameters(criteriaInfo.Id, reportName, currentPage, _userData));

                // Append logging params to the query & log to SDR
                if (_userData.LoggingFlag)
                    query = HandleLogging(book, query, true);

                //TODO:  Look at how we call DM ASHX service from ServiceClient project
                var request = (HttpWebRequest)WebRequest.Create(query.ToString());

                try
                {
                    using (WebResponse webResponse = request.GetResponse())
                    {
                        // USING statement to make sure the response is properly disposed of
                    }
                }
                catch (WebException exc)
                {
                    return exc.Status == WebExceptionStatus.Timeout ? "Request to Report Library timed out." : "Request to Report Library failed.";
                }

                return string.Empty;
            }
        }

        #endregion
        #region Private Methods
        private GenerateReportRequest SendToActuate(string reportName, CriteriaInfo criteriaInfo, StringBuilder query, bool HasExportToExcel, bool HasLastNameSearch)
        {
            return new GenerateReportRequest
            {
                ActuateJobID = GetActuateJobID(query),
                isActuate = "1",
                Parameters = query.ToString(),
                folderPath = ConfigSettings.FolderPath + _userData.UserId + "_" + _userData.CurrentGuid,
                filename = reportName,
                UserID = _userData.UserId + "_" + _userData.CurrentGuid,
                CriteriaID = criteriaInfo.Id.ToString(),
                Environment = ConfigSettings.Environment,
                HasExportToExcel = HasExportToExcel,
                HasLastNameSearch = HasLastNameSearch

            };
        }
        private string GetActuateJobID(StringBuilder query)
        {
            string firstetxt = @"is&nbsp;<font id=""fntNewRequestStatusHighlight"">";
            string secondtext = @"</font>";
            string ActuatejobID = "";

            var request = WebRequest.Create(query.ToString());
            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = webResponse.GetResponseStream();
                    StreamReader readStream = null;
                    if (webResponse.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(webResponse.CharacterSet));
                    }
                    string datastream = readStream.ReadToEnd();
                    ActuatejobID = getBetween(datastream, firstetxt, secondtext);

                    webResponse.Close();
                    readStream.Close();
                }
            }
            catch (WebException exc)
            {
                return exc.Status == WebExceptionStatus.Timeout ? "Request to Report Library timed out." : "Request to Report Library failed.";
            }
            return ActuatejobID;
        }
        private string GetReportTemplate(StringBuilder url)
        {



            var spliturlcom = url.ToString().Split('?');
            var spliturlparams = spliturlcom[1].Split('&');
            var loggingIDArray = spliturlparams.Where(s => s.Contains("SDRRequestID")).ToList();
            var testFamilyArray = spliturlparams.Where(s => s.Contains("TestfamilyGroupCode")).ToList();
            var CrudeTestFamily = testFamilyArray[0].ToString().Split('=');
            var CrudeloggingID = loggingIDArray[0].ToString().Split('=');
            var testFamilyGroupCode = CrudeTestFamily[1].ToString();
            var loggingiD = CrudeloggingID[1].ToString();
            var paramsplits = spliturlparams[0].Split('=');
            var Finalparams = paramsplits[1].Split('/');
            var ReportType = Finalparams[2].Replace(".rox", "");
            return $"{ReportType}+{loggingiD}+{testFamilyGroupCode}";
        }
        //Private Methods
        private async Task<string> CreatingTaskForBackground(GenerateReportRequest generateReportRequest)
        {


            var apiUrl = ConfigSettings.TaskUrl;
            generateReportRequest.Priority = "70";
            generateReportRequest.TaskcommandID = 69;
            generateReportRequest.Processorname = "TelerikReportServer1";

            //FACT: Do not try to add await, they are already being handeled 
            var call = await _reportingBackgroundRepository.SendTask(generateReportRequest, apiUrl);

            return call;
        }
        private static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        #endregion
        #region ForUiReportCalling
        public async Task<string> GetDataExportData(int fielId)
        {
            var JsonReturnString = "";
            var apiUrl = ConfigSettings.TaskUrl;
            JsonReturnString = await _reportingBackgroundRepository.GetDataExportData(apiUrl, fielId);
            return JsonReturnString;
        }
        
        public async Task<string> GetFileByUserID()
        {
            var JsonReturnString = "";
            var apiUrl = ConfigSettings.TaskUrl;
            var UserID = _userData.UserId + "_" + _userData.CurrentGuid;
            JsonReturnString = await _reportingBackgroundRepository.GetFilesTodisplay(apiUrl, UserID);
            return JsonReturnString;
        }
        public void DeletePDF(int fileId)
        {
            var apiUrl = ConfigSettings.TaskUrl;
            _reportingBackgroundRepository.GetSoftDelete(apiUrl, fileId);

        }

        #endregion
        private string MakeExcelQueryString(string queryString)
        {
            //a.	CURRENTLY DONE IN THE CALLING FUNCTION : Remove &SDRRequestID=#####&SDRComponentLogging=0101
            //b.	Replace executereport.do with executeexcel.do
            //c.	Replace __progressive=true with __progressive=false
            //d.	Append &Export=TRUE at the end
            //e.    Chop off Logging parameters

            var indexOfSdrRequest = queryString.IndexOf("&SDRRequestID", StringComparison.Ordinal);
            if (indexOfSdrRequest > -1)
                queryString = queryString.Remove(indexOfSdrRequest);

            var remake = queryString.Replace("executereport.do", "executeexcel.do")
                                    .Replace("__progressive=true", "__progressive=false");

            return $"{remake}&Export=TRUE";
        }

        private StringBuilder HandleLogging(OptionBook book, StringBuilder query, bool runInBackground)
        {
            var requestId = _dbLogger.LogRequestBegin(_userData.LogSessionId, book.GetCurrentPage().XmlDisplayOption.reportCode, Convert.ToInt32(_userData.CurrentCustomerInfo.CustomerId), _userData.CurrentGuid, runInBackground, query.ToString());
            //SCWO-270 - created/update in 1 proc
            //    if (requestId != -1)
            //    {
            query.Append($"&SDRRequestID={requestId}&SDRComponentLogging={_userData.ComponentLoggingString}");
            //         _dbLogger.UpdateOutputString(requestId, query.ToString());
            //     }
            return query;
        }
    }
}