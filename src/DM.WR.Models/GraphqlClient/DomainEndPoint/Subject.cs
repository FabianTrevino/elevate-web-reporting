using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.DomainEndPoint
{
    public class Subject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("subjectAbbrev")]
        public string SubjectAbbreviation { get; set; }
    }
}