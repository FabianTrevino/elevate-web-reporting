using DM.UI.Library.Models;
using DM.WR.Models.ViewModels;

namespace DM.WR.BL.Providers
{
    public interface ICriteriaProvider
    {
        CriteriaPageViewModel BuildViewModel();
        CriteriaTableViewModel BuildTableViewModel(string selectedAssessmentGroupCode, string selectedDisplayType);
        string SaveNewCriteria(string criteriaName, string criteriaDescription);
        string UpdateExistingCriteria(int criteriaId, string name, string summary);
        string LoadCriteria(int criteriaId, bool enableEditMode, string criteriaName = null, string criteriaDescription = null, string criteriaDate = null);
        bool DeleteCriteria(int criteriaId);
        ModalModel BuildDeleteCriteriaModal();
        ModalModel BuildUnsavedChangesModal();
        SaveCriteriaModalModel BuildSaveCriteriaModal();
        SaveCriteriaModalModel BuildRunInBackgroundModal();
    }
}