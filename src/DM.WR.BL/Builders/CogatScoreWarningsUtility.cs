using System;
using DM.WR.Models.CogAt.ViewModels;
using System.Collections.Generic;
using System.Linq;
using DM.WR.Models.CogAt;
using DM.WR.Models.Config;
using DM.WR.ServiceClient.ScoreManagerApi.Models;

namespace DM.WR.BL.Builders
{
    public class CogatScoreWarningsUtility
    {
        private string CogatUcs2TargetScoreName = "ts";
        private string CogatUcs2TargetScoreText = "Targeted score";
        private string CogatUcs2TargetScoreSymbol = "•";

        private string CogatUcs2ExtVariableResponseName = "irp";
        private string CogatUcs2ExtVariableResponseText = "Inconsistent response pattern";
        private string CogatUcs2ExtVariableResponseSymbol = "‡";

        private string CogatManyItemsOmittedName = "mio";
        private string CogatManyItemsOmittedText = "Many items omitted (slow and cautious response style)";
        private string CogatManyItemsOmittedSymbol = "^";

        private string CogatTooFewItemsName = "tfia";
        private string CogatTooFewItemsText = "Too few items attempted";
        private string CogatTooFewItemsSymbol = "#";

        private string CogatAgeUnusualSymbolaName = "aucl";
        private string CogatAgeUnusualSymbolaText = "Age unusual for coded level";
        private string CogatAgeUnusualSymbolaTilde = "ã";

        private string CogatEstimatedLevelName = "el";
        private string CogatEstimatedLevelText = "Estimated Level- Student Demographic";
        private string CogatEstimatedLevelSymbol = "~";

        private string CogatLevelUnusualName = "lucg";
        private string CogatLevelUnusualText = "Level unusual for coded grade";
        private string CogatLevelUnusualSymbol = "§";

        private string CogatAgeRangeName = "aoor";
        private string CogatAgeRangeText = "Age is out-of-range";
        private string CogatAgeRangeSymbol = "«";

        private string CogatAltVName = "scni";
        private string CogatAltVText = "Sentence Completion is not included in the Verbal Battery score";
        private string CogatAltVSymbol = "¥";

        private string CogatAltVNameScreener = "vats";
        private string CogatAltVTextScreener = "Verbal Analogies is not included in the Total Score";
        private string CogatAltVSymbolScreener = "¥";

        //private const string CogatAgeUnusualSymbol = "α";
        //private const string CogatExcludeAveragesSymbol = "°";
        //private const string CogatExcludeSentenceCompletionSymbol = "±";

        //public List<CogatScoreWarningItem> GetAllScoreWarningItems()
        //{
        //    return new List<CogatScoreWarningItem>
        //    {
        //        new CogatScoreWarningItem{Name = CogatUcs2TargetScoreName, Text = CogatUcs2TargetScoreText, Symbol = CogatUcs2TargetScoreSymbol },
        //        new CogatScoreWarningItem{Name = CogatUcs2ExtVariableResponseName, Text = CogatUcs2ExtVariableResponseText, Symbol = CogatUcs2ExtVariableResponseSymbol },
        //        new CogatScoreWarningItem{Name = CogatManyItemsOmittedName, Text = CogatManyItemsOmittedText, Symbol = CogatManyItemsOmittedSymbol },
        //        new CogatScoreWarningItem{Name = CogatTooFewItemsName, Text = CogatTooFewItemsText, Symbol = CogatTooFewItemsSymbol },
        //        new CogatScoreWarningItem{Name = CogatAgeUnusualSymbolaName, Text = CogatAgeUnusualSymbolaText, Symbol = CogatAgeUnusualSymbolaTilde },
        //        new CogatScoreWarningItem{Name = CogatEstimatedLevelName, Text = CogatEstimatedLevelText, Symbol = CogatEstimatedLevelSymbol },
        //        new CogatScoreWarningItem{Name = CogatLevelUnusualName, Text = CogatLevelUnusualText, Symbol = CogatLevelUnusualSymbol },
        //        new CogatScoreWarningItem{Name = CogatAgeRangeName, Text = CogatAgeRangeText, Symbol = CogatAgeRangeSymbol },
        //        new CogatScoreWarningItem{Name = CogatAltVName, Text = CogatAltVText, Symbol = CogatAltVSymbol },
        //        new CogatScoreWarningItem{Name = CogatAltVNameScreener, Text = CogatAltVTextScreener, Symbol = CogatAltVSymbolScreener }
        //    };
        //}

