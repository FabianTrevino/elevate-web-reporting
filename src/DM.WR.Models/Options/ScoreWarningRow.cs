using System;

namespace DM.WR.Models.Options
{
    public class ScoreWarningRow : Option
    {
        public ScoreWarningRow() { }

        //selection example:  AND:20:Include
        public ScoreWarningRow(string selection)
        {
            var split = selection.Split(':');

            if (split.Length < 3)
                return;

            Concatenation = string.IsNullOrEmpty(split[0]) ? ConcatOperatorEnum.None : (ConcatOperatorEnum)Enum.Parse(Concatenation.GetType(), split[0]);
            SequenceNumber = split[1];
            Switch = (ScoreWarningsFilterSwitchEnum)Enum.Parse(Switch.GetType(), split[2]);

        }

        public ConcatOperatorEnum Concatenation { get; set; }
        public string SequenceNumber { get; set; }
        public ScoreWarningsFilterSwitchEnum Switch { get; set; }

        public bool HasSelection => !string.IsNullOrEmpty(SequenceNumber);
    }
}