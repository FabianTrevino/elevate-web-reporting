using System;

namespace IRM_Library.Objects
{
    [Serializable]    
    public class ContentArea
    {
        public string Acronym { get; set; }
        public string Battery { get; set; }
        public string SubtestName { get; set; }
        public bool IsSelectedByDefault { get; set; }
    }      
}
