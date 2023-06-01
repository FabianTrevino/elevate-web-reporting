using System;
using System.Collections.Generic;
using System.Linq;

using DM.WR.Models.SMI;
namespace DM.WR.Models.BackgroundReport.Reports
{
    public class CatalogExporter
    {
        public string LoggingRequestId { get; set; }
        public string OutputSystemName { get; set; }
         public string ReportFormat { get; set; }
        public string TestFamilyGroupCode { get; set; }
        public string ExportFormat { get; set; }
        public string ExportFieldFormat { get; set; }
        public string ExportHeading { get; set; }
        public string DisaggLabel { get; set; }

        public string BuildingLabel { get; set; }

        public string ClassLabel { get; set; }

        public string DistrictLabel { get; set; }

        public string RegionLabel { get; set; }

        public string StateLabel { get; set; }

        public string SystemLabel { get; set; }
        public SMIBaseParameters SMIBaseParameters { get; set; }
        public SMISubtestParameters SMISubtestParameters { get; set; }
        public SMIFilteringParameters SMIFilteringParameters { get; set; }
        public SMISkillParameters SMISkillParameters { get; set; }
    }
}
