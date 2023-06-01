using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class GainAnalysis
    {
        [JsonProperty("longitudinalGain")]
        public List<LongitudinalGain> LongitudinalGain { get; set; }
    }
}