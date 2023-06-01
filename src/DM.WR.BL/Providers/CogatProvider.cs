using AutoMapper;
using DM.WR.BL.Builders;
using DM.WR.BL.Managers;
using DM.WR.Data.Repository;
using DM.WR.Models.CogAt;
using DM.WR.Models.CogAt.ViewModels;
using DM.WR.Models.Config;
using DM.WR.Models.ScoreManagerApi;
using DM.WR.Models.Types;
using DM.WR.Models.ViewModels;
using DM.WR.ServiceClient.ScoreManagerApi;
using DM.WR.ServiceClient.ScoreManagerApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DM.WR.Data.Repository.Types;
using DM.WR.ServiceClient.ElevateReportingEngine;
using DM.WR.ServiceClient.BackgroundReport;
using DM.WR.Models.BackgroundReport.ElevateReports;
using DM.WR.Models.BackgroundReport;
using DM.WR.Models.ElevateReportingEngine;

namespace DM.WR.BL.Providers
{
    public interface ICogatProvider
    {
        DashboardCogatViewModel BuildPageViewModel();
        CogatFiltersViewModel GetFilters(string appPath);
        void UpdateFilters(string filterTypeNumber, List<string> values, out CogatFiltersViewModel cogatFiltersViewModel);
        void ResetPage();
        Task<StudentRosterViewModel> GetAllDataAsync();
        Task<object> GetRecordsCountAsync(int testFetchSize, int smFetchSize);
        Task<object> GetCutScoreAsync();
        Task<object> GetCutScoreAsync(string groupingType, string filter);
        Task<StudentRosterViewModel> GetStudentRosterAsync(int take, int skip, string filter, string score, string orderType);
        Task<object> GetAgeStaninesAsync();
        Task<object> GetAbilityProfilesAsync();
        Task<object> GetGroupTotalsAsync();
        Task<string> GetDataExportData(int fielId);
        Task<string> GetFileByUserID();
        void DeletePDF(int fileId);
        Task<string> SendReportToBackgroundAsync(ElevateUiBackgroundRequest ECogatRequest);
        CogatBackgroundFiltersModel GetBackGroundGradeLocations();



        #region Debug
        List<ForegroundReporting.Lib.Models.Responses.StudentSubtest> GetStudentsDataDebug();
        List<GroupTotalPayload> GetGroupTotalsPayloadForDebug();
        #endregion
    }

    public class CogatProvider : ICogatProvider
    {
        public static List<ForegroundReporting.Lib.Models.Responses.StudentSubtest> _studentData;
        public static List<GroupTotalPayload> _groupTotalList;

        private readonly IReportingBackgroundRepository _reportingBackgroundRepository;
        private readonly ICogatFiltersBuilder _filtersBuilder;
        private readonly ISessionManager _sessionManager;
        private readonly ScoreManagerClient _smiClient; //TODO:  Hook up the interface
        private readonly ScoreManagerParametersBuilder _paramsBuilder; //TODO:  Hook up the interface
        private readonly IElevateReportingEngineClient _elevateReportingEngineClient;
        private readonly IDbClient _dbClient;
        private readonly IEREngineParamterBuilder _eREngineParamterBuilder;
        private readonly IBackgroundModelBuilder _backgroundModelBuilder;
        private readonly UserData _userData;

        private readonly CommonProviderFunctions _commonFunctions;
        private readonly CogatCommonFunctions _cogat;
        private readonly CogatScoreWarningsUtility _swUtil;

        private readonly IMapper _typesMapper;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CogatProvider(ICogatFiltersBuilder filtersBuilder, ISessionManager sessionManager, IUserDataManager userDataManager,
            IDbClient dbClient, IElevateReportingEngineClient elevateReportingEngineClient, IEREngineParamterBuilder eREngineParamterBuilder,
            IReportingBackgroundRepository reportingBackgroundRepository, IBackgroundModelBuilder backgroundModelBuilder)

        {
            _filtersBuilder = filtersBuilder;
            _sessionManager = sessionManager;
            _reportingBackgroundRepository = reportingBackgroundRepository;
            _smiClient = new ScoreManagerClient();
            _paramsBuilder = new ScoreManagerParametersBuilder();
            _elevateReportingEngineClient = elevateReportingEngineClient;
            _eREngineParamterBuilder = eREngineParamterBuilder;
            _backgroundModelBuilder = backgroundModelBuilder;
            _dbClient = dbClient;

            _userData = userDataManager.GetUserData();

            _commonFunctions = new CommonProviderFunctions();
            _cogat = new CogatCommonFunctions();
            _swUtil = new CogatScoreWarningsUtility();

            _typesMapper = new DbTypesMapper();

        }



        public DashboardCogatViewModel BuildPageViewModel()
        {
            var reportCenterUrl = "";
            return new DashboardCogatViewModel
            {
                IsGuidUser = _commonFunctions.IsGuidUser(_userData),
                IsProd = ConfigSettings.IsEnvironmentProd,
                UserID = _userData.UserId + "_" + _userData.CurrentGuid,
                Password = "",
                ActuateGenerateUrl = ConfigSettings.AcGeneratedReportUrlUi.ToString(),
                ActuateWebLocation = ConfigSettings.AcWebLocation.ToString(),
                IsTelerikReportFeatureEnabled = ConfigSettings.IsTelerilEnabled,
                ReturnUrl = ConfigSettings.ReturnUrl,
                QueryString = string.Join("&", reportCenterUrl)
            };
        }

