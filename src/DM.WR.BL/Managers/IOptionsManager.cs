using DM.WR.Models.Options;
using DM.WR.Models.Types;

namespace DM.WR.BL.Managers
{
    public interface IOptionsManager
    {
        OptionBook GetOptionBook();
        void UpdateOptionBook(OptionBook optionBook);
        void DeleteOptionBook();
        void AddOptionPage();
        void RemoveOptionPage();
        void FlipPage(int pageNumber);
        void EnableCriteriaEditMode(CriteriaInfo criteria);
        void DisableCriteriaEditMode();
    }
}