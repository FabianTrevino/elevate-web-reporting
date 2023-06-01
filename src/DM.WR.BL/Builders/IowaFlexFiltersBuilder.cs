using DM.WR.Models.GraphqlClient.UserEndPoint;
using DM.WR.Models.IowaFlex;
using DM.WR.Models.Options;
using System.Collections.Generic;
using System.Linq;
using DM.WR.Models.Config;

namespace DM.WR.BL.Builders
{
    public interface IIowaFlexFiltersBuilder
    {
        IowaFlexFilterPanel BuildFilters(IowaFlexFilterPanel currentData, User apiResponse, FilterType changedType);
    }

    public class IowaFlexFiltersBuilder : IIowaFlexFiltersBuilder
    {
        private IowaFlexFilterPanel _newPanel;
        private IowaFlexFilterPanel _currentPanel;

        private FilterType _changedType;

        public IowaFlexFilterPanel BuildFilters(IowaFlexFilterPanel currentPanel, User apiResponse, FilterType changedType)
        {
            _newPanel = new IowaFlexFilterPanel { RootNodes = currentPanel?.RootNodes, BreadCrumbs = currentPanel?.BreadCrumbs };
            _newPanel.LastUpdatedFilterType = currentPanel.LastUpdatedFilterType;
            _currentPanel = currentPanel;
            _changedType = changedType;

            BuildTestEvents(apiResponse.TestEvents);
            BuildGrades(apiResponse.TestEvents.First().Grades);
            BuildParentLocations(apiResponse.TestEvents.First().ParentLocations);

            var selecteParentLocationIds = _newPanel.GetSelectedValuesOf(FilterType.ParentLocations);
            var childLocations = new List<ChildLocation>();
            if (apiResponse.TestEvents.First().ParentLocations != null)
                foreach (var parentLocation in apiResponse.TestEvents.First().ParentLocations)
                {
                    if (!selecteParentLocationIds.Contains(parentLocation.Id.ToString())) continue;

                    foreach (var childLocation in parentLocation.ChildLocations)
                    {
                        childLocation.ParentId = parentLocation.Id.ToString();
                        childLocation.ParentName = parentLocation.Name;
                        childLocations.Add(childLocation);
                    }
                }

            BuildChildLocations(childLocations);
            BuildPopulationFilters(apiResponse.TestEvents.First().Populations);

            return _newPanel;
        }

        private void BuildTestEvents(List<TestEvent> testEvents)
        {
            if (_changedType == FilterType._INTERNAL_FIRST_)
            {
                var items = new List<FilterItem>();
                for (int c = 0; c < testEvents.Count; ++c)
                {
                    var testEvent = testEvents[c];

                    items.Add(new TestEventFilterItem
                    {
                        Value = testEvent.Id.ToString(),
                        Text = testEvent.Name,
                        Subject = testEvent.Subject,
                        //IsLongitudinal = testEvent.IsLongitudinal,
                        IsSelected = c == 0
                    });
                }

                _newPanel.AddFilter(new Filter(FilterType.TestEvent)
                {
                    InputControl = OptionsInputControl.SingleSelect,
                    DisplayName = "TEST EVENT",
                    Items = items
                });
            }
            else
            {
                _newPanel.AddFilter(_currentPanel.GetFilterByType(FilterType.TestEvent));
            }
        }

        private void BuildGrades(List<Grade> grades)
        {
            var applyDefaultSelection = _changedType < FilterType.Grade;
            var selectedIds = _currentPanel?.GetSelectedValuesOf(FilterType.Grade) ?? new List<string>();
            var items = new List<FilterItem>();

            for (int c = 0; c < grades.Count; ++c)
            {
                var grade = grades[c];

                //TODO:  Remove when K to 1 feature goes live
                var kTo1Grades = new List<string> { "K", "k", "0", "1" };
                if (!ConfigSettings.IsIowaFlexKto1FeatureEnabled && kTo1Grades.Contains(grade.Name))
                    continue;

                var applySelection = applyDefaultSelection ? c == 0 : selectedIds.Contains(grade.Name);

                items.Add(new FilterItem
                {
                    Value = grade.Name,
                    Text = grade.Name,
                    IsSelected = applySelection
                });
            }

            _newPanel.AddFilter(new Filter(FilterType.Grade)
            {
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = "GRADE",
                Items = items
            });
        }

        private void BuildParentLocations(List<ParentLocation> locations)
        {
            if (_changedType < FilterType.ParentLocations)
            {
                var items = new List<FilterItem>();

                foreach (var location in locations)
                {
                    var item = new FilterItem
                    {
                        Value = location.Id.ToString(),
                        Text = location.Name,
                        IsSelected = true
                    };

                    items.Add(item);
                }

                _newPanel.AddFilter(new LocationsFilter(FilterType.ParentLocations)
                {
                    InputControl = OptionsInputControl.MultiSelect,
                    DisplayName = locations.First().Level.ToUpper(),
                    Items = items,
                    MinToSelect = 1,
                    MaxToSelect = items.Count,
                    LocationNodeType = locations.First().Level
                });

            }
            else
            {
                _newPanel.AddFilter(_currentPanel.GetFilterByType(FilterType.ParentLocations));
            }
        }

        private void BuildChildLocations(List<ChildLocation> locations)
        {
            if (_changedType < FilterType.ChildLocations)
            {
                var items = new List<FilterItem>();

                foreach (var location in locations)
                {
                    var item = new PiledFilterItem
                    {
                        Value = location.Id.ToString(),
                        Text = location.Name,
                        IsSelected = true,
                        PileKey = location.ParentId,
                        PileLabel = location.ParentName
                    };

                    items.Add(item);
                }

                _newPanel.AddFilter(new LocationsFilter(FilterType.ChildLocations)
                {
                    InputControl = OptionsInputControl.PiledSingleSelect,
                    DisplayName = locations.First().Level.ToUpper(),
                    Items = items,
                    MinToSelect = 1,
                    MaxToSelect = locations.Count,
                    LocationNodeType = locations.First().Level
                });
            }
            else
            {
                _newPanel.AddFilter(_currentPanel.GetFilterByType(FilterType.ChildLocations));
            }
        }

        private void BuildPopulationFilters(List<Population> populationFilters)
        {
            var items = new List<FilterItem>();
            var persistUserSelection = _changedType == FilterType.PopulationFilters;
            var selectedItems = persistUserSelection ? _currentPanel.GetSelectedValuesOf(FilterType.PopulationFilters) : new List<string>();

            foreach (var populationFilter in populationFilters)
            {
                var pile = populationFilter.Values;
                var pileName = populationFilter.Key;

                foreach (var pileItem in pile)
                {
                    var applySelection = persistUserSelection && selectedItems.Contains(pileItem);

                    items.Add(new PiledFilterItem
                    {
                        Value = pileItem,
                        Text = pileItem,
                        PileKey = pileName,
                        PileLabel = pileName,
                        IsSelected = applySelection
                    });
                }
            }

            _newPanel.AddFilter(new Filter(FilterType.PopulationFilters)
            {
                InputControl = OptionsInputControl.PiledSingleSelect,
                DisplayName = "POPULATION",
                Items = items
            });
        }
    }
}