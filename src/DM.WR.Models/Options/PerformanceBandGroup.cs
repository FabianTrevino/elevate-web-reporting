using System.Linq;
using DM.WR.Models.Xml;

namespace DM.WR.Models.Options
{
    public class PerformanceBandGroup : OptionGroup
    {
        public PerformanceBandGroup() : base(XMLGroupType.PerformanceBands) { }

        public string BandKey { get; set; }

        public bool HasSelection => Options.Cast<PerformanceBandOption>().Any(o => !string.IsNullOrEmpty(o.LowValue) &&  !string.IsNullOrEmpty(o.HighValue));
    }
}