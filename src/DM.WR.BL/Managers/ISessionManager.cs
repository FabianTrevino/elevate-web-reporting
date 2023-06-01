namespace DM.WR.BL.Managers
{
    public interface ISessionManager
    {
        void ClearAllSession();
        object Retrieve(string key);
        void Delete(string key);
        void Store(object data, string key);
    }
}