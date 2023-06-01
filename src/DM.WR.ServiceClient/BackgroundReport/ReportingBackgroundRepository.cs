using DM.WR.Models.BackgroundReport;
using DM.WR.Models.Config;
using Flurl;
using Flurl.Http;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DM.WR.ServiceClient.BackgroundReport
{
    public interface IReportingBackgroundRepository
    {
        Task<string> SendTask(GenerateReportRequest reportRequest, string apiUrl);
        Task<string> GetFilesTodisplay(string apiUrl, string userId);
        Task<string> GetDataExportData(string apiUrl, int fielId);
        void GetSoftDelete(string apiUrl, int fileId);
    }

    public class ReportingBackgroundRepository : IReportingBackgroundRepository
    {

        public ReportingBackgroundRepository()
        {

        }

        public async Task<string> SendTask(GenerateReportRequest reportRequest, string apiUrl)
        {
            var result = "";
            try
            {
                result = $"{apiUrl}PostReportRequest";

                var query = await CreateReportRequest(reportRequest, result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<string> GetFilesTodisplay(string apiUrl, string userId)
        {
            var result = "";
            try
            {
                var createresult = apiUrl + "GetFilesTodisplay?" + "UserID=" + userId;
                result = await CallRestMethodAsync(createresult);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //exception handeling
            return result;
        }
        public async Task<string> GetDataExportData(string apiUrl, int fielId)
        {
            var result = "";
            try
            {
                var createresult = apiUrl + "GetDataExporterUiStringByFileId?fileId=" + fielId;
                result = await CallRestMethodAsync(createresult);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //exception handeling
            return result;
        }

        public void GetSoftDelete(string apiUrl, int fileId)
        {
            var createUrl = apiUrl + "GetSoftDelete?" + "FileID=" + fileId;
            var getResult = CallRestMethodAsync(createUrl);
        }

        private Task<string> CallRestMethodAsync(string url)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = WebRequestMethods.Http.Get;
                Task<WebResponse> task = Task.Factory.FromAsync(
                    request.BeginGetResponse,
                    asyncResult => request.EndGetResponse(asyncResult),
                    (object)null);

                return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }
        private async Task<string> CreateReportRequest(GenerateReportRequest reportRequest, string apiUrl)
        {
            string Response = "";
            HttpResponseMessage HttpResponseMessage = null;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    // serialize into json string
                    var myContent = jss.Serialize(reportRequest);

                    var httpContent = new StringContent(myContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage = await httpClient.PostAsync(apiUrl, httpContent);

                    if (HttpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        Response = HttpResponseMessage.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        Response = "Some error occured." + HttpResponseMessage.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                Response = $"{ex.Message}{HttpResponseMessage.StatusCode}";
            }
            return Response;
        }
    }
}
