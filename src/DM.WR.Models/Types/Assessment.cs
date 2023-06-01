namespace DM.WR.Models.Types
{
    public class Assessment
    {
        public int TestFamilyGroupId { get; set; }
        public string TestFamilyName { get; set; }
        public string TestFamilyDesc { get; set; }
        public string TestFamilyGroupCode { get; set; }
        public string SmVersion { get; set; }
        public bool IsIss => TestFamilyGroupCode == "ISSREAD" || TestFamilyGroupCode == "ISSMATH" || TestFamilyGroupCode == "ISSSCI";

    }
}