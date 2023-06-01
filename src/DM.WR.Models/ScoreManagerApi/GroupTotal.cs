// ReSharper disable InconsistentNaming
namespace DM.WR.Models.ScoreManagerApi
{
    public class GroupTotal
    {
        public string Acronym { get; set; }
        public string Subtest_name { get; set; }
        public string Subtest_mininame { get; set; }
        public Scores StateScores { get; set; }
        public Scores RegionScores { get; set; }
        public Scores SystemScores { get; set; }
        public Scores DistrictScores { get; set; }
        public Scores BuildingScores { get; set; }
        public Scores ClassScores { get; set; }
    }
}