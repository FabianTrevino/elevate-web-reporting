// ReSharper disable InconsistentNaming
namespace DM.WR.Models.ScoreManagerApi
{
    public class SMIGeneralProcessingParameters
    {
        public int TestFetchSize { get; set; }
        public int SMFetchSize { get; set; }
        //public string CustomSortString { get; set; }
        //public string RosterReturnMode { get; set; }
        public bool LoadLPRs { get; set; }
        public int LoggingSessionId { get; set; }
        public string LoggingCustomerId { get; set; }
        public string LoggingUserLocationGuid { get; set; }
        public string LoggingOutputSystemName { get; set; }
    }
}