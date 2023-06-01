using DM.WR.Models.SMI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.Models.BackgroundReport.Reports
{
    public class IowaListOfStudentScoresReportRequest
    {
        public string LoggingRequestId { get; set; }

        public string OutputSystemName { get; set; }
        public string ReportFormat { get; set; }
        public string RFormat { get; set; }
        public string SuppressProgramLabel { get; set; }
        public string TestFamilyGroupCode { get; set; }

        public string CogatComposite { get; set; }

        public string CogatDifferences { get; set; }
        public int ACTScoreGrade { get; set; }

        public string SuppressSubtests { get; set; }

        public string RankingDirection { get; set; }

        public string RankingSubtest { get; set; }

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
    }
}
