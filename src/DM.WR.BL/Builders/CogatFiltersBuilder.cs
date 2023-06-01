using AutoMapper;
using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.CogAt;
using DM.WR.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using DM.WR.Models.Config;
using DM.WR.ServiceClient.ElevateReportingEngine;
using Newtonsoft.Json;
using DM.WR.Models.CogAt.elevate;
using DM.WR.Models.ElevateReportingEngine;
using System.Net;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text;
using DM.WR.ServiceClient.DmServices.Models;

namespace DM.WR.BL.Builders
{
    public interface ICogatFiltersBuilder
    {
        FilterPanel BuildPanel(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData);
    }

    public class CogatFiltersBuilder : ICogatFiltersBuilder
    {
        private readonly IDbClient _dbClient;

        private readonly CogatFiltersMapper _mapper;
        private readonly IMapper _typesMapper;


        public Filter InvalidFilter { get; private set; }

        #region Debug Methods

        public static List<TestAdmin> TestAdminList { get; set; }
        public static List<GradeLevel> GradeLevelList { get; set; }
        public static List<DbContentScope> ContentScopeList { get; set; }
        public static List<Location> LocationList { get; set; }
        public static List<Student> StudentList { get; set; }
        public static Dictionary<string, List<Disaggregation>> PopulationList { get; set; }
        public static DbScoringOptions ScrOptions { get; set; }
        #endregion

        public readonly IElevateReportingEngineClient _elevateReportingEngineClient;

        public CogatFiltersBuilder(IDbClient dbClient, IElevateReportingEngineClient elevateReportingEngineClient)
        {
            _mapper = new CogatFiltersMapper();
            _typesMapper = new DbTypesMapper();
            _dbClient = dbClient;
            _elevateReportingEngineClient = elevateReportingEngineClient;
        }

        #region Debug Methods
        public List<TestAdmin> GetTestAdminList(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            return TestAdminList;
        }
        public List<GradeLevel> GetGradeLevelList(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            return GradeLevelList;
        }
        public List<DbContentScope> GetContentScopeList(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            return ContentScopeList;
        }
        public DbScoringOptions GetScoringOptions(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            return ScrOptions;
        }
        public List<Location> GetLocationsList(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            return LocationList;
        }

        public List<Student> GetStudentList(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            return StudentList;
        }
        public Dictionary<string, List<Disaggregation>> GetPopulationList(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            return PopulationList;
        }
        #endregion
        
