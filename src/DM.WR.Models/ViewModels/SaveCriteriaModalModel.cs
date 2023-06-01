using DM.UI.Library.Models;

namespace DM.WR.Models.ViewModels
{
    public class SaveCriteriaModalModel : ModalModel
    {
        public SaveCriteriaModalModel(SectionColor sectionColor) : base(sectionColor) { }

        public string ReportTypeName { get; set; }
    }
}