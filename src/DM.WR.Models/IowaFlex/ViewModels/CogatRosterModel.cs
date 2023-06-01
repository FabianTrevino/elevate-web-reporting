using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class CogatRosterModel
    {
        public CogatRosterModel(string firstColumnTitle, string firstColumnTitleFull)
        {
            Columns = new List<CogatRosterColumn>
            {
                new CogatRosterColumn
                {
                    Title = firstColumnTitle,
                    TitleFull = firstColumnTitleFull,
                    Field = "node_name"
                },
                new CogatRosterColumn
                {
                    Title = "SS",
                    TitleFull = "Standard Score",
                    Field = "SS"
                },
                new CogatRosterColumn
                {
                    Title = "NPR",
                    TitleFull = "National Percentile Rank",
                    Field = "NPR"
                },
                new CogatRosterColumn
                {
                    Title = "V",
                    TitleFull = "Verbal",
                    Field = "v"
                },
                new CogatRosterColumn
                {
                    Title = "Q",
                    TitleFull = "Quantitative",
                    Field = "q"
                },
                new CogatRosterColumn
                {
                    Title = "N",
                    TitleFull = "Non Verbal",
                    Field = "n"
                },
                new CogatRosterColumn
                {
                    Title = "VQ",
                    TitleFull = "Composite VQ",
                    Field = "vq"
                },
                new CogatRosterColumn
                {
                    Title = "VN",
                    TitleFull = "Composite VN",
                    Field = "vn"
                },
                new CogatRosterColumn
                {
                    Title = "QN",
                    TitleFull = "Composite QN",
                    Field = "qn"
                },
                new CogatRosterColumn
                {
                    Title = "VQN",
                    TitleFull = "Composite VQN",
                    Field = "vqn"
                },
            };

            Values = new List<CogatRosterValue>();
        }

        [JsonProperty("roster_type")]
        public string RosterType { get; set; }

        [JsonProperty("roster_level")]
        public string RosterLevel { get; set; }

        [JsonProperty("columns")]
        public List<CogatRosterColumn> Columns { get; set; }

        [JsonProperty("values")]
        public List<CogatRosterValue> Values { get; set; }

        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }
    }
}