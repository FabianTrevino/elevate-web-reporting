namespace DM.WR.Models.SMI
{
    public class SMIBaseParameters
    {
        public bool Accountability { get; set; }

        /// <summary>
        /// This property was added for Cogat... thought it is not on the ScoreManagerInterface version
        /// </summary>
        public string CogatComposite { get; set; }

        public string CustomerScoresetIDs { get; set; }

        public ExcludeType ExcludeMathComputation { get; set; }

        public bool ExcludeOUZ { get; set; }

        public bool ExcludeOUZSubtest { get; set; }

        public ExcludeType ExcludeWordAnalysisListening { get; set; }

        public string GradeLevelIDs { get; set; }

        public string Grades { get; set; }

        public string InnerGroup { get; set; }

        public int Language { get; set; }

        public string OptionalWhereClause { get; set; }

        public string OuterGroup { get; set; }

        public string ReportPopulationNodeIDs { get; set; }

        public string ReportPopulationNodeType { get; set; }

        public string TestadminGradeLevelIDs { get; set; }

        public string TestBatteries { get; set; }

        public string TestFamilies { get; set; }

        public string TestLevels { get; set; }

        public string TestPopulationNodeIDs { get; set; }

        public string TestPopulationNodeType { get; set; }        
    }
}
