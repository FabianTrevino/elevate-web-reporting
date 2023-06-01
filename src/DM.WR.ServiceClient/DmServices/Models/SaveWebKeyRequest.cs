// ReSharper disable InconsistentNaming
namespace DM.WR.ServiceClient.DmServices.Models
{
    public class SaveWebKeyRequest
    {
        public string key { get; set; }
        public string userID { get; set; }
        public string impersonatorID { get; set; }
    }
}