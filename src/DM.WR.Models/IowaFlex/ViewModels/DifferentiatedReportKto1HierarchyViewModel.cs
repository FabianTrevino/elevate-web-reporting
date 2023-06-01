using DM.WR.Models.GraphqlClient.UserEndPoint;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class DifferentiatedReportKto1HierarchyViewModel
    {
        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }
        [JsonProperty("values")]
        public List<DifferentiatedReportKto1> Values { get; set; }
    }
}