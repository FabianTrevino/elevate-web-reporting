using DM.WR.Models.Types;

namespace DM.WR.BL.Managers
{
    public interface IUserDataManager
    {
        UserData ChangeLocation(int nodeId);
        UserData GetUserData();
        void StoreUserData(UserData userData);
       // void AddLocation(UserDetails userDetails);
    }
}