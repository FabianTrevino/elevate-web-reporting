using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CogatRosterRecord
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("verbal")]
        public int? Verbal { get; set; }

        [JsonProperty("quantitative")]
        public int? Quantitative { get; set; }

        [JsonProperty("nonVerbal")]
        public int? NonVerbal { get; set; }

        [JsonProperty("compVQ")]
        // ReSharper disable once InconsistentNaming
        public int? CompVQ { get; set; }

        [JsonProperty("compQN")]
        // ReSharper disable once InconsistentNaming
        public int? CompQN { get; set; }

        [JsonProperty("compVN")]
        // ReSharper disable once InconsistentNaming
        public int? CompVN { get; set; }

        [JsonProperty("compVQN")]
        // ReSharper disable once InconsistentNaming
        public int? CompVQN { get; set; }

        [JsonProperty("testScore")]
        public int? TestScore { get; set; }

        [JsonProperty("npr")]
        public int? Npr { get; set; }
    }
}