        #region Fordebugging
        public List<GroupTotalPayload> GetGroupTotalsPayloadForDebug()
        {
            return _groupTotalList;
        }
        
        public List<ForegroundReporting.Lib.Models.Responses.StudentSubtest> GetStudentsDataDebug()
        {
            return _studentData;
        }
        #endregion
        public CogatFiltersViewModel GetFilters(string appPath)
        {

            List<DbAssessment> assessments = new List<DbAssessment>();
            DbAssessment cogatAssement = new DbAssessment();
            cogatAssement.TestFamilyGroupId = 1;
            cogatAssement.TestFamilyName = "'COGAT'";
            cogatAssement.TestFamilyDesc = "CogAT Assessments";
            cogatAssement.TestFamilyGroupCode = "COGAT";
            cogatAssement.SmVersion = "";

            assessments.Add(cogatAssement);

            try
            {



                if (!_userData.Assessments.Any(a => a.TestFamilyGroupCode.ToLower().Contains("cogat") || a.TestFamilyGroupCode.ToLower().Contains("ccat")))
                {
                    if (_userData.CustomerInfoList.Count > 1)
                    {
                        _userData.Assessments = null;

                        foreach (var customerInfo in _userData.CustomerInfoList)
                        {
                            //GET ASSEMENT TYPES -- ASSIGNMENT TYPES :COGAT/IOWFLEX
                            //TODO: 
                            //var assessments = _dbClient.GetAllAssessments(_typesMapper.Map<DbCustomerInfo>(customerInfo), _userData.ContractInstances);

                            if (assessments.Any(a => a.TestFamilyGroupCode.ToLower().Contains("cogat") || a.TestFamilyGroupCode.ToLower().Contains("ccat")))
                            {
                                _userData.Assessments = _typesMapper.Map<List<Assessment>>(assessments);
                                break;
                            }
                        }

                        if (_userData.Assessments == null)
                        {
                            var errorMessage = "<em>Elevate</em> currently could not find any data. This could be because tests have not been completed as of yet. Please try again later or contact the <em>Elevate</em> Support Center.<br/>" +
                                               "1-877-246-8337 | <a href=\"mailto:help@riversidedatamanager.com\">help@riversidedatamanager.com</a><br/>" +
                                               "<em>Elevate Support Center hours are Monday - Friday from 7:00 AM - 6:00 PM (CST).</em>";
                            Logger.Error($"CogAT Dashboard :: {errorMessage} :: GUIDs: {string.Join(",", _userData.CustomerInfoList.Select(c => c.Guid))} :: Contract Instances: {_userData.ContractInstances}");
                            return new CogatFiltersViewModel { ErrorMessage = errorMessage };
                        }
                    }
                }

                var filterPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);

                if (filterPanel == null)
                {
                    filterPanel = _filtersBuilder.BuildPanel(new FilterPanel(), FilterType._INTERNAL_FIRST_, _userData);
                    _sessionManager.Store(filterPanel, SessionKey.CogatFilters);
                }
                if (filterPanel.isRetry == true)
                {
                    filterPanel = _filtersBuilder.BuildPanel(filterPanel, FilterType._INTERNAL_FIRST_, _userData);
                    _sessionManager.Store(filterPanel, SessionKey.CogatFilters);
                }
                return new CogatFiltersViewModel
                {
                    Filters = filterPanel.GetAllFilters(),
                    RootNodeName = filterPanel.SelectedTestAdmin.NodeName,
                    RootNodeType = filterPanel.SelectedTestAdmin.NodeType,
                    Battery = filterPanel.Battery,
                    TestFamilyGroupCode = filterPanel.Assessment.TestFamilyGroupCode
                };
            }
            catch (Exception ex)
            {
                if (string.Equals(ex.Message,"Unauthorized",StringComparison.OrdinalIgnoreCase))
                {
                    var filterPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
                    if (filterPanel != null)
                    {
                        filterPanel.isRetry = true;
                    }
                    _sessionManager.Store(filterPanel, SessionKey.CogatFilters);
                    return new CogatFiltersViewModel
                    {
                        RootNodeType = ConfigSettings.ReturnUrl,
                        ErrorMessage = ex.Message
                    };
                }
                else
                {
                    throw ex;
                }
                
            }

        }

