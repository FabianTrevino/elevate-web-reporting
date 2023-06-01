using DM.WR.Models.ScoreManagerApi;
using DM.WR.ServiceClient.ScoreManagerApi.Models;
using Flurl.Http;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DM.WR.Models.Config;
using System;

namespace DM.WR.ServiceClient.ScoreManagerApi
{
    public class ScoreManagerClient //TODO: hook up interface 
    {
        private static readonly string ServiceUrl = ConfigSettings.Dashboard.CogAT.SmiApiUrl;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<List<StudentSubtest>> CallStudentRosterAsync(SmiApiParameters parameters)
        {
            var endpoint = "SMICogATDashboardRoster";
            var url = $"{ServiceUrl}{endpoint}";

            try
            {
                return await url.WithTimeout(TimeSpan.FromMinutes(60)).PostJsonAsync(parameters).ReceiveJson<List<StudentSubtest>>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(endpoint, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(endpoint, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<object> CallStudentRosterCountsAsync(SmiApiParameters parameters)
        {
            var endpoint = "SMICogATDashboardRoster";
            var url = $"{ServiceUrl}{endpoint}";

            try
            {
                return await url.PostJsonAsync(parameters).ReceiveJson<object>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(endpoint, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(endpoint, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<object> CallCutScoreAsync(SmiApiParameters parameters)
        {
            var endpoint = "SMICogATDashboardTestCountInfo";
            var url = $"{ServiceUrl}{endpoint}";

            try
            {
                return await url.PostJsonAsync(parameters).ReceiveJson<object>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(endpoint, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(endpoint, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<List<object>> CallGetAgeStaninesAsync(SmiApiParameters parameters)
        {
            var endpoint = "SMICogATDashboardStanineCounts";
            var url = $"{ServiceUrl}{endpoint}";

            try
            {
                return await url.PostJsonAsync(parameters).ReceiveJson<List<object>>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(endpoint, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(endpoint, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<List<AbilityProfile>> CallGetAbilityProfilesAsync(SmiApiParameters parameters)
        {
            var endpoint = "SMICogATDashboardProfileCounts";
            var url = $"{ServiceUrl}{endpoint}";

            try
            {
                return await url.PostJsonAsync(parameters).ReceiveJson<List<AbilityProfile>>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(endpoint, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(endpoint, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<List<GroupTotal>> CallGroupTotalsAsync(SmiApiParameters parameters)
        {
            var endpoint = "SMICogATDashboardGroupTotals";
            var url = $"{ServiceUrl}{endpoint}";

            try
            {
                return await url.PostJsonAsync(parameters).ReceiveJson<List<GroupTotal>>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(endpoint, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(endpoint, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        private string FlurlHttpTimeoutMessage(string endpoint, object parameters)
        {
            return $"SMI API Client :: Timeout :: Endpoint: {endpoint} :: Parameters: {JsonConvert.SerializeObject(parameters, Formatting.Indented)}";
        }

        private string FlurlHttpExceptionMessage(string endpoint, object parameters, string message, HttpStatusCode? httpStatus)
        {
            return $"SMI API Client :: Exception :: Endpoint: {endpoint} :: Parameters: {JsonConvert.SerializeObject(parameters, Formatting.Indented)} :: Message: {message} :: HTTP Status: {httpStatus}";
        }
    }
}