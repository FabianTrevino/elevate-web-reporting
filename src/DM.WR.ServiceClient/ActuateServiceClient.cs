using DM.WR.ServiceClient.ActuateApi;
using System;
using System.ServiceModel;
using System.Web.Services.Protocols;

namespace DM.WR.ServiceClient
{
    public class ActuateServiceClient : IActuateServiceClient
    {
        public bool CreateUser(string dmUser, string actuateUser, string volume, string userName, string password, out string error)
        {
            error = "";

            Header header = new Header { TargetVolume = volume };
            Login login = new Login { User = userName, Password = password };

            try
            {
                ActuateSoapPortClient soapPortClient = new ActuateSoapPortClient();
                LoginResponse loginResponse = soapPortClient.login(header, login);

                header.AuthId = loginResponse.AuthId;
                
                //	Prepare CreateUser operation
                CreateUser createUser = new CreateUser
                {
                    IgnoreDup = false,
                    IgnoreDupSpecified = true,
                    User = new User
                    {
                        Name = actuateUser,
                        Password = "",
                        HomeFolder = "Home/" + dmUser, // home folder will be created automatically
                        ViewPreference = UserViewPreference.DHTML,
                        ViewPreferenceSpecified = true,
                        SendEmailForSuccess = true,
                        SendNoticeForSuccessSpecified = true,
                        SendNoticeForFailureSpecified = true
                    }
                };

                AdminOperation lCreateUserOpt = new AdminOperation
                {
                    Item = createUser
                };
                AdminOperation[] lAdminRequest = { lCreateUserOpt };
                AdministrateResponse administrateResponse = soapPortClient.administrate(header, lAdminRequest);
            }
            catch (FaultException fe)
            {
                //Ignore the USER_EXISTS error
                if (fe.Code.Name == "Server")
                    return true;

                error = fe.Message;
                return false;
            }
            catch (SoapException se)
            {
                error = se.Message;
                return false;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }

            return true;
        }
    }
}