using DM.WR.Models.CogAt;

namespace DM.WR.BL.Builders
{
    public class CogatCommonFunctions
    {
        public bool LoadLprs(FilterPanel filterPanel)
        {
            return filterPanel.ScoringOptions.DisplayFlags["LPR"] || filterPanel.ScoringOptions.DisplayFlags["LS"];
        }
    }
}
