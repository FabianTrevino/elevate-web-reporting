
namespace DM.WR.Models.BackgroundReport
{
    public class GenerateReportResponse
    {
        public string ErrorMessage { get; set; }

        public bool WasSuccessful => string.IsNullOrEmpty(ErrorMessage);
    }
}