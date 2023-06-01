using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class DonutCardsKto1Model
    {
        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }

        [JsonProperty("cards")]
        public List<DonutCardKto1> Cards { get; set; }
    }
}