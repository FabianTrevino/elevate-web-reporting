using DM.UI.Library.Models;

namespace DM.WR.Models.ViewModels
{
    public class ReportingKeyModalModel : ModalModel
    {
        public ReportingKeyModalModel() : base(SectionColor.Green) { }

        public bool Success { get; set; }
        public string Key { get; set; }
    }
}