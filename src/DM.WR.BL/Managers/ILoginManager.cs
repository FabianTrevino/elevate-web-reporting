using DM.WR.Models.ViewModels;
using DM.WR.ServiceClient.DmServices.Models;

namespace DM.WR.BL.Managers
{
    public interface ILoginManager
    {
        SiteEntryModel ProcessSiteEntry(UserDetails model, string loginSource, string contractInstance);
        SiteEntryModel ProcessSiteEntryElevate(UserIdentityObject userIdentityObject, string rosteringRoles, string loginSource, string contractInstance, string role, string idToken);

        SiteEntryModel ProcessReEntry(string IdToken);

    }
}