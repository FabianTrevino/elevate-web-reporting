// ReSharper disable InconsistentNaming
namespace DM.WR.GraphQlClient.Models
{
    public class AuthenticationModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string auth_current_date_time { get; set; }
    }
}