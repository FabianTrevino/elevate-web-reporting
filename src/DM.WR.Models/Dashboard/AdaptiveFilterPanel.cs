using System.Collections.Generic;
using System.Linq;

namespace DM.WR.Models.Dashboard
{
    public class AdaptiveFilterPanel
    {
        //TODO: Remove after The Open Source is working properly, if ever.
        public string GraphqlQuery { get; set; }

        public AdaptiveFilterPanel()
        {
            _filters = new List<Filter>();
        }

        private readonly List<Filter> _filters;

        public List<LocationNode> RootNodes { get; set; }
        public List<LocationNode> BreadCrumbs { get; set; }
        

        public bool IsChildLocationStudent
        {
            get
            {
                var childLocations = (LocationsFilter)GetFilterByType(FilterType.Location);
                return childLocations.LocationNodeType.ToLower() == "student";
            }
        }

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

        public string GetSelectedValuesStringOf(FilterType type, bool wrapValuesInQuotes = false)
        {
            var values = wrapValuesInQuotes ? GetSelectedValuesOf(type).Select(v => $"\"{v}\"").ToList() : GetSelectedValuesOf(type);
            return string.Join(",", values);
        }
    }
}