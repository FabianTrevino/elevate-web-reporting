using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class PerformanceLevelMatrixModel
    {
        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }

        [JsonProperty("dataPoints")]
        public List<PerformanceLevelMatrixDataPointModel> DataPoints { get; set; }

    }
}