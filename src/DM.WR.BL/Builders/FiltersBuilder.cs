using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.Dashboard;
using DM.WR.Models.Types;

namespace DM.WR.BL.Builders
{
    public class FiltersBuilder : IFiltersBuilder
    {
        private readonly IDbClient _dbClient;

        private readonly FiltersMapper _mapper;
        private readonly IMapper _typesMapper;

        private const string ReportCode = "ISSRD";

        public Filter InvalidFilter { get; private set; }

        public FiltersBuilder(IDbClient dbClient)
        {
            _mapper = new FiltersMapper();
            _typesMapper = new DbTypesMapper();

            _dbClient = dbClient;
        }

        public FilterPanel BuildPanel(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            var dbCustomerInfo = _typesMapper.Map<CustomerInfo, DbCustomerInfo>(userData.CurrentCustomerInfo);
            var newPanel = new FilterPanel();

            // ----- Assessments ----- //
            {
                var currentFilter = currentPanel.GetFilterByType(FilterType.Assessment);
                if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                {
                    var newFilter = _mapper.MapAssessments(userData.Assessments, currentFilter, out Assessment selectedAssessment, SetInvalidFilter);
                    newPanel.AddFilter(newFilter);
                    newPanel.Assessment = selectedAssessment;
                }
                else
                {
                    newPanel.AddFilter(currentFilter);
                    newPanel.Assessment = currentPanel.Assessment;
                }
            }

            // ----- Test Admins ----- //
            {
                var currentFilter = currentPanel.GetFilterByType(FilterType.TestAdministrationDate);
                if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                {
                    var testAdmins = _dbClient.GetTestAdmins(dbCustomerInfo, newPanel.AssessmentValue, newPanel.AssessmentCode.ToString(), userData.ContractInstances);
                    var newFilter = _mapper.MapTestAdmins(testAdmins, currentFilter, SetInvalidFilter);
                    newPanel.AddFilter(newFilter);
                    newPanel.TestAdministrations = testAdmins;
                    newPanel.ScoreSetId = _dbClient.GetTestAdminScoreset(dbCustomerInfo, newPanel.AssessmentValue, newPanel.TestAdminValue).CustScoresetId;
                    newPanel.ScoringOptions = _typesMapper.Map<DbScoringOptions, ScoringOptions>(_dbClient.GetCustomerScoringOptions(newPanel.ScoreSetId, newPanel.AssessmentValue));
                }
                else
                {
                    newPanel.AddFilter(currentFilter);
                    newPanel.TestAdministrations = currentPanel.TestAdministrations;
                    newPanel.ScoreSetId = currentPanel.ScoreSetId;
                    newPanel.ScoringOptions = currentPanel.ScoringOptions;
                }
            }

            // ----- Grade Level ----- 
            {
                var currentFilter = currentPanel.GetFilterByType(FilterType.GradeLevel);
                if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                {
                    var gradeLevels = _dbClient.GetGradeLevels(dbCustomerInfo, newPanel.AssessmentValue, newPanel.TestAdminValue, newPanel.ScoreSetId, ReportCode);
                    var newFilter = _mapper.MapGradeLevels(gradeLevels, currentFilter, SetInvalidFilter);
                    newPanel.AddFilter(newFilter);
                }
                else
                {
                    newPanel.AddFilter(currentFilter);
                }
            }

            // ----- Subtest ----- 
            {
                var currentFilter = currentPanel.GetFilterByType(FilterType.Subtest);
                if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                {
                    var subtests = SubtestsLookup(newPanel.GetFilterByType(FilterType.Assessment));
                    if (subtests.Count > 0)
                    {
                        var newFilter = _mapper.MapSubtests(subtests, currentFilter, SetInvalidFilter);
                        newPanel.AddFilter(newFilter);
                    }
                }
                else
                {
                    newPanel.AddFilter(currentFilter);
                }
            }

            // ----- Location (single dropdown) ----- 
            {
                if (currentPanel.LocationsPath == null)
                    currentPanel.AddToLocationsPath(dbCustomerInfo.NodeId, dbCustomerInfo.NodeType, dbCustomerInfo.NodeName);

                var currentFilter = (LocationsFilter)currentPanel.GetFilterByType(FilterType.Location);
                if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                {
                    var nodeId = currentPanel.NodeId;
                    var nodeType = currentPanel.NodeType;

                    Filter newFilter;
                    if (nodeType == "CLASS")
                    {
                        var students = _dbClient.GetStudents(newPanel.ScoreSetId, nodeId, nodeType, newPanel.AssessmentValue, Convert.ToInt32(newPanel.GetSelectedValuesStringOf(FilterType.GradeLevel)), newPanel.ScoringOptions.AccountabilityFlag);
                        newFilter = _mapper.MapStudents(students, SetInvalidFilter);
                    }
                    else
                    {
                        var locations = _dbClient.GetChildLocations(nodeId, nodeType, dbCustomerInfo, newPanel.AssessmentValue, Convert.ToInt32(newPanel.Level), newPanel.TestAdminValue, newPanel.ScoreSetId, false);
                        newFilter = _mapper.MapLocations(locations, SetInvalidFilter);
                    }

                    newPanel.AddFilter(newFilter);
                }
                else
                {
                    newPanel.AddFilter(currentFilter);
                }

                newPanel.LocationsPath = currentPanel.LocationsPath;
            }

            // ----- Population Filters ----- 
            {
                var currentFilter = currentPanel.GetFilterByType(FilterType.PopulationFilters);
                if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                {
                    var populationFilters = _dbClient.GetDisaggregation(newPanel.ScoringOptions.GroupsetId, new List<int> { currentPanel.NodeId }, currentPanel.NodeType, newPanel.AssessmentValue, newPanel.TestAdminValue, Convert.ToInt32(newPanel.GetSelectedValuesStringOf(FilterType.GradeLevel)), newPanel.ScoreSetId, dbCustomerInfo);
                    var newFilter = _mapper.MapPopulationFilters(populationFilters, currentFilter, SetInvalidFilter);
                    newPanel.AddFilter(newFilter);
                }
                else
                {
                    newPanel.AddFilter(currentFilter);
                }
            }

            return newPanel;
        }


        private void SetInvalidFilter(Filter filter)
        {
            if (InvalidFilter != null)
                return;

            InvalidFilter = filter;
            InvalidFilter.DisplayName = filter.Type.ToString();
        }

        private List<string> SubtestsLookup(Filter assessments)
        {
            switch (assessments.SelectedValues.First())
            {
                case "ISSMATH":
                    return new List<string> { "'Maths'", "'Comput'", "'MatTotC'" };
                case "ISSREAD":
                    return new List<string> { "'RdgTot'", "'Vocab'", "'RdgTotL'" };
                case "ISSSCI":
                    return new List<string> { "'Science'" };
                default:
                    return new List<string>();
            }
        }
    }
}