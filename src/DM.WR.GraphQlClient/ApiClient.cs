using DM.WR.GraphQlClient.Models;
using DM.WR.Models.Config;
using DM.WR.Models.GraphqlClient.DomainEndPoint;
using DM.WR.Models.GraphqlClient.PerformanceLevelDescriptorsEndPoint;
using DM.WR.Models.GraphqlClient.RangeEndPoint;
using DM.WR.Models.GraphqlClient.UserEndPoint;
using DM.WR.Models.IowaFlex;
using Flurl.Http;
using GraphQL.Client;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Student = DM.WR.Models.GraphqlClient.StudentEndPoint.Student;

namespace DM.WR.GraphQlClient
{
    public interface IApiClient
    {
        Task<User> MakeUserCallAsync(string query);
        Task<User> MakeDifferentiatedReportCallAsync(string query, string cacheKey);
        Task<Student> MakeStudentCallAsync(string query);
        Task<SubjectGradeDomains> MakeProfileNarrativeLookupCallAsync(string query, string subject, string grade);
        Task<StandardScoreRange> MakeBandsLookupCallAsync(string query, string subject, string grade);
        Task<PerformanceLevelDescriptor> MakePerformanceLevelDescriptorCallAsync(string query, string subject, string pldName);
        Task<PerformanceLevelStatement> MakePerformanceLevelStatementCallAsync(string query, string subject, string pldName, int? pldLevel);
    }

    public class ApiClient : IApiClient
    {
        private readonly string _apiEndpoint;
        private readonly ICacheWrapper _cacheWrapper;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ApiClient(ICacheWrapper cacheWrapper)
        {
            _apiEndpoint = ConfigSettings.AdaptiveDashboard.GraphqlUrl;
            _cacheWrapper = cacheWrapper;
        }

