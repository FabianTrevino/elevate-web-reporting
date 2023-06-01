using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class StudentRoster
    {
        [JsonProperty("students")]
        public List<Student> Students { get; set; }
    }
}