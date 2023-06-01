using DM.WR.BL.Managers;
using DM.WR.Models.Config;
using DM.WR.Models.IowaFlex;
using HandyStuff;
using NLog;
using System.Collections.Generic;
using System.Linq;
using FilterType = DM.WR.Models.IowaFlex.FilterType;
using PiledFilterItem = DM.WR.Models.IowaFlex.PiledFilterItem;

namespace DM.WR.BL.Builders
{
    public interface IGraphQlQueryStringBuilder
    {
        string BuildFiltersQueryString(IowaFlexFilterPanel filterPanel, FilterType changedType, string userId);
        string BuildTestScoresQueryString(IowaFlexFilterPanel filterPanel, string userId, string performanceBand, string domainId, string domainLevel, string isCogat);
        string BuildDomainsQueryString(IowaFlexFilterPanel filterPanel, string domainId, string isCogat, string userId);
        string BuildRosterQueryString(IowaFlexFilterPanel filterPanel, string userId);
        string BuildStudentRosterQueryString(IowaFlexFilterPanel filterPanel, string userId, string performanceBand = "-1", string domainId = "-1", string domainLevel = "-1");
        string BuildProfileNarrativeQueryString(IowaFlexFilterPanel filterPanel, string studentId);
        string BuildProfileNarrativeLookupQueryString(string subject, string grade);
        string BuildTrendAnalysisQueryString(IowaFlexFilterPanel filterPanel, string userId);
        string BuildGainAnalysisQueryString(IowaFlexFilterPanel filterPanel, string testEventsIds, string userId);
        string BuildLongitudinalLocationRosterQueryString(IowaFlexFilterPanel filterPanel, string testEventsIds, string userId);
        string BuildLongitudinalStudentRosterQueryString(IowaFlexFilterPanel filterPanel, string testEventsIds, string userId);
        string BuildStandardScoreRangeQueryString(string grade, string subject);
        string BuildStandardScoreRangeQueryString(IowaFlexFilterPanel filterPanel);
        string BuildPerformanceScoresKto1QueryString(IowaFlexFilterPanel filterPanel, string userId);
        string BuildPerformanceDonutsKto1QueryString(IowaFlexFilterPanel filterPanel, string pldName, int? pldLevel, string userId);
        string BuildRosterKto1QueryString(IowaFlexFilterPanel filterPanel, string pldName, int? pldLevel, string userId);
        string BuildProfileNarrativeKto1QueryString(IowaFlexFilterPanel filterPanel, string studentId);
        string BuildPerformanceLevelDescriptorKto1QueryString(string subject, string pldName);
        string BuildPerformanceLevelStatementKto1QueryString(string subject, string pldName, int? pldLevel);
        string BuildDifferentiatedReportKto1QueryString(IowaFlexFilterPanel filterPanel, string userId);
        string BuildPerformanceLevelMatrixQueryString(IowaFlexFilterPanel filterPanel, string userId, string contentType, string contentName, string performanceBand, string domainId, string domainLevel);
        string BuildCogatRosterQueryString(IowaFlexFilterPanel filterPanel, int? performanceBand, int? domainId, int? domainLevel, int? cogatAbility, string cogatScore, string contentName, bool isStudentLevel, string userId);
    }

