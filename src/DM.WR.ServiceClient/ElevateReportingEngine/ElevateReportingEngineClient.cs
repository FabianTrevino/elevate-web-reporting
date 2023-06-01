using Flurl.Http;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DM.WR.Models.Config;
using System;
using DM.WR.Models.ElevateReportingEngine;
using DM.WR.ServiceClient.ScoreManagerApi.Models;
using DM.WR.Models.ScoreManagerApi;
using DM.WR.Models.Types;
using ForegroundReporting;
using System.Linq;

namespace DM.WR.ServiceClient.ElevateReportingEngine
{
    public interface IElevateReportingEngineClient
    {
        List<ForegroundReporting.Lib.Models.Responses.StudentSubtest> GetAllDataForCogatDashBoard(EREngineRequestModel ereRequestModel, UserData userData);

        EREngineAgeStanineResponse GetAgeStanines(EREngineRequestModel ereRequestModel, UserData userData);

        EREngineGroupTotalResponse GetGroupTotalForCogatDashBoard(EREngineGroupRequestModel ereRequestModel, UserData userData);

        //------------Call Filters----------------//

        EREngineFiltersTestAssignmentResponse GetFiltersTestAssignment(EREngineFilterTestAssignmentAndGradeRequest EREtaG, UserData userData);
        EREngineFiltersTestAssignmentResponse GetFiltersTestAssignmentListForRoles(EREngineFilterTestAssignmentAndGradeRequest EREtaG, UserData userData);
        EREngineFiltersGradeResponse GetFiltersGrades(EREngineFilterTestAssignmentAndGradeRequest EREtaG, UserData userData);

        EREngineFiltersContentResponse GetFiltersContent(EREngineFilterContentRequest ERECon, UserData userData);

        EREngineFiltersLocationResponse GetFilltersLocation(EREngineFilterDistrictLocationsRequest EREloc, UserData userData);
        EREngineFiltersLocationResponse GetBuildingFilltersLocation(EREngineFilterBuildingLocationsRequest EREloc, UserData userData);
        EREngineFiltersLocationResponse GetClassesFilltersLocation(EREngineFilterClassLocationsRequest EREloc, UserData userData);

        EREngineFiltersStudentResponse GetFilterStudents(EREngineFilterStudentsRequest EREStu, UserData userData);

