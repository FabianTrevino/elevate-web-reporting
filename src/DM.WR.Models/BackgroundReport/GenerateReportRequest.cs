namespace DM.WR.Models.BackgroundReport
{
    public class GenerateReportRequest
    {
        public string Environment { get; set; }
        public string Parameters { get; set; }
        public string ReportType { get; set; }
        public string UserID { get; set; }
        public string filename { get; set; }
        public string folderPath { get; set; }
        public string Processorname { get; set; }
        public string Priority { get; set; }
        public int TaskcommandID { get; set; }
        public string ActuateJobID { get; set; }
        public string isActuate { get; set; }
        public string CriteriaID { get; set; }
        public bool HasExportToExcel { get; set; }
        public bool HasLastNameSearch { get; set; }
    }
}

