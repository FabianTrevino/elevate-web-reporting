using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class StudentRosterHeaderCell
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("title_full")]
        public string TitleFull { get; set; }
        [JsonProperty("multi")]
        public int Multi { get; set; }
        [JsonProperty("field")]
        public string Field { get; set; }
        [JsonProperty("fields")]
        public List<StudentRosterHeaderField> Fields { get; set; }
    }
}