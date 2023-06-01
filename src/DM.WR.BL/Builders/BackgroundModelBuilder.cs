using AutoMapper;
using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.CogAt;
using DM.WR.Models.CogAt.elevate;
using DM.WR.Models.ElevateReportingEngine;
using DM.WR.Models.Types;
using DM.WR.ServiceClient.ElevateReportingEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.BL.Builders
{
    public interface IBackgroundModelBuilder
    {
        FilterPanel BuildBackgroundModal(FilterPanel currentPanel, UserData userData);
    }
    public class BackgroundModelBuilder : IBackgroundModelBuilder
    {
        private readonly CogatFiltersMapper _mapper;
        private readonly IMapper _typesMapper;
        public readonly IElevateReportingEngineClient _elevateReportingEngineClient;
        public Filter InvalidFilter { get; private set; }

        public BackgroundModelBuilder(IElevateReportingEngineClient elevateReportingEngineClient)
        {
            _mapper = new CogatFiltersMapper();
            _typesMapper = new DbTypesMapper();
            _elevateReportingEngineClient = elevateReportingEngineClient;
        }

        public FilterPanel BuildBackgroundModal(FilterPanel currentPanel,UserData userData)
        {
            var newPanel = new FilterPanel();
            var selectedLocation = "";
            var customError = new CustomError();
            List<GradeLevel> gradeLevelsList = new List<GradeLevel>();
            List<Class> listClass = new List<Class>();
            List<Building> listBuilding = new List<Building>();
            List<District> listDistrict = new List<District>();
            List<LocationPayload> location = new List<LocationPayload>();
            try
            {
                #region Grade
                {
                    var currentFilter = currentPanel.GetFilterByType(FilterType.Grade);
                    {
                        var selectedTestAdmin = currentPanel.SelectedTestAdmin;

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
                        var newFilter = _mapper.MapGradeLevelsBackground(gradeLevelsList, currentFilter, SetInvalidFilter);
                        newPanel.AddFilter(newFilter);
                    }
                }
                #endregion

                #region Location
                {
                    var selectedTestAdmin = currentPanel.SelectedTestAdmin;
                    var locationLevels = GetLocationLevelsList(selectedTestAdmin.NodeType.ToUpper());
                    List<Location> locationsList = new List<Location>();
                    District district = new District();
                    Building building = new Building();
                    Class classes = new Class();

                    

                    #region MULTIPLE API CALL
                    
                    if (userData.ElevateRole == "district_admin")
                    {
                        var districtIds = currentPanel.LocationFilters.Where(x => x.DisplayName == "Building").Select(x => x.SelectedDistrictIds).ToList();
                        EREngineBackgroundFilterLocationRequest eREngineFilterLocationsRequest = new EREngineBackgroundFilterLocationRequest();
                        eREngineFilterLocationsRequest.nodeId = new List<string>{ userData.CustomerInfoList[0].NodeId};
                        eREngineFilterLocationsRequest.assignmentId = selectedTestAdmin.Id;
                        eREngineFilterLocationsRequest.grades = gradeLevelsList.Select(x => Convert.ToInt32(x.Grade)).ToList();
                        eREngineFilterLocationsRequest.nodeType = selectedTestAdmin.NodeType;
                        location = _elevateReportingEngineClient.GetBackgroundFilltersLocation(eREngineFilterLocationsRequest, userData).payload;
                        var deBugRequest = JsonConvert.SerializeObject(eREngineFilterLocationsRequest);
                        var deBug = JsonConvert.SerializeObject(location);
                        customError.isNoData = location.Count < 3 ? true : false;
                        customError.ResponseCount = $"The Location Call Returned with {location.Count} rows";
                    }
                    else if (userData.ElevateRole == "Staff")
                    {
                        List<string> buildingIds = new List<string>();
                        foreach (var item in userData.ParticipatedLocations.staff.schools)
                        {
                            var buildingId = item.id;
                            buildingIds.Add(buildingId);
                        }
                        EREngineBackgroundFilterLocationRequest eREngineFilterLocationsRequest = new EREngineBackgroundFilterLocationRequest();
                        eREngineFilterLocationsRequest.nodeId = buildingIds;
                        eREngineFilterLocationsRequest.assignmentId = selectedTestAdmin.Id;
                        eREngineFilterLocationsRequest.grades = gradeLevelsList.Select(x => Convert.ToInt32(x.Grade)).ToList();
                        eREngineFilterLocationsRequest.nodeType = selectedTestAdmin.NodeType;
                        location = _elevateReportingEngineClient.GetBackgroundFilltersLocation(eREngineFilterLocationsRequest, userData).payload;
                        var deBugRequest = JsonConvert.SerializeObject(eREngineFilterLocationsRequest);
                        var deBug = JsonConvert.SerializeObject(location);
                        customError.isNoData = location.Count < 2 ? true : false;
                        customError.ResponseCount = $"The Localtion Call Returned with {location.Count} rows";
                        locationLevels = new List<string> { "BUILDING", "CLASS" };
                    }
                    if (userData.ElevateRole == "Teacher")
                    {
                        List<string> classIds = new List<string>();
                        foreach (var item in userData.ParticipatedLocations.teacher.schools)
                        {
                            foreach (var sectionItem in item.sections)
                            {
                                var sectionId  = sectionItem.id;
                                classIds.Add(sectionId);
                            }
                        }
                        EREngineBackgroundFilterLocationRequest eREngineFilterLocationsRequest = new EREngineBackgroundFilterLocationRequest();
                        eREngineFilterLocationsRequest.nodeId = classIds;
                        eREngineFilterLocationsRequest.assignmentId = selectedTestAdmin.Id;
                        eREngineFilterLocationsRequest.grades = gradeLevelsList.Select(x => Convert.ToInt32(x.Grade)).ToList();
                        eREngineFilterLocationsRequest.nodeType = "class";
                        location = _elevateReportingEngineClient.GetBackgroundFilltersLocation(eREngineFilterLocationsRequest, userData).payload;
                        var deBugRequest = JsonConvert.SerializeObject(eREngineFilterLocationsRequest);
                        var deBug = JsonConvert.SerializeObject(location);
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

                        var locationsOfLevel = location.Where(l => l.nodeType.ToUpper() == locationLevel).ToList();
                        if (userData.ElevateRole == "district_admin")
                        {
                            newFilter = _mapper.MapChildLocationsForbackground(location, currentFilter, locationLevelEnum, newPanel, currentPanel);
                        }
                        else if (userData.ElevateRole == "Staff" || userData.ElevateRole == "Teacher")
                        {
                            newFilter = _mapper.StaffAndTeacherMapChildLocationsForbackground(location, currentFilter, locationLevelEnum, newPanel, currentPanel);
                        }
                        newPanel.AddFilter(newFilter);
                    }
                }
                #endregion


                #region Students
                if (userData.ElevateRole == "Staff")
                {
                    var currentFilter = currentPanel.GetFilterByType(FilterType.Student);
                    var selectedTestAdmin = currentPanel.SelectedTestAdmin;
                    //List<string> buildingIds = new List<string>();
                    //foreach (var item in userData.ParticipatedLocations.staff.schools)
                    //{
                    //    var buildingId = item.id;
                    //    buildingIds.Add(buildingId);
                    //}

                    //foreach (var item in userData.ParticipatedLocations.teacher.schools)
                    //{
                    //    foreach (var sectionItem in item.sections)
                    //    {
                    //        var sectionId = sectionItem.id;
                    //        classIds.Add(sectionId);
                    //    }
                    //}
                    #region MULTIPLE API CALSS
                    EREngineBackgroundStudentsRequest eREngineFilterStudentsRequest = new EREngineBackgroundStudentsRequest();
                    var buildingIds = location.Where(x => x.nodeType == "BUILDING").Select(y => y.id).ToList();
                    var classIds = location.Where(x => x.nodeType == "CLASS").Select(y => y.id).ToList();
                    eREngineFilterStudentsRequest.nodeIdList = buildingIds;
                    eREngineFilterStudentsRequest.customerId = selectedTestAdmin.CustomerId.ToString();
                    eREngineFilterStudentsRequest.nodeType = selectedTestAdmin.NodeType;
                    eREngineFilterStudentsRequest.assignmentId = selectedTestAdmin.Id;
                    eREngineFilterStudentsRequest.grades = gradeLevelsList.Select(x => Convert.ToInt32(x.Grade)).ToList(); ;
                    List<string> contentStringIds = new List<string>();
                    List<int> contentIds = new List<int>();
                    if (currentPanel.ContentIds.Count == 0)
                    {
                        contentStringIds = new List<string> { "1", "2", "3", "4", "5", "6", "7" };
                    }
                    else
                    {
                        contentStringIds = currentPanel.ContentIds;
                    }
                    contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
                    eREngineFilterStudentsRequest.contentIds = contentIds;
                    eREngineFilterStudentsRequest.classIds = classIds;
                    var students = _elevateReportingEngineClient.GetFilterBackgroundStudents(eREngineFilterStudentsRequest, userData).payload;
                    var deBugRequest = JsonConvert.SerializeObject(eREngineFilterStudentsRequest);
                    var deBug = JsonConvert.SerializeObject(students);
                    customError.isNoData = students.Count == 0 ? true : false;
                    customError.ResponseCount = $"The Students Call Returned with {students.Count} rows";
                    #endregion
                    var newFilter = _mapper.MapStudentsForBackground(students, currentFilter);
                    newPanel.AddFilter(newFilter);
                }
                if (userData.ElevateRole == "Teacher")
                {
                    var currentFilter = currentPanel.GetFilterByType(FilterType.Student);
                    var selectedTestAdmin = currentPanel.SelectedTestAdmin;

                    var classIds = location.Where(x => x.nodeType == "CLASS").Select(y => y.id).ToList();

                    #region MULTIPLE API CALSS
                    EREngineBackgroundStudentsRequest eREngineFilterStudentsRequest = new EREngineBackgroundStudentsRequest();
                    eREngineFilterStudentsRequest.nodeIdList = classIds;
                    eREngineFilterStudentsRequest.customerId = selectedTestAdmin.CustomerId.ToString();
                    eREngineFilterStudentsRequest.nodeType = "class";
                    eREngineFilterStudentsRequest.assignmentId = selectedTestAdmin.Id;
                    eREngineFilterStudentsRequest.grades = gradeLevelsList.Select(x => Convert.ToInt32(x.Grade)).ToList(); ;
                    List<string> contentStringIds = new List<string>();
                    List<int> contentIds = new List<int>();
                    if (currentPanel.ContentIds.Count == 0)
                    {
                        contentStringIds = new List<string> { "1", "2", "3", "4", "5", "6", "7" };
                    }
                    else
                    {
                        contentStringIds = currentPanel.ContentIds;
                    }
                    contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
                    eREngineFilterStudentsRequest.contentIds = contentIds;
                    eREngineFilterStudentsRequest.classIds = classIds;
                    var students = _elevateReportingEngineClient.GetFilterBackgroundStudents(eREngineFilterStudentsRequest, userData).payload;
                    var deBugRequest = JsonConvert.SerializeObject(eREngineFilterStudentsRequest);
                    var deBug = JsonConvert.SerializeObject(students);
                    customError.isNoData = students.Count == 0 ? true : false;
                    customError.ResponseCount = $"The Students Call Returned with {students.Count} rows";
                    #endregion
                    var newFilter = _mapper.MapStudentsForBackground(students, currentFilter);
                    newPanel.AddFilter(newFilter);
                }
                #endregion
            }
            catch (Exception ex)
            {
                var custError = JsonConvert.SerializeObject(customError);
                throw new Exception(custError);
            }
            return newPanel;
        }

        private List<string> GetLocationLevelsList(string parentNodeType)
        {
            var types = new List<string> { "STATE", "REGION", "SYSTEM", "DISTRICT", "BUILDING", "CLASS" };
            var index = types.IndexOf(parentNodeType.ToUpper());
            return types.GetRange(index + 1, types.Count - index - 1);
        }

        private void SetInvalidFilter(Filter filter)
        {
            if (InvalidFilter != null)
                return;

            InvalidFilter = filter;
            InvalidFilter.DisplayName = filter.Type.ToString();
        }
    }
}
