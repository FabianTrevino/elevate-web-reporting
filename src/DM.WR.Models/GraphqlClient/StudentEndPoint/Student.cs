using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class Student
    {
        [JsonProperty("currentTestEvent")]
        public CurrentTestEvent CurrentTestEvent { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("testEvents")]
        public List<TestEvent> TestEvents { get; set; }
    }
}
