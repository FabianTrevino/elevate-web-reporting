using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class CogatRoster
    {
        [JsonProperty("cogatRosterRecords")]
        public List<CogatRosterRecord> Records { get; set; }
    }
}