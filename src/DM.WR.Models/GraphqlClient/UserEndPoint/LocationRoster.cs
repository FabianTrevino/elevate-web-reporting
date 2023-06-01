using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class LocationRoster
    {
        [JsonProperty("locations")]
        public List<Location> Locations { get; set; }
    }
}