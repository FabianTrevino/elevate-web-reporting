using DM.WR.Models.Xml;

namespace DM.WR.Models.Options
{
    public class PerformanceBandOption : Option
    {
        public XMLBandColorEnum BandColor { get; set; }
        public string LowValue { get; set; }
        public string HighValue { get; set; }
    }
}