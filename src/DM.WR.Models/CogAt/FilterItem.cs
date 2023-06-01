using System.Collections.Generic;

namespace DM.WR.Models.CogAt
{
    public class FilterItem
    {
        public string Text { get; set; }
        public string Value { get; set;}
        public string AltValue { get; set; }
        public bool IsSelected { get; set;}  
        public int TestGroupId { get; set; }
        public string DistrictIds { get; set; }
        public int ClassIds { get; set; }
        public string GradeId { get; set; }
    }
}