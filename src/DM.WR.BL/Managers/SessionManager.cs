using System.Web;

namespace DM.WR.BL.Managers
{
    public class SessionManager : ISessionManager
    {
        public void Store(object data, string key)
        {
            HttpContext.Current.Session[key] = data;
        }

        public object Retrieve(string key)
        {
            return HttpContext.Current.Session[key];
        }

        public void Delete(string key)
        {
            HttpContext.Current.Session[key] = null;
        }

        public void ClearAllSession()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}