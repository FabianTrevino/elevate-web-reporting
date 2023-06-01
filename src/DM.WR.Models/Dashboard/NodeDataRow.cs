using System.Collections.Generic;

namespace DM.WR.Models.Dashboard
{
    public class NodeDataRow
    {
        public NodeDataRow()
        {
            Scores = new Dictionary<string, string>();
        }

        public string NodeName { get; set; }
        public int NodeId { get; set; }
        public string NodeType { get; set; }
        public string SubtestName { get; set; }
        public string SkillName { get; set; }
        public int BinNumber { get; set; }
        public string Form { get; set; }
        public bool IsParentSkill { get; set; }
        public string ExternalSkillId { get; set; }
        public string Acronym { get; set; }

        public string Npr { get; set; }
        public string SkillNce { get; set; }

        public Dictionary<string, string> Scores { get; set; }
    }
}