        EREngineFiltersPopulationResponse GetFiltersPopulation(EREngineFilterPopulationRequest EREpop,UserData userData);
        EREngineFiltersPopulationResponse GetBuildingFiltersPopulation(EREngineFilterBuildingPopulationRequest EREpop, UserData userData);
        EREngineFiltersLocationResponse GetBackgroundFilltersLocation(EREngineBackgroundFilterLocationRequest EREloc, UserData userData);
        EREngineBackgrondStudentResponse GetFilterBackgroundStudents(EREngineBackgroundStudentsRequest EREStu, UserData userData);

    }
    public class ElevateReportingEngineClient : IElevateReportingEngineClient
    {
        ///testassignments
        public string elevFiltersUrl = ConfigSettings.ElevateReportingEngineUrl + @"reporting/filters/";
        
        
        public EREngineFiltersPopulationResponse GetFiltersPopulation(EREngineFilterPopulationRequest EREpop, UserData userData)
        {
            var endpoint = elevFiltersUrl + "populationfilters";
            EREngineFiltersPopulationResponse population = new EREngineFiltersPopulationResponse();
            Task<string> retJson = null;
            try
            {
                retJson = endpoint.WithHeader("Authorization",userData.ElevateIdToken).PostJsonAsync(EREpop).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                population = JsonConvert.DeserializeObject<EREngineFiltersPopulationResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return population;
        }

        public EREngineFiltersPopulationResponse GetBuildingFiltersPopulation(EREngineFilterBuildingPopulationRequest EREpop, UserData userData)
        {
            var endpoint = elevFiltersUrl + "PopulationFilters";
            EREngineFiltersPopulationResponse population = new EREngineFiltersPopulationResponse();
            Task<string> retJson = null;
            try
            {
                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREpop).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                population = JsonConvert.DeserializeObject<EREngineFiltersPopulationResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return population;
        }

        public EREngineFiltersStudentResponse GetFilterStudents(EREngineFilterStudentsRequest EREStu, UserData userData)
        {
            var endpoint = elevFiltersUrl + "students";
            EREngineFiltersStudentResponse students = new EREngineFiltersStudentResponse();
            Task<string> retJson = null;
            try
            {
                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREStu).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<EREngineFiltersStudentResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return students;
        }

        public EREngineBackgrondStudentResponse GetFilterBackgroundStudents(EREngineBackgroundStudentsRequest EREStu, UserData userData)
        {
            var endpoint = elevFiltersUrl + "printreport/students";
            EREngineBackgrondStudentResponse students = new EREngineBackgrondStudentResponse();
            Task<string> retJson = null;
            try
            {
                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREStu).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<EREngineBackgrondStudentResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return students;
        }
        public EREngineFiltersLocationResponse GetFilltersLocation(EREngineFilterDistrictLocationsRequest EREloc, UserData userData)
        {
            var endpoint = elevFiltersUrl + "locations"; 
            EREngineFiltersLocationResponse location = new EREngineFiltersLocationResponse();
            Task<string> retJson = null;
            try
            {

                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREloc).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                location = JsonConvert.DeserializeObject<EREngineFiltersLocationResponse>(retJson.Result); 

            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return location;


        }
        public EREngineFiltersLocationResponse GetBackgroundFilltersLocation(EREngineBackgroundFilterLocationRequest EREloc, UserData userData)
        {
            var endpoint = elevFiltersUrl + "printreport/locations";
            EREngineFiltersLocationResponse location = new EREngineFiltersLocationResponse();
            Task<string> retJson = null;
            try
            {

                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREloc).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                location = JsonConvert.DeserializeObject<EREngineFiltersLocationResponse>(retJson.Result);

            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return location;
        }
        public EREngineFiltersLocationResponse GetBuildingFilltersLocation(EREngineFilterBuildingLocationsRequest EREloc, UserData userData)
        {
            var endpoint = elevFiltersUrl + "Buildings";
            EREngineFiltersLocationResponse location = new EREngineFiltersLocationResponse();
            Task<string> retJson = null;
            try
            {

                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREloc).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                location = JsonConvert.DeserializeObject<EREngineFiltersLocationResponse>(retJson.Result);

            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return location;


        }
        public EREngineFiltersLocationResponse GetClassesFilltersLocation(EREngineFilterClassLocationsRequest EREloc, UserData userData)
        {
            var endpoint = elevFiltersUrl + "Classes";
            EREngineFiltersLocationResponse location = new EREngineFiltersLocationResponse();
            Task<string> retJson = null;
            try
            {

                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREloc).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                location = JsonConvert.DeserializeObject<EREngineFiltersLocationResponse>(retJson.Result);

            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw;
            }
            return location;


        }
        public EREngineFiltersContentResponse GetFiltersContent(EREngineFilterContentRequest ERECon, UserData userData)
        {
            var endpoint = elevFiltersUrl + "contents";
            EREngineFiltersContentResponse content = new EREngineFiltersContentResponse();
            Task<string> retJson = null;
            try
            {
                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(ERECon).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                content = JsonConvert.DeserializeObject<EREngineFiltersContentResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {

                throw ex;
            }
            return content;
        }

        public EREngineFiltersGradeResponse GetFiltersGrades(EREngineFilterTestAssignmentAndGradeRequest EREtaG, UserData userData)
        {
            var endpoint = elevFiltersUrl + "grades";
            EREngineFiltersGradeResponse grades = new EREngineFiltersGradeResponse();
            Task<string> retJson = null;
            try
            {
                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREtaG).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                grades = JsonConvert.DeserializeObject <EREngineFiltersGradeResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {

                throw;
            }
            catch (FlurlHttpException ex)
            {
                throw ex;
            }
            return grades;
        }
        public EREngineFiltersTestAssignmentResponse GetFiltersTestAssignment( EREngineFilterTestAssignmentAndGradeRequest EREtaG, UserData userData)
        {
            var endpoint = elevFiltersUrl + "testassignments";
            EREngineFiltersTestAssignmentResponse testAssignments = new EREngineFiltersTestAssignmentResponse();
            Task<string> retJson = null;
            var status = "";
            try
            {
                retJson = endpoint.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREtaG)
                    .GetAwaiter().GetResult().Content.ReadAsStringAsync();
                testAssignments = JsonConvert.DeserializeObject<EREngineFiltersTestAssignmentResponse>(retJson.Result);

            }
            catch (FlurlHttpTimeoutException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                
                throw ex;
            }
            return testAssignments;
        }

