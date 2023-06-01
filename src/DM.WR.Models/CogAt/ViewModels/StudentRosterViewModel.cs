using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class StudentRosterViewModel
    {
        [JsonProperty("columns")]
        public List<StudentRosterHeaderCell> Columns { get; set; }

        [JsonProperty("values")]
        public List<Dictionary<string, object>> Values { get; set; }

        [JsonProperty("api_params")]
        public object ApiParams { get; set; }

        [JsonProperty("extra_params")]
        public StudentRosterExtraParams ExtraParams { get; set; }
    }
}