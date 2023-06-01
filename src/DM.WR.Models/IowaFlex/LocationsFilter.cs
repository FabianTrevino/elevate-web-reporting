namespace DM.WR.Models.IowaFlex
{
    public class LocationsFilter : Filter
    {
        public LocationsFilter() { }//parameterless ctor must be here for this object to serialize properly for critera
        public LocationsFilter(string type) : base(type) { }
        public LocationsFilter(FilterType type) : base(type) { }

        public string LocationNodeType { get; set; }
    }
}