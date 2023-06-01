using System.Collections.Generic;
using System.Linq;

namespace DM.WR.Models.IowaFlex
{
    public class IowaFlexFilterPanel
    {
        public string GraphqlQuery { get; set; }

        public IowaFlexFilterPanel()
        {
            _filters = new List<Filter>();
        }

        private readonly List<Filter> _filters;

        //public string PanelName { get; set; }

        public List<LocationNode> RootNodes { get; set; }
        public List<LocationNode> BreadCrumbs { get; set; }
        public bool IsCogat { get; set; }
        public FilterType LastUpdatedFilterType { get; set; }


        public bool IsChildLocationStudent
        {
            get
            {
                var childLocations = (LocationsFilter)GetFilterByType(FilterType.ChildLocations);
                return childLocations.LocationNodeType.ToLower() == "student";
            }
        }

        //public bool IsLongitudinal
        //{
        //    get
        //    {
        //        var selectedTestEvent = GetFilterByType(FilterType.TestEvent).Items.Cast<TestEventFilterItem>().FirstOrDefault(i => i.IsSelected);

        //        if (selectedTestEvent == null)
        //            return false;

        //        return selectedTestEvent.IsLongitudinal;
        //    }
        //}

        public List<Filter> GetAllFilters()
        {
            return _filters;
        }

        public Filter GetFilterByType(FilterType type)
        {
            return _filters.FirstOrDefault(g => g.Type == type);
        }

        public void AddFilter(Filter filter)
        {
            _filters.Add(filter);
        }

        public List<string> GetSelectedValuesOf(FilterType type)
        {
            var filter = GetFilterByType(type);
            if (filter == null)
                return new List<string>();

            return filter.SelectedValues;
        }

        public List<string> GetSelectedAltValuesOf(FilterType type)
        {
            var filter = GetFilterByType(type);
            if (filter == null)
                return new List<string>();

            return filter.SelectedAltValues;
        }

        public string GetSelectedValuesStringOf(FilterType type, bool wrapValuesInQuotes = false)
        {
            var values = wrapValuesInQuotes ? GetSelectedValuesOf(type).Select(v => $"\"{v}\"").ToList() : GetSelectedValuesOf(type);
            return string.Join(",", values);
        }

        public string GetSelectedAltValuesStringOf(FilterType type, bool wrapValuesInQuotes = false)
        {
            var values = wrapValuesInQuotes ? GetSelectedAltValuesOf(type).Select(v => $"\"{v}\"").ToList() : GetSelectedValuesOf(type);
            return string.Join(",", values);
        }

        public string GetSubject(bool wrapInQuotes = false)
        {
            var selectedTestEvent = GetFilterByType(FilterType.TestEvent).Items.Cast<TestEventFilterItem>().FirstOrDefault(i => i.IsSelected);

            if (selectedTestEvent == null)
                return "";

            return wrapInQuotes ? $"\"{selectedTestEvent.Subject}\"" : selectedTestEvent.Subject;
        }
    }
}