        public Dictionary<string, object> GetScoreLevelWarnings(ForegroundReporting.Lib.Models.Responses.StudentSubtest record, FilterPanel filterPanel)
        {
            var fewItemsInt = record.Cc == 0 && record.Na > 0 ? 1 : 0;

            var result = new Dictionary<string, object>
                {
                    {CogatUcs2TargetScoreName, TargetedScore(record)},
                    {CogatUcs2ExtVariableResponseName, record.ev_flag == null? 0 : record.ev_flag},
                    {CogatManyItemsOmittedName, record.tmo_flag == null? 0 : record.tmo_flag},
                    {CogatTooFewItemsName, fewItemsInt}
                };

            if (record.Subtest_mininame == "V" || filterPanel.Battery.ToUpper() == "SCREENER")
            {
                var altV = GetAltV(record, filterPanel);

                foreach (var warning in altV)
                    result.Add(warning.Key, warning.Value);
            }

            return result;
        }

        public Dictionary<string, object> GetStudentLevelWarnings(ForegroundReporting.Lib.Models.Responses.StudentSubtest record, FilterPanel filterPanel)
        {
            return new Dictionary<string, object>
            {
                {CogatEstimatedLevelName,record.levelrange_flag == null ? 0: record.levelrange_flag },
                {CogatLevelUnusualName,record.levelassigned_flag == null ? 0 :record.levelassigned_flag },
                {CogatAgeUnusualSymbolaName, AgeUnusualValue(record)},
                {CogatAgeRangeName, AgeRangeValue(record, filterPanel)}
            };
        }

        public Dictionary<string, object> GetAltV(ForegroundReporting.Lib.Models.Responses.StudentSubtest record, FilterPanel filterPanel)
        {
            var completeVal = filterPanel.Battery.ToUpper() == "COMPLETE" ? 1 : 0;
            var screenerVal = filterPanel.Battery.ToUpper() == "SCREENER" ? 1 : 0;
            if (record.Norm_code == "V" || record.Norm_code == "W")
                return new Dictionary<string, object>
                {
                    {CogatAltVName, completeVal},
                    {CogatAltVNameScreener, screenerVal}
                };

            return new Dictionary<string, object>
            {
                {CogatAltVName, 0},
                {CogatAltVNameScreener, 0}
            };
        }

        //public int TargetedScore(StudentSubtest record)
        //{
        //    if (record.chance_flag == 0 || record.chance_flag == 1)
        //        if (record.Rs_score != null && record.Rs_score >= 1)
        //            return 1;

        //    if (record.chance_flag == 0 || record.chance_flag == 1)
        //        if (record.Rs_score == null)
        //            return 0;

        //    return 0;
        //}

        public int? TargetedScore(ForegroundReporting.Lib.Models.Responses.StudentSubtest record)
        {
            if (record.Rs_score != null && record.Rs_score >= 1)
                return record.chance_flag;

            return 0;
        }

        private int AgeUnusualValue(ForegroundReporting.Lib.Models.Responses.StudentSubtest record)
        {
            return record.Age_unusual ? 1 : 0;
        }

        private int AgeRangeValue(ForegroundReporting.Lib.Models.Responses.StudentSubtest record, FilterPanel filterPanel)
        {
            if (filterPanel.Battery != "SCREENER")
                return record.sas_flag == 0 ? 1 : 0;

            if (filterPanel.Battery == "SCREENER" && record.Subtest_name == "ACOMP")
                return record.sas_flag == 0 ? 1 : 0;

            return 0;
        }
    }
}