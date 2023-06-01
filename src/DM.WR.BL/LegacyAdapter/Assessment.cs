using System;

namespace IRM_Library.Objects
{
    [Serializable]
    public class Assessment //was called ProductInfo
    {
        public int TestFamilyGroupId { get; set; }
        public string TestFamilyName { get; set; }
        public string TestFamilyDesc { get; set; }
        public string TestFamilyGroupCode { get; set; }
        public string SmVersion { get; set; }
        public bool IsISS { get { return ((TestFamilyGroupCode == "ISSREAD") || (TestFamilyGroupCode == "ISSMATH") || (TestFamilyGroupCode == "ISSSCI")); } }
    }
}
