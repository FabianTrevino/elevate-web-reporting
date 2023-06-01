// ReSharper disable InconsistentNaming
namespace DM.WR.Models.ScoreManagerApi
{
    public class SMIGroupParameters
    {
        public bool GroupScoresByForm { get; set; }
        public bool GroupScoresByLevel { get; set; }
        public bool GroupScoresByBattery { get; set; }
        public string CogatProfileGroupScoreMode { get; set; }
    }
}