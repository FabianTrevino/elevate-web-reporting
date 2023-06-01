using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class JsonResponseModel
    {

        [JsonProperty("model")]
        public object Model { get; set; }

        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }
    }
}