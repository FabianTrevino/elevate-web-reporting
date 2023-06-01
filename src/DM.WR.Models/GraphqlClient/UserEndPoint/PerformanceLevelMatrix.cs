using DM.WR.Models.IowaFlex.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class PerformanceLevelMatrix
    {
        [JsonProperty("dataPoints")]
        public List<PerformanceLevelMatrixDataPoint> DataPoints { get; set; }
    }
}