        public void UpdateFilters(string filterTypeNumber, List<string> values, out CogatFiltersViewModel cogatFiltersViewModel)
        {
            cogatFiltersViewModel = new CogatFiltersViewModel { };
            try
            {
                var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);

                Enum.TryParse(filterTypeNumber, out FilterType filterType);
                var filterToUpdate = currentPanel.GetFilterByType(filterType);

                filterToUpdate.Items.ForEach(i => i.IsSelected = false);
                if (values != null)
                    filterToUpdate.Items.Where(i => values.Contains(i.Value)).ToList().ForEach(i => i.IsSelected = true);

                if (filterType == FilterType.TestEvent || filterType == FilterType.Grade)
                    currentPanel.RemoveLocationFilters();

                
                var newPanel = _filtersBuilder.BuildPanel(currentPanel, filterType, _userData);

                _sessionManager.Store(newPanel, SessionKey.CogatFilters);

            }
            catch (Exception ex)
            {
                if (ex.Message == "Unauthorized")
                {
                    var filterPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
                    if (filterPanel != null)
                    {
                        filterPanel.isRetry = true;
                    }
                    _sessionManager.Store(filterPanel, SessionKey.CogatFilters);
                    cogatFiltersViewModel = new CogatFiltersViewModel { ErrorMessage = "Unauthorized" };
                }
            }
        }
        public async Task<string> SendReportToBackgroundAsync(ElevateUiBackgroundRequest ECogatRequest)
        {

            if (ConfigSettings.IsTelerikReportFeatureEnabled)
            {
                var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);

                GenerateReportRequest generateReportRequest = new GenerateReportRequest();
                if (ECogatRequest.ReportTemplate == "CLSS")
                {
                    var ERECogatListReq = _eREngineParamterBuilder.BuildCogatListOfStudentScoresBackground(ECogatRequest, currentPanel, _userData);
                    generateReportRequest.ActuateJobID = "0";
                    generateReportRequest.isActuate = "0";
                    generateReportRequest.CriteriaID = "0";
                    generateReportRequest.folderPath = ConfigSettings.FolderPath + _userData.CurrentGuid;
                    generateReportRequest.HasExportToExcel = false;
                    generateReportRequest.HasLastNameSearch = false;
                    generateReportRequest.filename = ECogatRequest.FileName;
                    generateReportRequest.Environment = ConfigSettings.Environment;
                    generateReportRequest.ReportType = "CogAT7ListStudentScores";
                    generateReportRequest.UserID = _userData.CurrentGuid;
                    generateReportRequest.Parameters = JsonConvert.SerializeObject(ERECogatListReq);
                }
                else if (ECogatRequest.ReportTemplate == "CPN")
                {
                    var ERECogatListReq = _eREngineParamterBuilder.BuildCogatProfileNarrativeBackground(ECogatRequest, currentPanel, _userData);
                    generateReportRequest.ActuateJobID = "0";
                    generateReportRequest.isActuate = "0";
                    generateReportRequest.CriteriaID = "0";
                    generateReportRequest.folderPath = ConfigSettings.FolderPath + _userData.CurrentGuid;
                    generateReportRequest.HasExportToExcel = false;
                    generateReportRequest.HasLastNameSearch = false;
                    generateReportRequest.filename = ECogatRequest.FileName;
                    generateReportRequest.Environment = ConfigSettings.Environment;
                    generateReportRequest.ReportType = "CogAT7ProfileNarrative";
                    generateReportRequest.UserID = _userData.CurrentGuid;
                    generateReportRequest.Parameters = JsonConvert.SerializeObject(ERECogatListReq);

                }
                else if (ECogatRequest.ReportTemplate == "CGS")
                {
                    var ERECogatListReq = _eREngineParamterBuilder.BuildCogatGroupSummaryBackground(ECogatRequest, currentPanel, _userData);
                    generateReportRequest.ActuateJobID = "0";
                    generateReportRequest.isActuate = "0";
                    generateReportRequest.CriteriaID = "0";
                    generateReportRequest.folderPath = ConfigSettings.FolderPath + _userData.CurrentGuid;
                    generateReportRequest.HasExportToExcel = false;
                    generateReportRequest.HasLastNameSearch = false;
                    generateReportRequest.filename = ECogatRequest.FileName;
                    generateReportRequest.Environment = ConfigSettings.Environment;
                    generateReportRequest.ReportType = "CogAT7GroupSummary";
                    generateReportRequest.UserID = _userData.CurrentGuid;
                    generateReportRequest.Parameters = JsonConvert.SerializeObject(ERECogatListReq);
                }
                else if (ECogatRequest.ReportTemplate == "DE")
                {
                    var ERECogatDataReq = _eREngineParamterBuilder.BuildCogatDataExporterBackground(ECogatRequest, currentPanel, _userData);
                    generateReportRequest.ActuateJobID = "0";
                    generateReportRequest.isActuate = "0";
                    generateReportRequest.CriteriaID = "0";
                    generateReportRequest.folderPath = ConfigSettings.FolderPath + _userData.CurrentGuid;
                    generateReportRequest.HasExportToExcel = false;
                    generateReportRequest.HasLastNameSearch = false;
                    generateReportRequest.filename = ECogatRequest.FileName;
                    generateReportRequest.Environment = ConfigSettings.Environment;
                    generateReportRequest.ReportType = "CatalogExporter";
                    generateReportRequest.UserID = _userData.CurrentGuid;
                    generateReportRequest.Parameters = JsonConvert.SerializeObject(ERECogatDataReq);
                }
                await CreatingTaskForBackground(generateReportRequest);
            }
            return string.Empty;
        }
        public void ResetPage()
        {
            _sessionManager.Delete(SessionKey.CogatFilters);
        }

        public async Task<StudentRosterViewModel> GetAllDataAsync()
        {

            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);

            var apiParams = _eREngineParamterBuilder.BuildGetAllDataParameters(currentPanel, _userData);

            var studentSubtests = _elevateReportingEngineClient.GetAllDataForCogatDashBoard(apiParams, _userData);

            var model = BuildRosterViewModel(studentSubtests, currentPanel);
            model.ApiParams = apiParams;

            //WritePerformanceDebug(methodName, "On Exit");

            return model;
        }

        private void WritePerformanceDebug(string methodName, string traceAction, SmiApiParameters smiApiParams = null)
        {
            if (!ConfigSettings.TurnOnCogatPerformanceLogging) return;

            Logger.Trace($"CogAT Perf :: GUID: {_userData.CurrentGuid} :: Method: {methodName} :: Trace: {traceAction}");

            if (smiApiParams != null)
                Logger.Trace($"CogAT Perf :: GUID: {_userData.CurrentGuid} :: Method: {methodName} :: API params: {JsonConvert.SerializeObject(smiApiParams)}");
        }

        public async Task<object> GetRecordsCountAsync(int testFetchSize, int smFetchSize)
        {
            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
            var apiParams = _paramsBuilder.BuildRecordsCountParams(currentPanel, _userData, testFetchSize, smFetchSize);

            var response = await _smiClient.CallStudentRosterCountsAsync(apiParams);

            return new
            {
                values = response,
                api_params = apiParams
            };
        }

        public async Task<object> GetCutScoreAsync()
        {
            var retObj = @"{'values':[{'Region_name':'2020IFR','Region_id':56352,'Region_code':'','System_name':'2020IFS','System_id':72569,'System_code':'','District_name':'2020IFD','District_id':79805,'District_code':'','Building_name':'2020IFD1','Building_id':93773,'Building_code':'','Class_name':'','Class_id':-1,'Class_code':'','Grade':3,'Visual_level':'9','Test_count':1}],'api_params':{'SMIBaseParameters':{'GradeLevelIDs':2587,'CustomerScoresetIDs':'78518','TestPopulationNodeIDs':'93773','TestPopulationNodeType':'BUILDING','ReportPopulationNodeIDs':'214382','ReportPopulationNodeType':'CLASS','OuterGroup':'BUILDING','InnerGroup':null,'OptionalWhereClause':null,'Accountability':1},'SMIFilteringParameters':{'GenderList':'','EthnicityList':'','ProgramList':'','AdminValueList':''},'SMIGeneralProcessingParameters':{'TestFetchSize':0,'SMFetchSize':0,'LoadLPRs':false,'LoggingSessionId':227215,'LoggingCustomerId':72344,'LoggingUserLocationGuid':'95E23F1C87B942FA876FBC74D6774803','LoggingOutputSystemName':'DASHBOARD'},'SMIGroupParameters':{},'SMISubtestParameters':{'SubtestAcronyms':''Verbal','Quant','NonVerb','CompVQ','CompVN','CompQN','CompVQN'','RankingScoreDirection':null,'RankingSubtestAcronym':null}}}";
            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
            var apiParams = _paramsBuilder.BuildCutScoreParams(currentPanel, _userData);

            var response = await _smiClient.CallCutScoreAsync(apiParams);
            response = retObj;
            return new
            {
                values = response,
                api_params = apiParams
            };
        }

        public async Task<object> GetCutScoreAsync(string groupingType, string filter)
        {
            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
            var apiParams = _paramsBuilder.BuildCutScoreParams(currentPanel, _userData, groupingType, filter);

            var response = await _smiClient.CallCutScoreAsync(apiParams);

            return new
            {
                values = response,
                api_params = apiParams
            };
        }

        public async Task<object> GetAgeStaninesAsync()
        {

            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
            var apiParams = _eREngineParamterBuilder.BuidAgeStanineParamters(currentPanel, _userData);

            var groupSubtests = _elevateReportingEngineClient.GetAgeStanines(apiParams, _userData);
            var checkpoint = "";
            return new
            {
                values = groupSubtests.payload,
                api_params = apiParams
            };
        }

        public async Task<object> GetAbilityProfilesAsync()
        {
            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
            var apiParams = _paramsBuilder.BuildAbilityProfilesParams(currentPanel, _userData);

            var abilityProfiles = await _smiClient.CallGetAbilityProfilesAsync(apiParams);

            foreach (var abilityProfile in abilityProfiles)
            {
                if (abilityProfile.Profile_extreme == "E")
                {
                    var chars = abilityProfile.Profile_display.ToCharArray();
                    chars[1] = 'E';
                    abilityProfile.Profile_display = new string(chars);
                }
            }

            return new
            {
                values = abilityProfiles,
                api_params = apiParams
            };
        }


        public async Task<object> GetGroupTotalsAsync()
        {
            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
            var apiParams = _eREngineParamterBuilder.BuildGetGroupTotalParamters(currentPanel, _userData);
            var groupTotals = _elevateReportingEngineClient.GetGroupTotalForCogatDashBoard(apiParams, _userData).payload;
            _groupTotalList = groupTotals;
            var contents = groupTotals.Select(s => new { Name = s.subtest_name, MiniName = s.subtest_mininame }).Distinct().ToList();

            var isVqnSelected = currentPanel.GetFilterByType(FilterType.Content).IsValueSelected("'CompVQN'");
            var hasLprScore = _cogat.LoadLprs(currentPanel);

            try
            {


                var groupTotal = new Dictionary<string, object> { { "node_name", "Group Total" } };
                if (currentPanel.GroupId[0] == 1)
                {
                    for (int c = 0; c < contents.Count; ++c)
                    {
                        var content = contents[c];

                        if (!isVqnSelected && content.MiniName == "VQN")
                            continue;

                        var currentGroupTotal = groupTotals.FirstOrDefault(gt => gt.subtest_mininame == content.MiniName);
                        if (currentGroupTotal == null)
                            continue;

                        var scores = GetGroupTotalScores(currentGroupTotal, currentPanel);

                       

                        groupTotal.Add($"APR{c}",scores.apr_score);
                        groupTotal.Add($"AS{c}", scores.as_score);
                        groupTotal.Add($"GPR{c}", scores.gpr_score);
                        groupTotal.Add($"GS{c}", scores.gs_score);
                        groupTotal.Add($"USS{c}", scores.uss_score == null ? "" : $"{scores.uss_score:0.0}");
                        groupTotal.Add($"SAS{c}", scores.sas_score == null ? "" : $"{scores.sas_score:0.0}");
                        groupTotal.Add($"RS{c}", scores.rs_score== null ? "" : $"{scores.rs_score:0.0}");
                        groupTotal.Add($"NANI{c}", scores.na_score == null ? "" : $"{scores.na_score}/{scores.total_item_score}");

                        if (!hasLprScore) continue;

                        groupTotal.Add($"LPR{c}", scores.lpr_score);
                        groupTotal.Add($"LS{c}", scores.ls_score);
                    }
                }
                else if (currentPanel.GroupId[0] == 2)
                {
                    for (int c = 0; c < contents.Count; ++c)
                    {
                        var content = contents[c];

                        var currentGroupTotal = groupTotals.FirstOrDefault(gt => gt.subtest_mininame == content.MiniName);
                        if (currentGroupTotal == null)
                            continue;

                        var scores = GetGroupTotalScores(currentGroupTotal, currentPanel);



                        groupTotal.Add($"APR{c}", scores.apr_score);
                        groupTotal.Add($"AS{c}", scores.as_score);
                        groupTotal.Add($"GPR{c}", scores.gpr_score);
                        groupTotal.Add($"GS{c}", scores.gs_score);
                        groupTotal.Add($"USS{c}", scores.uss_score == null ? "" : $"{scores.uss_score:0.0}");
                        groupTotal.Add($"SAS{c}", scores.sas_score == null ? "" : $"{scores.sas_score:0.0}");
                        groupTotal.Add($"RS{c}", scores.rs_score == null ? "" : $"{scores.rs_score:0.0}");
                        groupTotal.Add($"NANI{c}", scores.na_score == null ? "" : $"{scores.na_score}/{scores.total_item_score}");

                        if (!hasLprScore) continue;

                        groupTotal.Add($"LPR{c}", scores.lpr_score);
                        groupTotal.Add($"LS{c}", scores.ls_score);
                    }
                }

                return new
                {
                    group_total = groupTotal,
                    api_params = apiParams
                };
            }
            catch (Exception ex)
            {
                return new {
                    group_total = ex.Message,
                };
            }
        }



        public async Task<StudentRosterViewModel> GetStudentRosterAsync(int take, int skip, string filter, string score, string orderType)
        {
            StudentRosterViewModel model = new StudentRosterViewModel();
            var methodName = "GetStudentRoster";
            WritePerformanceDebug(methodName, "On Enter");

            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);
            var numRows = currentPanel.GetFilterByType(FilterType.Content).SelectedValues.Count;

            var apiParams = _paramsBuilder.BuildStudentRosterParams(currentPanel, _userData, skip + take, skip, filter, score, orderType);

            WritePerformanceDebug(methodName, "Before API call", apiParams);

            var studentSubtests = await _smiClient.CallStudentRosterAsync(apiParams);

            WritePerformanceDebug(methodName, "After API call");

            studentSubtests.RemoveRange(0, skip * numRows);

            //model = BuildRosterViewModel(studentSubtests, currentPanel);

            model.ApiParams = apiParams;

            WritePerformanceDebug(methodName, "On Exit");

            return model;
        }

        #region Background and ReportCenter

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
            var UserID = _userData.UserId;
            JsonReturnString = await _reportingBackgroundRepository.GetFilesTodisplay(apiUrl, UserID);
            return JsonReturnString;
        }
        public void DeletePDF(int fileId)
        {
            var apiUrl = ConfigSettings.TaskUrl;
            _reportingBackgroundRepository.GetSoftDelete(apiUrl, fileId);

        }

        public CogatBackgroundFiltersModel GetBackGroundGradeLocations()
        {
            var filterPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);

            var backgroundPanel = _backgroundModelBuilder.BuildBackgroundModal(filterPanel, _userData);

            return new CogatBackgroundFiltersModel
            {
                Filters = backgroundPanel.GetAllFilters(),
            };
        }
       

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


        #endregion
        private StudentRosterViewModel BuildRosterViewModel(List<ForegroundReporting.Lib.Models.Responses.StudentSubtest> allData, FilterPanel currentPanel)
        {
            StudentRosterViewModel result = new StudentRosterViewModel();
            try
            {
                var contents = allData.Select(s => new { Name = s.Subtest_name, MiniName = s.Subtest_mininame }).Where(s=> !s.Name.Contains("Alt")).Distinct().ToList();

                _studentData = allData;

                //<-Ask Abdul. Why is isVqnSelected forced to be false? is this a workaround?
                //var isVqnSelected = false;//currentPanel.GetFilterByType(FilterType.Content).IsValueSelected("'CompVQN'");
                var isVqnSelected = currentPanel.GetFilterByType(FilterType.Content).IsValueSelected("'CompVQN'");

                var hasLprScore = _cogat.LoadLprs(currentPanel);

                var headerCells = new List<StudentRosterHeaderCell>
                {
                    new StudentRosterHeaderCell
                    {
                        Title = "Student Name",
                        TitleFull = "Student Name",
                        Multi = 0,
                        Field = "node_name"
                    }
                };

                for (int c = 0; c < contents.Count; ++c)
                {
                    var content = contents[c];

                    if (!isVqnSelected && content.MiniName == "VQN")
                        continue;

                    var studentRosterHeaderCell = new StudentRosterHeaderCell
                    {
                        Title = currentPanel.Battery == "Screener" ? "Total Score" : content.MiniName,
                        TitleFull = content.Name,
                        Multi = 1,
                        Fields = new List<StudentRosterHeaderField>
                        {

                            new StudentRosterHeaderField
                            {
                                Field = $"APR{c}",
                                Title = "APR",
                                TitleFull = "Age Percentile Rank"
                            },
                            new StudentRosterHeaderField
                            {
                                Field = $"AS{c}",
                                Title = "AS",
                                TitleFull = "Age Stanine"
                            },
                            new StudentRosterHeaderField
                            {
                                Field = $"GPR{c}",
                                Title = "GPR",
                                TitleFull = "Grade Percentile Rank"
                            },
                            new StudentRosterHeaderField
                            {
                                Field = $"GS{c}",
                                Title = "GS",
                                TitleFull = "Grade Stanine"
                            },
                            new StudentRosterHeaderField
                            {
                                Field = $"USS{c}",
                                Title = "USS",
                                TitleFull = "Universal Scale Score"
                            },
                            new StudentRosterHeaderField
                            {
                                Field = $"SAS{c}",
                                Title = "SAS",
                                TitleFull = "Standard Age Score"
                            },
                            new StudentRosterHeaderField
                            {
                                Field = $"RS{c}",
                                Title = "RS",
                                TitleFull = "Raw Score"
                            },
                            new StudentRosterHeaderField
                            {
                                Field = $"NANI{c}",
                                Title ="NA/NI",
                                TitleFull = "Number Attempted/Number of Items"
                            }
                        }
                    };

                    headerCells.Add(studentRosterHeaderCell);

                    if (!hasLprScore) continue;

                    studentRosterHeaderCell.Fields.Add(new StudentRosterHeaderField
                    {
                        Field = $"LPR{c}",
                        Title = "LPR",
                        TitleFull = "Local Percentile Rank"
                    });
                    studentRosterHeaderCell.Fields.Add(new StudentRosterHeaderField
                    {
                        Field = $"LS{c}",
                        Title = "LS",
                        TitleFull = "Local Stanine"
                    });
                }

                var studentRosterRows = new List<Dictionary<string, object>>();
                var previousRecordStudentId = "";
                var row = new Dictionary<string, object>();
                var currentWarningsSubset = new List<string>();

                foreach (var record in allData)
                {
                    if (previousRecordStudentId != record.Student_id)
                    {
                        row = new Dictionary<string, object>
                            {
                                { "node_id", record.Student_id.ToString()},
                                { "node_name", $"{record.Last_name}, {record.First_name}" },
                                { "link", "" },
                                { "birth_date", record.Birth_date },
                                { "age", record.Test_age_yy_mm_score },
                                { "district", record.District_name },
                                { "district_id", record.District_id },
                                { "building", record.Building_name },
                                { "building_id", record.Building_id },
                                { "class_name", record.Class_names },
                                { "class_id", record.Class_ids }
                            };

                        var studentWarnings = _swUtil.GetStudentLevelWarnings(record, currentPanel);

                        foreach (var warning in studentWarnings)
                        {
                            row.Add(warning.Key, warning.Value);

                            if (Convert.ToInt32(warning.Value) == 1 && !currentWarningsSubset.Contains(warning.Key))
                                currentWarningsSubset.Add(warning.Key);
                        }

                        row.Add("ts", 0);
                        row.Add("irp", 0);
                        row.Add("mio", 0);
                        row.Add("tfia", 0);
                        row.Add("scni", 0);
                        row.Add("vats", 0);

                        studentRosterRows.Add(row);
                    }

                    // REMOVING CONDITION AND ADDING NULL CHECK FOR ELEVATE COGAT REPORTING AS ABILITY PROFILE SHOULD BE AVALIAVLE FOR UI
                    //if (record.Subtest_mininame == "VQN")
                    //    row.Add("ability_profile", record.Prof_score);
                    if (row.ContainsKey("ability_profile"))
                    {
                        row["ability_profile"] = record.Prof_score == null ? "" : record.Prof_score;
                    }
                    else
                    {
                        row.Add("ability_profile", record.Prof_score == null ? "" : record.Prof_score);
                    }

                    previousRecordStudentId = record.Student_id;


                    if (!isVqnSelected && record.Subtest_mininame == "VQN")
                        continue;

                    if (record.Subtest_name.Contains("Alt"))
                    {
                        continue;
                    }
                    //if ((record.Battery.Contains("AV") && !record.Is_alt) || (record.Is_alt))
                    //{
                    //    continue;
                    //}
                    //if (record.Is_alt || !record.Battery.Contains("AV"))

                    // if battery contains AV show records
                    var contentIndex = contents.FindIndex(c => c.MiniName == record.Subtest_mininame);
                    row.Add($"APR{contentIndex}", record.Apr_score);
                    row.Add($"AS{contentIndex}", record.As_score);
                    row.Add($"GPR{contentIndex}", record.Gpr_score);
                    row.Add($"GS{contentIndex}", record.Gs_score);
                    row.Add($"USS{contentIndex}", record.Uss_score);
                    row.Add($"SAS{contentIndex}", record.Sas_score);
                    row.Add($"RS{contentIndex}", record.Rs_score);
                    row.Add($"NANI{contentIndex}", record.Na_score == null ? "" : $"{record.Na_score}/{record.Total_item_score}");


                    var scoreWarnings = _swUtil.GetScoreLevelWarnings(record, currentPanel);

                    foreach (var warning in scoreWarnings)
                    {
                        row.Add($"{warning.Key}{contentIndex}", warning.Value);

                        if (Convert.ToInt32(warning.Value) == 1)
                            row[warning.Key] = 1;

                        if (Convert.ToInt32(warning.Value) == 1 && !currentWarningsSubset.Contains(warning.Key))
                            currentWarningsSubset.Add(warning.Key);
                    }

                    if (!hasLprScore) continue;

                    row.Add($"LPR{contentIndex}", record.Lpr_score);
                    row.Add($"LS{contentIndex}", record.Ls_score);
                }

                result = new StudentRosterViewModel
                {
                    Columns = headerCells,
                    Values = studentRosterRows,
                    ExtraParams = new StudentRosterExtraParams
                    {
                        Form = string.Join(",", allData.Select(d => d.Form).Distinct()),
                        NormYear = allData.Any() ? $"{allData.First().Quartermonth_desc} {allData.First().Rpt_normyear_desc}" : "",
                        HasLprScore = _cogat.LoadLprs(currentPanel),
                        HasLsScore = _cogat.LoadLprs(currentPanel),
                        // ScoreWarningsLookup = _swUtil.GetAllScoreWarningItems(),
                        WarningsList = currentWarningsSubset
                    }
                };

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }
        
        private DashboardGroupScores GetGroupTotalScores(GroupTotalPayload currentGroupTotal, FilterPanel currentPanel)
        {
            //var locationFilters = currentPanel.LocationFilters;
            //string scoresLocationType = locationFilters.Count >= 2 ? locationFilters[locationFilters.Count - 2].Type.ToString().ToUpper() : currentPanel.SelectedTestAdmin.NodeType;
            string scoresLocationType = currentPanel.SelectedTestAdmin.NodeType.ToUpper();
            if (scoresLocationType == "DISTRICT")
            {
                scoresLocationType = "BUILDING";
            }
            switch (scoresLocationType)
            {
                case "STATE":
                    return currentGroupTotal.stateScores;
                case "REGION":
                    return currentGroupTotal.regionScores;
                case "SYSTEM":
                    return currentGroupTotal.systemScores;
                case "DISTRICT":
                    return currentGroupTotal.districtScores;
                case "BUILDING":
                    return currentGroupTotal.buildingScores;
                default:
                    return currentGroupTotal.classScores;
            }
        }

        #region old code
        /* public async Task<object> GetGroupTotalsAsync()
        {
            //var groupTotalsDefault = @"{'group_total':{'node_name':'Group Total','APR0':null,'AS0':null,'GPR0':81,'GS0':7,'USS0':'155.4','SAS0':'','RS0':'27.2','NANI0':'','LPR0':null,'LS0':null,'APR1':null,'AS1':null,'GPR1':98,'GS1':9,'USS1':'175.4','SAS1':'','RS1':'24.6','NANI1':'','LPR1':null,'LS1':null,'APR2':null,'AS2':null,'GPR2':85,'GS2':7,'USS2':'163.9','SAS2':'','RS2':'24.6','NANI2':'','LPR2':null,'LS2':null,'APR3':null,'AS3':null,'GPR3':96,'GS3':9,'USS3':'165.5','SAS3':'','RS3':'','NANI3':'','LPR3':null,'LS3':null,'APR4':null,'AS4':null,'GPR4':85,'GS4':7,'USS4':'159.6','SAS4':'','RS4':'','NANI4':'','LPR4':null,'LS4':null,'APR5':null,'AS5':null,'GPR5':94,'GS5':8,'USS5':'169.3','SAS5':'','RS5':'','NANI5':'','LPR5':null,'LS5':null,'APR6':null,'AS6':null,'GPR6':93,'GS6':8,'USS6':'165.0','SAS6':'','RS6':'','NANI6':'','LPR6':null,'LS6':null},'api_params':{'SMIBaseParameters':{'GradeLevelIDs':2521,'CustomerScoresetIDs':'53386','TestPopulationNodeIDs':'58325','TestPopulationNodeType':'SYSTEM','ReportPopulationNodeIDs':'123352','ReportPopulationNodeType':'CLASS','OuterGroup':'SYSTEM','InnerGroup':'SYSTEM','OptionalWhereClause':null,'Accountability':0},'SMIFilteringParameters':{'GenderList':'','EthnicityList':'','ProgramList':'','AdminValueList':''},'SMIGeneralProcessingParameters':{'TestFetchSize':0,'SMFetchSize':0,'LoadLPRs':false,'LoggingSessionId':219411,'LoggingCustomerId':61104,'LoggingUserLocationGuid':'D86D6691F4BD4119A599279AE1AEC421','LoggingOutputSystemName':'DASHBOARD'},'SMIGroupParameters':null,'SMISubtestParameters':{'SubtestAcronyms':''Verbal','Quant','NonVerb','CompVQ','CompVN','CompQN','CompVQN'','RankingScoreDirection':null,'RankingSubtestAcronym':null}}}";
            var currentPanel = (FilterPanel)_sessionManager.Retrieve(SessionKey.CogatFilters);

            var groupTotalParams = _paramsBuilder.BuildGroupTotalParams(currentPanel, _userData);
            //var groupTotals = await _smiClient.CallGroupTotalsAsync(groupTotalParams);
            var groupTotalsString = @"[{'Acronym':'Verbal','Subtest_name':'Verbal','Subtest_mininame':'V','StateScores':null,'RegionScores':null,'SystemScores':null,'DistrictScores':{'Apr_score':45.0,'As_score':5.0,'Gpr_score':44.0,'Gs_score':5.0,'Uss_score':149.0,'Sas_score':97.9,'Rs_score':28.2,'Na_score':null,'Total_item_score':null,'Ls_score':null,'Lpr_score':null},'BuildingScores':null,'ClassScores':null},{'Acronym':'Quant','Subtest_name':'Quantitative','Subtest_mininame':'Q','StateScores':null,'RegionScores':null,'SystemScores':null,'DistrictScores':{'Apr_score':56.0,'As_score':5.0,'Gpr_score':57.0,'Gs_score':5.0,'Uss_score':151.0,'Sas_score':102.4,'Rs_score':20.9,'Na_score':null,'Total_item_score':null,'Ls_score':null,'Lpr_score':null},'BuildingScores':null,'ClassScores':null},{'Acronym':'NonVerb','Subtest_name':'Nonverbal','Subtest_mininame':'N','StateScores':null,'RegionScores':null,'SystemScores':null,'DistrictScores':{'Apr_score':51.0,'As_score':5.0,'Gpr_score':54.0,'Gs_score':5.0,'Uss_score':152.6,'Sas_score':100.4,'Rs_score':25.6,'Na_score':null,'Total_item_score':null,'Ls_score':null,'Lpr_score':null},'BuildingScores':null,'ClassScores':null},{'Acronym':'CompVQ','Subtest_name':'Composite (VQ)','Subtest_mininame':'VQ','StateScores':null,'RegionScores':null,'SystemScores':null,'DistrictScores':{'Apr_score':51.0,'As_score':5.0,'Gpr_score':50.0,'Gs_score':5.0,'Uss_score':149.9,'Sas_score':100.4,'Rs_score':0.0,'Na_score':null,'Total_item_score':null,'Ls_score':null,'Lpr_score':null},'BuildingScores':null,'ClassScores':null},{'Acronym':'CompVN','Subtest_name':'Composite (VN)','Subtest_mininame':'VN','StateScores':null,'RegionScores':null,'SystemScores':null,'DistrictScores':{'Apr_score':48.0,'As_score':5.0,'Gpr_score':47.0,'Gs_score':5.0,'Uss_score':150.8,'Sas_score':99.0,'Rs_score':0.0,'Na_score':null,'Total_item_score':null,'Ls_score':null,'Lpr_score':null},'BuildingScores':null,'ClassScores':null},{'Acronym':'CompQN','Subtest_name':'Composite (QN)','Subtest_mininame':'QN','StateScores':null,'RegionScores':null,'SystemScores':null,'DistrictScores':{'Apr_score':55.0,'As_score':5.0,'Gpr_score':55.0,'Gs_score':5.0,'Uss_score':151.9,'Sas_score':102.0,'Rs_score':0.0,'Na_score':null,'Total_item_score':null,'Ls_score':null,'Lpr_score':null},'BuildingScores':null,'ClassScores':null},{'Acronym':'CompVQN','Subtest_name':'Composite (VQN)','Subtest_mininame':'VQN','StateScores':null,'RegionScores':null,'SystemScores':null,'DistrictScores':{'Apr_score':50.0,'As_score':5.0,'Gpr_score':49.0,'Gs_score':5.0,'Uss_score':150.8,'Sas_score':100.2,'Rs_score':0.0,'Na_score':null,'Total_item_score':null,'Ls_score':null,'Lpr_score':null},'BuildingScores':null,'ClassScores':null}]";

            var groupTotals = JsonConvert.DeserializeObject<List<GroupTotal>>(groupTotalsString);


            var contents = groupTotals.Select(s => new { Name = s.Subtest_name, MiniName = s.Subtest_mininame }).Distinct().ToList();

            var isVqnSelected = currentPanel.GetFilterByType(FilterType.Content).IsValueSelected("'CompVQN'");
            var hasLprScore = _cogat.LoadLprs(currentPanel);

            var groupTotal = new Dictionary<string, object> { { "node_name", "Group Total" } };
            for (int c = 0; c < contents.Count; ++c)
            {
                var content = contents[c];

                if (!isVqnSelected && content.MiniName == "VQN")
                    continue;

                var currentGroupTotal = groupTotals.FirstOrDefault(gt => gt.Subtest_mininame == content.MiniName);
                if (currentGroupTotal == null)
                    continue;

                var scores = GetGroupTotalScores(currentGroupTotal, currentPanel);

                groupTotal.Add($"APR{c}", scores.Apr_score);
                groupTotal.Add($"AS{c}", scores.As_score);
                groupTotal.Add($"GPR{c}", scores.Gpr_score);
                groupTotal.Add($"GS{c}", scores.Gs_score);
                groupTotal.Add($"USS{c}", scores.Uss_score.IsNullOrZero() ? "" : $"{scores.Uss_score:0.0}");
                groupTotal.Add($"SAS{c}", scores.Sas_score.IsNullOrZero() ? "" : $"{scores.Sas_score:0.0}");
                groupTotal.Add($"RS{c}", scores.Rs_score.IsNullOrZero() ? "" : $"{scores.Rs_score:0.0}");
                groupTotal.Add($"NANI{c}", scores.Na_score == null ? "" : $"{scores.Na_score}/{scores.Total_item_score}");

                if (!hasLprScore) continue;

                groupTotal.Add($"LPR{c}", scores.Lpr_score);
                groupTotal.Add($"LS{c}", scores.Ls_score);
            }

            return new
            {
                group_total = groupTotal,
                api_params = groupTotalParams
            };
            //return groupTotalsDefault;

        }
*/
        #endregion
    }
}