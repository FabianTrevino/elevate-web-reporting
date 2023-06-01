using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class LongitudinalRosterColumn
    {
        [JsonProperty("id")]
        public string TestEventId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_full")]
        public string TitleFull { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }
    }
}