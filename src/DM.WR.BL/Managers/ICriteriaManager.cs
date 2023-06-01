using DM.WR.Models.Types;

namespace DM.WR.BL.Managers
{
    public interface ICriteriaManager
    {
        string LoadCriteria(int criteriaId, bool enableEditMode, string criteriaName = null,
            string criteriaDescription = null, string criteriaDate = null);
        CriteriaInfo SaveNewCriteria(string criteriaName, string criteriaDescription, bool submitToReportCenter);
        string UpdateExistingCriteria(int criteriaId, string name, string summary);
        bool DeleteCriteria(int criteriaId);
    }
}