        public EREngineFiltersTestAssignmentResponse GetFiltersTestAssignmentListForRoles(EREngineFilterTestAssignmentAndGradeRequest EREtaG, UserData userData)
        {
            var endpointNoLocation = elevFiltersUrl + "testassignmentsnolocations";
            var endpointLocation = elevFiltersUrl + "testassignmentslocations";

            EREngineFiltersTestAssignmentResponse ListTestAssignments = new EREngineFiltersTestAssignmentResponse();
            var retJson = "";
            EREngineFiltersTestAssignmentResponse testAssignments = new EREngineFiltersTestAssignmentResponse();
            EREngineFiltersTestAssignmentsLocationResponse testAssignmentsLocation = new EREngineFiltersTestAssignmentsLocationResponse();
            var index = 0;
            try
            {
                retJson = endpointNoLocation.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREtaG).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result;
                testAssignments = JsonConvert.DeserializeObject<EREngineFiltersTestAssignmentResponse>(retJson);
                if (EREtaG.assignmentId == 0)
                {
                    EREtaG.assignmentId = testAssignments.payload[0].id;
                }
                retJson = endpointLocation.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(EREtaG).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result;
                testAssignmentsLocation = JsonConvert.DeserializeObject<EREngineFiltersTestAssignmentsLocationResponse>(retJson);
                
                foreach (var item in testAssignments.payload)
                {
                    item.nodeId = testAssignmentsLocation.payload[0].nodeId;
                    item.nodeName = testAssignmentsLocation.payload[0].nodeName;
                    item.nodeType = testAssignmentsLocation.payload[0].nodeType;
                    index++;
                }
            }
            catch (FlurlHttpTimeoutException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                throw new Exception(ex.Call.Response.StatusCode.ToString());
            }
            return testAssignments;
        }
        public EREngineAgeStanineResponse GetAgeStanines(EREngineRequestModel ereRequestModel, UserData userData)
        {
            EREngineAgeStanineResponse retVal = new EREngineAgeStanineResponse();
            Task<string> retJson = null;
            try
            {
                var url = ConfigSettings.ElevateBaseUrl + "/elevate-reporting-engine/api/AgeStanine/GetAgeStanine";
                retJson = url.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(ereRequestModel).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                retVal = JsonConvert.DeserializeObject<EREngineAgeStanineResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                throw ex;
            }

            return retVal;
        }

        public List<ForegroundReporting.Lib.Models.Responses.StudentSubtest> GetAllDataForCogatDashBoard(EREngineRequestModel ereRequestModel, UserData userData)
        {
            List<ForegroundReporting.Lib.Models.Responses.StudentSubtest> studentSubtests = new List<ForegroundReporting.Lib.Models.Responses.StudentSubtest>();
            Task<string> retJson = null;
            try
            {
                //var url = ConfigSettings.ElevateReportingEngineUrl + "reporting/studentscore";
                //retJson = url.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(ereRequestModel).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                //studentSubtests = JsonConvert.DeserializeObject<List<StudentSubtest>>(retJson.Result);
                ForegroundReporting.Lib.Models.Requests.StudentScoresRequest req = new ForegroundReporting.Lib.Models.Requests.StudentScoresRequest();
                req.APL = ereRequestModel.apl;
                req.AssignmentId = ereRequestModel.assignmentId;
                req.BuildingIds = ereRequestModel.buildingIds;
                req.ClassIds = ereRequestModel.classIds;
                req.ContentIds = ereRequestModel.contentIds;
                req.CustomerId = ereRequestModel.customerId;
                req.DistrictIds = ereRequestModel.districtIds;
                req.GradeLevel = ereRequestModel.gradeLevel;
                req.PageLimit = ereRequestModel.pageLimit;
                req.PageOffset = ereRequestModel.pageOffset;
                req.ReportCriteria = ereRequestModel.reportCriteria;
                req.Role = ereRequestModel.role;
                req.StudentIds = ereRequestModel.studentIds;
                req.DimensionGroupValueIds = ereRequestModel.dimensonValueList.Select(s => Convert.ToInt32(s)).ToList();
                var connString = ConfigSettings.ConnectionString;
                ForegroundReporting.Lib.Service.StudentScoreService studentScoreService = new ForegroundReporting.Lib.Service.StudentScoreService(connString);
                studentSubtests = studentScoreService.GetStudentsScore(req);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return studentSubtests;
        }
        //GroupTotal
        public EREngineGroupTotalResponse GetGroupTotalForCogatDashBoard(EREngineGroupRequestModel ereRequestModel, UserData userData)
        {
            EREngineGroupTotalResponse retVal = new EREngineGroupTotalResponse();
            Task<string> retJson = null;
            try
            {
                var url = ConfigSettings.ElevateReportingEngineUrl + "reporting/groupscore";
                retJson = url.WithHeader("Authorization", userData.ElevateIdToken).PostJsonAsync(ereRequestModel).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                retVal = JsonConvert.DeserializeObject<EREngineGroupTotalResponse>(retJson.Result);
            }
            catch (FlurlHttpTimeoutException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                throw new Exception(ex.Call.Response.StatusCode.ToString());
            }

            return retVal;
        }
    }
}