    public class GraphQlQueryStringBuilder : IGraphQlQueryStringBuilder
    {
        private readonly ISessionManager _sessionManager;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GraphQlQueryStringBuilder(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public string BuildFiltersQueryString(IowaFlexFilterPanel filterPanel, FilterType changedType, string userId)
        {
            var parameters = new List<string>();
            var gradeParameters = "";
            var query = new List<string>();

            var nodeLevel = filterPanel.BreadCrumbs == null || filterPanel.BreadCrumbs.Count < 2 ? filterPanel.RootNodes.First().NodeType : filterPanel.BreadCrumbs.Last().NodeType;
            var nodeIds = filterPanel.BreadCrumbs == null || filterPanel.BreadCrumbs.Count < 2 ? string.Join(",", filterPanel.RootNodes.Select(n => n.NodeId)) : filterPanel.BreadCrumbs.Last().NodeId.ToString();

            if (changedType >= FilterType.TestEvent)
            {
                parameters.Add($"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}");

                if (filterPanel.IsCogat)
                    parameters.Add($"isCogat: true");
            }
            else
                query.Add("testEventId testEventName subject isDefault");


            if (changedType >= FilterType.Grade)
            {
                parameters.Add($"selectedGrade:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}");
                gradeParameters = $"(grades:[{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}])";
            }

            query.Add("grades { name }");

            if (changedType >= FilterType.ParentLocations)
                parameters.Add($"{NodeParameterName(nodeLevel)}:[{nodeIds}]");


            if (changedType >= FilterType.ChildLocations)
            {
                var childLevel = ((LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations)).LocationNodeType.ToLower();
                parameters.Add($"{NodeParameterName(childLevel)}:[{filterPanel.GetSelectedValuesStringOf(FilterType.ChildLocations)}]");
            }

            query.Add($"parentLocations {gradeParameters} {{ id level name childLocations {{ id level name }} }}");
            query.Add($"populations {gradeParameters} {{ key values }}");

            var paramsString = parameters.Count > 0 ? $"({string.Join(",", parameters)})" : "";

            var result = $"{{ user(userId:{userId},locationIds:[{nodeIds}],level:\"{nodeLevel}\") {{ level testEvents{paramsString} {{ {string.Join(" ", query)} }} }} }}";

            LogQuery("Filters", result);

            return result;
        }

        public string BuildTestScoresQueryString(IowaFlexFilterPanel filterPanel, string userId, string performanceBand, string domainId, string domainLevel, string isCogat)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);

            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);
            var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);

            var testEventParams = $"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}, {NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}], selectedGrade:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}";

            var paramsList = new List<string>
            {
                $"grades:[{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}]",
                $"{NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}]",
                $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]"
            };

            if (!string.IsNullOrEmpty(isCogat))
                paramsList.Add($"isCogat: true");

            if (filterPanel.GetSelectedValuesOf(FilterType.PopulationFilters).Any())
                paramsList.Add(BuildPopuplationFiltersParams(filterPanel));

            if (performanceBand != "-1")
                paramsList.Add($"performanceBand:{performanceBand}");

            if (domainId != "-1" && domainLevel != "-1")
                paramsList.Add($"domainFilter:{{ id:{domainId}, performanceLevel:{domainLevel} }}");

            var testScoresParams = string.Join(",", paramsList);

            var result = $"{{ user({userParams}){{ level testEvents({testEventParams}) {{ isLongitudinal isCogat testScore ({testScoresParams}) {{ id subject standardScore scores {{ id type value performanceBands {{ id lower upper nOfStudents percent name npr standardScore }} }} }} }} }} }}";

            LogQuery("Test Scores", result);

            return result;
        }

        public string BuildDomainsQueryString(IowaFlexFilterPanel filterPanel, string domainId, string isCogat, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);

            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);
            var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);

            var testEventParams = $"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}, {NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}]";

            var paramsList = new List<string>
            {
                $"grades:[{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}]",
                $"{NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}]",
                $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]"
            };

            if (!string.IsNullOrEmpty(isCogat))
                paramsList.Add($"isCogat: true");

            if (filterPanel.GetSelectedValuesOf(FilterType.PopulationFilters).Any())
                paramsList.Add(BuildPopuplationFiltersParams(filterPanel));

            if (domainId != null)
                paramsList.Add($"performanceBand:{domainId}");

            var domainsParams = string.Join(",", paramsList);

            var result = $"{{ user({userParams}) {{ level testEvents({testEventParams}) {{ domainScores ({domainsParams}) {{ id desc performanceLevels {{ id desc nOfStudents percent }} }} }} }} }}";

            LogQuery("Domains", result);

            return result;
        }

        public string BuildRosterQueryString(IowaFlexFilterPanel filterPanel, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);

            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);

            var testEventParams = $"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}, {NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}]";

            var locationLevel = ((LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations)).LocationNodeType;
            var paramsList = new List<string>
            {
                $"grades:[{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}]",
                $"{NodeParameterName(locationLevel)}:[{filterPanel.GetSelectedValuesStringOf(FilterType.ChildLocations)}]",
                $"level:\"{((LocationsFilter) filterPanel.GetFilterByType(FilterType.ChildLocations)).LocationNodeType}\"",
                //"page:0",
                //$"size:{_pageSize}",
                "sortOrder:\"asc\"",
                "sortField:\"firstName\""
            };

            if (filterPanel.GetSelectedValuesOf(FilterType.PopulationFilters).Any())
                paramsList.Add(BuildPopuplationFiltersParams(filterPanel));

            var rosterParams = string.Join(",", paramsList);

            var result = $"{{ user({userParams}) {{ level testEvents({testEventParams}) {{ locationRoster({rosterParams}) {{ locations {{ id name averageScore nprAverageScore domainScores {{ desc name guid id score performanceLevels {{ desc id nOfStudents percent score }} }} }} }} }} }} }}";

            LogQuery("Location Roster", result);

            return result;
        }

        public string BuildStudentRosterQueryString(IowaFlexFilterPanel filterPanel, string userId, string performanceBand = "-1", string domainId = "-1", string domainLevel = "-1")
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);

            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);
            var testEventId = filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent);

            var testEventParams = $"testEventId:{testEventId}, {NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}]";

            var locationLevel = ((LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations)).LocationNodeType;

            var paramsList = new List<string>
            {
                $"testEventId:{testEventId}",
                $"grades:[{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}]",
                $"{NodeParameterName(locationLevel)}:[{filterPanel.GetSelectedValuesStringOf(FilterType.ChildLocations)}]",
                //"page:0",
                //$"size:{_pageSize}",
                "sortOrder:\"asc\"",
                "sortField:\"firstName\""
            };

            if (filterPanel.GetSelectedValuesOf(FilterType.PopulationFilters).Any())
                paramsList.Add(BuildPopuplationFiltersParams(filterPanel));

            if (performanceBand != "-1")
                paramsList.Add($"performanceBand:{performanceBand}");

            if (domainId != "-1" && domainLevel != "-1")
                paramsList.Add($"domainFilter:{{ id:{domainId}, performanceLevel:{domainLevel} }}");

            var rosterParams = string.Join(",", paramsList);

            var result = $"{{ user({userParams}) {{ level testEvents ({testEventParams}) {{ studentRoster({rosterParams}) {{ students {{ id externalId name {{ firstName lastName }} testScore npr domainScores {{ desc name guid id score performanceLevels {{ desc id nOfStudents percent score }} }} }} }} }} }} }}";

            LogQuery("Student Roster", result);

            return result;
        }

        public string BuildProfileNarrativeQueryString(IowaFlexFilterPanel filterPanel, string studentId)
        {
            var testEventId = filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent);
            var subject = filterPanel.GetSubject(true);

            var result = $"{{ student(userId: {studentId}) {{ userId externalId username name {{ firstName lastName middleName }} currentTestEvent(testEventId: {testEventId}, userId: {studentId}) {{ testEventId testEventName subject subjectFullName testDate grade {{ name }} testScore {{ id standardScore scores {{ id type value performanceBands {{ id lower upper nOfStudents percent }} }} }} domainScores {{ id desc guid performanceLevels {{ id desc nOfStudents percent }} }} district {{ id level name childLocations {{ id level name childLocations {{ id level name }} }} }} }}  testEvents(userId: {studentId}, subject: {subject}) {{ testEventId testEventName testDate grade {{ name }} subject testScore {{ standardScore }} }} }} }}";

            LogQuery("Profile Narrative", result);

            return result;
        }

        public string BuildProfileNarrativeLookupQueryString(string subject, string grade)
        {
            var result = $"{{ subjectGradeDomains(sub: \"{subject}\", grade: \"{grade}\") {{ subject {{ name subjectAbbrev }} domains {{ id name text guid shortName }} performanceLevels {{ id name text }} }} }}";

            LogQuery("Profile Narrative Lookup", result);

            return result;
        }

        public string BuildTrendAnalysisQueryString(IowaFlexFilterPanel filterPanel, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildLongitudinalTestEventsParams(filterPanel, true);

            return $"{{ user({userParams}) {{ testEvents({testEventsParams}) {{ longitudinalTestEvents {{  testEventId  testEventName testEventDate  isDefault  performanceBands {{  id  name  percent  }} }} }} }} }}";
        }

        public string BuildGainAnalysisQueryString(IowaFlexFilterPanel filterPanel, string testEventsIds, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildLongitudinalTestEventsParams(filterPanel, true);

            //Special case per Prathima's request - 6/24/2020 - Only when on class level, send building ID since the API doesn't know what building this class is in.
            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);
            if (parentLocations.LocationNodeType == "class" && filterPanel.BreadCrumbs.Any(bc => bc.NodeType == "building"))
                testEventsParams = $"{testEventsParams}, buildings:[{filterPanel.BreadCrumbs.First(bc => bc.NodeType == "building").NodeId}]";

            var gainAnalysisParamsList = new List<string>
            {
                $"testEventIds: [{testEventsIds}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}"
            };
            var gainAnalysisParams = string.Join(",", gainAnalysisParamsList);

            return $"{{ user({userParams}) {{ testEvents({testEventsParams}) {{ testEventId isDefault gainAnalysis({gainAnalysisParams}) {{ longitudinalGain {{ testEventId testEventName districtAvgSS schoolAvgSS classAvgSS }} }} }} }} }}";
        }

        public string BuildLongitudinalLocationRosterQueryString(IowaFlexFilterPanel filterPanel, string testEventsIds, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildLongitudinalTestEventsParams(filterPanel, true);

            var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);
            var locationsParamsList = new List<string>
            {
                $"testEventIds: [{testEventsIds}]",
                $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };
            var locationsParams = string.Join(",", locationsParamsList);

            return $"{{ user({userParams}) {{ testEvents({testEventsParams}) {{ testEventId testEventName locations({locationsParams}) {{ id locationName testScores {{ testEventId testEventName standardScore }} }} }} }} }}";
        }

        public string BuildLongitudinalStudentRosterQueryString(IowaFlexFilterPanel filterPanel, string testEventsIds, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildLongitudinalTestEventsParams(filterPanel, true);

            var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);
            var studentsParamsList = new List<string>
            {
                $"testEventIds: [{testEventsIds}]",
                $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };
            var studentsParams = string.Join(",", studentsParamsList);

            return $"{{ user({userParams}) {{ testEvents({testEventsParams}) {{ students({studentsParams}) {{ id name {{ firstName middleName lastName }} testScores {{ testEventId testEventName standardScore }} }} }} }} }}";
        }

        public string BuildStandardScoreRangeQueryString(string grade, string subject)
        {
            var g = $"\"{grade}\"";
            var s = $"\"{subject}\"";

            return $"{{ standardScoreRange(grade: {g}, subject: {s}) {{ lower upper plevel ranges {{ lower plevel upper }} }} }}";
        }

        public string BuildStandardScoreRangeQueryString(IowaFlexFilterPanel filterPanel)
        {
            var grade = filterPanel.GetSelectedValuesStringOf(FilterType.Grade);
            var subject = filterPanel.GetSubject();

            return BuildStandardScoreRangeQueryString(grade, subject);
        }

        public string BuildPerformanceScoresKto1QueryString(IowaFlexFilterPanel filterPanel, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildKto1TestEventsParams(filterPanel);

            //var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);
            var rosterCardParamsList = new List<string>
            {
                $"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
               // $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };

            var rosterCardParams = $"({string.Join(",", rosterCardParamsList)})";

            return $"{{ user({userParams}) {{ level testEvents({testEventsParams}) {{ subject isLongitudinal isCogat rosterCard{rosterCardParams} {{ performanceScoreGraph {{ subject totalCount PLDStages {{ percent PLDStage PLDStageNum studentCount }} }} }} }} }} }}";
        }

        public string BuildPerformanceDonutsKto1QueryString(IowaFlexFilterPanel filterPanel, string pldStage, int? pldLevel, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildKto1TestEventsParams(filterPanel);

            //var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);
            var rosterCardParamsList = new List<string>
            {
                $"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
             //  $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };

            if (!string.IsNullOrEmpty(pldStage))
                rosterCardParamsList.Add($"PLDStage:\"{pldStage}\"");

            if (pldLevel != null)
                rosterCardParamsList.Add($"PLDLevel:{pldLevel}");

            var rosterCardParams = $"({string.Join(",", rosterCardParamsList)})";

            return $"{{ user({userParams}) {{ level testEvents({testEventsParams}) {{ rosterCard{rosterCardParams} {{ performanceLevelDonuts {{ PLDLevel PLDStage PLDStageNum percent studentCount }} }} }} }} }}";
        }

        public string BuildRosterKto1QueryString(IowaFlexFilterPanel filterPanel, string pldName, int? pldLevel, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildKto1TestEventsParams(filterPanel);

            //var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);
            var rosterCardParamsList = new List<string>
            {
                $"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
               // $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };

            if (!string.IsNullOrEmpty(pldName))
                rosterCardParamsList.Add($"PLDStage:\"{pldName}\"");

            if (pldLevel != null)
                rosterCardParamsList.Add($"PLDLevel:{pldLevel}");

            var rosterCardParams = $"({string.Join(",", rosterCardParamsList)})";

            return $"{{ user({userParams}) {{ level testEvents({testEventsParams}) {{ rosterCard{rosterCardParams} {{ roster {{ PLDLevel PLDStage PLDStageNum studentCount percent rosterList {{ id level name externalStudentId PLDLevel PLDStage PLDStageNum preEmerging emerging beginning transitioning independent }} }} }} }} }} }}";
        }

        public string BuildProfileNarrativeKto1QueryString(IowaFlexFilterPanel filterPanel, string studentId)
        {
            var testEventId = filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent);

            var result = $"{{ student(userId: {studentId}) {{ userId externalId username name {{ firstName lastName middleName }} currentTestEvent(testEventId: {testEventId}, userId: {studentId}) {{ testEventId testEventName subject subjectFullName testDate grade {{ name }} pldName pldLevel district {{ id level name childLocations {{ id level name childLocations {{ id level name }} }} }} }} }} }}";

            LogQuery("Profile Narrative K to 1", result);

            return result;
        }

        public string BuildPerformanceLevelDescriptorKto1QueryString(string subject, string pldName)
        {
            var result = $"{{ performanceLevelDescriptor(subject: \"{subject}\", pldName: \"{pldName}\") {{ pldName pldAltName pldDesc }} }}";

            LogQuery("Performance Level Descriptor K to 1", result);

            return result;
        }

        public string BuildPerformanceLevelStatementKto1QueryString(string subject, string pldName, int? pldLevel)
        {
            var result = $"{{ performanceLvlStmt(subject: \"{subject}\", pldName: \"{pldName}\", pldLvl: {pldLevel}) {{ pldLvlName canStmt readyStmt practiceStmt iCanDesc needDesc readyDesc }} }} ";

            LogQuery("Performance Level Statement K to 1", result);

            return result;
        }

        public string BuildDifferentiatedReportKto1QueryString(IowaFlexFilterPanel filterPanel, string userId)
        {
            var rootNodeLevel = filterPanel.RootNodes.First().NodeType;
            var rootNodesIds = string.Join(",", filterPanel.RootNodes.Select(n => n.NodeId).ToList());

            var userParams = $"userId:{userId}, locationIds:[{rootNodesIds}], level:\"{rootNodeLevel}\"";

            var testEventAndReportParamsList = new List<string>
            {
                $"testEventId: {filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
                $"{NodeParameterName(rootNodeLevel)}:[{rootNodesIds}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };
            var testEventAndReportParams = string.Join(",", testEventAndReportParamsList);

            var buildingLevelFields = rootNodeLevel.ToLower() == "building" ? "classId className" : "";
            var classLevelFields = rootNodeLevel.ToLower() == "class" ? "classId className" : "";

            return $"{{ user({userParams}) {{ level testEvents({testEventAndReportParams}) {{ testEventId testEventName testEventDate k1DifferentiatedReport({testEventAndReportParams}) {{ districtId districtName buildingId buildingName grade subject {classLevelFields} studentList {{ studentId studentName studentExternalId pldLevel pldStage pldStageNum {buildingLevelFields} }} }} }} }} }}";

        }

        public string BuildPerformanceLevelMatrixQueryString(IowaFlexFilterPanel filterPanel, string userId, string contentType, string contentName, string performanceBand, string domainId, string domainLevel)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildTestEventsParams(filterPanel, false);

            var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);
            var performanceLevelParamsList = new List<string>
            {
                $"testEventId:{filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]"
                ,"isCogat:true"
            };

            if (!string.IsNullOrEmpty(contentType))
                performanceLevelParamsList.Add($"contentType:\"{contentType}\"");

            if (!string.IsNullOrEmpty(contentName))
                performanceLevelParamsList.Add($"contentName:\"{contentName}\"");

            if (!string.IsNullOrEmpty(performanceBand) && performanceBand != "-1")
                performanceLevelParamsList.Add($"performanceBand:{performanceBand}");

            if (!string.IsNullOrEmpty(domainId) && !string.IsNullOrEmpty(domainLevel) && domainId != "-1" && domainLevel != "-1")
                performanceLevelParamsList.Add($"domainFilter:{{ id:{domainId}, performanceLevel:{domainLevel} }}");

            var performanceLevelParams = $"({string.Join(",", performanceLevelParamsList)})";

            return $"{{ user({userParams}) {{ level testEvents({testEventsParams}) {{ performanceLevelMatrix{performanceLevelParams} {{ dataPoints {{ abilityAchievement studCount }} }} }} }} }}";
        }

        public string BuildCogatRosterQueryString(IowaFlexFilterPanel filterPanel, int? performanceBand, int? domainId, int? domainLevel, int? cogatAbility, string cogatScore, string contentName, bool isStudentLevel, string userId)
        {
            var userParams = BuildUserTopLevelParams(filterPanel, userId);
            var testEventsParams = BuildTestEventsParams(filterPanel, false);

            var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);
            var cogatRosterParamsList = new List<string>
            {
                $"testEventId: {filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]",
                "isCogat: true"
            };

            //if (isStudentLevel)


            if (performanceBand != null)
                cogatRosterParamsList.Add($"performanceBand: {performanceBand}");

            if (domainId != null && domainLevel != null)
                cogatRosterParamsList.Add($"domainFilter: {{id: {domainId}, performanceLevel: {domainLevel}}}");

            if (cogatAbility != null)
                cogatRosterParamsList.Add($"abilityBand: {cogatAbility}");

            if (!string.IsNullOrEmpty(cogatScore))
                cogatRosterParamsList.Add($"contentType: \"{cogatScore}\"");

            if (!string.IsNullOrEmpty(contentName))
                cogatRosterParamsList.Add($"contentName: \"{contentName}\"");

            var cogatRosterParams = string.Join(",", cogatRosterParamsList);

            return $"{{ user({userParams}) {{ level testEvents({testEventsParams}) {{ testEventId testEventName cogatRoster({cogatRosterParams}) {{ cogatRosterRecords {{ name id verbal quantitative nonVerbal compVQ compQN compVN compVQN testScore npr }} }} }} }} }}";
        }

        public void LogQuery(string elementName, string query)
        {
            var isQueryLoggingEnabled = _sessionManager.Retrieve(SessionKey.EnableQuerryLogging);

            if (isQueryLoggingEnabled != null && isQueryLoggingEnabled.ToBoolean())
                Logger.Warn($"Query Logging :: {elementName} :: query: {query}");
        }

        private string BuildUserTopLevelParams(IowaFlexFilterPanel filterPanel, string userId)
        {
            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);
            return $"userId:{userId}, locationIds:[{parentLocations.SelectedValuesString}], level:\"{parentLocations.LocationNodeType}\"";
        }

        private string BuildPopuplationFiltersParams(IowaFlexFilterPanel filterPanel)
        {
            var items = filterPanel.GetFilterByType(FilterType.PopulationFilters).Items.Where(i => i.IsSelected).Cast<PiledFilterItem>().GroupBy(i => i.PileKey);

            var popParams = new List<string>();
            foreach (var item in items)
                popParams.Add($"{{ key:\"{item.Key}\", values:[ {string.Join(",", item.Select(i => $"\"{i.Value}\""))} ] }}");

            return $"populations:[{string.Join(",", popParams)}]";
        }

        private string BuildTestEventsParams(IowaFlexFilterPanel filterPanel, bool isLongitudinal)
        {
            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);

            var testEventsParamsList = new List<string>
            {
                $"testEventId: {filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
                $"{NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };

            if (isLongitudinal)
                testEventsParamsList.Add("isLongitudinal: true");

            if (filterPanel.GetSelectedValuesOf(FilterType.PopulationFilters).Any())
                testEventsParamsList.Add(BuildPopuplationFiltersParams(filterPanel));

            return string.Join(",", testEventsParamsList);
        }

        private string BuildLongitudinalTestEventsParams(IowaFlexFilterPanel filterPanel, bool isLongitudinal)
        {
            var parentLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ParentLocations);

            var testEventsParamsList = new List<string>
            {
                $"testEventId: {filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
                $"{NodeParameterName(parentLocations.LocationNodeType)}:[{parentLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };

            if (isLongitudinal)
                testEventsParamsList.Add("isLongitudinal: true");

            if (filterPanel.GetSelectedValuesOf(FilterType.PopulationFilters).Any())
                testEventsParamsList.Add(BuildPopuplationFiltersParams(filterPanel));

            return string.Join(",", testEventsParamsList);
        }

        private string BuildKto1TestEventsParams(IowaFlexFilterPanel filterPanel)
        {
            var childLocations = (LocationsFilter)filterPanel.GetFilterByType(FilterType.ChildLocations);

            var testEventsParamsList = new List<string>
            {
                $"testEventId: {filterPanel.GetSelectedValuesStringOf(FilterType.TestEvent)}",
                $"{NodeParameterName(childLocations.LocationNodeType)}:[{childLocations.SelectedValuesString}]",
                $"grades:{filterPanel.GetSelectedValuesStringOf(FilterType.Grade, true)}",
                $"subject: {filterPanel.GetSubject(true)}"
            };

            if (filterPanel.GetSelectedValuesOf(FilterType.PopulationFilters).Any())
                testEventsParamsList.Add(BuildPopuplationFiltersParams(filterPanel));

            return string.Join(",", testEventsParamsList);
        }

        private string NodeParameterName(string level)
        {
            switch (level.ToLower())
            {
                case "district":
                    return "districts";
                case "building":
                    return "buildings";
                case "class":
                    return "classes";
                case "student":
                    return "students";
                default:
                    return $"{level} did not match.";
            }
        }
    }
}