// ReSharper disable InconsistentNaming
namespace DM.WR.Models.ScoreManagerApi
{
    public class SmiApiParameters
    {
        public SMIBaseParameters SMIBaseParameters { get; set; }
        //public SMIFilteringParameters SMIFilteringParameters { get; set; }
        public object SMIFilteringParameters { get; set; }
        //public SMIGeneralProcessingParameters SMIGeneralProcessingParameters { get; set; }
        public object SMIGeneralProcessingParameters { get; set; }
        //public SMIGroupParameters SMIGroupParameters { get; set; }
        public object SMIGroupParameters { get; set; }
        public SMISubtestParameters SMISubtestParameters { get; set; }
    }
}