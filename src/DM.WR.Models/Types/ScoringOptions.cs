using System.Collections.Generic;

namespace DM.WR.Models.Types
{
    public class ScoringOptions
    {
        public ScoringOptions()
        {
            DisplayFlags = new Dictionary<string, bool>();
        }

        public Dictionary<string, bool> DisplayFlags { get; set; }

        public bool HideScore(string scoreValue)
        {
            return DisplayFlags.ContainsKey(scoreValue) && !DisplayFlags[scoreValue];
        }

        public int SkillSetId { get; set; }
        public int WordskillSetId { get; set; }
        public int GroupsetId { get; set; }
        public string GroupsetCode { get; set; }
        public string GpdGroupsetCode { get; set; }
        public int DefaultAdmz { get; set; }
        public string LprNodeLevel { get; set; }
        public string LprNodeList { get; set; }
        public string TestType { get; set; }
        public int AllowLexileScore { get; set; }
        public int AllowLprScore { get; set; }
        public int AllowCathprivFlag { get; set; }
        public int ExcludeMathcompDefault { get; set; }
        public string PredSubtestGroupType { get; set; }
        public string SubtestCutscoreFamilyId { get; set; }
        public int LongitudinalFlag { get; set; }
        public string DefaultCogatDiff { get; set; }
        public string PredictedSubtestAcronym { get; set; }
        public int CcoreSkillsetId { get; set; }
        public string CcoreSubtestGroupType { get; set; }
        public int ElaTotal { get; set; }  
        public string SplitDate { get; set; }
        public bool HasItemsFlag { get; set; }
        public string AlternativeNormYear { get; set; }
        public int AccountabilityFlag { get; set; }
        public string PrintOnlyReports { get; set; }
        public string RFormat { get; set; }
    }
}