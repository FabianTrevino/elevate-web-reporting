using DM.WR.Models.ViewModels;
using System.Threading.Tasks;

namespace DM.WR.BL.Providers
{
    public interface ILibraryProvider
    {
        Task<LibraryPageViewModel> BuildModelAsync();

        //Task<byte[]> GetReportAsync(string reportId);

        void UpdateOptions(string parameters);
        void DeletdataExporterSession();
    }
}