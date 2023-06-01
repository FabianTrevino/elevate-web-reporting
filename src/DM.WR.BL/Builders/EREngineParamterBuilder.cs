using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.WR.Models.ElevateReportingEngine;
using DM.WR.Models.CogAt;
using DM.WR.Models.Types;
using DM.WR.ServiceClient.ElevateReportingEngine;
using DM.WR.Models.BackgroundReport.ElevateReports;
using DM.WR.Models.Config;

namespace DM.WR.BL.Builders
{
    public interface IEREngineParamterBuilder
    {
        EREngineRequestModel BuildGetAllDataParameters(FilterPanel currentFilter, UserData userData);
        EREngineRequestModel BuidAgeStanineParamters(FilterPanel currentFilter, UserData userData);
        ElevateCogatListOfStudentScores BuildCogatListOfStudentScoresBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData);
        EREngineGroupRequestModel BuildGetGroupTotalParamters(FilterPanel currentFilter, UserData userData);
        ElevateCogatProfileNarrative BuildCogatProfileNarrativeBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData);
        ElevateCogatGroupSummary BuildCogatGroupSummaryBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData);
        ElevateCogatDataExporter BuildCogatDataExporterBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData);
    }
    public class EREngineParamterBuilder : IEREngineParamterBuilder
    {
        private readonly IElevateReportingEngineClient _elevateReportingEngineClient;
        public EREngineParamterBuilder(IElevateReportingEngineClient elevateReportingEngineClient)
        {
            _elevateReportingEngineClient = elevateReportingEngineClient;
        }
        public EREngineRequestModel BuildGetAllDataParameters(FilterPanel currentFilter, UserData userData)
        {

            Population population = new Population();
            StudentProgramCode studentProgramCode = new StudentProgramCode();
            StudentAdminCode studentAdminCode = new StudentAdminCode();
            EREngineRequestModel eREngineRequestModel = new EREngineRequestModel();

            var populationList = currentFilter.PopulationList;


            //---------------------------------------------//
            try
            {
                string reportCriteria = "";
                string apl = null;
                int pageOffset = 1;
                int pageLimit = 1000000;
                string sortField = "studentname";
                string sortDirection = "ASC";



                var classIdsString = currentFilter.LastLocationFilter == null ? currentFilter.SelectedTestAdmin.NodeId.ToString() : currentFilter.LastLocationFilter.SelectedValuesString;
                List<string> classIds = new List<string>();
                classIds = classIdsString.Split(',').ToList();

                eREngineRequestModel.assignmentId = currentFilter.SelectedTestAdmin.Id;
                eREngineRequestModel.customerId = userData.CustomerInfoList[0].CustomerId.ToString();
                eREngineRequestModel.role = userData.ElevateRole;
                if (userData.ElevateRole == "district_admin")
                {
                    var buildingIds = currentFilter.LocationFilters.Where(x => x.DisplayName == "Building").Select(x => x.SelectedValues).ToList();
                    eREngineRequestModel.buildingIds = buildingIds[0];
                }
                var districtIdList = currentFilter.LocationFilters.Where(x => x.DisplayName == "Building").Select(x => x.Items.Select(y => y.DistrictIds)).ToList();
                if (currentFilter.HasStudentsFilter) // TODO: Make Other flags for filters
                {
                    var studentIds = currentFilter.GetFilterByType(FilterType.Student).SelectedValues;
                    eREngineRequestModel.studentIds = studentIds;
                }
                
                //var ClassIds = currentFilter.LocationFilters.Where(x => x.DisplayName == "Section").Select(x => x.SelectedAltValues).ToList();
                eREngineRequestModel.districtIds = districtIdList[0].ToList();
                eREngineRequestModel.classIds = classIds;
                List<string> contentStringIds = new List<string>();
                List<int> contentIds = new List<int>();
                if (currentFilter.ContentIds.Count == 0)
                {
                    contentStringIds = new List<string> { "1" };
                }
                else
                {
                    contentStringIds = currentFilter.ContentIds;
                }
                contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
                eREngineRequestModel.contentIds = contentIds;
                eREngineRequestModel.gradeLevel = Convert.ToInt32(currentFilter.Grade);
                eREngineRequestModel.population = population;
                eREngineRequestModel.reportCriteria = reportCriteria;
                eREngineRequestModel.apl = apl;
                eREngineRequestModel.pageOffset = pageOffset;
                eREngineRequestModel.pageLimit = pageLimit;
                eREngineRequestModel.sortField = sortField;
                eREngineRequestModel.sortDirection = sortDirection;
                eREngineRequestModel.dimensonValueList = populationList.Keys.ToList();


            }
            catch (Exception ex)
            {

                throw ex;
            }

            return eREngineRequestModel;
        }
        public EREngineGroupRequestModel BuildGetGroupTotalParamters(FilterPanel currentFilter, UserData userData)
        {

            StudentRaces studentRaces = new StudentRaces();
            StudentPrograms studentPrograms = new StudentPrograms();
            StudentCustomCodings studentCustomCodings = new StudentCustomCodings();
            PopulationNew populationNew = new PopulationNew();
            EREngineGroupRequestModel eREngineRequestModel = new EREngineGroupRequestModel();

            string reportCriteria = "";
            object apl = null;
            int pageOffset = 1;
            int pageLimit = 1000;
            string sortField = "studentname";
            string sortDirection = "ASC";
            string groupRole = "";
            
            
            
            studentRaces.id = 0;
            studentRaces.name = "";
            studentPrograms.programId = 0;
            studentPrograms.name = "";
            studentCustomCodings.name = "";
            studentCustomCodings.value = "";


            var populationKeysAndValues = GetPopulationKeyAndValue(currentFilter.PopulationList);
            var populationTextList = currentFilter.PopulationList.Values.ToList();
            if (populationTextList.Contains("Hispanic or Latino"))
            {
                populationNew.isHispanic = populationTextList.Contains("Hispanic or Latino");
            }
            populationNew.genders = GetGenderString(populationTextList);
            var programPopulationDict = GetProgramPopulationNew(populationKeysAndValues);
            var ethPopulationDict = GetEthnicityPopulation(populationKeysAndValues);
            populationNew.studentRaces = MapStudentRaces(ethPopulationDict);
            populationNew.studentPrograms = MapStudentPrograms(programPopulationDict);
            populationNew.studentCustomCodings = new List<StudentCustomCodings>();
            eREngineRequestModel.assignmentId = currentFilter.SelectedTestAdmin.Id;
            eREngineRequestModel.customerId = null;
            if (userData.ElevateRole == "district_admin")
            {
                groupRole = "district";
            }
            else if (userData.ElevateRole == "Staff")
            {
                groupRole = "building";
            }
            else if (userData.ElevateRole == "Teacher")
            {
                groupRole = "building";
            }

            if (currentFilter.HasStudentsFilter) // TODO: Make Other flags for filters
            {
                var studentIds = currentFilter.GetFilterByType(FilterType.Student).SelectedValues;
                eREngineRequestModel.studentIds = studentIds;
            }
            var districtIdList = currentFilter?.LocationFilters?.Where(x => x.DisplayName == "Building").Select(x => x.Items.Select(y => y.DistrictIds)).ToList();
            eREngineRequestModel.districtIds = new List<string>();//districtIdList[0].ToList(); //TODO: question for abdul, doesn't this need to populated?
            eREngineRequestModel.role = groupRole;
            var buildingIds = currentFilter?.LocationFilters?.Where(x => x.DisplayName == "Building").Select(x => x.SelectedValues).ToList();
            eREngineRequestModel.buildingIds = buildingIds?.FirstOrDefault().ToList();
            var classIds = currentFilter?.LocationFilters?.Where(x => x.DisplayName == "Section").Select(x => x.SelectedValues).ToList();
            eREngineRequestModel.classIds = classIds?.FirstOrDefault().ToList();
            eREngineRequestModel.contents = GetContentAcronym(currentFilter.Content);
            eREngineRequestModel.gradeLevel = Convert.ToInt32(currentFilter.Grade);
            eREngineRequestModel.gradeName = currentFilter.GroupId[0] == 1 ? "" : "Screener";
            eREngineRequestModel.population = populationNew;
            eREngineRequestModel.reportCriteria = reportCriteria;
            eREngineRequestModel.apl = apl;
            eREngineRequestModel.pageOffset = pageOffset;
            eREngineRequestModel.pageLimit = pageLimit;
            eREngineRequestModel.sortField = sortField;
            eREngineRequestModel.sortDirection = sortDirection;



            return eREngineRequestModel;
        }

        public EREngineRequestModel BuidAgeStanineParamters(FilterPanel currentFilter, UserData userData)
        {
            Population population = new Population();
            StudentProgramCode studentProgramCode = new StudentProgramCode();
            StudentAdminCode studentAdminCode = new StudentAdminCode();
            EREngineRequestModel eREngineRequestModel = new EREngineRequestModel();
            var genderPopSelectedList = currentFilter.PopulationGenderValue;
            var programCodePopSelectedList = currentFilter.PopulationProgramValue;
            var raceEthnicityPopSelectedList = currentFilter.PopulationEthnicityValue;




            string defaultAdminValue = "x";
            string reportCriteria = "";
            string apl = null;
            int pageOffset = 1;
            int pageLimit = 50;
            string sortField = "studentname";
            string sortDirection = "ASC";
            var selectedProgramCodePopfil = GetSelectedPopulation(programCodePopSelectedList);
            studentProgramCode.fiveoFour = selectedProgramCodePopfil.Contains("fiveoFour") ? selectedProgramCodePopfil : null;
            studentProgramCode.se = selectedProgramCodePopfil.Contains("se") ? selectedProgramCodePopfil : null;
            studentProgramCode.migrant = selectedProgramCodePopfil.Contains("migrant") ? selectedProgramCodePopfil : null;
            studentProgramCode.frl = selectedProgramCodePopfil.Contains("frl") ? selectedProgramCodePopfil : null;
            studentProgramCode.title1Math = selectedProgramCodePopfil.Contains("title1Math") ? selectedProgramCodePopfil : null;
            studentProgramCode.titlel1Reading = selectedProgramCodePopfil.Contains("titlel1Reading") ? selectedProgramCodePopfil : null;
            studentProgramCode.gt = selectedProgramCodePopfil.Contains("gt") ? selectedProgramCodePopfil : null;
            studentProgramCode.other = selectedProgramCodePopfil.Contains("other") ? selectedProgramCodePopfil : null;
            studentProgramCode.iep = selectedProgramCodePopfil.Contains("iep") ? selectedProgramCodePopfil : null;
            studentAdminCode.code_a = defaultAdminValue;
            studentAdminCode.code_b = defaultAdminValue;
            studentAdminCode.code_c = defaultAdminValue;
            studentAdminCode.code_d = defaultAdminValue;
            studentAdminCode.code_e = defaultAdminValue;
            studentAdminCode.code_f = defaultAdminValue;
            studentAdminCode.code_g = defaultAdminValue;
            studentAdminCode.code_h = defaultAdminValue;
            studentAdminCode.code_i = defaultAdminValue;
            studentAdminCode.code_j = defaultAdminValue;
            studentAdminCode.code_k = defaultAdminValue;
            studentAdminCode.code_l = defaultAdminValue;
            studentAdminCode.code_m = defaultAdminValue;
            studentAdminCode.code_n = defaultAdminValue;
            studentAdminCode.code_o = defaultAdminValue;
            studentAdminCode.code_p = defaultAdminValue;
            studentAdminCode.code_q = defaultAdminValue;
            studentAdminCode.code_r = defaultAdminValue;
            studentAdminCode.code_s = defaultAdminValue;
            studentAdminCode.code_t = defaultAdminValue;
            var selectedGenderPopfil = GetSelectedPopulation(genderPopSelectedList);
            population.gender = selectedGenderPopfil;
            var selectedRaceEthPopfil = GetSelectedPopulation(raceEthnicityPopSelectedList);
            population.raceEthnicity = selectedRaceEthPopfil;
            population.studentProgramCode = studentProgramCode;
            population.studentAdminCode = studentAdminCode;


            var ageStanineRole = "";
            eREngineRequestModel.assignmentId = currentFilter.SelectedTestAdmin.Id;
            eREngineRequestModel.customerId = userData.CustomerInfoList[0].CustomerId.ToString();
            if (userData.ElevateRole == "district_admin")
            {
                ageStanineRole = "district";
            }
            else if (userData.ElevateRole == "Staff")
            {
                ageStanineRole = "building";
            }
            else if (userData.ElevateRole == "Teacher")
            {
                ageStanineRole = "class";
            }
            eREngineRequestModel.role = ageStanineRole;
            var buildingIds = currentFilter.LocationFilters.Where(x => x.DisplayName == "Building").Select(x => x.Items.Select(y => y.Value)).ToList();
            var ClassIds = currentFilter.LocationFilters.Where(x => x.DisplayName == "Section").Select(x => x.SelectedAltValues).ToList();

            eREngineRequestModel.districtIds = new List<string>() { userData.CustomerInfoList[0].NodeId.ToString() };
            eREngineRequestModel.buildingIds = buildingIds[0].ToList();
            eREngineRequestModel.classIds = ClassIds[0].ToList();
            if (ageStanineRole == "building" || ageStanineRole == "class")
            {
                var students = GetStudentsToGetAllData(currentFilter, userData);
                List<string> studentIds = new List<string>();
                foreach (var item in students)
                {
                    var studentId = item.Id;
                    studentIds.Add(studentId);
                }
                eREngineRequestModel.studentIds = studentIds;

            }
            List<string> contentStringIds = new List<string>();
            List<int> contentIds = new List<int>();
            if (currentFilter.ContentIds.Count == 0)
            {
                contentStringIds = new List<string> { "1" };
            }
            else
            {
                contentStringIds = currentFilter.ContentIds;
            }
            contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
            eREngineRequestModel.contentIds = contentIds;
            eREngineRequestModel.gradeLevel = Convert.ToInt32(currentFilter.Grade);
            eREngineRequestModel.population = population;
            eREngineRequestModel.reportCriteria = reportCriteria;
            eREngineRequestModel.apl = apl;
            eREngineRequestModel.pageOffset = pageOffset;
            eREngineRequestModel.pageLimit = pageLimit;
            eREngineRequestModel.sortField = sortField;
            eREngineRequestModel.sortDirection = sortDirection;

            return eREngineRequestModel;
        }

        public ElevateCogatListOfStudentScores BuildCogatListOfStudentScoresBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData)
        {

            Population population = new Population();
            StudentProgramCode studentProgramCode = new StudentProgramCode();
            StudentAdminCode studentAdminCode = new StudentAdminCode();
            ElevateCogatListOfStudentScores ERECogatListReq = new ElevateCogatListOfStudentScores();
            ElevateStudentReportRequest queryParameters = new ElevateStudentReportRequest();




            string defaultAdminValue = "x";
            studentProgramCode.fiveoFour = null;
            studentProgramCode.se = null;
            studentProgramCode.migrant = null;
            studentProgramCode.frl = null;
            studentProgramCode.title1Math = null;
            studentProgramCode.titlel1Reading = null;
            studentProgramCode.gt = null;
            studentProgramCode.other = null;
            studentAdminCode.code_a = defaultAdminValue;
            studentAdminCode.code_b = defaultAdminValue;
            studentAdminCode.code_c = defaultAdminValue;
            studentAdminCode.code_d = defaultAdminValue;
            studentAdminCode.code_e = defaultAdminValue;
            studentAdminCode.code_f = defaultAdminValue;
            studentAdminCode.code_g = defaultAdminValue;
            studentAdminCode.code_h = defaultAdminValue;
            studentAdminCode.code_i = defaultAdminValue;
            studentAdminCode.code_j = defaultAdminValue;
            studentAdminCode.code_k = defaultAdminValue;
            studentAdminCode.code_l = defaultAdminValue;
            studentAdminCode.code_m = defaultAdminValue;
            studentAdminCode.code_n = defaultAdminValue;
            studentAdminCode.code_o = defaultAdminValue;
            studentAdminCode.code_p = defaultAdminValue;
            studentAdminCode.code_q = defaultAdminValue;
            studentAdminCode.code_r = defaultAdminValue;
            studentAdminCode.code_s = defaultAdminValue;
            studentAdminCode.code_t = defaultAdminValue;
            population.gender = "";
            population.raceEthnicity = null;
            population.hispanicEthnicity = null;
            population.studentProgramCode = studentProgramCode;
            population.studentAdminCode = studentAdminCode;



            ERECogatListReq.FileName = ECogatRequest.FileName;
            ERECogatListReq.ReportTemplate = ECogatRequest.ReportTemplate;
            ERECogatListReq.ReportFormat = "COGAT";
            ERECogatListReq.RFormat = "Catalog";
            ERECogatListReq.SuppressProgramLabel = ECogatRequest.SuppressProgramLabel == null ? "" : ECogatRequest.SuppressProgramLabel;
            ERECogatListReq.CogatComposite = ECogatRequest.CogatComposite;
            ERECogatListReq.RankingDirection = ECogatRequest.RankingDirection;
            ERECogatListReq.GraphType = ECogatRequest.GraphType;
            ERECogatListReq.RankingSubtest = ECogatRequest.RankingSubtest == null ? "" : ECogatRequest.RankingSubtest;
            ERECogatListReq.RankingScore = ECogatRequest.RankingScore;
            ERECogatListReq.DisaggLabel = "MULTIPLE";
            ERECogatListReq.BuildingLabel = ECogatRequest.BuildingLabel;
            ERECogatListReq.ClassLabel = ECogatRequest.ClassLabel;
            ERECogatListReq.DistrictLabel = ECogatRequest.DistrictLabel;
            ERECogatListReq.RegionLabel = "SUPPRESS";
            ERECogatListReq.StateLabel = "SUPPRESS";
            ERECogatListReq.SystemLabel = "SUPPRESS";
            queryParameters.customerId = userData.CustomerInfoList[0].CustomerId;
            queryParameters.assignmentId = currentPanel.SelectedTestAdmin.Id;
            queryParameters.role = userData.ElevateRole;
            queryParameters.districtIds = ECogatRequest.QueryParameters.DistrictIds;
            queryParameters.buildingIds = ECogatRequest.QueryParameters.BuildingIds;
            queryParameters.classIds = ECogatRequest.QueryParameters.ClassIds;
            queryParameters.studentIds = ECogatRequest.QueryParameters.StudentIds;
            queryParameters.gradeLevels = ECogatRequest.QueryParameters.GradeLevels;
            queryParameters.contentIds = ECogatRequest.QueryParameters.ContentIds;
            queryParameters.scores = ECogatRequest.QueryParameters.Scores;
            queryParameters.population = population;
            queryParameters.reportCriteria = ECogatRequest.QueryParameters.ReportCriteria;
            ERECogatListReq.QueryParameters = queryParameters;


            return ERECogatListReq;
        }
        public ElevateCogatProfileNarrative BuildCogatProfileNarrativeBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData)
        {

            ElevateCogatProfileNarrative EREProfileNarrativeReq = new ElevateCogatProfileNarrative();
            ElevateStudentProfileNarrativeReportRequest queryParameters = new ElevateStudentProfileNarrativeReportRequest();

            var populationList = currentPanel.PopulationList;

            ECogatRequest.QueryParameters.ContentIds = new List<int> { 1, 2, 3, 7 };
            EREProfileNarrativeReq.FileName = ECogatRequest.FileName;
            EREProfileNarrativeReq.ReportTemplate = ECogatRequest.ReportTemplate;
            EREProfileNarrativeReq.ReportFormat = "COGAT";
            EREProfileNarrativeReq.RFormat = "Catalog";
            EREProfileNarrativeReq.CogatComposite = ECogatRequest.CogatComposite;
            EREProfileNarrativeReq.GraphType = ECogatRequest.GraphType;
            EREProfileNarrativeReq.ReportGrouping = ECogatRequest.ReportGrouping;
            EREProfileNarrativeReq.DisaggLabel = "MULTIPLE";
            EREProfileNarrativeReq.BuildingLabel = ECogatRequest.BuildingLabel;
            EREProfileNarrativeReq.ClassLabel = ECogatRequest.ClassLabel;
            EREProfileNarrativeReq.DistrictLabel = ECogatRequest.DistrictLabel;
            EREProfileNarrativeReq.RegionLabel = "SUPPRESS";
            EREProfileNarrativeReq.StateLabel = "SUPPRESS";
            EREProfileNarrativeReq.SystemLabel = "SUPPRESS";
            queryParameters.customerId = userData.CustomerInfoList[0].CustomerId;
            queryParameters.assignmentId = currentPanel.SelectedTestAdmin.Id;
            queryParameters.role = userData.ElevateRole;
            queryParameters.districtIds = ECogatRequest.QueryParameters.DistrictIds;
            queryParameters.buildingIds = ECogatRequest.QueryParameters.BuildingIds;
            queryParameters.classIds = ECogatRequest.QueryParameters.ClassIds;
            queryParameters.studentIds = ECogatRequest.QueryParameters.StudentIds;
            queryParameters.gradeLevels = ECogatRequest.QueryParameters.GradeLevels;
            queryParameters.contentIds = ECogatRequest.QueryParameters.ContentIds;
            queryParameters.scores = ECogatRequest.QueryParameters.Scores;
            queryParameters.dimensiongroupvalueids = populationList.Keys.ToList().ConvertAll(int.Parse);
            queryParameters.reportCriteria = ECogatRequest.QueryParameters.ReportCriteria;
            queryParameters.displayLanguage = ECogatRequest.QueryParameters.homeReporting;
            queryParameters.includeAbilityProfile = ECogatRequest.QueryParameters.includeAbilityProfile;
            EREProfileNarrativeReq.QueryParameters = queryParameters;



            return EREProfileNarrativeReq;
        }

        public ElevateCogatGroupSummary BuildCogatGroupSummaryBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData)
        {

            Population population = new Population();
            StudentProgramCode studentProgramCode = new StudentProgramCode();
            StudentAdminCode studentAdminCode = new StudentAdminCode();
            ElevateCogatGroupSummary EREProfileNarrativeReq = new ElevateCogatGroupSummary();
            ElevateGroupSummaryReportRequest queryParameters = new ElevateGroupSummaryReportRequest();




            string defaultAdminValue = "x";
            studentProgramCode.fiveoFour = null;
            studentProgramCode.se = null;
            studentProgramCode.migrant = null;
            studentProgramCode.frl = null;
            studentProgramCode.title1Math = null;
            studentProgramCode.titlel1Reading = null;
            studentProgramCode.gt = null;
            studentProgramCode.other = null;
            studentAdminCode.code_a = defaultAdminValue;
            studentAdminCode.code_b = defaultAdminValue;
            studentAdminCode.code_c = defaultAdminValue;
            studentAdminCode.code_d = defaultAdminValue;
            studentAdminCode.code_e = defaultAdminValue;
            studentAdminCode.code_f = defaultAdminValue;
            studentAdminCode.code_g = defaultAdminValue;
            studentAdminCode.code_h = defaultAdminValue;
            studentAdminCode.code_i = defaultAdminValue;
            studentAdminCode.code_j = defaultAdminValue;
            studentAdminCode.code_k = defaultAdminValue;
            studentAdminCode.code_l = defaultAdminValue;
            studentAdminCode.code_m = defaultAdminValue;
            studentAdminCode.code_n = defaultAdminValue;
            studentAdminCode.code_o = defaultAdminValue;
            studentAdminCode.code_p = defaultAdminValue;
            studentAdminCode.code_q = defaultAdminValue;
            studentAdminCode.code_r = defaultAdminValue;
            studentAdminCode.code_s = defaultAdminValue;
            studentAdminCode.code_t = defaultAdminValue;
            population.gender = "";
            population.raceEthnicity = null;
            population.hispanicEthnicity = null;
            population.studentProgramCode = studentProgramCode;
            population.studentAdminCode = studentAdminCode;



            EREProfileNarrativeReq.FileName = ECogatRequest.FileName;
            EREProfileNarrativeReq.ReportTemplate = ECogatRequest.ReportTemplate;
            EREProfileNarrativeReq.ReportFormat = "COGAT";
            EREProfileNarrativeReq.RFormat = "Catalog";
            EREProfileNarrativeReq.CogatComposite = ECogatRequest.CogatComposite;
            EREProfileNarrativeReq.GraphType = ECogatRequest.GraphType;
            EREProfileNarrativeReq.ReportGrouping = ECogatRequest.ReportGrouping;
            EREProfileNarrativeReq.DisaggLabel = "MULTIPLE";
            EREProfileNarrativeReq.BuildingLabel = ECogatRequest.BuildingLabel;
            EREProfileNarrativeReq.ClassLabel = ECogatRequest.ClassLabel;
            EREProfileNarrativeReq.DistrictLabel = ECogatRequest.DistrictLabel;
            EREProfileNarrativeReq.RegionLabel = "SUPPRESS";
            EREProfileNarrativeReq.StateLabel = "SUPPRESS";
            EREProfileNarrativeReq.SystemLabel = "SUPPRESS";
            queryParameters.customerId = userData.CustomerInfoList[0].CustomerId;
            queryParameters.assignmentId = currentPanel.SelectedTestAdmin.Id;
            queryParameters.role = userData.ElevateRole;
            queryParameters.districtIds = ECogatRequest.QueryParameters.DistrictIds;
            queryParameters.buildingIds = ECogatRequest.QueryParameters.BuildingIds;
            queryParameters.classIds = ECogatRequest.QueryParameters.ClassIds;
            queryParameters.studentIds = ECogatRequest.QueryParameters.StudentIds;
            queryParameters.gradeLevels = ECogatRequest.QueryParameters.GradeLevels;
            queryParameters.contentIds = ECogatRequest.QueryParameters.ContentIds;
            queryParameters.scores = ECogatRequest.QueryParameters.Scores;
            queryParameters.population = population;
            queryParameters.reportCriteria = ECogatRequest.QueryParameters.ReportCriteria;
            queryParameters.homeReporting = ECogatRequest.QueryParameters.homeReporting;
            queryParameters.includeAbilityProfile = ECogatRequest.QueryParameters.includeAbilityProfile;
            EREProfileNarrativeReq.QueryParameters = queryParameters;


            //ElevateGroupSummaryReportRequest
            return EREProfileNarrativeReq;
        }

        public ElevateCogatDataExporter BuildCogatDataExporterBackground(ElevateUiBackgroundRequest ECogatRequest, FilterPanel currentPanel, UserData userData)
        {

            Population population = new Population();
            StudentProgramCode studentProgramCode = new StudentProgramCode();
            StudentAdminCode studentAdminCode = new StudentAdminCode();
            ElevateCogatDataExporter EREDataExportReq = new ElevateCogatDataExporter();
            ElevateGroupSummaryReportRequest queryParameters = new ElevateGroupSummaryReportRequest();




            string defaultAdminValue = "x";
            studentProgramCode.fiveoFour = null;
            studentProgramCode.se = null;
            studentProgramCode.migrant = null;
            studentProgramCode.frl = null;
            studentProgramCode.title1Math = null;
            studentProgramCode.titlel1Reading = null;
            studentProgramCode.gt = null;
            studentProgramCode.other = null;
            studentAdminCode.code_a = defaultAdminValue;
            studentAdminCode.code_b = defaultAdminValue;
            studentAdminCode.code_c = defaultAdminValue;
            studentAdminCode.code_d = defaultAdminValue;
            studentAdminCode.code_e = defaultAdminValue;
            studentAdminCode.code_f = defaultAdminValue;
            studentAdminCode.code_g = defaultAdminValue;
            studentAdminCode.code_h = defaultAdminValue;
            studentAdminCode.code_i = defaultAdminValue;
            studentAdminCode.code_j = defaultAdminValue;
            studentAdminCode.code_k = defaultAdminValue;
            studentAdminCode.code_l = defaultAdminValue;
            studentAdminCode.code_m = defaultAdminValue;
            studentAdminCode.code_n = defaultAdminValue;
            studentAdminCode.code_o = defaultAdminValue;
            studentAdminCode.code_p = defaultAdminValue;
            studentAdminCode.code_q = defaultAdminValue;
            studentAdminCode.code_r = defaultAdminValue;
            studentAdminCode.code_s = defaultAdminValue;
            studentAdminCode.code_t = defaultAdminValue;
            population.gender = "";
            population.raceEthnicity = null;
            population.hispanicEthnicity = null;
            population.studentProgramCode = studentProgramCode;
            population.studentAdminCode = studentAdminCode;



            EREDataExportReq.FileName = ECogatRequest.FileName;
            EREDataExportReq.ReportTemplate = ECogatRequest.ReportTemplate;
            EREDataExportReq.ReportFormat = "COGAT";
            EREDataExportReq.RFormat = "Catalog";
            EREDataExportReq.ExporterTemplate = ECogatRequest.ExportTemplate;
            EREDataExportReq.ExportFormat = ECogatRequest.ExportFormat;
            EREDataExportReq.DisaggLabel = "MULTIPLE";
            EREDataExportReq.BuildingLabel = ECogatRequest.BuildingLabel;
            EREDataExportReq.ClassLabel = ECogatRequest.ClassLabel;
            EREDataExportReq.DistrictLabel = ECogatRequest.DistrictLabel;
            EREDataExportReq.RegionLabel = "SUPPRESS";
            EREDataExportReq.StateLabel = "SUPPRESS";
            EREDataExportReq.SystemLabel = "SUPPRESS";
            queryParameters.customerId = userData.CustomerInfoList[0].CustomerId;
            queryParameters.assignmentId = currentPanel.SelectedTestAdmin.Id;
            queryParameters.role = userData.ElevateRole;
            queryParameters.districtIds = ECogatRequest.QueryParameters.DistrictIds;
            queryParameters.buildingIds = ECogatRequest.QueryParameters.BuildingIds;
            queryParameters.classIds = ECogatRequest.QueryParameters.ClassIds;
            queryParameters.studentIds = ECogatRequest.QueryParameters.StudentIds;
            queryParameters.gradeLevels = ECogatRequest.QueryParameters.GradeLevels;
            queryParameters.battery = currentPanel.Battery;
            queryParameters.population = population;
            
            EREDataExportReq.QueryParameters = queryParameters;


            //ElevateGroupSummaryReportRequest
            return EREDataExportReq;
        }


        private List<StudentPayload> GetStudentsToGetAllData(FilterPanel currentFilter, UserData userData)
        {
            EREngineFilterStudentsRequest eREngineFilterStudentsRequest = new EREngineFilterStudentsRequest();
            var classIdsString = currentFilter.LastLocationFilter == null ? currentFilter.SelectedTestAdmin.NodeId.ToString() : currentFilter.LastLocationFilter.SelectedValuesString;
            eREngineFilterStudentsRequest.nodeIdList = new List<string> { currentFilter.SelectedTestAdmin.NodeId.ToString() };
            eREngineFilterStudentsRequest.customerId = currentFilter.SelectedTestAdmin.CustomerId.ToString();
            eREngineFilterStudentsRequest.nodeType = currentFilter.SelectedTestAdmin.NodeType;
            eREngineFilterStudentsRequest.assignmentId = currentFilter.SelectedTestAdmin.Id;

            eREngineFilterStudentsRequest.gradeLevelId = currentFilter.GradeLevel;
            List<string> contentStringIds = new List<string>();
            List<int> contentIds = new List<int>();
            if (currentFilter.ContentIds.Count == 0)
            {
                contentStringIds = new List<string> { "1" };
            }
            else
            {
                contentStringIds = currentFilter.ContentIds;
            }
            contentIds = contentStringIds.Select(x => int.Parse(x)).ToList();
            eREngineFilterStudentsRequest.contentIds = contentIds;
            List<string> classIds = new List<string>();
            classIds = classIdsString.Split(',').ToList();
            eREngineFilterStudentsRequest.classIds = classIds;//TBD after working with elevatecustomer
            var students = _elevateReportingEngineClient.GetFilterStudents(eREngineFilterStudentsRequest, userData).payload;
            return students;
        }



        private List<string> GetContentAcronym(List<string> ContentList)
        {
            var listContent = new List<string>();
            foreach (var item in ContentList)
            {
                switch (item)
                {
                    case "'Verbal'":
                        {
                            string content = "V";
                            listContent.Add(content);
                            break;
                        }
                    case "'Quant'":
                        {
                            string content = "Q";
                            listContent.Add(content);
                            break;
                        }
                    case "'NonVerb'":
                        {
                            string content = "N";
                            listContent.Add(content);
                            break;
                        }
                    case "'CompVQ'":
                        {
                            string content = "VQ";
                            listContent.Add(content);
                            break;
                        }
                    case "'CompVN'":
                        {
                            string content = "VN";
                            listContent.Add(content);
                            break;
                        }
                    case "'CompQN'":
                        {
                            string content = "QN";
                            listContent.Add(content);
                            break;
                        }
                    case "'CompVQN'":
                        {
                            string content = "VQN";
                            listContent.Add(content);
                            break;
                        }
                    default:
                        break;
                }
            }
            return listContent;
        }

        #region Oldpopulation
        private string GetSelectedPopulation(string inCommingString)
        {
            var convertedKey = "";
            switch (inCommingString)
            {
                case "ETHNICITY_CODE = 'AMERICANINDIAN'":
                    {
                        convertedKey = "AMERICANINDIAN";
                        break;
                    }
                case "ETHNICITY_CODE = 'ASIAN'":
                    {
                        convertedKey = "ASIAN";
                        break;
                    }
                case "ETHNICITY_CODE = 'AFRICANAMERICAN'":
                    {
                        convertedKey = "AFRICANAMERICAN";
                        break;
                    }
                case "ETHNICITY_CODE = 'HISPANIC'":
                    {
                        convertedKey = "HISPANIC";
                        break;
                    }
                case "ETHNICITY_CODE = 'NATIVEHAWPACIFIC'":
                    {
                        convertedKey = "NATIVEHAWPACIFIC";
                        break;
                    }
                case "ETHNICITY_CODE='WHITE'":
                    {
                        convertedKey = "WHITE";
                        break;
                    }
                case "ETHNICITY_CODE='MULTIETHNIC'":
                    {
                        convertedKey = "MULTIETHNIC";
                        break;
                    }
                case "ETHNICITY_CODE='OTHER'":
                    {
                        convertedKey = "OTHER";
                        break;
                    }
                case "NO_PROGRAMS_CODED=1":
                    {
                        convertedKey = "other";
                        break;
                    }
                case "PROG_504=1":
                    {
                        convertedKey = "fiveoFour";
                        break;
                    }
                case "PROG_GT=1":
                    {
                        convertedKey = "gt";
                        break;
                    }
                case "PROG_MG=1":
                    {
                        convertedKey = "migrant";
                        break;
                    }
                case "PROG_SE=1":
                    {
                        convertedKey = "se";
                        break;
                    }
                case "PROG_FRL=1":
                    {
                        convertedKey = "frl";
                        break;
                    }
                case "PROG_T1MATH=1":
                    {
                        convertedKey = "title1Math";
                        break;
                    }
                case "PROG_T1LANG=1":
                    {
                        convertedKey = "title1Reading";
                        break;
                    }
                case "PROG_IEP=1":
                    {
                        convertedKey = "iep";
                        break;
                    }
                case "GENDER='F'":
                    {
                        convertedKey = "F";
                        break;
                    }
                case "GENDER='M'":
                    {
                        convertedKey = "M";
                        break;
                    }
                case "GENDER IS NULL":
                    {
                        convertedKey = "U";
                        break;
                    }

                default:
                    break;
            }
            return convertedKey;
        }
        #endregion


        #region NewPopulation
        private Dictionary<string,string> GetEthnicityPopulation(List<string> inCommingStringList)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            foreach (var item in inCommingStringList)
            {
                var searchItem = item.Split(',');
                if (item.Equals("76,Unknown"))
                {
                    searchItem[1] = "";
                }
                switch (searchItem[1])
                {
                    case "Asian":
                        {
                            keyValuePairs.Add("1", "Asian");
                            break;
                        }
                    case "American Indian/Alaska Native":
                        {
                            keyValuePairs.Add("2", "American Indian/Alaska Native");
                            break;
                        }
                    case "Black or African American":
                        {
                            keyValuePairs.Add("3", "Black or African American");
                            break;
                        }
                    case "Caucasian":
                        {
                            keyValuePairs.Add("4", "Caucasian");
                            break;
                        }
                    case "Native Hawaiian or Other Pacific Islander":
                        {
                            keyValuePairs.Add("5", "Native Hawaiian or Other Pacific Islander");
                            break;
                        }
                    case "Two or More Races":
                        {
                            keyValuePairs.Add("6", "Two or more Races");
                            break;
                        }
                    case "Other":
                        {
                            keyValuePairs.Add("7", "Other");
                            break;
                        }
                    case "Unknown":
                        {
                            keyValuePairs.Add("8", "Unknown");
                            break;
                        }


                    default:
                        break;
                }
                
            }
            return keyValuePairs;
        }
        private Dictionary<string, string> GetProgramPopulationNew(List<string> inCommingStringList)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            foreach (var inCommingString in inCommingStringList)
            {

                var searchItem = inCommingString.Split(',');
                switch (searchItem[1])
                {
                    case "Free/Reduced Lunch":
                        {
                            keyValuePairs.Add("34", "Free/Reduced Lunch");
                            break;
                        }
                    case "IEP":
                        {
                            keyValuePairs.Add("35", "IEP");
                            break;
                        }
                    case "504":
                        {
                            keyValuePairs.Add("39", "504");
                            break;
                        }
                    case "Migrant":
                        {
                            keyValuePairs.Add("40", "Migrant");
                            break;
                        }
                    case "Title 1 Math":
                        {
                            keyValuePairs.Add("41", "Title 1 Math");
                            break;
                        }
                    case "Title 1 Read":
                        {
                            keyValuePairs.Add("42", "Title 1 Read");
                            break;
                        }
                    case "Other ":
                        {
                            keyValuePairs.Add("43", "Other");
                            break;
                        }
                    case "Other 2":
                        {
                            keyValuePairs.Add("44", "Other 2");
                            break;
                        }
                    case "ELL":
                        {
                            keyValuePairs.Add("36", "ELL");
                            break;
                        }
                    case "GT":
                        {
                            keyValuePairs.Add("37", "GT");
                            break;
                        }
                    case "SE":
                        {
                            keyValuePairs.Add("38", "SE");
                            break;
                        }

                    default:
                        break;
                }

            }
            return keyValuePairs;
        }

        private List<string> GetGenderString(List<string> inCommingString)
        {
            var retVal = new List<string>();
            var retString = "";
            if (inCommingString.Contains("Male"))
            {
                retString = "M";
                retVal.Add(retString);
            }

            if (inCommingString.Contains("Female"))
            {
                retString = "F";
                retVal.Add(retString);
            }

            if (inCommingString.Contains("Unknown "))
            {
                retString = "U";
                retVal.Add(retString);
            }
            return retVal;
        }

        private List<string> GetPopulationKeyAndValue(Dictionary<string, string> inCommingStringList)
        {
            var retVal = new List<string>();
            foreach (var item in inCommingStringList)
            {
                var retItem = item.Key +","+ item.Value;
                retVal.Add(retItem);
            }
            return retVal;
        }
        private List<StudentRaces> MapStudentRaces(Dictionary<string, string> inCommingDict)
        {
            List<StudentRaces> studentRacesList = new List<StudentRaces>();
            foreach (var item in inCommingDict)
            {
                StudentRaces studentRaces = new StudentRaces();
                studentRaces.id = Convert.ToInt32(item.Key);
                studentRaces.name = item.Value;
                studentRacesList.Add(studentRaces);
            }
            return studentRacesList;
        }

        private List<StudentPrograms> MapStudentPrograms(Dictionary<string, string> inCommingDict)
        {
            List<StudentPrograms> studentPrograms = new List<StudentPrograms>();
            foreach (var item in inCommingDict)
            {
                StudentPrograms programs = new StudentPrograms();
                programs.programId = Convert.ToInt32(item.Key);
                programs.name = item.Value;
                studentPrograms.Add(programs);
            }
            return studentPrograms;
        }


        private List<StudentCustomCodings> MapcustomCodings(Dictionary<string, string> inCommingDict)
        {
            List<StudentCustomCodings> studentCustomCodings = new List<StudentCustomCodings>();
            foreach (var item in inCommingDict)
            {
                StudentCustomCodings customCodings = new StudentCustomCodings();
                customCodings.name = item.Key;
                customCodings.value = item.Value;
                studentCustomCodings.Add(customCodings);
            }
            return studentCustomCodings;
        }
        #endregion

    }
}