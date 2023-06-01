using System.Collections.Generic;

namespace DM.WR.Models.ViewModels
{
    public class DashboardDifferentiatedInstructionReportViewModel
    {
        public IEnumerable<string> Node { get; set; }
        public IEnumerable<Dictionary<string, string>> Mod { get; set; }
        public string Profiles { get; set; }
        public string TestEvent { get; set; }
        public string TestFamilyGroupCode { get; set; }
        public string Grade { get; set; }
        public string District { get; set; }
        public string SchoolName { get; set; }
        public string SchoolLabel { get; set; }
        public string ClassName { get; set; }
        public string ClassLabel { get; set; }
    }
}