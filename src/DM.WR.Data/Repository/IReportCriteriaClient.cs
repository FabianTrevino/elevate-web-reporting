using System.Collections.Generic;
using DM.WR.Data.Repository.Types;

namespace DM.WR.Data.Repository
{
    public interface IReportCriteriaClient
    {
        void ReportCriteria_Delete(List<int> criteriaIds);
        ReportCriteria ReportCriteria_Insert(ReportCriteria inCriteria);
        string ReportCriteria_LoadOptions(int criteriaId);
        List<ReportCriteria> ReportCriteria_Select(string dmUserId, string locationGuid, object assessmentId = null, object displayType = null, object criteriaName = null);
        ReportCriteria ReportCriteria_Update(int inCriteriaId, string newName, string newDescription, string newOptionsXml);
    }
}