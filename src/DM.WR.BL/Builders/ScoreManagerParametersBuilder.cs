using DM.WR.Models.CogAt;
using DM.WR.Models.ScoreManagerApi;
using System.Collections.Generic;
using System;
using DM.WR.Models.Types;
using Newtonsoft.Json;

namespace DM.WR.BL.Builders
{
    public class ScoreManagerParametersBuilder
    {
        private readonly CogatCommonFunctions _cogat;

        public ScoreManagerParametersBuilder()
        {
            _cogat = new CogatCommonFunctions();
        }

        public SmiApiParameters BuildCogatAllDataParams(FilterPanel filterPanel, UserData userData)
        {
            var optionalWhereClause = "";
            if (filterPanel.HasStudentsFilter)
            {
                var studentFilter = filterPanel.GetFilterByType(FilterType.Student);
                if (studentFilter.SelectedValues.Count != studentFilter.Items.Count)
                    optionalWhereClause = $"sdr_testinstance.testinstance_id IN ({studentFilter.SelectedAltValuesString})";
            }

            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    //RosterReturnMode = "DataRows",
                    LoadLPRs = _cogat.LoadLprs(filterPanel),
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = filterPanel.SelectedTestAdmin.NodeType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = _cogat.LoadLprs(filterPanel) ? filterPanel.ScoringOptions.LprNodeLevel : filterPanel.SelectedTestAdmin.NodeType, //root node
                    TestPopulationNodeIDs = _cogat.LoadLprs(filterPanel) ? filterPanel.ScoringOptions.LprNodeList : filterPanel.SelectedTestAdmin.NodeId.ToString(),
                    Accountability = filterPanel.ScoringOptions.AccountabilityFlag,
                    OptionalWhereClause = optionalWhereClause
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = CreateSubtestAcronymsParam(filterPanel.GetFilterByType(FilterType.Content).SelectedValues)
                },
                SMIFilteringParameters = new
                {
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                }
            };

            // var json = JsonConvert.SerializeObject(parameters);

