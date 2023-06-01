using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class ContentAreaHeader
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("field")]
        public string Field { get; set; }
    }
}