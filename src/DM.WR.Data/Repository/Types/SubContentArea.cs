namespace DM.WR.Data.Repository.Types
{
    public class SubContentArea
    {
        public SubContentArea()
        {
            Acronym = " ";
            SkillId = " ";
            SkillName = "None";
            ParentFlag = "0";
            CognitiveSkillFlag = "0";
            IsSelectedByDefault = true;
        }

        public string Acronym { get; set; }
        public string SkillId { get; set; }
        public string SkillName { get; set; }
        public string ParentFlag { get; set; }
        public string CognitiveSkillFlag { get; set; }
        public bool IsSelectedByDefault { get; set; }
    }
}
