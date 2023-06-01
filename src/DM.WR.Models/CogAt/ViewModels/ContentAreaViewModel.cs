using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class ContentAreaViewModel
    {
        [JsonProperty("columns")]
        public List<ContentAreaHeader> Columns { get; set; }
        [JsonProperty("percent_nationally")]
        public ContentAreaNationalPercent PercentNationally { get; set; }
        [JsonProperty("values")]
        public List<ContentAreaRow> Values { get; set; }
    }
}
