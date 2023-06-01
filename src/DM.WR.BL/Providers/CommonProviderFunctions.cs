using System.Collections.Generic;
using DM.UI.Library.Models;
using DM.WR.Models.Config;
using DM.WR.BL.Managers;
using DM.WR.Models.Types;

namespace DM.WR.BL.Providers
{
    public class CommonProviderFunctions
    {
        public bool IsGuidUser(UserData userData)
        {
            return userData != null && userData.UserDisplayName == Constants.GuidUser;
        }

        public bool IsAdaptive()
        {
            return new SessionManager().Retrieve(SessionKey.UserData) is UserData userData && userData.IsAdaptive;
        }

        public List<MagicMenuItem> GetMainMenu()
        {
            var userData = (UserData)new SessionManager().Retrieve(SessionKey.UserData);

            return userData?.MainMenu;
        }
    }
}