        public async Task<User> MakeUserCallAsync(string query)
        {
            using (var graphQlClient = await GetAuthenticatedGraphQlClientAsync("user"))
            {
                var result = await graphQlClient.PostQueryAsync(query);

                if (result.Errors.Length == 0)
                    return result.GetDataFieldAs<User>("user");

                if (result.Errors.First().Message.Contains("Unable to find any test events"))
                {
                    Logger.Error(NoTestEventsMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                    throw new Exception("Adaptive: No data");
                }

                if (result.Errors.First().Message.Contains("Unable to find students matching AND condition for populations"))
                {
                    Logger.Error(NoDataMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                    return null;
                }

                Logger.Error(ExceptionMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                throw new Exception("Call to the 'user' endpoint of the GraphQL API returned errors.");
            }
        }

        public async Task<User> MakeDifferentiatedReportCallAsync(string query, string cacheKey)
        {
            return await _cacheWrapper.GetFromCacheAsync(cacheKey, async () => await MakeUserCallAsync(query));
        }

        public async Task<Student> MakeStudentCallAsync(string query)
        {
            using (var graphQlClient = await GetAuthenticatedGraphQlClientAsync("student"))
            {
                var result = await graphQlClient.PostQueryAsync(query);
                if (result.Errors.Length > 0)
                {
                    Logger.Error(ExceptionMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                    throw new Exception("Call to the 'student' endpoint of the GraphQL API returned errors.");
                }

                return result.GetDataFieldAs<Student>("student");
            }
        }

        public async Task<SubjectGradeDomains> MakeProfileNarrativeLookupCallAsync(string query, string subject, string grade)
        {
            var cacheKey = $"PN{subject}{grade}";
            return await _cacheWrapper.GetFromCacheAsync(cacheKey, async () =>
            {
                using (var graphQlClient = await GetAuthenticatedGraphQlClientAsync("domain"))
                {
                    var result = await graphQlClient.PostQueryAsync(query);
                    if (result.Errors.Length > 0)
                    {
                        Logger.Error(ExceptionMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                        throw new Exception("Call to the 'domain' endpoint of the GraphQL API returned errors.");
                    }

                    return result.GetDataFieldAs<SubjectGradeDomains>("subjectGradeDomains");
                }
            });
        }

        public async Task<StandardScoreRange> MakeBandsLookupCallAsync(string query, string subject, string grade)
        {
            var cacheKey = $"B{subject}{grade}";
            return await _cacheWrapper.GetFromCacheAsync(cacheKey, async () =>
            {
                using (var graphQlClient = await GetAuthenticatedGraphQlClientAsync("range"))
                {
                    var result = await graphQlClient.PostQueryAsync(query);
                    if (result.Errors.Length > 0)
                    {
                        Logger.Error(ExceptionMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                        throw new Exception("Call to the 'range' endpoint of the GraphQL API returned errors.");
                    }

                    return result.GetDataFieldAs<StandardScoreRange>("standardScoreRange");
                }
            });
        }

        public async Task<PerformanceLevelDescriptor> MakePerformanceLevelDescriptorCallAsync(string query, string subject, string pldName)
        {
            var cacheKey = $"PLD{subject}{pldName}";
            return await _cacheWrapper.GetFromCacheAsync(cacheKey, async () =>
            {
                using (var graphQlClient = await GetAuthenticatedGraphQlClientAsync("perfLevelDescriptors"))
                {
                    var result = await graphQlClient.PostQueryAsync(query);
                    if (result.Errors.Length > 0)
                    {
                        Logger.Error(ExceptionMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                        throw new Exception("Call to the 'perfLevelDescriptors' endpoint of the GraphQL API returned errors.");
                    }

                    return result.GetDataFieldAs<PerformanceLevelDescriptor>("performanceLevelDescriptor");
                }
            });
        }

        public async Task<PerformanceLevelStatement> MakePerformanceLevelStatementCallAsync(string query, string subject, string pldName, int? pldLevel)
        {
            var cacheKey = $"PLS{subject}{pldName}{pldLevel}";
            return await _cacheWrapper.GetFromCacheAsync(cacheKey, async () =>
            {
                using (var graphQlClient = await GetAuthenticatedGraphQlClientAsync("perfLevelDescriptors"))
                {
                    var result = await graphQlClient.PostQueryAsync(query);
                    if (result.Errors.Length > 0)
                    {
                        Logger.Error(ExceptionMessage(graphQlClient.EndPoint.OriginalString, query, result.Errors.First().Message));
                        throw new Exception("Call to the 'perfLevelDescriptors' endpoint of the GraphQL API returned errors.");
                    }

                    return result.GetDataFieldAs<PerformanceLevelStatement>("performanceLvlStmt");
                }
            });
        }

        private async Task<GraphQLClient> GetAuthenticatedGraphQlClientAsync(string url)
        {
            var authenticationModel = await GetAuthenticationModelAsync();

            var graphQlClient = new GraphQLClient($"{_apiEndpoint}{url}");
            graphQlClient.DefaultRequestHeaders.Add("Authorization", authenticationModel.access_token);
            graphQlClient.DefaultRequestHeaders.Add("authCurrentDateTime", authenticationModel.auth_current_date_time);

            return graphQlClient;
        }

        private async Task<AuthenticationModel> GetAuthenticationModelAsync()
        {
            const string key = "AdaptiveApiClient.GetAuthenticationTokenAsync";
            return await _cacheWrapper.GetFromCacheAsync(key, async () =>
            {
                var obj = new
                {
                    client_id = ConfigSettings.AdaptiveDashboard.Auth.ClientId,
                    client_secret = ConfigSettings.AdaptiveDashboard.Auth.ClientSecret
                };

                try
                {
                    return await ConfigSettings.AdaptiveDashboard.Auth.Url.PostJsonAsync(obj).ReceiveJson<AuthenticationModel>();
                }
                catch (FlurlHttpTimeoutException)
                {
                    Logger.Error(FlurlHttpTimeoutMessage(ConfigSettings.AdaptiveDashboard.Auth.Url, obj));
                    throw;
                }
                catch (FlurlHttpException ex)
                {
                    Logger.Error(FlurlHttpExceptionMessage(ConfigSettings.AdaptiveDashboard.Auth.Url, obj, ex.Message, ex.Call.HttpStatus));
                    throw;
                }
            });
        }

        private string FlurlHttpTimeoutMessage(string endpoint, object parameters)
        {
            return $"Authentication Client :: Timeout :: Endpoint: {endpoint} :: Parameters: {JsonConvert.SerializeObject(parameters, Formatting.Indented)}";
        }

        private string FlurlHttpExceptionMessage(string endpoint, object parameters, string message, HttpStatusCode? httpStatus)
        {
            return $"Authentication Client :: Exception :: Endpoint: {endpoint} :: Parameters: {JsonConvert.SerializeObject(parameters, Formatting.Indented)} :: Message: {message} :: HTTP Status: {httpStatus}";
        }

        private string ExceptionMessage(string url, string query, string message)
        {
            return $"GraphQL Client :: Exception :: url: {url} :: query: {query} :: Message: {message}";
        }

        private string NoTestEventsMessage(string url, string query, string message)
        {
            return $"GraphQL Client :: No Test Events :: url: {url} :: query: {query} :: Message: {message}";
        }

        private string NoDataMessage(string url, string query, string message)
        {
            return $"GraphQL Client :: No Data :: url: {url} :: query: {query} :: Message: {message}";
        }
    }
}