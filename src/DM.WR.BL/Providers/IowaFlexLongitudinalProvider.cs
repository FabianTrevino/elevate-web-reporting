using DM.WR.BL.Builders;
using DM.WR.BL.Managers;
using DM.WR.GraphQlClient;
using DM.WR.Models.Config;
using DM.WR.Models.GraphqlClient.UserEndPoint;
using DM.WR.Models.IowaFlex;
using DM.WR.Models.IowaFlex.ViewModels;
using DM.WR.Models.Types;
using HandyStuff;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DM.WR.BL.Providers
{
    public interface IIowaFlexLongitudinalProvider
    {
        IowaFlexLongitudinalViewModel BuildPageViewModel(string enableQueryLogging);
        IowaFlexLongitudinalFiltersViewModel GetFilters(string appPath);
        Task UpdateFiltersAsync(string filterTypeNumber, List<string> values);
        void ResetFilters();
        void PersistLongitudinalFilters(bool persist);
        Task GoToRootNodeAsync();
        Task DrillDownLocationsPathAsync(LocationNode node);
        Task DrillUpLocationsPathAsync(LocationNode node);
        Task<JsonResponseModel> GetTrendAnalysisAsync();
        Task<JsonResponseModel> GetGainsAnalysisAsync(string testEventIds);
        Task<LongitudinalRosterModel> GetRosterAsync(string testEventsIds, string appPath);
    }

    public class IowaFlexLongitudinalProvider : IIowaFlexLongitudinalProvider
    {
        private readonly IApiClient _adaptiveApiClient;
        //private readonly IIowaFlexFiltersBuilder _adaptiveFiltersBuilder;
        //private readonly IDashboardIowaFlexProviderBuilder _dashboardIowaFlexProviderBuilder;
        private readonly IGraphQlQueryStringBuilder _graphQlQueryStringBuilder;
        private readonly ISessionManager _sessionManager;

        private readonly UserData _userData;

        private readonly CommonProviderFunctions _commonFunctions;
        private readonly IIowaFlexCommonProviderFunctions _commonFlexFunctions;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IowaFlexLongitudinalProvider(IApiClient adaptiveApiClient, /* IIowaFlexFiltersBuilder adaptiveFiltersBuilder, IDashboardIowaFlexProviderBuilder dashboardIowaFlexProviderBuilder,*/ IGraphQlQueryStringBuilder graphQlQueryStringBuilder, ISessionManager sessionManager, IUserDataManager userDataManager, IIowaFlexCommonProviderFunctions commonFlexFunctions)
        {
            _adaptiveApiClient = adaptiveApiClient;
            //_adaptiveFiltersBuilder = adaptiveFiltersBuilder;
            //_dashboardIowaFlexProviderBuilder = dashboardIowaFlexProviderBuilder;
            _graphQlQueryStringBuilder = graphQlQueryStringBuilder;
            _sessionManager = sessionManager;

            _userData = userDataManager.GetUserData();

            _commonFunctions = new CommonProviderFunctions();
            _commonFlexFunctions = commonFlexFunctions;
        }

        public IowaFlexLongitudinalViewModel BuildPageViewModel(string enableQueryLogging)
        {
            var originalFilterPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexFilters);
            var filterPanel = originalFilterPanel.Copy();

            _sessionManager.Store(filterPanel, SessionKey.IowaFlexLongitudinalFilters);

            return new IowaFlexLongitudinalViewModel { IsAdaptive = true, IsProd = ConfigSettings.IsEnvironmentProd, IsDemo = _userData.IsDemo, IsGuidUser = _commonFunctions.IsGuidUser(_userData) };
        }

        public IowaFlexLongitudinalFiltersViewModel GetFilters(string appPath)
        {
            var filterPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);
            if (filterPanel == null)
            {
                var originalFilterPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexFilters);
                filterPanel = originalFilterPanel.Copy();

                _sessionManager.Store(filterPanel, SessionKey.IowaFlexLongitudinalFilters);
            }

            return new IowaFlexLongitudinalFiltersViewModel
            {
                Filters = filterPanel.GetAllFilters(),
                LocationsBreadCrumbs = _commonFlexFunctions.MakeBreadCrumbs(filterPanel, $"{appPath}/IowaFlexLongitudinal"),
                GraphqlQuery = filterPanel.GraphqlQuery
            };
        }

        public async Task UpdateFiltersAsync(string filterTypeNumber, List<string> values)
        {
            var currentPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);
            Enum.TryParse(filterTypeNumber, out FilterType filterType);

            currentPanel = _commonFlexFunctions.ChangeFiltersSelection(currentPanel, filterType, values);

            var newPanel = await _commonFlexFunctions.RecreateFiltersAsync(currentPanel, filterType, _userData.UserId);

            if (filterType <= FilterType.ParentLocations)
                newPanel.BreadCrumbs = new List<LocationNode> { _commonFlexFunctions.MakeRootBreadCrumb(newPanel.GetFilterByType(FilterType.ParentLocations)) };

            _sessionManager.Store(newPanel, SessionKey.IowaFlexLongitudinalFilters);
        }

        public void ResetFilters()
        {
            _sessionManager.Delete(SessionKey.IowaFlexLongitudinalFilters);
        }

        public void PersistLongitudinalFilters(bool persist)
        {
            if (persist)
            {
                var longitudinalPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);
                _sessionManager.Store(longitudinalPanel, SessionKey.IowaFlexFilters);
            }
            _sessionManager.Delete(SessionKey.IowaFlexLongitudinalFilters);
        }

        public async Task GoToRootNodeAsync()
        {
            var filterPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);

            filterPanel.BreadCrumbs = null;

            var newPanel = await _commonFlexFunctions.RecreateFiltersAsync(filterPanel, FilterType.Grade, _userData.UserId);
            newPanel.BreadCrumbs = new List<LocationNode> { _commonFlexFunctions.MakeRootBreadCrumb(newPanel.GetFilterByType(FilterType.ParentLocations)) };

            _sessionManager.Store(newPanel, SessionKey.IowaFlexLongitudinalFilters);
        }

        public async Task DrillDownLocationsPathAsync(LocationNode node)
        {
            var currentPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);
            currentPanel = _commonFlexFunctions.DrillDownLocationsPathAsync(currentPanel, node);

            var newPanel = await _commonFlexFunctions.RecreateFiltersAsync(currentPanel, FilterType.ParentLocations, _userData.UserId);

            _sessionManager.Store(newPanel, SessionKey.IowaFlexLongitudinalFilters);
        }

        public async Task DrillUpLocationsPathAsync(LocationNode node)
        {
            var currentPanel = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);
            currentPanel = _commonFlexFunctions.DrillUpLocationsPathAsync(currentPanel, node);

            var newPanel = await _commonFlexFunctions.RecreateFiltersAsync(currentPanel, FilterType.ParentLocations, _userData.UserId);

            _sessionManager.Store(newPanel, SessionKey.IowaFlexLongitudinalFilters);
        }

        public async Task<JsonResponseModel> GetTrendAnalysisAsync()
        {
            var currentFilters = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);

            var query = _graphQlQueryStringBuilder.BuildTrendAnalysisQueryString(currentFilters, _userData.UserId);
            var user = await _adaptiveApiClient.MakeUserCallAsync(query);

            var model = new List<TrendAnalysisModel>();
            foreach (var testEvent in user.TestEvents)
            {
                foreach (var longitudinalTestEvent in testEvent.LongitudinalTestEvents)
                {
                    var trendAnalysis = new TrendAnalysisModel
                    {
                        Id = longitudinalTestEvent.TestEventId,
                        Name = longitudinalTestEvent.TestEventName,
                        IsDefault = longitudinalTestEvent.IsDefault,
                        Date = longitudinalTestEvent.TestEventDate.ToShortDateString(),
                        Values = new List<TrendAnalysisPerformanceBand>()
                    };

                    foreach (var performanceBand in longitudinalTestEvent.PerformanceBands)
                    {
                        trendAnalysis.Values.Add(new TrendAnalysisPerformanceBand
                        {
                            Caption = performanceBand.Name,
                            Number = "NUM FROM API",
                            Percent = performanceBand.Percent,
                            Range = performanceBand.Id,
                            RangeBand = "0:0"
                        });
                    }
                    model.Add(trendAnalysis);
                }
            }

            return new JsonResponseModel { Model = model, GraphQlQuery = query };
        }

        public async Task<JsonResponseModel> GetGainsAnalysisAsync(string testEventIds)
        {
            var currentFilters = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);

            var query = _graphQlQueryStringBuilder.BuildGainAnalysisQueryString(currentFilters, testEventIds, _userData.UserId);
            var user = await _adaptiveApiClient.MakeUserCallAsync(query);

            if (user.TestEvents == null || !user.TestEvents.Any() || user.TestEvents.First().GainAnalysis == null || !user.TestEvents.First().GainAnalysis.LongitudinalGain.Any())
                throw new Exception("No Data in Longitudinal GetStudentRoster");

            var bandsLookUpQuery = _graphQlQueryStringBuilder.BuildStandardScoreRangeQueryString(currentFilters);
            var bandsRanges = await _adaptiveApiClient.MakeBandsLookupCallAsync(bandsLookUpQuery, currentFilters.GetSubject(), currentFilters.GetSelectedValuesStringOf(FilterType.Grade));

            //ranges
            var bands = _commonFlexFunctions.BuildBands(bandsRanges);

            var splitIds = testEventIds.Split(',');
            var values = new List<GainsAnalysisValue>();
            foreach (var id in splitIds)
            {
                var longitudinalGain = user.TestEvents[0].GainAnalysis.LongitudinalGain.FirstOrDefault(te => te.TestEventId == Convert.ToInt32(id));

                if (longitudinalGain == null)
                    continue;

                values.Add(new GainsAnalysisValue
                {
                    Id = longitudinalGain.TestEventId,
                    Title = longitudinalGain.TestEventName,
                    DistrictAverage = longitudinalGain.DistrictAvgSs,
                    BuildingAverage = longitudinalGain.SchoolAvgSs,
                    ClassAverage = longitudinalGain.ClassAvgSs
                });
            }

            var model = new GainsAnalysisModel
            {
                Bands = bands,
                Values = values
            };

            return new JsonResponseModel { Model = model, GraphQlQuery = query };
        }

        public async Task<LongitudinalRosterModel> GetRosterAsync(string testEventsIds, string appPath)
        {
            var currentFilters = (IowaFlexFilterPanel)_sessionManager.Retrieve(SessionKey.IowaFlexLongitudinalFilters);

            var query = currentFilters.IsChildLocationStudent ?
                _graphQlQueryStringBuilder.BuildLongitudinalStudentRosterQueryString(currentFilters, testEventsIds, _userData.UserId) :
                _graphQlQueryStringBuilder.BuildLongitudinalLocationRosterQueryString(currentFilters, testEventsIds, _userData.UserId);
            var user = await _adaptiveApiClient.MakeUserCallAsync(query);

            if (user.TestEvents == null || !user.TestEvents.Any())
                throw new Exception($"No Data in Longitudinal GetRoster. Query: {query}");

            var rosterLevel = currentFilters.IsChildLocationStudent ? "Student" : ((LocationsFilter)currentFilters.GetFilterByType(FilterType.ChildLocations)).LocationNodeType;

            var bandsLookUpQuery = _graphQlQueryStringBuilder.BuildStandardScoreRangeQueryString(currentFilters);
            var bandsRanges = await _adaptiveApiClient.MakeBandsLookupCallAsync(bandsLookUpQuery, currentFilters.GetSubject(), currentFilters.GetSelectedValuesStringOf(FilterType.Grade));

            //ranges
            var bands = _commonFlexFunctions.BuildBands(bandsRanges);

            //columns
            var columnTitles = new List<LongitudinalRosterColumn>
            {
                new LongitudinalRosterColumn
                {
                    Title = currentFilters.IsChildLocationStudent ? "Student Name" : $"{rosterLevel.FirstCharToUpper()} Comparison",
                    TitleFull = currentFilters.IsChildLocationStudent ? "Student Name" :  $"{rosterLevel.FirstCharToUpper()} Comparison",
                    Field = "node_name"
                }
            };

            var nodes = currentFilters.IsChildLocationStudent ? user.TestEvents.First().Students : user.TestEvents.First().Locations;
            var splitIds = testEventsIds.Split(',');
            var ssGuide = new Dictionary<string, int> { { "SS0", -1 }, { "SS1", -1 }, { "SS2", -1 } };

            for (int c = 0; c < splitIds.Length; ++c)
            {
                var testEventId = splitIds[c];
                ssGuide[$"SS{c}"] = Convert.ToInt32(testEventId);

                var foundTestEvent = false;
                foreach (var node in nodes)
                {
                    foreach (var testScore in node.TestScores)
                    {
                        if (testScore.TestEventId == ssGuide[$"SS{c}"])
                        {
                            columnTitles.Add(new LongitudinalRosterColumn
                            {
                                TestEventId = testEventId,
                                Title = string.IsNullOrEmpty(testScore.TestEventName) ? "???" : testScore.TestEventName,
                                TitleFull = testScore.TestEventName,
                                Field = $"SS{c}"
                            });
                            foundTestEvent = true;
                            break;
                        }
                    }

                    if (foundTestEvent)
                        break;
                }

                if (!foundTestEvent)
                    columnTitles.Add(new LongitudinalRosterColumn
                    {
                        TestEventId = testEventId,
                        Title = "???",
                        TitleFull = "???",
                        Field = $"SS{c}"
                    });
            }

            columnTitles.Add(new LongitudinalRosterColumn
            {
                Title = "Gain",
                TitleFull = "Gain",
                Field = "gain"
            });

            //values
            var values = new List<LongitudinalRosterValue>();

            foreach (var node in nodes)
            {
                var firstScore = node.TestScores.FirstOrDefault()?.StandardScore;
                var lastScore = node.TestScores.LastOrDefault()?.StandardScore;

                var gain = firstScore != null && lastScore != null ? lastScore - firstScore : null;
                var link = currentFilters.IsChildLocationStudent ? "" : $"{appPath}/IowaFlexLongitudinal/DrillDownLocations?id={node.Id}&name={node.LocationName}&type={rosterLevel}";

                values.Add(new LongitudinalRosterValue
                {
                    Link = link,
                    NodeId = node.Id,
                    Gain = gain,
                    NodeName = currentFilters.IsChildLocationStudent ? $"{node.Name.LastName}, {node.Name.FirstName}" : node.LocationName,
                    Ss0 = node.TestScores.FirstOrDefault(ts => ts.TestEventId == ssGuide["SS0"])?.StandardScore,
                    Ss1 = node.TestScores.FirstOrDefault(ts => ts.TestEventId == ssGuide["SS1"])?.StandardScore,
                    Ss2 = node.TestScores.FirstOrDefault(ts => ts.TestEventId == ssGuide["SS2"])?.StandardScore
                });
            }

            return new LongitudinalRosterModel
            {
                RosterType = currentFilters.IsChildLocationStudent ? "students" : "compare",
                RosterLevel = rosterLevel,
                Bands = bands,
                Columns = columnTitles,
                Values = values,
                GraphQlQuery = ConfigSettings.IsEnvironmentProd ? "" : query
            };
        }
    }
}