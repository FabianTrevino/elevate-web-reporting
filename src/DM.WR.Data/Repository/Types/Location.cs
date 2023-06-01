namespace DM.WR.Data.Repository.Types
{
    public class Location
    {
        public int Id { get; set; }
        public string NodeType { get; set; }
        public string NodeName { get; set; }
        public string Guid { get; set; }
        public string ParentGuid { get; set; }
        public string NodeTypeDisplay { get; set; }
    }
}