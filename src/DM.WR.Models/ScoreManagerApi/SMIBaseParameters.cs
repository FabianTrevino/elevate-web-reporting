// ReSharper disable InconsistentNaming
namespace DM.WR.Models.ScoreManagerApi
{
    public class SMIBaseParameters
    {
        public int GradeLevelIDs { get; set; }
        public string CustomerScoresetIDs { get; set; }
        public string TestPopulationNodeIDs { get; set; }
        public string TestPopulationNodeType { get; set; }
        public string ReportPopulationNodeIDs { get; set; }
        public string ReportPopulationNodeType { get; set; }
        public string OuterGroup { get; set; }
        public string InnerGroup { get; set; }
        public string OptionalWhereClause { get; set; }
        public int Accountability { get; set; }
    }
}