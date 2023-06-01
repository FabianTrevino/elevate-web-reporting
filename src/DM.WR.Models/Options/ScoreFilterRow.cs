using System;

namespace DM.WR.Models.Options
{
    public class ScoreFilterRow
    {
        public ScoreFilterRow() { }

        //Query string example:  "|AND|K:SKILL_ISSD:R012:BETWEEN:1,89"
        public ScoreFilterRow(string queryString)
        {
            string[] split;

            var concatSplit = queryString.Split('|');
            if (concatSplit.Length == 3)
            {
                Concatenation = (ConcatOperatorEnum)Enum.Parse(Concatenation.GetType(), concatSplit[1]);
                split = concatSplit[2].Split(':');
            }
            else
            {
                split = queryString.Split(':');
            }

            FilterValue = split[0];
            ScoreValue = split[1];
            ContentAreaValue = split[2];
            ComparisonOperator= split[3];
            Value = split.Length > 4 ? split[4] : "";
        }

        public ConcatOperatorEnum Concatenation { get; set; }
        public string FilterValue { get; set; }
        public string ScoreValue { get; set; }
        public string ContentAreaValue { get; set; }
        public string ContentAreaKey { get; set; }
        public string ComparisonOperator { get; set; }
        public string Value { get; set; }


        public bool HasSelection => !string.IsNullOrEmpty(Value);

        public string QueryString
        {
            get
            {
                if(!HasSelection)
                    return "";

                var concat = Concatenation == ConcatOperatorEnum.None ? "" : $"|{Concatenation.ToString()}|";
                return $"{concat}{FilterValue}:{ScoreValue}:{ContentAreaValue}:{ComparisonOperator}:{Value}";
            }
        }
        public string MultimeasureQueryString
        {
            get
            {
                if(!HasSelection)
                    return "";

                var concat = Concatenation == ConcatOperatorEnum.None ? "" : $"|{Concatenation.ToString()}|";
                return $"{concat}{FilterValue}:{ScoreValue}:__PAGE_NUM__:{ComparisonOperator}:{Value}";
            }
        }
    }
}