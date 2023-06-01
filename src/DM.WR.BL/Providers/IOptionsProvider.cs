using System.Collections.Generic;
using DM.WR.Models.Options;
using DM.WR.Models.ViewModels;

namespace DM.WR.BL.Providers
{
    public interface IOptionsProvider
    {
        OptionsPageViewModel BuildDefaultModel();
        LocationChangeModalModel BuildLocationChangeModalModel();
        OptionsViewModel GetOptions();
        OptionGroup GetGroup(string groupType);
        void SwitchLocation(int nodeId);
        void UpdateOptions(string groupType, List<string> values);
        void ResetOptions();
        void AddMultimeasureColumn();
        void RemoveCurrentMultimeasureColumn();
        void GoToMultimeasureColumn(int pageNumber);
        void ClearCriteriaEditMode();
    }
}