            return parameters;
        }

        public SmiApiParameters BuildStudentRosterParams(FilterPanel filterPanel, UserData userData, int numRecords, int skipPages, string filter, string score, string orderType)
        {
            string optionalWhereClause = "";
            var classIdPos = filter.IndexOf("AND|S:class_id:=:", 0, StringComparison.Ordinal);
            var buildingIdPos = filter.IndexOf("AND|S:building_id:=:", 0, StringComparison.Ordinal);
            if (classIdPos != -1)
            {
                optionalWhereClause = "sdr_customerhierarchy.class_id=" + filter.Substring(classIdPos + 17);
                filter = filter.Substring(0, classIdPos - 1);
            }
            if (buildingIdPos != -1)
            {
                optionalWhereClause = "sdr_customerhierarchy.building_id=" + filter.Substring(buildingIdPos + 20);
                filter = filter.Substring(0, buildingIdPos - 1);
            }

            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    //RosterReturnMode = "DataRows",
                    TestFetchSize = numRecords,
                    SMFetchSize = numRecords,
                    //CustomSortString = "last_name, first_name, student_id, testfamily_id, sequence_no",
                    LoadLPRs = _cogat.LoadLprs(filterPanel),
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = filterPanel.SelectedTestAdmin.NodeType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = _cogat.LoadLprs(filterPanel) ? filterPanel.ScoringOptions.LprNodeLevel : filterPanel.SelectedTestAdmin.NodeType, //root node
                    TestPopulationNodeIDs = _cogat.LoadLprs(filterPanel) ? filterPanel.ScoringOptions.LprNodeList : filterPanel.SelectedTestAdmin.NodeId.ToString(), //root node
                    Accountability = filterPanel.ScoringOptions.AccountabilityFlag,
                    OptionalWhereClause = optionalWhereClause
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = CreateSubtestAcronymsParam(filterPanel.GetFilterByType(FilterType.Content).SelectedValues),
                    RankingSubtestAcronym = score,
                    RankingScoreDirection = orderType
                },
                SMIFilteringParameters = new
                {
                    FiltersString = filter,
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                }
            };

            return parameters;
        }

        public SmiApiParameters BuildAgeStaninesParams(FilterPanel filterPanel, UserData userData)
        {
            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = filterPanel.SelectedTestAdmin.NodeType,
                    InnerGroup = filterPanel.SelectedTestAdmin.NodeType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = filterPanel.SelectedTestAdmin.NodeType,
                    TestPopulationNodeIDs = filterPanel.SelectedTestAdmin.NodeId.ToString()
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = CreateSubtestAcronymsParam(filterPanel.GetFilterByType(FilterType.Content).SelectedValues)
                },
                SMIFilteringParameters = new
                {
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                }
            };

            return parameters;
        }

        public SmiApiParameters BuildAbilityProfilesParams(FilterPanel filterPanel, UserData userData)
        {
            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = filterPanel.SelectedTestAdmin.NodeType,
                    InnerGroup = filterPanel.SelectedTestAdmin.NodeType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = filterPanel.SelectedTestAdmin.NodeType,
                    TestPopulationNodeIDs = filterPanel.SelectedTestAdmin.NodeId.ToString()
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = CreateSubtestAcronymsParam(filterPanel.GetFilterByType(FilterType.Content).SelectedValues)
                },
                SMIFilteringParameters = new
                {
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                }
            };

            return parameters;
        }

        public SmiApiParameters BuildRecordsCountParams(FilterPanel filterPanel, UserData userData, int testFetchSize, int smFetchSize)
        {
            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    //RosterReturnMode = "NumberOfEntityBatches",
                    TestFetchSize = testFetchSize,
                    SMFetchSize = smFetchSize,
                    //CustomSortString = "last_name, first_name, student_id, testfamily_id, sequence_no"
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = filterPanel.SelectedTestAdmin.NodeType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = filterPanel.SelectedTestAdmin.NodeType,
                    TestPopulationNodeIDs = filterPanel.SelectedTestAdmin.NodeId.ToString(),
                    Accountability = filterPanel.ScoringOptions.AccountabilityFlag,
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = CreateSubtestAcronymsParam(filterPanel.GetFilterByType(FilterType.Content).SelectedValues)
                },
                SMIFilteringParameters = new
                {
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                }
            };

            return parameters;
        }

        public SmiApiParameters BuildCutScoreParams(FilterPanel filterPanel, UserData userData)
        {
            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = filterPanel.SelectedTestAdmin.NodeType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = filterPanel.SelectedTestAdmin.NodeType,
                    TestPopulationNodeIDs = filterPanel.SelectedTestAdmin.NodeId.ToString(),
                    Accountability = filterPanel.ScoringOptions.AccountabilityFlag
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = CreateSubtestAcronymsParam(filterPanel.GetFilterByType(FilterType.Content).SelectedValues)
                },
                SMIFilteringParameters = new
                {
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                },
                SMIGroupParameters = new { }
            };

            return parameters;
        }

        public SmiApiParameters BuildCutScoreParams(FilterPanel filterPanel, UserData userData, string groupingType, string filter)
        {
            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = groupingType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = filterPanel.SelectedTestAdmin.NodeType,
                    TestPopulationNodeIDs = filterPanel.SelectedTestAdmin.NodeId.ToString()
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = CreateSubtestAcronymsParam(filterPanel.GetFilterByType(FilterType.Content).SelectedValues)
                },
                SMIFilteringParameters = new
                {
                    FiltersString = filter,
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                },
                SMIGroupParameters = new { }
            };

            return parameters;
        }

        public SmiApiParameters BuildGroupTotalParams(FilterPanel filterPanel, UserData userData)
        {
            var parameters = new SmiApiParameters
            {
                SMIGeneralProcessingParameters = new SMIGeneralProcessingParameters
                {
                    LoggingSessionId = userData.LogSessionId,
                    LoggingCustomerId = userData.CurrentCustomerInfo.CustomerId,
                    LoggingUserLocationGuid = userData.CurrentGuid,
                    LoggingOutputSystemName = "DASHBOARD"
                },
                SMIBaseParameters = new SMIBaseParameters
                {
                    GradeLevelIDs = filterPanel.GradeLevel,
                    CustomerScoresetIDs = filterPanel.ScoreSetId.ToString(),
                    OuterGroup = filterPanel.SelectedTestAdmin.NodeType,
                    InnerGroup = filterPanel.SelectedTestAdmin.NodeType,
                    ReportPopulationNodeType = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeType : filterPanel.LastLocationFilter.Type.ToString().ToUpper(), // most child node
                    ReportPopulationNodeIDs = filterPanel.LastLocationFilter == null ? filterPanel.SelectedTestAdmin.NodeId.ToString() : filterPanel.LastLocationFilter.SelectedValuesString, // most child node
                    TestPopulationNodeType = _cogat.LoadLprs(filterPanel) ? filterPanel.ScoringOptions.LprNodeLevel : filterPanel.SelectedTestAdmin.NodeType, //root node
                    TestPopulationNodeIDs = _cogat.LoadLprs(filterPanel) ? filterPanel.ScoringOptions.LprNodeList : filterPanel.SelectedTestAdmin.NodeId.ToString() //root node
                },
                SMISubtestParameters = new SMISubtestParameters
                {
                    SubtestAcronyms = string.Join(",", filterPanel.GetFilterByType(FilterType.Content).SelectedValues)
                },
                SMIFilteringParameters = new
                {
                    GenderList = filterPanel.PopulationGenderValue,
                    EthnicityList = filterPanel.PopulationEthnicityValue,
                    ProgramList = filterPanel.PopulationProgramValue,
                    AdminValueList = filterPanel.PopulationAdminValue
                }
            };

            return parameters;
        }

        private string CreateSubtestAcronymsParam(List<string> subtestAcronyms)
        {
            if (!subtestAcronyms.Contains("'CompVQN'"))
                subtestAcronyms.Add("'CompVQN'");

            return string.Join(",", subtestAcronyms);
        }

        private string GroupTotalInnerGroup(FilterPanel filterPanel)
        {
            var locationFilters = filterPanel.LocationFilters;
            if (locationFilters.Count >= 2)
                return locationFilters[locationFilters.Count - 2].Type.ToString().ToUpper();

            return filterPanel.SelectedTestAdmin.NodeType;
        }
    }
}