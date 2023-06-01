using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DM.UI.Library.Models;
using DM.WR.Models.Xml;
using DM.WR.Models.Types;

namespace DM.WR.Models.Options
{
    public class ScoreWarningsGroup : OptionGroup
    {
        public ScoreWarningsGroup() : base(XMLGroupType.ScoreWarnings)
        {
            ScoreWarningObjects = new List<ScoreWarning>();
        }

        [XmlIgnore] public List<ScoreWarning> ScoreWarningObjects;

        public List<DropdownItem> GetSequenceDropdown()
        {
            return ScoreWarningObjects.Select(sw => new DropdownItem { Text = sw.FilterDesc, Value = sw.DisplaySeq }).ToList();
        }

        public List<ScoreWarningRow> Rows { get; set; }


        public bool HasSelection => Rows.Any(r => r.HasSelection);

        public string SelectionText
        {
            get
            {
                if (!HasSelection)
                    return "None selected";

                var result = new List<string>();
                foreach (var row in Rows)
                {
                    if (!row.HasSelection) continue;

                    var concat = row.Concatenation == ConcatOperatorEnum.None ? "" : $"{row.Concatenation.ToString()} ";
                    var sequence = ScoreWarningObjects.First(sw => sw.DisplaySeq == row.SequenceNumber).FilterDesc;

                    result.Add($"{concat}{sequence}: {row.Switch}");
                }
                return string.Join(" ", result);
            }
        }

        public string QueryString
        {
            get
            {
                if (!HasSelection)
                    return "";

                var result = new List<string>();
                foreach (var row in Rows)
                {
                    if (!ScoreWarningObjects.Exists(o => o.DisplaySeq == row.SequenceNumber))
                        continue;

                    var scoreWarning = ScoreWarningObjects.First(o => o.DisplaySeq == row.SequenceNumber);
                    var filter = row.Switch == ScoreWarningsFilterSwitchEnum.Include ? scoreWarning.IncludeFilter : scoreWarning.ExcludeFilter;
                    var concat = row.Concatenation == ConcatOperatorEnum.None ? "" : row.Concatenation.ToString();
                    result.Add($" {concat} {filter} ");
                }
                return $"( {string.Join("", result)} )";
            }
        }
    }
}