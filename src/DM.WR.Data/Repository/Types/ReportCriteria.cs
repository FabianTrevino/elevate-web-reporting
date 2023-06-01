using System;

namespace DM.WR.Data.Repository.Types
{
    public class ReportCriteria
    {
        public int CriteriaId;
        public string DmUserId;
        public string LocationGuid;
        public int AssessmentId;
        public string AssessmentGroupCode;
        public string DisplayType;
        public string CriteriaName;
        public string CriteriaDescription;
        public DateTime CreatedDate;
        public string OptionsXml;
        public bool SubmitToReportCenter;
    }
}