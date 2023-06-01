using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class StudentRosterHeaderField
    {
        [JsonProperty("field")]
        public string Field { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("title_full")]
        public string TitleFull { get; set; }
    }
}