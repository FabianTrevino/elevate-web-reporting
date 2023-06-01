namespace DM.WR.Models.ViewModels
{
    public class OptionsPageViewModel
    {
        public string CurrentLocationName { get; set; }
        public bool IsMultiLocation { get; set; }
        public bool IsGuidUser { get; set; }


        public bool IsInEditCriteriaMode { get; set; }
        public int CriteriaId { get; set; }
        public string CriteriaName { get; set; }
        public string CriteriaDescription { get; set; }
        public string CriteriaDate { get; set; }
    }
}