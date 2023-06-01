using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class GainsAnalysisModel
    {
        [JsonProperty("bands")]
        public List<Band> Bands { get; set; }
        
        [JsonProperty("values")]
        public List<GainsAnalysisValue> Values { get; set; }
    }
}