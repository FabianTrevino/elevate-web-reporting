using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.Models.ElevateReportingEngine
{
    public class StudentProgramCode
    {
        public object fiveoFour { get; set; }
        public object se { get; set; }
        public object migrant { get; set; }
        public object frl { get; set; }
        public object iep { get; set; }
        public object title1Math { get; set; }
        public object titlel1Reading { get; set; }
        public object gt { get; set; }
        public object other { get; set; }
    }

    public class StudentAdminCode
    {
        public string code_a { get; set; }
        public string code_b { get; set; }
        public string code_c { get; set; }
        public string code_d { get; set; }
        public string code_e { get; set; }
        public string code_f { get; set; }
        public string code_g { get; set; }
        public string code_h { get; set; }
        public string code_i { get; set; }
        public string code_j { get; set; }
        public string code_k { get; set; }
        public string code_l { get; set; }
        public string code_m { get; set; }
        public string code_n { get; set; }
        public string code_o { get; set; }
        public string code_p { get; set; }
        public string code_q { get; set; }
        public string code_r { get; set; }
        public string code_s { get; set; }
        public string code_t { get; set; }
    }

    public class Population
    {
        public string gender { get; set; }
        public object raceEthnicity { get; set; }
        public object hispanicEthnicity { get; set; }
        public StudentProgramCode studentProgramCode { get; set; }
        public StudentAdminCode studentAdminCode { get; set; }
    }

    public class EREngineRequestModel
    {
        public string customerId { get; set; }
        public int assignmentId { get; set; }
        public string role { get; set; }
        public List<string> districtIds { get; set; }
        public List<string> buildingIds { get; set; }
        public List<string> classIds { get; set; }
        public List<string> studentIds { get; set; }
        public List<int> contentIds { get; set; }
        public int gradeLevel { get; set; }
        public Population population { get; set; }
        public PopulationNew populationNew { get; set; }
        public string apl { get; set; }
        public List<string> dimensonValueList { get; set; }
        public int pageOffset { get; set; }
        public int pageLimit { get; set; }
        public string sortField { get; set; }
        public string sortDirection { get; set; }
        public string reportCriteria { get; set; }
    }
    public class StudentRaces
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class StudentPrograms
    {
        public int programId { get; set; }
        public string name { get; set; }
    }
    public class StudentCustomCodings
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class PopulationNew
    {
        public List<string> genders { get; set; }
        public bool? isHispanic { get; set; } = null;
        public List<StudentRaces> studentRaces { get; set; }
        public List<StudentPrograms> studentPrograms { get; set; }
        public List<StudentCustomCodings> studentCustomCodings { get; set; }
    }
    public class EREngineGroupRequestModel
    {
        public int assignmentId { get; set; }
        public string customerId { get; set; }
        public string role { get; set; }
        public List<string> districtIds { get; set; }
        public List<string> buildingIds { get; set; }
        public List<string> classIds { get; set; }
        public List<string> studentIds { get; set; }
        public List<int> contentIds { get; set; }
        public List<string> contents { get; set; }
        public int gradeLevel { get; set; }
        public string gradeName { get; set; }
        public int aS9 { get; set; }
        public PopulationNew population { get; set; }
        public object apl { get; set; }
        public string reportCriteria { get; set; }
        public int pageOffset { get; set; }
        public int pageLimit { get; set; }
        public string sortField { get; set; }
        public string sortDirection { get; set; }
    }

    public class EREngineFilterTestAssignmentAndGradeRequest
    {
        public List<string> nodeIdList { get; set; }
        public string customerId { get; set; }
        public string nodeType { get; set; }
        public int assignmentId { get; set; }
        public int userId { get; set; }
        public string userRole { get; set; }
    }

    public class EREngineFilterContentRequest
    {
        public int testGroupId { get; set; }
    }

    public class EREngineFilterDistrictLocationsRequest
    {
        public string nodeId { get; set; }
        public int assignmentId { get; set; }
        public int gradeLevelId { get; set; }
        public List<Buildings> buildings { get; set; }
        public string userRole { get; set; }

    }
    public class EREngineBackgroundFilterLocationRequest
    {
        public List<string> nodeId { get; set; }
        public string nodeType { get; set; }
        public int assignmentId { get; set; }
        public List<int> grades { get; set; }

    }
    public class EREngineFilterBuildingLocationsRequest
    {
        public string nodeId { get; set; }
        public List<string> nodeIdList { get; set; }
        public string customerId { get; set; }
        public string nodeType { get; set; }
        public int assignmnetProfileId { get; set; }
        public int gradeLevelId { get; set; }
        public List<int> contentIds { get; set; }
        public string districtId { get; set; }
    }
    public class EREngineFilterClassLocationsRequest
    {
        public string nodeId { get; set; }
        public List<string> nodeIdList { get; set; }
        public string customerId { get; set; }
        public string nodeType { get; set; }
        public int assignmnetProfileId { get; set; }
        public int gradeLevelId { get; set; }
        public List<int> contentIds { get; set; }
        public List<string> buildingIds { get; set; }
        public string districtId { get; set; }
    }

    public class EREngineFilterStudentsRequest
    {
        public string customerId { get; set; }
        public List<string> nodeIdList { get; set; }
        public string nodeType { get; set; }
        public int assignmentId { get; set; }
        public int gradeLevelId { get; set; }
        public List<int> contentIds { get; set; }
        public List<string> classIds { get; set; }
    }

    public class EREngineBackgroundStudentsRequest
    {
        public string customerId { get; set; }
        public List<string> nodeIdList { get; set; }
        public string nodeType { get; set; }
        public int assignmentId { get; set; }
        public List<int> grades { get; set; }
        public List<int> contentIds { get; set; }
        public List<string> classIds { get; set; }
    }


    public class EREngineFilterPopulationRequest
    {
        public string nodeId { get; set; }
        public List<string> nodeIdList { get; set; }
        public string customerId { get; set; }
        public string nodeType { get; set; }
        public int assignmentId { get; set; }
        public int gradeLevelId { get; set; }
        public int testGroupId { get; set; }
        public List<int> contentIds { get; set; }
        public string districtId { get; set; }
        public List<string> buildingIds { get; set; }
        public List<string> classIds { get; set; }
        public List<string> studentIds { get; set; }
    }
    public class EREngineFilterBuildingPopulationRequest
    {
        public string nodeId { get; set; }
        public List<string> nodeIdList { get; set; }
        public string customerId { get; set; }
        public string nodeType { get; set; }
        public int assignmentId { get; set; }
        public int gradeLevelId { get; set; }
        public List<int> contentIds { get; set; }
        public string districtId { get; set; }
        public List<string> buildingIds { get; set; }
        public List<string> classIds { get; set; }
        public List<string> studentIds { get; set; }
        
    }

    public class ElevateStudentReportRequest
    {
        public string customerId { get; set; }
        public int assignmentId { get; set; }
        public string role { get; set; }
        public List<string> districtIds { get; set; }
        public List<string> buildingIds { get; set; }
        public List<string> classIds { get; set; }
        public List<string> studentIds { get; set; }
        public List<int> gradeLevels { get; set; }
        public List<int> contentIds { get; set; }
        public List<string> scores { get; set; }
        public Population population { get; set; }
        public string reportCriteria { get; set; }
    }

    public class ElevateStudentProfileNarrativeReportRequest
    {
        public string customerId { get; set; }
        public int assignmentId { get; set; }
        public string role { get; set; }
        public List<string> districtIds { get; set; }
        public List<string> buildingIds { get; set; }
        public List<string> classIds { get; set; }
        public List<string> studentIds { get; set; }
        public List<int> gradeLevels { get; set; }
        public List<int> contentIds { get; set; }
        public List<string> scores { get; set; }
        public Population population { get; set; }
        public List<int> dimensiongroupvalueids { get; set; }
        public string reportCriteria { get; set; }
        public string displayLanguage { get; set; }
        public bool includeAbilityProfile { get; set; }
    }

    public class ElevateGroupSummaryReportRequest
    {
        public string customerId { get; set; }
        public int assignmentId { get; set; }
        public string role { get; set; }
        public List<string> districtIds { get; set; }
        public List<string> buildingIds { get; set; }
        public List<string> classIds { get; set; }
        public List<string> studentIds { get; set; }
        public List<int> gradeLevels { get; set; }
        public List<int> contentIds { get; set; }
        public List<string> scores { get; set; }
        public Population population { get; set; }
        public string reportCriteria { get; set; }
        public string homeReporting { get; set; }
        public string battery { get; set; }
        public bool includeAbilityProfile { get; set; }
    }

    public class ElevateCogatListOfStudentScores
    {
        public string FileName { get; set; }
        public string ReportTemplate { get; set; }
        public string ReportFormat { get; set; }
        public string RFormat { get; set; }
        public string SuppressProgramLabel { get; set; }
        public string CogatComposite { get; set; }
        public string RankingDirection { get; set; }
        public string GraphType { get; set; }
        public string RankingSubtest { get; set; }
        public string RankingScore { get; set; }
        public string DisaggLabel { get; set; }
        public string BuildingLabel { get; set; }
        public string ClassLabel { get; set; }
        public string DistrictLabel { get; set; }
        public string RegionLabel { get; set; }
        public string StateLabel { get; set; }
        public string SystemLabel { get; set; }
        public ElevateStudentReportRequest QueryParameters { get; set; }
    }

    public class ElevateCogatProfileNarrative
    {
        public string FileName { get; set; }
        public string ReportTemplate { get; set; }
        public string ReportFormat { get; set; }
        public string RFormat { get; set; }
        public object CogatComposite { get; set; }
        public bool SuppressProfile { get; set; }
        public string GraphType { get; set; }
        public bool HomeReporting { get; set; }
        public string ReportGrouping { get; set; }
        public string DisaggLabel { get; set; }
        public string BuildingLabel { get; set; }
        public string ClassLabel { get; set; }
        public string DistrictLabel { get; set; }
        public string RegionLabel { get; set; }
        public string StateLabel { get; set; }
        public string SystemLabel { get; set; }
        public ElevateStudentProfileNarrativeReportRequest QueryParameters { get; set; }
    }
    public class ElevateCogatGroupSummary
    {
        public string FileName { get; set; }
        public string ReportTemplate { get; set; }
        public string ReportFormat { get; set; }
        public string RFormat { get; set; }
        public object CogatComposite { get; set; }
        public bool SuppressProfile { get; set; }
        public string GraphType { get; set; }
        public string ReportGrouping { get; set; }
        public string DisaggLabel { get; set; }
        public string BuildingLabel { get; set; }
        public string ClassLabel { get; set; }
        public string DistrictLabel { get; set; }
        public string RegionLabel { get; set; }
        public string StateLabel { get; set; }
        public string SystemLabel { get; set; }
        public ElevateGroupSummaryReportRequest QueryParameters { get; set; }
    }

    public class ElevateCogatDataExporter
    {
        public string FileName { get; set; }
        public string ReportTemplate { get; set; }
        public string ReportFormat { get; set; }
        public string RFormat { get; set; }
        public string ExporterTemplate { get; set; }
        public string ExportFormat { get; set; }
        public string DisaggLabel { get; set; }
        public string BuildingLabel { get; set; }
        public string ClassLabel { get; set; }
        public string DistrictLabel { get; set; }
        public string RegionLabel { get; set; }
        public string StateLabel { get; set; }
        public string SystemLabel { get; set; }
        public ElevateGroupSummaryReportRequest QueryParameters { get; set; }
    }

    public class Buildings
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Sections> sections { get; set; }
    }


    public class Sections
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
