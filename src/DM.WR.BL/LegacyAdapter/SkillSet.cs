using System;

namespace IRM_Library.Objects
{
    [Serializable]    
    public class SkillSet
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string DefaultSkill { get; set; }
        public string SubtestGroupType { get; set; }
    }
}
