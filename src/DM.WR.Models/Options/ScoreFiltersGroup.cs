using System.Collections.Generic;
using System.Linq;
using DM.UI.Library.Models;
using DM.WR.Models.Xml;

namespace DM.WR.Models.Options
{
    public class ScoreFiltersGroup : OptionGroup
    {
        public ScoreFiltersGroup() : base(XMLGroupType.ScoreFilters)
        {
            _scores = new List<ScoreFiltersScore>();
            _contentAreas = new Dictionary<string, List<DropdownItem>>();

            _comparisonOperators = new List<DropdownItem>
                {
                    new DropdownItem{ Text = "is equal to", Value = "=" },
                    new DropdownItem{ Text = "is greater than", Value = ">" },
                    new DropdownItem{ Text = "is less than", Value = "<" },
                    new DropdownItem{ Text = "is greater than or equal to", Value = ">=" },
                    new DropdownItem{ Text = "is less than or equal to", Value = "<=" },
                    new DropdownItem{ Text = "is in between", Value = "BETWEEN" }
                };

            Rows = new List<ScoreFilterRow>();
        }

        private List<ScoreFiltersScore> _scores;
        public List<ScoreFiltersScore> GetScores()
        {
            var clone = new List<ScoreFiltersScore>();
            foreach (var score in _scores)
            {
                clone.Add(new ScoreFiltersScore
                {
                    ContentAreaKey = score.ContentAreaKey,
                    FilterValue = score.FilterValue,
                    ScoreValue = score.ScoreValue,
                    ScoreText = score.ScoreText
                });
            }
            return clone;
        }
        public void SetScores(List<ScoreFiltersScore> scores)
        {
            _scores = scores;
        }

        private Dictionary<string, List<DropdownItem>> _contentAreas;
        public Dictionary<string, List<DropdownItem>> GetContentAreas()
        {
            var clone = new Dictionary<string, List<DropdownItem>>();
            foreach (var area in _contentAreas)
            {
                var dropdownList = new List<DropdownItem>();
                foreach (var item in area.Value)
                {
                    dropdownList.Add(new DropdownItem
                    {
                        Text = item.Text,
                        Value = item.Value
                    });
                }
                clone.Add(area.Key, dropdownList);
            }
            return clone;
        }
        public void SetContentAreas(Dictionary<string, List<DropdownItem>> contentAreas)
        {
            _contentAreas = contentAreas;
        }

        private readonly List<DropdownItem> _comparisonOperators;
        public List<DropdownItem> GetComparisonOperators()
        {
            var clone = new List<DropdownItem>();
            foreach (var item in _comparisonOperators)
            {
                clone.Add(new DropdownItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }
            return clone;
        }


        public List<ScoreFilterRow> Rows { get; set; }

        public bool HasSelection => Rows.Any(r => r.HasSelection);
        
        public string SelectionText
        {
            get
            {
                if (!HasSelection)
                    return "None selected";

                var result = new List<string>();
                foreach(var row in Rows)
                {
                    if(!row.HasSelection) continue;

                    var concat = row.Concatenation == ConcatOperatorEnum.None ? "" : $"{row.Concatenation.ToString()} ";
                    var score = _scores.First(s => s.ScoreValue == row.ScoreValue).ScoreText;
                    var contentArea = _contentAreas[row.ContentAreaKey].First(ca => ca.Value.Replace("'", "") == row.ContentAreaValue.Replace("'", "")).Text;
                    var comparisonOperator = _comparisonOperators.First(s => s.Value == row.ComparisonOperator).Text;
                    var value = row.Value.Replace(",", " and ");
                    
                    result.Add($"{concat} {score} in {contentArea} {comparisonOperator} {value}");
                }
                return string.Join(" ", result);
            }
        }
    }
}