        public FilterPanel BuildPanel(FilterPanel currentPanel, FilterType updatedFilterType, UserData userData)
        {
            var newPanel = new FilterPanel();
            var selectedLocation = "";
            bool permissionCheck;
            var customError = new CustomError();
            currentPanel.isRetry = false;
            //var elevateRostering = JsonConvert.DeserializeObject<RosterRolesElevate>(rosteringRoles);

            try
            {

                #region Assessments
                newPanel.Assessment = currentPanel.Assessment ?? userData.Assessments.FirstOrDefault(a => a.TestFamilyGroupCode.ToLower().Contains("cogat") || a.TestFamilyGroupCode.ToLower().Contains("ccat"));
                #endregion



                #region Test Event

                {
                    var currentFilter = currentPanel.GetFilterByType(FilterType.TestEvent);

                    if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                    {
                        var allTestAdmins = new List<TestAdmin>();
                        var debugAllTestAdmins = new List<TestAdmin>();

                        Dictionary<string, bool> displayFlags = new Dictionary<string, bool>();

                        #region MultipleAPI Calls
                        EREngineFilterTestAssignmentAndGradeRequest eREngineFilterTestAssignmentAndGradeRequest = new EREngineFilterTestAssignmentAndGradeRequest();
                        List<TestAssignmentPayload> testAssignFromElev = new List<TestAssignmentPayload>();

                        if (userData.ElevateRole == "district_admin")
                        {
                            eREngineFilterTestAssignmentAndGradeRequest.nodeIdList = userData.CustomerInfoList.Select(x => x.NodeId).ToList();
                            eREngineFilterTestAssignmentAndGradeRequest.customerId = userData.CustomerInfoList[0].CustomerId.ToString();
                            eREngineFilterTestAssignmentAndGradeRequest.nodeType = "district";
                            eREngineFilterTestAssignmentAndGradeRequest.assignmentId = 0;
                            testAssignFromElev = _elevateReportingEngineClient.GetFiltersTestAssignmentListForRoles(eREngineFilterTestAssignmentAndGradeRequest, userData).payload;
                            customError.isNoData = testAssignFromElev.Count == 0 ? true : false;
                            customError.ResponseCount = $"The TestAssignment Call Returned with {testAssignFromElev.Count} rows";

                        }
                        else if (userData.ElevateRole == "Staff")
                        {

                            EREngineFiltersTestAssignmentResponse response = new EREngineFiltersTestAssignmentResponse();
                            EREngineFilterTestAssignmentAndGradeRequest eREngineFilterTestAssignmentAndGradeRequestSingle = new EREngineFilterTestAssignmentAndGradeRequest();
                            eREngineFilterTestAssignmentAndGradeRequestSingle.nodeIdList = userData.ParticipatedLocations.staff.schools.Select(x => x.id).ToList();
                            eREngineFilterTestAssignmentAndGradeRequestSingle.customerId = userData.CustomerInfoList[0].CustomerId.ToString();
                            eREngineFilterTestAssignmentAndGradeRequestSingle.nodeType = "building";
                            eREngineFilterTestAssignmentAndGradeRequestSingle.assignmentId = 0;
                            response = _elevateReportingEngineClient.GetFiltersTestAssignmentListForRoles(eREngineFilterTestAssignmentAndGradeRequestSingle, userData);
                            testAssignFromElev = response.payload;
                            customError.isNoData = testAssignFromElev.Count == 0 ? true : false;
                            customError.ResponseCount = $"The TestAssignment Call Returned with {testAssignFromElev.Count} rows";

                        }
                        else if (userData.ElevateRole == "Teacher")
                        {

                            EREngineFiltersTestAssignmentResponse response = new EREngineFiltersTestAssignmentResponse();
                            EREngineFilterTestAssignmentAndGradeRequest eREngineFilterTestAssignmentAndGradeRequestSingle = new EREngineFilterTestAssignmentAndGradeRequest();
                            eREngineFilterTestAssignmentAndGradeRequestSingle.nodeIdList = userData.ParticipatedLocations.teacher.schools.Select(x => x.id).ToList();
                            eREngineFilterTestAssignmentAndGradeRequestSingle.customerId = userData.CustomerInfoList[0].CustomerId.ToString();
                            eREngineFilterTestAssignmentAndGradeRequestSingle.nodeType = "building";
                            eREngineFilterTestAssignmentAndGradeRequestSingle.assignmentId = 0;
                            response = _elevateReportingEngineClient.GetFiltersTestAssignmentListForRoles(eREngineFilterTestAssignmentAndGradeRequestSingle, userData);
                            testAssignFromElev = response.payload;
                            customError.isNoData = testAssignFromElev.Count == 0 ? true : false;
                            customError.ResponseCount = $"The TestAssignment Call Returned with {testAssignFromElev.Count} rows";

                        }
                        foreach (var elevItem in testAssignFromElev)
                        {
                            TestAdmin singletestAssignemnt = new TestAdmin();
                            singletestAssignemnt.Name = elevItem.name;
                            singletestAssignemnt.Date = elevItem.date;
                            singletestAssignemnt.Id = elevItem.id;
                            singletestAssignemnt.NodeId = elevItem.nodeId;
                            singletestAssignemnt.NodeType = elevItem.nodeType;
                            singletestAssignemnt.NodeName = elevItem.nodeName;
                            singletestAssignemnt.CustomerId = elevItem.customerId;
                            singletestAssignemnt.AllowCovidReportFlag = elevItem.allowCovidReportFlag;
                            singletestAssignemnt.NodeGuid = elevItem.nodeId;
                            allTestAdmins.Add(singletestAssignemnt);
                        }
                        #endregion


                        var newFilter = _mapper.MapTestAdmins(allTestAdmins, currentFilter, SetInvalidFilter);
                        newPanel.AddFilter(newFilter);
                        newPanel.TestAdmins = allTestAdmins;
                        TestAdminList = allTestAdmins;
                        var selectedTestAdmin = newPanel.SelectedTestAdmin;


                        #region defaultScoreSetOptions

                        DbScoringOptions DbScoringOptions = new DbScoringOptions();

                        displayFlags.Add("AltSS", false);
                        displayFlags.Add("AltPR1", false);
                        displayFlags.Add("AltST1", false);
                        displayFlags.Add("AltGE", false);
                        displayFlags.Add("LEXILE", false);
                        displayFlags.Add("LPR", true);
                        displayFlags.Add("LS", true);
                        displayFlags.Add("PRIV", false);
                        displayFlags.Add("SCHPR", true);
                        DbScoringOptions.DisplayFlags = displayFlags;
                        DbScoringOptions.SkillSetId = 101;
                        DbScoringOptions.WordskillSetId = -1;
                        DbScoringOptions.GroupsetId = 281;
                        DbScoringOptions.GroupsetCode = "CATALOG_IRM40";
                        DbScoringOptions.GpdGroupsetCode = null;
                        DbScoringOptions.DefaultAdmz = 0;
                        DbScoringOptions.LprNodeLevel = "DISTRICT";
                        DbScoringOptions.LprNodeList = "79805";
                        DbScoringOptions.AllowLexileScore = 1;
                        DbScoringOptions.AllowLprScore = 0;
                        DbScoringOptions.AllowCathprivFlag = 0;
                        DbScoringOptions.ExcludeMathcompDefault = 0;
                        DbScoringOptions.PredSubtestGroupType = "null";
                        DbScoringOptions.SubtestCutscoreFamilyId = "null";
                        DbScoringOptions.LongitudinalFlag = 1;
                        DbScoringOptions.DefaultCogatDiff = "null";
                        DbScoringOptions.PredictedSubtestAcronym = "null";
                        DbScoringOptions.CcoreSkillsetId = 0;
                        DbScoringOptions.CcoreSubtestGroupType = "null";
                        DbScoringOptions.ElaTotal = 1;
                        DbScoringOptions.HasItemsFlag = false;
                        DbScoringOptions.AlternativeNormYear = null;
                        DbScoringOptions.AccountabilityFlag = 1;
                        DbScoringOptions.PrintOnlyReports = "";
                        DbScoringOptions.RFormat = "Catalog";
                        newPanel.ScoringOptions = _typesMapper.Map<DbScoringOptions, ScoringOptions>(DbScoringOptions);
                        #endregion

                    }
                    else
                    {
                        newPanel.AddFilter(currentFilter);
                        newPanel.TestAdmins = currentPanel.TestAdmins;
                        newPanel.ScoreSetId = currentPanel.ScoreSetId;
                        newPanel.ScoringOptions = currentPanel.ScoringOptions;
                    }
                }

                #endregion

                #region Grade
                {
                    var currentFilter = currentPanel.GetFilterByType(FilterType.Grade);
                    if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                    {
                        var selectedTestAdmin = newPanel.SelectedTestAdmin;


                        List<GradeLevel> gradeLevelsList = new List<GradeLevel>();
                        #region Multiple Api Calls
                        EREngineFilterTestAssignmentAndGradeRequest eREngineFilterTestAssignmentAndGradeRequest = new EREngineFilterTestAssignmentAndGradeRequest();
                        eREngineFilterTestAssignmentAndGradeRequest.nodeIdList = new List<string>() { selectedTestAdmin.NodeId.ToString() };
                        eREngineFilterTestAssignmentAndGradeRequest.customerId = selectedTestAdmin.CustomerId.ToString();
                        eREngineFilterTestAssignmentAndGradeRequest.nodeType = selectedTestAdmin.NodeType;
                        eREngineFilterTestAssignmentAndGradeRequest.assignmentId = selectedTestAdmin.Id;
                        eREngineFilterTestAssignmentAndGradeRequest.userId = Convert.ToInt32(userData.ElevateUserId);
                        eREngineFilterTestAssignmentAndGradeRequest.userRole = userData.ElevateRole;
                        var grades = _elevateReportingEngineClient.GetFiltersGrades(eREngineFilterTestAssignmentAndGradeRequest, userData).payload;
                        customError.isNoData = grades.Count == 0 ? true : false;
                        customError.ResponseCount = $"The Grades Call Returned with {grades.Count} rows";
                        foreach (var gradeItem in grades)
                        {
                            GradeLevel singleGrade = new GradeLevel();
                            singleGrade.Level = gradeItem.level;
                            singleGrade.Battery = gradeItem.battery;
                            singleGrade.Grade = gradeItem.gradeNum.ToString();
                            singleGrade.GradeText = gradeItem.gradeText;
                            singleGrade.isBundled = gradeItem.isBundled;
                            singleGrade.isAlt = gradeItem.isAlt;
                            singleGrade.canMerge = gradeItem.canMerge;
                            singleGrade.testGroupId = gradeItem.testGroupId;

                            gradeLevelsList.Add(singleGrade);
                        }
                        #endregion

                        var newFilter = _mapper.MapGradeLevels(gradeLevelsList, currentFilter, SetInvalidFilter);

                        newPanel.AddFilter(newFilter);
                        GradeLevelList = gradeLevelsList;

                    }
                    else
                    {
                        newPanel.AddFilter(currentFilter);
                    }
                }
                #endregion




                #region Content
                {
                    var currentFilter = currentPanel.GetFilterByType(FilterType.Content);

                    if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                    {
                        List<DbContentScope> dbContentScopesList = new List<DbContentScope>();

                        var selectedTestAdmin = newPanel.SelectedTestAdmin;

                        #region Multiple APi calls
                        EREngineFilterContentRequest eREngineFilterContentRequest = new EREngineFilterContentRequest();
                        newPanel.GroupId[0] = newPanel.Battery == "Screener" ? 1 : 2;
                        eREngineFilterContentRequest.testGroupId = newPanel.GroupId[0];
                        var content = _elevateReportingEngineClient.GetFiltersContent(eREngineFilterContentRequest, userData).payload;
                        customError.isNoData = content.Count == 0 ? true : false;
                        customError.ResponseCount = $"The Content Call Returned with {content.Count} rows";
                        foreach (var contentItem in content)
                        {

                            DbContentScope singleDbContentScope = new DbContentScope();
                            singleDbContentScope.Acronym = "'" + contentItem.acronym + "'";
                            singleDbContentScope.Battery = contentItem.battery;
                            singleDbContentScope.IsDefault = contentItem.isDefault;
                            singleDbContentScope.SubtestName = contentItem.subtestName;
                            singleDbContentScope.isAlt = contentItem.isAlt;
                            singleDbContentScope.contentID = contentItem.id;
                            dbContentScopesList.Add(singleDbContentScope);
                        }

                        #endregion

                        var newFilter = _mapper.MapContentScope(dbContentScopesList, currentFilter, SetInvalidFilter);
                        newPanel.AddFilter(newFilter);
                        ContentScopeList = dbContentScopesList;

                    }
                    else
                    {
                        newPanel.AddFilter(currentFilter);
                    }
                }
                #endregion



                #region Location
                {
                    var selectedTestAdmin = newPanel.SelectedTestAdmin;
                    var locationLevels = GetLocationLevelsList(selectedTestAdmin.NodeType.ToUpper());
                    List<Location> locationsList = new List<Location>();
                    List<Buildings> buildingList = new List<Buildings>();
                    List<LocationPayload> location = new List<LocationPayload>();


                    #region MULTIPLE API CALL

                    if (userData.ElevateRole == "district_admin")
                    {

                        foreach (var item in userData.ParticipatedLocations.district_admin.schools)
                        {
                            Buildings build = new Buildings();
                            build.id = Convert.ToInt32(item.id);
                            build.name = item.name;
                            buildingList.Add(build);
                        }
                        EREngineFilterDistrictLocationsRequest eREngineFilterLocationsRequest = new EREngineFilterDistrictLocationsRequest();
                        eREngineFilterLocationsRequest.nodeId = selectedTestAdmin.NodeId.ToString();//TBD after working with elevatecustomer
                        eREngineFilterLocationsRequest.assignmentId = selectedTestAdmin.Id;//TBD after working with elevatecustomer
                        eREngineFilterLocationsRequest.gradeLevelId = newPanel.GradeLevel;//TBD after working with elevatecustomer
                        eREngineFilterLocationsRequest.buildings = buildingList;
                        eREngineFilterLocationsRequest.userRole = userData.ElevateRole;
                        var deBug = JsonConvert.SerializeObject(eREngineFilterLocationsRequest);
                        location = _elevateReportingEngineClient.GetFilltersLocation(eREngineFilterLocationsRequest, userData).payload;
                        customError.isNoData = location.Count < 3 ? true : false;
                        customError.ResponseCount = $"The Location Call Returned with {location.Count} rows";
                    }
                    else if (userData.ElevateRole == "Staff")
                    {

                        foreach (var item in userData.ParticipatedLocations.staff.schools)
                        {
                            Buildings build = new Buildings();
                            List<Sections> sections = new List<Sections>();

                            build.id = Convert.ToInt32(item.id);
                            build.name = item.name;

                            foreach (var itemS in item.sections)
                            {
                                Sections sSec = new Sections();
                                sSec.id = Convert.ToInt32(itemS.id);
                                sSec.name = itemS.name;
                                sections.Add(sSec);
                            }
                            build.sections = sections;
                            buildingList.Add(build);
                        }
                        EREngineFilterDistrictLocationsRequest eREngineFilterLocationsRequest = new EREngineFilterDistrictLocationsRequest();
                        eREngineFilterLocationsRequest.nodeId = userData.CustomerInfoList[0].NodeId.ToString();
                        eREngineFilterLocationsRequest.assignmentId = selectedTestAdmin.Id;
                        eREngineFilterLocationsRequest.gradeLevelId = newPanel.GradeLevel;
                        eREngineFilterLocationsRequest.buildings = buildingList;
                        eREngineFilterLocationsRequest.userRole = userData.ElevateRole;
                        var deBug = JsonConvert.SerializeObject(eREngineFilterLocationsRequest);
                        location = _elevateReportingEngineClient.GetFilltersLocation(eREngineFilterLocationsRequest, userData).payload;
                        customError.isNoData = location.Count < 2 ? true : false;
                        customError.ResponseCount = $"The Localtion Call Returned with {location.Count} rows";
                        locationLevels = new List<string> { "BUILDING", "CLASS" };
                    }
                    else if (userData.ElevateRole == "Teacher")
                    {
                        foreach (var item in userData.ParticipatedLocations.teacher.schools)
                        {
                            Buildings build = new Buildings();
                            List<Sections> sections = new List<Sections>();

                            build.id = Convert.ToInt32(item.id);
                            build.name = item.name;

                            foreach (var itemS in item.sections)
                            {
                                Sections sSec = new Sections();
                                sSec.id = Convert.ToInt32(itemS.id);
                                sSec.name = itemS.name;
                                sections.Add(sSec);
                            }
                            build.sections = sections;
                            buildingList.Add(build);
                        }
                        EREngineFilterDistrictLocationsRequest eREngineFilterLocationsRequest = new EREngineFilterDistrictLocationsRequest();
                        eREngineFilterLocationsRequest.nodeId = userData.CustomerInfoList[0].NodeId.ToString();
                        eREngineFilterLocationsRequest.assignmentId = selectedTestAdmin.Id;
                        eREngineFilterLocationsRequest.gradeLevelId = newPanel.GradeLevel;
                        eREngineFilterLocationsRequest.buildings = buildingList;
                        eREngineFilterLocationsRequest.userRole = userData.ElevateRole;
                        var deBug = JsonConvert.SerializeObject(eREngineFilterLocationsRequest);
                        location = _elevateReportingEngineClient.GetFilltersLocation(eREngineFilterLocationsRequest, userData).payload;
                        customError.isNoData = location.Count < 2 ? true : false;
                        customError.ResponseCount = $"The Localtion Call Returned with {location.Count} rows";
                        locationLevels = new List<string> { "BUILDING", "CLASS" };
                    }


                    #endregion
                    Filter newFilter = new Filter();
                    foreach (var locationLevel in locationLevels)
                    {
                        Enum.TryParse(locationLevel, true, out FilterType locationLevelEnum);
                        var currentFilter = currentPanel.GetFilterByType(locationLevelEnum);

                        if ((int)locationLevelEnum <= (int)updatedFilterType && currentFilter != null)
                        {
                            newPanel.AddFilter(currentFilter);
                            continue;
                        }
                        var locationsOfLevel = location.Where(l => l.nodeType.ToUpper() == locationLevel).ToList();
                        if (userData.ElevateRole == "district_admin")
                        {
                            newFilter = _mapper.MapChildLocations(location, currentFilter, locationLevelEnum, newPanel);
                        }
                        else if (userData.ElevateRole == "Staff" || userData.ElevateRole == "Teacher")
                        {
                            newFilter = _mapper.StaffAndTeacherMapChildLocations(location, currentFilter, locationLevelEnum, newPanel);
                        }
                        newPanel.AddFilter(newFilter);
                    }
                }

                #endregion



                #region Students
                if (newPanel.SelectedTestAdmin.NodeType == "building" || newPanel.SelectedTestAdmin.NodeType == "class")
                {
                    var currentFilter = currentPanel.GetFilterByType(FilterType.Student);
                    var selectedTestAdmin = newPanel.SelectedTestAdmin;
                    if (currentFilter == null || (int)updatedFilterType < currentFilter.TypeCode)
                    {
                        var classIdsString = newPanel.LastLocationFilter == null ? newPanel.SelectedTestAdmin.NodeId.ToString() : newPanel.LastLocationFilter.SelectedValuesString;

                        #region MULTIPLE API CALSS
                        EREngineFilterStudentsRequest eREngineFilterStudentsRequest = new EREngineFilterStudentsRequest();
                        var parentIds = newPanel.LocationFilters.Where(x => x.DisplayName == "Section").Select(x => x.Items.Select(y => y.DistrictIds)).ToList();
                        eREngineFilterStudentsRequest.nodeIdList = parentIds[0].ToList();
                        eREngineFilterStudentsRequest.customerId = selectedTestAdmin.CustomerId.ToString();
                        eREngineFilterStudentsRequest.nodeType = selectedTestAdmin.NodeType;
                        eREngineFilterStudentsRequest.assignmentId = selectedTestAdmin.Id;
                        eREngineFilterStudentsRequest.gradeLevelId = newPanel.GradeLevel;
                        List<string> contentStringIds = new List<string>();
                        List<int> contentIds = new List<int>();
                        if (newPanel.ContentIds.Count == 0)
                        {
                            contentStringIds = new List<string> { "1", "2", "3", "4", "5", "6", "7" };
                        }
                        else
                        {
                            contentStringIds = newPanel.ContentIds;
                        }
                        contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
                        eREngineFilterStudentsRequest.contentIds = contentIds;
                        List<string> classIds = new List<string>();
                        classIds = classIdsString.Split(',').ToList();
                        eREngineFilterStudentsRequest.classIds = classIds;
                        var students = _elevateReportingEngineClient.GetFilterStudents(eREngineFilterStudentsRequest, userData).payload;
                        customError.isNoData = students.Count == 0 ? true : false;
                        customError.ResponseCount = $"The Students Call Returned with {students.Count} rows";
                        #endregion

                        var newFilter = _mapper.MapStudents(students, currentFilter, updatedFilterType);
                        newPanel.AddFilter(newFilter);
                    }
                    else
                    {
                        newPanel.AddFilter(currentFilter);
                    }
                }
                #endregion

                #region Population filters
                {
                    var supressFlag = userData.PermissionIds.Contains(ServiceClient.DmServices.Models.PermissionsConstantElevate.AdminDataPrivacySuppressProgramDescriptionsFromReports);
                    var currentFilter = currentPanel.GetFilterByType(FilterType.PopulationFilters);
                    var selectedTestAdmin = newPanel.SelectedTestAdmin;
                    var classIdsString = newPanel.LastLocationFilter == null ? newPanel.SelectedTestAdmin.NodeId.ToString() : newPanel.LastLocationFilter.SelectedValuesString;
                    List<string> classIds = new List<string>();
                    classIds = classIdsString.Split(',').ToList();
                    if (currentFilter == null || (int)updatedFilterType <= currentFilter.TypeCode)
                    {
                        if (userData.ElevateRole == "district_admin")
                        {
                            var districtIdsList = newPanel.LocationFilters.Select(x => x.SelectedDistrictIds).ToList()[0];
                            var districtId = districtIdsList[0];
                            EREngineFilterPopulationRequest eREngineFilterPopulationRequest = new EREngineFilterPopulationRequest();
                            eREngineFilterPopulationRequest.nodeId = "";
                            eREngineFilterPopulationRequest.nodeIdList = new List<string> { selectedTestAdmin.NodeId.ToString() };
                            eREngineFilterPopulationRequest.customerId = selectedTestAdmin.CustomerId.ToString();
                            eREngineFilterPopulationRequest.nodeType = selectedTestAdmin.NodeType;
                            eREngineFilterPopulationRequest.testGroupId = newPanel.GroupId[0];
                            eREngineFilterPopulationRequest.assignmentId = selectedTestAdmin.Id;
                            eREngineFilterPopulationRequest.gradeLevelId = newPanel.GradeLevel;
                            eREngineFilterPopulationRequest.districtId = districtId;
                            List<string> contentStringIds = new List<string>();
                            List<int> contentIds = new List<int>();
                            if (newPanel.ContentIds.Count == 0)
                            {
                                contentStringIds = new List<string> { "7" };
                            }
                            else
                            {
                                contentStringIds = newPanel.ContentIds;
                            }
                            contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
                            eREngineFilterPopulationRequest.contentIds = contentIds;
                            var buildingIds = newPanel.LocationFilters.Where(x => x.DisplayName == "Building").Select(x => x.Items.Select(y => y.Value)).ToList();
                            List<string> studentIds = new List<string>() { "" };
                            eREngineFilterPopulationRequest.buildingIds = buildingIds[0].ToList();
                            eREngineFilterPopulationRequest.classIds = classIds;//classIds[0].ToList();
                            eREngineFilterPopulationRequest.studentIds = studentIds;
                            var JSON = _elevateReportingEngineClient.GetFiltersPopulation(eREngineFilterPopulationRequest, userData);
                            Dictionary<string, List<Disaggregation>> populationFilters = new Dictionary<string, List<Disaggregation>>();
                            populationFilters = JsonConvert.DeserializeObject<Dictionary<string, List<Disaggregation>>>(JSON.payload.ToString());
                            PopulationList = populationFilters;
                            var newFilter = _mapper.MapPopulationFilters(populationFilters, currentFilter, SetInvalidFilter);
                            newPanel.AddFilter(newFilter);
                        }
                        else if (userData.ElevateRole == "Staff")
                        {
                            var districtIdsList = newPanel.LocationFilters.Select(x => x.SelectedDistrictIds).ToList()[0];
                            var districtId = districtIdsList[0];
                            EREngineFilterPopulationRequest eREngineFilterPopulationRequest = new EREngineFilterPopulationRequest();
                            eREngineFilterPopulationRequest.nodeId = "";
                            eREngineFilterPopulationRequest.nodeIdList = new List<string> { selectedTestAdmin.NodeId.ToString() };
                            eREngineFilterPopulationRequest.customerId = selectedTestAdmin.CustomerId.ToString();
                            eREngineFilterPopulationRequest.nodeType = selectedTestAdmin.NodeType;
                            eREngineFilterPopulationRequest.testGroupId = newPanel.GroupId[0];
                            eREngineFilterPopulationRequest.assignmentId = selectedTestAdmin.Id;
                            eREngineFilterPopulationRequest.gradeLevelId = newPanel.GradeLevel;
                            eREngineFilterPopulationRequest.districtId = districtId;
                            List<string> contentStringIds = new List<string>();
                            List<int> contentIds = new List<int>();
                            if (newPanel.ContentIds.Count == 0)
                            {
                                contentStringIds = new List<string> { "7" };
                            }
                            else
                            {
                                contentStringIds = newPanel.ContentIds;
                            }
                            contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
                            eREngineFilterPopulationRequest.contentIds = contentIds;
                            var buildingIds = newPanel.LocationFilters.Where(x => x.DisplayName == "Building").Select(x => x.Items.Select(y => y.Value)).ToList();
                            List<string> studentIds = new List<string>() { "" };
                            eREngineFilterPopulationRequest.buildingIds = buildingIds[0].ToList();
                            eREngineFilterPopulationRequest.classIds = classIds;
                            eREngineFilterPopulationRequest.studentIds = studentIds;
                            var deBug = JsonConvert.SerializeObject(eREngineFilterPopulationRequest);
                            var JSON = _elevateReportingEngineClient.GetFiltersPopulation(eREngineFilterPopulationRequest, userData);
                            Dictionary<string, List<Disaggregation>> populationFilters = new Dictionary<string, List<Disaggregation>>();
                            populationFilters = JsonConvert.DeserializeObject<Dictionary<string, List<Disaggregation>>>(JSON.payload.ToString());
                            PopulationList = populationFilters;
                            var newFilter = _mapper.MapPopulationFilters(populationFilters, currentFilter, SetInvalidFilter);
                            newPanel.AddFilter(newFilter);
                        }
                        else if (userData.ElevateRole == "Teacher")
                        {
                            var districtIdsList = newPanel.LocationFilters.Select(x => x.SelectedDistrictIds).ToList()[0];
                            var districtId = districtIdsList[0];
                            EREngineFilterPopulationRequest eREngineFilterPopulationRequest = new EREngineFilterPopulationRequest();
                            eREngineFilterPopulationRequest.nodeId = "";
                            eREngineFilterPopulationRequest.nodeIdList = new List<string> { selectedTestAdmin.NodeId.ToString() };
                            eREngineFilterPopulationRequest.customerId = selectedTestAdmin.CustomerId.ToString();
                            eREngineFilterPopulationRequest.nodeType = selectedTestAdmin.NodeType;
                            eREngineFilterPopulationRequest.testGroupId = newPanel.GroupId[0];
                            eREngineFilterPopulationRequest.assignmentId = selectedTestAdmin.Id;
                            eREngineFilterPopulationRequest.gradeLevelId = newPanel.GradeLevel;
                            eREngineFilterPopulationRequest.districtId = districtId;
                            List<string> contentStringIds = new List<string>();
                            List<int> contentIds = new List<int>();
                            if (newPanel.ContentIds.Count == 0)
                            {
                                contentStringIds = new List<string> { "7" };
                            }
                            else
                            {
                                contentStringIds = newPanel.ContentIds;
                            }
                            contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
                            eREngineFilterPopulationRequest.contentIds = contentIds;
                            var buildingIds = newPanel.LocationFilters.Where(x => x.DisplayName == "Building").Select(x => x.Items.Select(y => y.Value)).ToList();
                            List<string> studentIds = new List<string>() { "" };
                            eREngineFilterPopulationRequest.buildingIds = buildingIds[0].ToList();
                            eREngineFilterPopulationRequest.classIds = classIds;
                            eREngineFilterPopulationRequest.studentIds = studentIds;
                            var deBug = JsonConvert.SerializeObject(eREngineFilterPopulationRequest);
                            var JSON = _elevateReportingEngineClient.GetFiltersPopulation(eREngineFilterPopulationRequest, userData);
                            Dictionary<string, List<Disaggregation>> populationFilters = new Dictionary<string, List<Disaggregation>>();
                            populationFilters = JsonConvert.DeserializeObject<Dictionary<string, List<Disaggregation>>>(JSON.payload.ToString());
                            PopulationList = populationFilters;
                            var newFilter = _mapper.MapPopulationFiltersTeacher(populationFilters, currentFilter, SetInvalidFilter, supressFlag);
                            newPanel.AddFilter(newFilter);
                        }

                        #region if students need to call
                        /*  if (newPanel.SelectedTestAdmin.NodeType == "building" || newPanel.SelectedTestAdmin.NodeType == "class")
                       {
                           EREngineFilterStudentsRequest eREngineFilterStudentsRequest = new EREngineFilterStudentsRequest();
                           eREngineFilterStudentsRequest.nodeId = newPanel.SelectedTestAdmin.NodeId.ToString();
                           eREngineFilterStudentsRequest.customerId = newPanel.SelectedTestAdmin.CustomerId.ToString();
                           eREngineFilterStudentsRequest.nodeType = newPanel.SelectedTestAdmin.NodeType;
                           eREngineFilterStudentsRequest.assignmnetProfileId = newPanel.SelectedTestAdmin.Id;
                           eREngineFilterStudentsRequest.gradeLevelId = newPanel.GradeLevel;
                           eREngineFilterStudentsRequest.contentIds = contentIds;
                           eREngineFilterStudentsRequest.ClassIds = classIds[0].ToList();
                           var students = _elevateReportingEngineClient.GetFilterStudents(eREngineFilterStudentsRequest).payload;
                           foreach (var item in students)
                           {
                               var studentId = item.Id;
                               studentIds.Add(studentId);
                           }
                       }*/
                        #endregion
                    }
                    else
                    {
                        newPanel.AddFilter(currentFilter);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (string.Equals(ex.Message, "Unauthorized", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(ex.Message);
                }
                var custError = JsonConvert.SerializeObject(customError);
                throw new Exception(custError);
            }
            return newPanel;
        }

        #region LEFTOVERS
        //public List<ScoreWarning> GetScoreWarnings(FilterPanel panel)
        //{
        //    var locationIds = panel.LastLocationFilter == null ? panel.SelectedTestAdmin.NodeId.ToString() : panel.LastLocationFilter.SelectedValuesString;
        //    var locationLevel = (panel.LastLocationFilter == null ? panel.SelectedTestAdmin.NodeType : panel.LastLocationFilter.Type.ToString()).ToUpper();

        //    var scoreWarnings = _dbClient.GetScoreWarnings(panel.ScoreSetId,
        //        locationIds,
        //        locationLevel, panel.AssessmentValue, panel.GradeLevel, panel.GetSelectedValuesStringOf(FilterType.Content),
        //        panel.ScoringOptions.AccountabilityFlag);

        //    return _typesMapper.Map<List<DbScoreWarning>, List<ScoreWarning>>(scoreWarnings);
        //}
        #endregion


        private void SetInvalidFilter(Filter filter)
        {
            if (InvalidFilter != null)
                return;

            InvalidFilter = filter;
            InvalidFilter.DisplayName = filter.Type.ToString();
        }

        private List<string> GetLocationLevelsList(string parentNodeType)
        {
            var types = new List<string> { "STATE", "REGION", "SYSTEM", "DISTRICT", "BUILDING", "CLASS" };
            var index = types.IndexOf(parentNodeType.ToUpper());
            return types.GetRange(index + 1, types.Count - index - 1);
        }
    }
    public class CustomError
    {
        public bool isNoData { get; set; }
        public string ResponseCount { get; set; }
    }

}