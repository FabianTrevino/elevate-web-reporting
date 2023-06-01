using DM.WR.Data.Repository.Types;
using DM.WR.Models.Dashboard;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DM.WR.BL.Builders
{
    public class FiltersMapper
    {
        private readonly BuildersHelper _helper;

        public FiltersMapper()
        {
            _helper = new BuildersHelper();
        }

        public Filter MapAssessments(List<Assessment> assessments, Filter currentFilter, out Assessment selectedAssessment, Action<Filter> setInvalidFilter)
        {
            selectedAssessment = null;
            var applyDefaultSelection = currentFilter == null;

            var items = new List<FilterItem>();
            for (int c = 0; c < assessments.Count; ++c)
            {
                var assessment = assessments[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentFilter.IsValueSelected(assessment.TestFamilyGroupCode);
                var item = new FilterItem
                {
                    Value = assessment.TestFamilyGroupCode,
                    Text = assessment.TestFamilyDesc,
                    IsSelected = applySelection
                };

                if (applySelection)
                    selectedAssessment = assessment;

                items.Add(item);
            }

            if (!items.Any(o => o.IsSelected))
            {
                items.First().IsSelected = true;
                selectedAssessment = assessments.FirstOrDefault();

                setInvalidFilter(currentFilter);
            }

            return new Filter(FilterType.Assessment)
            {
                DisplayName = "Assessment",
                Items = items
            };
        }

        public Filter MapTestAdmins(List<TestAdmin> testAdmins, Filter currentFilter, Action<Filter> setInvalidFilter)
        {
            var items = new List<FilterItem>();
            var applyDefaultSelection = currentFilter == null;

            for (int c = 0; c < testAdmins.Count; ++c)
            {
                var testAdmin = testAdmins[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentFilter.IsValueSelected(testAdmin.Id);

                items.Add(new FilterItem
                {
                    Value = testAdmin.Id.ToString(),
                    Text = $"{Convert.ToDateTime(testAdmin.Date):MM/dd/yyyy} - {testAdmin.Name}",
                    IsSelected = applySelection
                });
            }

            if (!items.Any(o => o.IsSelected))
            {
                items.First().IsSelected = true;
                setInvalidFilter(currentFilter);
            }

            return new Filter(FilterType.TestAdministrationDate)
            {
                DisplayName = "Test Administration Date",
                Items = items
            };
        }

        public Filter MapGradeLevels(List<GradeLevel> gradeLevels, Filter currentFilter, Action<Filter> setInvalidFilter)
        {
            var items = new List<FilterItem>();
            var applyDefaultSelection = currentFilter == null;

            for (int c = 0; c < gradeLevels.Count; ++c)
            {
                var gradeLevel = gradeLevels[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentFilter.IsValueSelected(gradeLevel.Level);
                items.Add(new FilterItem
                {
                    Value = gradeLevel.Level.ToString(),
                    AltValue = gradeLevel.Grade,
                    Text = gradeLevel.GradeText,
                    IsSelected = applySelection //gradeLevel.GradeText.Contains("Grade 3")
                });
            }

            if (!items.Any(o => o.IsSelected))
            {
                items.First().IsSelected = true;

                setInvalidFilter(currentFilter);
            }

            return new Filter(FilterType.GradeLevel)
            {
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = "Grade",
                Items = items
            };
        }

        public LocationsFilter MapLocations(List<Location> locations, Action<Filter> setInvalidFilter)
        {
            var items = new List<FilterItem>();

            foreach (var location in locations)
            {
                items.Add(new FilterItem
                {
                    Value = location.Id.ToString(),
                    AltValue = location.NodeType,
                    Text = location.ChildNodeName,
                    IsSelected = true
                });
            }

            return new LocationsFilter(FilterType.Location)
            {
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = $"{locations[0].NodeTypeDisplay}S",
                Items = items,
                LocationNodeType = locations[0].NodeType,
                MinToSelect = 0,
                MaxToSelect = items.Count
            };
        }

        public LocationsFilter MapStudents(List<Student> students, Action<Filter> setInvalidGroup)
        {
            var items = new List<FilterItem>();

            foreach (var student in students)
            {
                items.Add(new FilterItem
                {
                    Value = student.Id,
                    AltValue = student.TestInstanceId,
                    Text = student.Name,
                    IsSelected = true
                });
            }

            return new LocationsFilter(FilterType.Location)
            {
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = "Students",
                Items = items,
                LocationNodeType = "STUDENT",
                MinToSelect = 0,
                MaxToSelect = items.Count
            };
        }

        public Filter MapSubtests(List<string> subtests, Filter currentFilter, Action<Filter> setInvalidFilter)
        {
            var items = new List<FilterItem>();
            var applyDefaultSelection = currentFilter == null;

            for (int c = 0; c < subtests.Count; ++c)
            {
                var subtest = subtests[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentFilter.IsValueSelected(subtest);

                items.Add(new FilterItem
                {
                    Value = subtest,
                    Text = subtest.Replace("'", ""),
                    IsSelected = applySelection
                });
            }

            if (!items.Any(o => o.IsSelected))
            {
                items.First().IsSelected = true;
                setInvalidFilter(currentFilter);
            }

            return new Filter(FilterType.Subtest)
            {
                DisplayName = "Subtests",
                Items = items
            };
        }

        public Filter MapPopulationFilters(Dictionary<string, List<Disaggregation>> populationFilters, Filter currentFilter, Action<Filter> setInvalidFilter)
        {
            var items = new List<PiledFilterItem>();

            foreach (var pile in populationFilters)
            {
                var pileKey = _helper.CreatePopulationFiltersPileKey(pile.Key);
                var pileLabel = _helper.CreatePopulationFiltersPileLabel(pile.Key);

                foreach (var item in pile.Value)
                {
                    var applySelection = currentFilter != null && currentFilter.IsValueSelected(item.GroupKey);
                    items.Add(new PiledFilterItem
                    {
                        Value = item.GroupKey,
                        Text = item.GroupValue,
                        IsSelected = applySelection,
                        PileKey = pileKey,
                        PileLabel = pileLabel
                    });
                }
            }

            return new Filter(FilterType.PopulationFilters)
            {
                InputControl = OptionsInputControl.PiledSingleSelect,
                DisplayName = "Population Filters",
                Items = items.Cast<FilterItem>().ToList()
            };
        }
    }
}