using DM.WR.Models.Xml;

namespace DM.WR.Models.Options
{
    public class LocationGroup : OptionGroup
    {
        public LocationGroup() { }//parameterless ctor must be here for this object to serialize properly for critera
        public LocationGroup(string type) : base(type) { }
        public LocationGroup(XMLGroupType type) : base(type) { }

        public string LocationNodeType { get; set; }
    }
}