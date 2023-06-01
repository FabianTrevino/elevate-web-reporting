using DM.WR.Data.Repository.Types;
using DM.WR.Models.CogAt;
using DM.WR.Models.ElevateReportingEngine;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using HandyStuff;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DM.WR.BL.Builders
{
    public class CogatFiltersMapper
    {
        private readonly BuildersHelper _helper;

        public CogatFiltersMapper()
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
                var value = $"{testAdmin.Id}_{testAdmin.NodeId}";
                var applySelection = applyDefaultSelection ? c == 0 : currentFilter.IsValueSelected(value);

                items.Add(new FilterItem
                {
                    Value = value,
                    AltValue = testAdmin.Id.ToString(),
                    Text = testAdmin.Name,
                    IsSelected = applySelection
                });
            }

            if (!items.Any(o => o.IsSelected))
            {
                items.First().IsSelected = true;
                setInvalidFilter(currentFilter);
            }

            return new Filter(FilterType.TestEvent)
            {
                DisplayName = "Test Assignment",
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
                    AltValue = gradeLevel.Battery,
                    Text = gradeLevel.GradeText,
                    IsSelected = applySelection,  
                    TestGroupId = gradeLevel.testGroupId
                });
            }

            if (!items.Any(o => o.IsSelected))
            {
                items.First().IsSelected = true;

                setInvalidFilter(currentFilter);
            }

            return new Filter(FilterType.Grade)
            {
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = "Grade/Level",
                Items = items
            };
        }

        public Filter MapGradeLevelsBackground(List<GradeLevel> gradeLevels, Filter currentFilter, Action<Filter> setInvalidFilter)
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
                    AltValue = gradeLevel.Battery,
                    Text = gradeLevel.GradeText,
                    IsSelected = applySelection,
                    TestGroupId = gradeLevel.testGroupId
                });
            }

            if (!items.Any(o => o.IsSelected))
            {
                items.First().IsSelected = true;

                setInvalidFilter(currentFilter);
            }

            return new Filter(FilterType.Grade)
            {
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = "Grades",
                Items = items
            };
        }

        public Filter MapContentScope(List<DbContentScope> contentAreas, Filter currentFilter, Action<Filter> setInvalidFilter)
        {
            var items = new List<FilterItem>();
            var applyDefaultSelection = currentFilter == null || currentFilter.Items.Count != contentAreas.Count;

            foreach (var contentArea in contentAreas)
            {
                var applySelection = applyDefaultSelection ? contentArea.IsDefault : currentFilter.IsValueSelected(contentArea.Acronym);

                items.Add(new FilterItem
                {
                    Value = contentArea.Acronym,
                    Text = contentArea.SubtestName,
                    AltValue =  contentArea.contentID.ToString(),
                    IsSelected = applySelection
                });
            }

            return new Filter(FilterType.Content)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = "Content",
                Items = items,
                HasSelectAll = true,
                HasSelectNone = true,
                MinToSelect = 1,
                MaxToSelect = items.Count
            };
        }

        public Filter MapChildLocations(List<LocationPayload> locations, Filter currentFilter, FilterType filterType, FilterPanel newPanel)
        {
            var items = new List<LocationFilterItem>();
            var itemsDictionary = new SortedList<string, List<LocationFilterItem>>();
            var applyDefaultSelection = currentFilter == null;

            var parentLocationsFilter = newPanel.LastLocationFilter ?? new Filter { Items = new List<LocationFilterItem> { new LocationFilterItem { AltValue = locations.FirstOrDefault(x=>x.nodeTypeDisplay == "DISTRICT").id, IsSelected = true } }.Cast<FilterItem>().ToList() };

            foreach (var location in locations)
            {
                if (!parentLocationsFilter.IsAltValueSelected(location.parentId))
                    continue;

                var applySelection = applyDefaultSelection || currentFilter.IsValueSelected(location.id) || parentLocationsFilter.IsAltValueSelected(location.parentId);
                var parentName = parentLocationsFilter.Items.Cast<LocationFilterItem>().FirstOrDefault(i => i.AltValue == location.parentId)?.Text ?? location.parentId;

                if (!itemsDictionary.ContainsKey(parentName))
                    itemsDictionary.Add(parentName, new List<LocationFilterItem>());

                itemsDictionary[parentName].Add(new LocationFilterItem
                {
                    Value = location.id.ToString(),
                    AltValue = location.id,
                    Text = location.nodeName,
                    ParentLocationId = location.parentId,
                    ParentLocationName = parentName,
                    DistrictIds = location.parentId,
                    IsSelected = applySelection
                });
            }

            foreach (var itemsList in itemsDictionary)
            {
                itemsList.Value.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));
                items.AddRange(itemsList.Value);
            }

            //special case when the first locations filter is suppressed, the first VISIBLE locations filter must be sorted by item names only (not parents) 
            if (newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden))
                items.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));

            var isFirstVisibleFilter = newPanel.LastLocationFilter == null || newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden);
            var isHiddenFilter = locations.Any(l => l.nodeTypeDisplay == "SUPPRESS");
            var sectionString = "Section";
            return new Filter(filterType)
            {
                InputControl = isFirstVisibleFilter || isHiddenFilter ? OptionsInputControl.MultiSelect : OptionsInputControl.PiledSingleSelect,
                DisplayName = filterType == FilterType.Class ? sectionString : $"{filterType}",
                Items = items.Cast<FilterItem>().ToList(),
                HasSelectNone = true,
                MinToSelect = 1,
                MaxToSelect = items.Count,
                IsLocation = true,
                IsHidden = isHiddenFilter
            };
        }




        public Filter StaffAndTeacherMapChildLocations(List<LocationPayload> locations, Filter currentFilter, FilterType filterType, FilterPanel newPanel)
        {
            var items = new List<LocationFilterItem>();
            var itemsDictionary = new SortedList<string, List<LocationFilterItem>>();
            var applyDefaultSelection = currentFilter == null;

            var parentLocationsFilter = newPanel.LastLocationFilter ?? new Filter { Items = new List<LocationFilterItem> { new LocationFilterItem { AltValue = locations.FirstOrDefault(x => x.nodeTypeDisplay == "DISTRICT").id, IsSelected = true } }.Cast<FilterItem>().ToList() };

            foreach (var location in locations)
            {
                if (!parentLocationsFilter.IsAltValueSelected(location.parentId))
                    continue;

                var applySelection = applyDefaultSelection || currentFilter.IsValueSelected(location.id) || parentLocationsFilter.IsAltValueSelected(location.parentId);
                var parentName = parentLocationsFilter.Items.Cast<LocationFilterItem>().FirstOrDefault(i => i.AltValue == location.parentId)?.Text ?? location.parentId;

                if (!itemsDictionary.ContainsKey(parentName))
                    itemsDictionary.Add(parentName, new List<LocationFilterItem>());

                itemsDictionary[parentName].Add(new LocationFilterItem
                {
                    Value = location.id.ToString(),
                    AltValue = location.id,
                    Text = location.nodeName,
                    ParentLocationId = location.parentId,
                    ParentLocationName = parentName,
                    DistrictIds = location.parentId,
                    IsSelected = applySelection
                }); 
            }

            foreach (var itemsList in itemsDictionary)
            {
                itemsList.Value.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));
                items.AddRange(itemsList.Value);
            }

            //special case when the first locations filter is suppressed, the first VISIBLE locations filter must be sorted by item names only (not parents) 
            if (newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden))
                items.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));

            var isFirstVisibleFilter = newPanel.LastLocationFilter == null || newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden);
            var isHiddenFilter = locations.Any(l => l.nodeTypeDisplay == "SUPPRESS");
            var sectionString = "Section";
            return new Filter(filterType)
            {
                InputControl = isFirstVisibleFilter || isHiddenFilter ? OptionsInputControl.MultiSelect : OptionsInputControl.PiledSingleSelect,
                DisplayName = filterType == FilterType.Class ? sectionString : $"{filterType}",
                Items = items.Cast<FilterItem>().ToList(),
                HasSelectNone = true,
                MinToSelect = 1,
                MaxToSelect = items.Count,
                IsLocation = true,
                IsHidden = isHiddenFilter
            };
        }

        public Filter MapChildLocationsForbackground(List<LocationPayload> locations, Filter currentFilter, FilterType filterType, FilterPanel newPanel, FilterPanel currentPanel)
        {
            var items = new List<LocationFilterItem>();
            var itemsDictionary = new SortedList<string, List<LocationFilterItem>>();
            var applyDefaultSelection = currentFilter == null;

            var parentLocationsFilter = newPanel.LastLocationFilter ?? new Filter { Items = new List<LocationFilterItem> { new LocationFilterItem { AltValue = locations.FirstOrDefault(x => x.nodeTypeDisplay == "DISTRICT").id, IsSelected = true } }.Cast<FilterItem>().ToList() };

            foreach (var location in locations)
            {
                if (!parentLocationsFilter.IsAltValueSelected(location.parentId))
                    continue;

                var applySelection = applyDefaultSelection || currentFilter.IsValueSelected(location.id) || parentLocationsFilter.IsAltValueSelected(location.parentId);
                var parentName = parentLocationsFilter.Items.Cast<LocationFilterItem>().FirstOrDefault(i => i.AltValue == location.parentId)?.Text ?? location.parentId;

                if (!itemsDictionary.ContainsKey(parentName))
                    itemsDictionary.Add(parentName, new List<LocationFilterItem>());

                itemsDictionary[parentName].Add(new LocationFilterItem
                {
                    Value = location.id.ToString(),
                    AltValue = location.id,
                    Text = location.nodeName,
                    ParentLocationId = location.parentId,
                    ParentLocationName = parentName,
                    DistrictIds = location.parentId,
                    GradeId = location.grade,
                    IsSelected = applySelection
                }) ;
            }

            foreach (var itemsList in itemsDictionary)
            {
                itemsList.Value.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));
                items.AddRange(itemsList.Value);
            }

            //special case when the first locations filter is suppressed, the first VISIBLE locations filter must be sorted by item names only (not parents) 
            if (newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden))
                items.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));

            var isFirstVisibleFilter = newPanel.LastLocationFilter == null || newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden);
            var isHiddenFilter = locations.Any(l => l.nodeTypeDisplay == "SUPPRESS");
            var sectionString = "Section";
            return new Filter(filterType)
            {
                InputControl = isFirstVisibleFilter || isHiddenFilter ? OptionsInputControl.MultiSelect : OptionsInputControl.PiledSingleSelect,
                DisplayName = filterType == FilterType.Class ? sectionString : $"{filterType}",
                Items = items.Cast<FilterItem>().ToList(),
                HasSelectNone = true,
                MinToSelect = 1,
                MaxToSelect = items.Count,
                IsLocation = true,
                IsHidden = isHiddenFilter
            };
        }

        public Filter StaffAndTeacherMapChildLocationsForbackground(List<LocationPayload> locations, Filter currentFilter, FilterType filterType, FilterPanel newPanel ,FilterPanel currentPanel)
        {
            var items = new List<LocationFilterItem>();
            var itemsDictionary = new SortedList<string, List<LocationFilterItem>>();
            var applyDefaultSelection = currentFilter == null;

            var parentLocationsFilter = newPanel.LastLocationFilter ?? new Filter { Items = new List<LocationFilterItem> { new LocationFilterItem { AltValue = currentPanel.FirstLocationFilter.Items.Select(x=>x.DistrictIds).ToList()[0], IsSelected = true } }.Cast<FilterItem>().ToList() };

            foreach (var location in locations)
            {
                if (!parentLocationsFilter.IsAltValueSelected(location.parentId))
                    continue;

                var applySelection = applyDefaultSelection || currentFilter.IsValueSelected(location.id) || parentLocationsFilter.IsAltValueSelected(location.parentId);
                var parentName = parentLocationsFilter.Items.Cast<LocationFilterItem>().FirstOrDefault(i => i.AltValue == location.parentId)?.Text ?? location.parentId;

                if (!itemsDictionary.ContainsKey(parentName))
                    itemsDictionary.Add(parentName, new List<LocationFilterItem>());

                itemsDictionary[parentName].Add(new LocationFilterItem
                {
                    Value = location.id.ToString(),
                    AltValue = location.id,
                    Text = location.nodeName,
                    ParentLocationId = location.parentId,
                    ParentLocationName = parentName,
                    DistrictIds = location.parentId,
                    GradeId = location.grade,
                    IsSelected = applySelection
                });
            }

            foreach (var itemsList in itemsDictionary)
            {
                itemsList.Value.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));
                items.AddRange(itemsList.Value);
            }

            //special case when the first locations filter is suppressed, the first VISIBLE locations filter must be sorted by item names only (not parents) 
            if (newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden))
                items.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));

            var isFirstVisibleFilter = newPanel.LastLocationFilter == null || newPanel.LastLocationFilter != null && newPanel.LocationFilters.All(f => f.IsHidden);
            var isHiddenFilter = locations.Any(l => l.nodeTypeDisplay == "SUPPRESS");
            var sectionString = "Section";
            return new Filter(filterType)
            {
                InputControl = isFirstVisibleFilter || isHiddenFilter ? OptionsInputControl.MultiSelect : OptionsInputControl.PiledSingleSelect,
                DisplayName = filterType == FilterType.Class ? sectionString : $"{filterType}",
                Items = items.Cast<FilterItem>().ToList(),
                HasSelectNone = true,
                MinToSelect = 1,
                MaxToSelect = items.Count,
                IsLocation = true,
                IsHidden = isHiddenFilter
            };
        }

        //public Filter MapChildLocations(List<Location> locations, Filter currentFilter, FilterType filterType, List<string> selectedParentGuids,/* List<string> userSelectedParentGuids,*/ List<Location> allLocations)
        //{
        //    var items = new List<LocationFilterItem>();
        //    var itemsDictionary = new SortedList<string, List<LocationFilterItem>>();
        //    var applyDefaultSelection = currentFilter == null;

        //    foreach (var location in locations)
        //    {
        //        if (!selectedParentGuids.Contains(location.ParentGuid))
        //            continue;

        //        var applySelection = applyDefaultSelection || currentFilter.IsValueSelected(location.Id);// || userSelectedParentGuids.Contains(location.ParentGuid);
        //        var parentName = allLocations?.First(l => l.Guid == location.ParentGuid)?.NodeName ?? location.ParentGuid;

        //        if (!itemsDictionary.ContainsKey(parentName))
        //            itemsDictionary.Add(parentName, new List<LocationFilterItem>());

        //        itemsDictionary[parentName].Add(new LocationFilterItem
        //        {
        //            Value = location.Id.ToString(),
        //            AltValue = location.Guid,
        //            Text = location.NodeName,
        //            ParentLocationId = location.ParentGuid,
        //            ParentLocationName = parentName,
        //            IsSelected = applySelection
        //        });
        //    }

        //    foreach (var itemsList in itemsDictionary)
        //    {
        //        itemsList.Value.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));
        //        items.AddRange(itemsList.Value);
        //    }

        //    //special case when the first locations filter is suppressed, the first VISIBLE locations filter must be sorted by item names only (not parents) 
        //    if (allLocations == null)
        //        items.Sort((i, q) => string.Compare(i.Text, q.Text, StringComparison.Ordinal));

        //    return new Filter(filterType)
        //    {
        //        InputControl = allLocations == null ? OptionsInputControl.MultiSelect : OptionsInputControl.PiledSingleSelect,
        //        DisplayName = $"{locations[0].NodeTypeDisplay.FirstCharToUpper()}",
        //        Items = items.Cast<FilterItem>().ToList(),
        //        HasSelectNone = true,
        //        MinToSelect = 1,
        //        MaxToSelect = items.Count,
        //        IsLocation = true,
        //        IsHidden = locations.Any(l => l.NodeTypeDisplay == "SUPPRESS")
        //    };
        //}

        public Filter MapStudents(List<StudentPayload> students, Filter currentFilter, FilterType changedType)
        {
            var items = new List<FilterItem>();
            var applyDefaultSelection = currentFilter == null || changedType == FilterType.TestEvent || changedType == FilterType.Grade || IsTypeLocation(changedType);

            foreach (var student in students)
            {
                var applySelection = applyDefaultSelection || currentFilter.IsValueSelected(student.Id);
                items.Add(new FilterItem
                {
                    Value = student.Id,
                    AltValue = student.TestInstanceId.ToString(),//was int converter to string
                    Text = student.Name,
                    IsSelected = applySelection
                });
            }

            return new Filter(FilterType.Student)
            {
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = "Students",
                Items = items,
                MinToSelect = 1,
                MaxToSelect = items.Count
            };
        }


        public Filter MapStudentsForBackground(List<StudentbackgroundPayload> students, Filter currentFilter)
        {
            var items = new List<FilterItem>();
          

            foreach (var student in students)
            {
                var applySelection = currentFilter.IsValueSelected(student.id);
                items.Add(new FilterItem
                {
                    Value = student.id.ToString(),
                    AltValue = student.testInstanceId.ToString(),//was int converter to string
                    GradeId = student.grade.ToString(),
                    Text = student.name,
                    ClassIds = student.classId,
                    IsSelected = applySelection
                });
            }

            return new Filter(FilterType.Student)
            {
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = "Students",
                Items = items,
                MinToSelect = 1,
                MaxToSelect = items.Count
            };
        }

        public Filter MapPopulationFilters(Dictionary<string, List<Disaggregation>> populationFilters, Filter currentFilter, Action<Filter> setInvalidFilter)
        {
            var items = new List<PiledFilterItem>();

            foreach (var pile in populationFilters)
            {
                var TPKey = GetPopulationKeyForUI(pile.Key);
                var pileKey = _helper.CreatePopulationFiltersPileKey(TPKey);
                var pileLabel = _helper.CreatePopulationFiltersPileLabel(TPKey);
                foreach (var item in pile.Value)
                {
                    if (pileKey == "ProgramList" && item.GroupValue == "Other")
                    {
                        item.GroupValue = "Other ";
                    }
                    if (pileKey == "GenderList" && item.GroupValue == "Unknown")
                    {
                        item.GroupValue = "Unknown ";
                    }
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
                DisplayName = "Population",
                Items = items.Cast<FilterItem>().ToList(),
            };
        }

        public Filter MapPopulationFiltersTeacher(Dictionary<string, List<Disaggregation>> populationFilters, Filter currentFilter, Action<Filter> setInvalidFilter, bool suppressFlag)
        {
            var items = new List<PiledFilterItem>();

            foreach (var pile in populationFilters)
            {
                var TPKey = GetPopulationKeyForUI(pile.Key);
                var pileKey = _helper.CreatePopulationFiltersPileKey(TPKey);
                var pileLabel = _helper.CreatePopulationFiltersPileLabel(TPKey);
                if (pileKey == "ProgramList" && suppressFlag == true)
                {
                    continue;
                }
                foreach (var item in pile.Value)
                {
                    if (pileKey == "ProgramList" && item.GroupValue == "Other")
                    {
                        item.GroupValue = "Other ";
                    }
                    if (pileKey == "GenderList" && item.GroupValue == "Unknown")
                    {
                        item.GroupValue = "Unknown ";
                    }
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
                DisplayName = "Population",
                Items = items.Cast<FilterItem>().ToList(),
            };
        }

        private string GetPopulationKeyForUI(string Key)
        {
            switch (Key)
            {
                case "genders":
                    {
                        return "Gender";
                    }
                case "races":
                    {
                        return "Race";
                    }
                case "ethnicities":
                    {
                        return "Ethnicity";
                    };
                case "programs":
                    {
                        return "Programs";
                    }
                case "administratorCodes":
                    {
                        return "Admin Codes";
                    }
                default:
                    return Key;
            }
        }

        private bool IsTypeLocation(FilterType changedType)
        {
            return changedType == FilterType.State ||
                   changedType == FilterType.Region ||
                   changedType == FilterType.System ||
                   changedType == FilterType.District ||
                   changedType == FilterType.Building ||
                   changedType == FilterType.Class;
        }
    }
}