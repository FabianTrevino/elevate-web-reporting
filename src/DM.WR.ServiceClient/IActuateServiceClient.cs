namespace DM.WR.ServiceClient
{
    public interface IActuateServiceClient
    {
        bool CreateUser(string dmUser, string actuateUser, string volume, string userName, string password, out string error);
    }
}