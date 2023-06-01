using DM.WR.Models.ViewModels;
using System.Threading.Tasks;

namespace DM.WR.BL.Providers
{
    public interface IReportProvider
    {
        ReportViewerModel BuildReportViewerModel();
        string CreateExcelQueryString(string query);
        string ApplyLastNameSearch(string lastName);
        void UpdateOptions(string queryString);
        Task<string> SendReportToBackgroundAsync(string reportName);
        Task<string> GetFileByUserID();
        void DeletePDF(int fileID);
        Task<string> GetDataExportData(int fielId);
    }
}