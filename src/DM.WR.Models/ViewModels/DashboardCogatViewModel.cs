namespace DM.WR.Models.ViewModels
{
    public class DashboardCogatViewModel
    {
        public bool IsGuidUser { get; set; }
        public bool IsProd { get; set; }
        public string AppPath { get; set; }
        public string QueryString { get; set; }
        public string ActuateGenerateUrl { get; set; }
        public string ActuateWebLocation { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string IsTelerikReportFeatureEnabled { get; set; }
        public string ReturnUrl { get; set; }
    }
}