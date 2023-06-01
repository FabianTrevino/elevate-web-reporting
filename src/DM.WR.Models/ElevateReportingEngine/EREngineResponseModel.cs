using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.Models.ElevateReportingEngine
{
    //---------Request payloads------//
    #region Payloads


    //-------Test Assignment------//
    public class TestAssignmentPayload
    {
        public string name { get; set; }
        public string date { get; set; }
        public int id { get; set; }
        public string nodeId { get; set; }
        public string nodeType { get; set; }
        public string nodeName { get; set; }
        public object nodeGuid { get; set; }
        public string customerId { get; set; }
        public object userId { get; set; }
        public bool allowCovidReportFlag { get; set; }
    }

    public class TestAssignmentLocationPayload
    {
        public string nodeId { get; set; }
        public string nodeType { get; set; }
        public string nodeName { get; set; }
    }

    //-------Grade------//
    public class GradePayload
    {
        public int level { get; set; }
        public string gradeText { get; set; }
        public string battery { get; set; }
        public bool isBundled { get; set; }
        public bool isAlt { get; set; }
        public bool canMerge { get; set; }
        public int gradeNum { get; set; }
        public int testGroupId { get; set; }
    }

    //-------Content------//
    public class ContentPayload
    {
        public string acronym { get; set; }
        public string battery { get; set; }
        public string subtestName { get; set; }
        public bool isDefault { get; set; }
        public bool isAlt { get; set; }
        public int id { get; set; }
    }


    //-------Locations------//
    public class LocationPayload
    {
        public string id { get; set; }
        public string nodeType { get; set; }
        public string nodeName { get; set; }
        public string nodeTypeDisplay { get; set; }
        public string Guid { get; set; }
        public string parentId { get; set; }
        public string grade { get; set; }
    }

    //-------Students------//

    public class StudentPayload
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TestInstanceId { get; set; }
    }

    public class StudentbackgroundPayload
    {
        public int id { get; set; }
        public string name { get; set; }
        public int grade { get; set; }
        public int classId { get; set; }
        public int testInstanceId { get; set; }
    }

    //--------Age Stanines---------//
    public class AgeStaninePayload
    {
        public int assigmentId { get; set; }
        public int contentId { get; set; }
        public int ageStanineScale { get; set; }
        public int counter { get; set; }
    }

    #endregion



    //---------Filters  Requests------//


    #region Requests
    //-------Test Assignment------//
    public class EREngineFiltersTestAssignmentResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<TestAssignmentPayload> payload { get; set; }
    }

    public class EREngineFiltersTestAssignmentsLocationResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<TestAssignmentLocationPayload> payload { get; set; }
    }

    //-------Grades------//
    public class EREngineFiltersGradeResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<GradePayload> payload { get; set; }
    }


    //-------Content------//

    public class EREngineFiltersContentResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<ContentPayload> payload { get; set; }
    }

    //-------Locations------//
    public class EREngineFiltersLocationResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<LocationPayload> payload { get; set; }
    }

    //-------Students------//

    public class EREngineFiltersStudentResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<StudentPayload> payload { get; set; }
    }

    public class EREngineBackgrondStudentResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<StudentbackgroundPayload> payload { get; set; }
    }

    //-------Population-------//
    public class EREngineFiltersPopulationResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public object payload { get; set; }
    }

    //---------AgeStanine--------//
    public class EREngineAgeStanineResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<AgeStaninePayload> payload { get; set; }
    }

    #endregion


    public class DashboardGroupScores
    {
        public string apr_score { get; set; }
        public string as_score { get; set; }
        public string gpr_score { get; set; }
        public string gs_score { get; set; }
        public string uss_score { get; set; }
        public string sas_score { get; set; }
        public string rs_score { get; set; }
        public string na_score { get; set; }
        public string total_item_score { get; set; }
        public string ls_score { get; set; }
        public string lpr_score { get; set; }
    }

    public class GroupTotalPayload
    {
        public string acronym { get; set; }
        public string subtest_name { get; set; }
        public string subtest_mininame { get; set; }
        public DashboardGroupScores stateScores { get; set; }
        public DashboardGroupScores regionScores { get; set; }
        public DashboardGroupScores systemScores { get; set; }
        public DashboardGroupScores districtScores { get; set; }
        public DashboardGroupScores buildingScores { get; set; }
        public DashboardGroupScores classScores { get; set; }
    }

    public class EREngineGroupTotalResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<GroupTotalPayload> payload { get; set; }
    }
}
