namespace DM.WR.Models.Types
{
    public class CustomerInfo
    {
        public string NodeType { get; set; }
        public string NodeId { get; set; }
        public string NodeName { get; set; }
        public int DmUserId { get; set; }  //was called    public int CustomerIdMorse
        public string CustomerId { get; set; }
        public int TotalLevels { get; set; }
        public string Guid { get; set; }
        public int NodeLevel { get; set; }
    }
}