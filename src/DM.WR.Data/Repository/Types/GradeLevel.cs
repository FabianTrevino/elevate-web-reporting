namespace DM.WR.Data.Repository.Types
{
    public class GradeLevel
    {
        public int Level { get; set; }
        public string Grade { get; set; }
        public string GradeText { get; set; }
        public string Battery { get; set; }
        public bool isBundled { get; set; }
        public bool isAlt { get; set; }
        public bool canMerge { get; set; }
        public int gradeNum { get; set; }
        public int testGroupId { get; set; }
    }
}