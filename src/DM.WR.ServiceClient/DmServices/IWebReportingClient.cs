using DM.WR.ServiceClient.DmServices.Models;

namespace DM.WR.ServiceClient.DmServices
{
    public interface IWebReportingClient
    {
        SaveWebKeyResponse SaveWebKey(string dmBasServicesUrl, string dmUserId, string dmImpersonatorId, string reportingKey);
    }
}