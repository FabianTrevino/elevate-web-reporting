namespace DM.WR.Data.Repository.Types
{
    public class DbContentScope
    {
        public string Acronym { get; set; }
        public string Battery { get; set; }
        public string SubtestName { get; set; }
        public bool IsDefault { get; set; }
        public bool isAlt { get; set; }
        public int contentID { get; set; }

    }      
}
