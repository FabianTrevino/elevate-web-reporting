using System;

namespace DM.WR.Data.Repository.Types
{
    public class LongTestAdminScoreset
    {
        public int CustomerScoresetId { get; set; }
        public int TestAdminId { get; set; }
        public int ProgramId { get; set; }
        public int ScoresetId { get; set; }
        public string ScoresetDesc { get; set; }
        public DateTime TestDate { get; set; }
        public string TestFamilyId { get; set; }
        public bool IsDefaultTestGrade { get; set; }
        public string TestGradeId { get; set; }
        public string Battery { get; set; }
        public string TestAdminGradeLevelId { get; set; }
    }      
}