using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class RosterKto1Column
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_full")]
        public string TitleFull { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }
    }
}