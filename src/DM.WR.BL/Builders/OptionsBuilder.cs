using AutoMapper;
using DM.WR.Models.Config;
using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DM.WR.BL.Builders
{
    public class OptionsBuilder : IOptionsBuilder
    {
        private readonly IDbClient _dbClient;

        private readonly XmlLoader _xmlLoader;

        private readonly OptionsMapper _mapper;
        private readonly IMapper _typesMapper;

        public OptionGroup InvalidGroup { get; private set; }

        public OptionsBuilder(IDbClient dbClient)
        {
            _mapper = new OptionsMapper();
            _typesMapper = new DbTypesMapper();

            _dbClient = dbClient;
            _xmlLoader = XmlLoader.GetInstance(ConfigSettings.XmlAbsolutePath);
        }

        public OptionPage BuildOptions(OptionPage currentPage, XMLGroupType updatedGroupType, UserData userData, int pageIndex, bool isActuateHyperLink = false)
        {
            var ci = _typesMapper.Map<CustomerInfo, DbCustomerInfo>(userData.CurrentCustomerInfo);
            var newPage = new OptionPage(ConfigSettings.XmlAbsolutePath);

            bool didAssessmentChange = false;
            bool didGradeLevelChange = false;
            bool didScoreSetChange = false;

            // ----- Assessments ----- //
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.Assessment);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    OptionGroup newGroup = new OptionGroup();
                    Assessment selectedAssessment = new Assessment();
                    if (userData.Assessments.Any(a => a.TestFamilyGroupCode.ToLower().Contains("cogat") || a.TestFamilyGroupCode.ToLower().Contains("ccat")))
                    {
                        newGroup = _mapper.MapAssessments(userData.Assessments, currentGroup, out selectedAssessment, SetInvalidGroup);
                        //newGroup = userData.Assessments = _typesMapper.Map<List<Assessment>>(selectedAssessment);
                    }
                    newPage.AddGroup(newGroup);
                    newPage.Assessment = selectedAssessment;
                    didAssessmentChange = true;
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.Assessment = currentPage.Assessment;
                }
            }

            // ----- Test Admins ----- //
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.TestAdministrationDate);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var testAdmins = _dbClient.GetTestAdmins(Convert.ToInt32(ci.CustomerId), ci.NodeId, ci.NodeType, ci.NodeName, newPage.AssessmentValue, newPage.AssessmentCode.ToString(), userData.ContractInstances);
                    var newGroup = _mapper.MapTestAdmins(testAdmins, currentGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                    newPage.TestAdministrations = testAdmins;
                    newPage.ScoreSetId = _dbClient.GetTestAdminScoreset(Convert.ToInt32(ci.CustomerId), Convert.ToInt32(ci.NodeId), ci.NodeType, newPage.AssessmentValue, newPage.TestAdminValue).CustScoresetId;
                    newPage.ScoringOptions = _typesMapper.Map<DbScoringOptions, ScoringOptions>(_dbClient.GetCustomerScoringOptions(newPage.ScoreSetId, newPage.AssessmentValue));
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.TestAdministrations = currentPage.TestAdministrations;
                    newPage.ScoreSetId = currentPage.ScoreSetId;
                    newPage.ScoringOptions = currentPage.ScoringOptions;
                }
            }

            // ----- Display Type (Report) -----//
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.DisplayType);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    
                    var displayTypes = _xmlLoader.GetReports(XMLProductCodeEnum.COGAT, userData.RoleId.ToString());
                    var newGroup = _mapper.MapDisplayTypes(displayTypes, currentGroup, newPage.ScoringOptions, newPage.SelectedTestAdmin, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Grade Level ----- //and SelectedBattery
            if (!newPage.ReportXml.isGradeMultiSelect)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.GradeLevel);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var gradeLevels = _dbClient.GetGradeLevels(Convert.ToInt32(ci.NodeId), ci.NodeType, newPage.AssessmentValue, newPage.TestAdminValue, newPage.ScoreSetId, newPage.XmlDisplayOption.reportCode);
                    var newGroup = _mapper.MapGradeLevels(gradeLevels, currentGroup, out GradeLevel selectedGradeLevel, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                    newPage.GradeLevel = selectedGradeLevel;
                    didGradeLevelChange = true;
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.GradeLevel = currentPage.GradeLevel;
                }
            }
            else //PDF
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.GradePaper);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var gradeLevels = _dbClient.GetGrades_Paper(ci, newPage.AssessmentValue, newPage.TestAdminValue, newPage.ScoreSetId);
                    var newGroup = _mapper.MapGradePaper(gradeLevels, currentGroup, out List<Grade> selectedGrades, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                    newPage.Grades = selectedGrades;
                    didGradeLevelChange = true;
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.Grades = currentPage.Grades;
                }
            }

            // ----- Level Of Analysis ----- //
            if (newPage.ReportXml.LevelOfAnalysis.show)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.LevelofAnalysis);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = _mapper.MapLevelOfAnalysis(newPage.ReportXml.LevelOfAnalysis, currentGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Display Options / Data Filtering Option ----- //
            if (newPage.XmlAnalysisType.DisplayOptions.show && !newPage.IsMultimeasure)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.DisplayOptions);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var selectedGrades = newPage.ReportXml.isGradeMultiSelect ?
                                            newPage.GetSelectedValuesOf(XMLGroupType.GradePaper) :
                                            new List<string> { GetGradeNumber((GradeLevel)newPage.GradeLevel) };
                    var newGroup = _mapper.MapDisplayOptions(newPage.XmlAnalysisType.DisplayOptions, currentGroup, selectedGrades, newPage.SelectedTestAdmin, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Skill Set ----- //
            if (newPage.XmlDataFilteringOptions.Skillset != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.SkillDomainClassification);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    //TODO:  this dip vs. no dip is super messed up.  Needs discussion.
                    //Check Skillset or check for Single Grade or check both?
                    var performDatabaseDip = newPage.XmlDataFilteringOptions.Skillset.show;

                    var level = performDatabaseDip ? ((GradeLevel)newPage.GradeLevel).Level : -1;
                    var scoringOptions = newPage.ScoringOptions;
                    var skillSets = performDatabaseDip ?
                        _dbClient.GetSkillSets(ci.CustomerId.ToString(), newPage.TestAdminValue, level, newPage.XmlDisplayOption.reportCode) :
                        null;
                    var newGroup = _mapper.MapSkillSets(skillSets, currentGroup, newPage.XmlDataFilteringOptions.Skillset, scoringOptions, out SkillSet selectedSkillSet, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                    newPage.SkillSet = selectedSkillSet;
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.SkillSet = currentPage.SkillSet;
                }
            }

            // ----- Lower Graph Type ----- //
            if (newPage.XmlDataFilteringOptions.LowerGraphType != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.LowerGraphType);
                var lowerGraphType = newPage.XmlDataFilteringOptions.LowerGraphType;
                var newGroup = _mapper.MapLowerGraphType(lowerGraphType, currentGroup);
                newPage.AddGroup(newGroup);
            }

            // ----- Graph Score(s) ----- //
            if (newPage.XmlDataFilteringOptions.GraphScoreTypes != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.GraphScores);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = newPage.XmlDataFilteringOptions.GraphScoreTypes.isMultiselect ?
                                   _mapper.MapGraphScoreTypesForMultiSelect(newPage.XmlDataFilteringOptions.GraphScoreTypes, currentGroup, newPage.ScoringOptions, newPage.SkillSet as SkillSet, SetInvalidGroup) :
                                   _mapper.MapGraphScoreTypesForSingleSelect(newPage.XmlDataFilteringOptions.GraphScoreTypes, currentGroup, newPage.ScoringOptions, newPage.SkillSet as SkillSet, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Score(s) ----- //
            if (newPage.XmlDataFilteringOptions.ScoreTypes != null)
            {//TODO:  Absolutely have to revisit these piles of garbage.
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.Scores);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    //Avoid ability score for Cogat and Student roster
                    string avoidScoreType = "";
                    if (!newPage.ReportXml.isGradeMultiSelect && newPage.ReportXml.mediaType == XMLReportMediaType.ROI && (newPage.GradeLevel as GradeLevel)?.Battery == "SCREEN"
                        && (newPage.AssessmentCode == XMLProductCodeEnum.COGAT || newPage.AssessmentCode == XMLProductCodeEnum.CCAT) && newPage.XmlDisplayType == XMLReportType.SR)
                        avoidScoreType = "PROF";

                    var hideGroup = newPage.XmlDataFilteringOptions.ScoreTypes.hide;

                    var newGroup = _mapper.MapScoreTypes(newPage.XmlDataFilteringOptions.ScoreTypes, currentGroup, newPage.ScoringOptions, avoidScoreType, newPage.SkillSet as SkillSet, hideGroup, newPage.IsCovidReport, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                    didScoreSetChange = true;
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Export Template ----- //
            if (newPage.XmlDataFilteringOptions.ExportTemplate != null)
            {//TODO:  We always rebuild this group, what is the condition for not rebuilding it?
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.ExportTemplate);
                var exportTemplate = newPage.XmlDataFilteringOptions.ExportTemplate;
                var newGroup = _mapper.MapExportTemplate(exportTemplate, currentGroup);
                newPage.AddGroup(newGroup);
            }

            // ----- Export Format ----- //
            if (newPage.XmlDataFilteringOptions.ExportFormat != null)
            {//TODO:  We always rebuild this group, what is the condition for not rebuilding it?
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.ExportFormat);
                var exportFormat = newPage.XmlDataFilteringOptions.ExportFormat;
                var newGroup = _mapper.MapExportFormat(exportFormat, currentGroup);
                newPage.AddGroup(newGroup);
            }

            // ----- Export Headings ----- //
            {//TODO:  We always rebuild this group, what is the condition for not rebuilding it?
                var xmlGroup = newPage.XmlDataFilteringOptions.ExportHeadings;
                if (BuildGroupBasedOnCondition(xmlGroup, newPage))
                {
                    var currentGroup = currentPage.GetGroupByType(XMLGroupType.ExportHeadings);
                    var exportFormatGroup = newPage.GetGroupByType(XMLGroupType.ExportFormat);
                    var exportHeadings = newPage.XmlDataFilteringOptions.ExportHeadings;
                    var newGroup = _mapper.MapExportHeadings(exportHeadings, exportFormatGroup, currentGroup);
                    newPage.AddGroup(newGroup);
                }
            }

            // ----- AbilityProfile ----- //
            if (newPage.XmlDataFilteringOptions.AbilityProfile != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.AbilityProfile);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = _mapper.MapAbilityProfile(currentGroup, newPage.XmlDataFilteringOptions.AbilityProfile, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Column Z ----- //
            if (newPage.XmlDataFilteringOptions.ExcludeZMenu != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.ColumnZ);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = _mapper.MapColumnZ(currentGroup, newPage.ScoringOptions, newPage.XmlDataFilteringOptions.ExcludeZMenu, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Math Computation ----- //
            if (newPage.XmlDataFilteringOptions.CompositeCalcMenu != null && //TODO:  Hardcoded "SURVEY" needs to go.
                (newPage.ReportXml.mediaType == XMLReportMediaType.PDF || (newPage.GradeLevel as GradeLevel).Battery != "SURVEY"))
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.CompositeCalculationOptions);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = _mapper.MapMathComputation(currentGroup, newPage, updatedGroupType, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Show Reading Total ----- //
            if (newPage.XmlDataFilteringOptions.ShowReadingTotal != null)
            {
                newPage.AddGroup(BuildCommonXmlGroup(newPage, currentPage, XMLGroupType.ShowReadingTotal, updatedGroupType));
            }

            // ----- Home Reporting ----- //
            if (newPage.XmlDataFilteringOptions.HomeReporting != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.HomeReporting);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = _mapper.MapHomeReporting(newPage.XmlDataFilteringOptions.HomeReporting, currentGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Content Scope ----- //
            if (newPage.XmlDataFilteringOptions.ContentAreasMenu != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.ContentScope);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {//TODO:  Lots of hardcoded garbage here.  Also, revisit logic.
                    string subtestGroupType; //Future req: should be retreiving the subtest group type from DB

                    //TODO: Future req: should be retreiving the subtest group type from DB
                    //For staar report use the predicted subtest group type from scoring options and pass it on to the content scope
                    var reportTypesOfPredictedSubtest = new[] { XMLReportType.SPS, XMLReportType.SSR, XMLReportType.GPS, XMLReportType.GSR };
                    if (reportTypesOfPredictedSubtest.Contains(newPage.XmlDisplayType))
                        subtestGroupType = newPage.ScoringOptions.PredSubtestGroupType;
                    else if (newPage.AssessmentCode == XMLProductCodeEnum.GMRT)
                        subtestGroupType = "DEFAULT";
                    else if (newPage.Assessment.IsIss)
                        subtestGroupType = newPage.AssessmentCode.ToString();
                    else
                        subtestGroupType = newPage.XmlAnalysisType.code != XMLLevelOfAnalysisType.TC ? "ONLRPT" : "ONLRPTNI";

                    string scoreType = !newPage.IsMultimeasure ? (newPage.DisplayOptionValue == "BALG" ? "BAS" : "") : "";
                    if (newPage.XmlDataFilteringOptions.Parent.code == "CR")
                        scoreType = newPage.XmlDataFilteringOptions.Parent.code;
                    List<DbContentScope> content;

                    if (!newPage.XmlDataFilteringOptions.ContentAreasMenu.showWordSkills)
                        content = !newPage.ReportXml.isGradeMultiSelect && newPage.ReportXml.mediaType == XMLReportMediaType.ROI ?
                                  _dbClient.GetContentArea(Convert.ToInt32(ci.NodeId), ci.NodeType, newPage.AssessmentValue, newPage.TestAdminValue, (newPage.GradeLevel as GradeLevel).Level, newPage.ScoreSetId, newPage.ExcludeMathComputation, newPage.ExcludeEla, subtestGroupType, scoreType) :
                                  _dbClient.GetContentArea_Paper(ci, newPage.AssessmentValue, newPage.TestAdminValue, newPage.Grades as List<Grade>, newPage.ScoreSetId, newPage.ExcludeMathComputation, newPage.ExcludeEla, subtestGroupType, scoreType);
                    else
                        content = _dbClient.GetContentArea_WordSkills(ci, newPage.AssessmentValue, (newPage.GradeLevel as GradeLevel).Level, newPage.ScoreSetId, newPage.ScoringOptions.WordskillSetId);

                    if (!isActuateHyperLink && currentGroup != null && !currentGroup.IsFromSavedCriteria &&
                        (//TODO:  Can't we just say that for any group with a lower enum value than Content Scope????
                        updatedGroupType == XMLGroupType.GradeLevel ||
                        updatedGroupType == XMLGroupType.CompositeCalculationOptions ||
                        updatedGroupType == XMLGroupType.Assessment ||
                        updatedGroupType == XMLGroupType.LevelofAnalysis ||
                        updatedGroupType == XMLGroupType.TestAdministrationDate ||
                        updatedGroupType == XMLGroupType.DisplayOptions ||
                        currentGroup?.IsMultiSelect != newPage.XmlDataFilteringOptions.ContentAreasMenu.isMultiselect
                        ))
                        currentGroup = null;

                    if (content.Count > 0) // check if contentscope has any rows to display otherwise don't show the menu
                    {
                        List<DbContentScope> selectedContentAreas;
                        var newGroup = newPage.XmlDataFilteringOptions.ContentAreasMenu.isMultiselect ?
                            _mapper.MapContentAreasForMultiSelect(content, currentGroup, newPage.AssessmentCode, out selectedContentAreas, SetInvalidGroup) :
                            _mapper.MapContentAreasForSingleSelect(content, currentGroup, newPage.AssessmentCode, out selectedContentAreas, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                        newPage.ContentScopeList = _typesMapper.Map<List<DbContentScope>, List<ContentScope>>(selectedContentAreas);
                    }
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.ContentScopeList = currentPage.ContentScopeList;
                }
            }

            // ----- CompositeTypes ----- //
            if (newPage.XmlDataFilteringOptions.CompositeTypes != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.CompositeTypes);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    //TODO:  Why hardcoded value?
                    string subtestGroupType = "DEFAULT";
                    List<CompositeType> compositeTypes = _dbClient.GetNodeCompositeTypes_Paper(ci, newPage.AssessmentValue, newPage.TestAdminValue, newPage.Grades as List<Grade>, newPage.ScoreSetId, subtestGroupType);

                    if (currentGroup != null && !currentGroup.IsFromSavedCriteria && currentGroup?.IsMultiSelect != newPage.XmlDataFilteringOptions.CompositeTypes.isMultiselect)
                        currentGroup = null;

                    var newGroup = _mapper.MapCompositeTypes(compositeTypes, newPage.XmlDataFilteringOptions.CompositeTypes, currentGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- ShowCollegeReadiness ----- //
            if (newPage.XmlDataFilteringOptions.CollegeReadiness != null)
            {
                var buildGroup = true;
                //TODO:  Too much code to check a condition
                if (!string.IsNullOrEmpty(newPage.XmlDataFilteringOptions.CollegeReadiness.showIfGrade))
                {
                    var selectedGrades = newPage.GetSelectedValuesOf(XMLGroupType.GradePaper);
                    buildGroup = selectedGrades.Any(g => newPage.XmlDataFilteringOptions.CollegeReadiness.showIfGrade.Split(',').Contains(g));
                }

                if (buildGroup)
                {
                    var currentGroup = currentPage.GetGroupByType(XMLGroupType.CollegeReadiness);
                    if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                    {
                        var newGroup = _mapper.MapCommonXmlGroup(newPage.XmlDataFilteringOptions.CollegeReadiness, currentGroup, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }
                    else
                    {
                        newPage.AddGroup(currentGroup);
                    }
                }
            }

            // ----- Sub Content Areas ----- //
            if (newPage.XmlDataFilteringOptions.ContentAreasMenu != null //TODO: bad XML? - has subcontent area, but no content area?
                && newPage.XmlDataFilteringOptions.SubContentAreasMenu != null && newPage.ReportXml.mediaType == XMLReportMediaType.ROI)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.SubContentScope);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    //TODO:  This "ONLRPT" : "ONLRPTNI" should not be hardcoded.
                    string subtestGroupType = newPage.XmlAnalysisType.code != XMLLevelOfAnalysisType.TC ? "ONLRPT" : "ONLRPTNI";
                    //TODO:  We have this somewhere too - SkillSet.Id vs. SkillSetId from ScoringOptions
                    //TODO:  This is wrong.  We also have AI for LOA now.  Should check against LOA.SD, not != TC
                    int skillsetId = newPage.XmlAnalysisType.code != XMLLevelOfAnalysisType.TC ?
                        Convert.ToInt32((newPage.SkillSet as SkillSet).Id) : newPage.ScoringOptions.SkillSetId;

                    var subContentArea = _dbClient.GetSubContentArea(ci, newPage.AssessmentValue, newPage.TestAdminValue, skillsetId, (newPage.GradeLevel as GradeLevel).Level, newPage.ScoreSetId, subtestGroupType, newPage.ContentScopeAcronyms, newPage.ExcludeMathComputation, newPage.ExcludeEla, newPage.XmlDisplayOption.reportCode);
                    bool disableParentSkill = newPage.XmlDataFilteringOptions.SubContentAreasMenu.hideParentSkill;
                    bool disableCognitiveskill = newPage.XmlDataFilteringOptions.SubContentAreasMenu.hideCognitiveSkill;

                    if (newPage.XmlDataFilteringOptions.SubContentAreasMenu.isMultiselect)
                    {
                        if (!isActuateHyperLink && currentGroup != null && !currentGroup.IsFromSavedCriteria &&
                            (
                            updatedGroupType == XMLGroupType.Assessment ||
                            updatedGroupType == XMLGroupType.TestAdministrationDate ||
                            updatedGroupType == XMLGroupType.GradeLevel ||
                            updatedGroupType == XMLGroupType.ContentScope ||
                            updatedGroupType == XMLGroupType.CompositeCalculationOptions ||
                            updatedGroupType == XMLGroupType.LevelofAnalysis ||
                            updatedGroupType == XMLGroupType.SkillDomainClassification ||
                            updatedGroupType == XMLGroupType.Student ||
                            currentGroup?.IsMultiSelect != newPage.XmlDataFilteringOptions.ContentAreasMenu.isMultiselect))
                            currentGroup = null;
                    }
                    var newGroup = _mapper.MapSubContentAreaList(subContentArea, currentGroup, disableParentSkill, disableCognitiveskill, newPage.XmlDataFilteringOptions.SubContentAreasMenu.isMultiselect, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- SortDirections ----- //
            if (newPage.XmlDataFilteringOptions.SortDirections != null && newPage.ReportXml.mediaType == XMLReportMediaType.PDF)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.SortDirection);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = _mapper.MapSortDirections(newPage.XmlDataFilteringOptions.SortDirections, currentGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Sort By Subtest ----- //
            if (newPage.XmlDataFilteringOptions.SortBySubtest != null &&
                newPage.ReportXml.mediaType == XMLReportMediaType.PDF)
            {
                if (newPage.XmlDataFilteringOptions.SortBySubtest.hideIfSortDirection == null
                 || newPage.XmlDataFilteringOptions.SortBySubtest.hideIfSortDirection != newPage.GetSelectedValuesStringOf(XMLGroupType.SortDirection))
                {
                    var currentGroup = currentPage.GetGroupByType(XMLGroupType.SortBySubtest);
                    if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                    {
                        string subtestGroupType = "DEFAULT";//TODO - Hardcoded.  
                        List<SubtestFamily> sortBySubtestFamlies = _dbClient.GetNodeSubtestFamilies_Paper(ci, newPage.AssessmentValue, newPage.TestAdminValue, newPage.Grades as List<Grade>, newPage.ScoreSetId, newPage.ExcludeMathComputation, newPage.ExcludeEla, subtestGroupType);

                        var newGroup = _mapper.MapSortBySubtest(sortBySubtestFamlies, currentGroup, out SubtestFamily subtestFamliy, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                        //newPage.SubtestFamliy = subtestFamliy;
                    }
                    else
                    {
                        newPage.AddGroup(currentGroup);
                    }
                }
            }

            // ----- SortTypes ----- // ST-487 - If SortDirection is by StudentLastname in LOSS report - skip this group
            if (newPage.XmlDataFilteringOptions.SortTypes != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.SortType);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var newGroup = _mapper.MapCommonXmlGroup(newPage.XmlDataFilteringOptions.SortTypes, currentGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Performance Bands ----- //
            if (newPage.XmlDataFilteringOptions.PerformanceBands != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.PerformanceBands);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    XMLPerformanceBands xmlBands;
                    if (newPage.ReportXml.Parent.productCode == XMLProductCodeEnum.IOWA_INTERIM)
                    {
                        var scoreValues = newPage.GetSelectedValuesOf(XMLGroupType.Scores);
                        var score = scoreValues?.First();
                        xmlBands = _xmlLoader.GetInterimPerformanceBands(score, newPage.ContentScopeAcronyms, (newPage.GradeLevel as GradeLevel).Grade);
                    }
                    else
                    {
                        xmlBands = _xmlLoader.Root.GlobalTypes.PerformanceBands;
                    }

                    var newGroup = _mapper.MapPerformanceBands(xmlBands, (PerformanceBandGroup)currentGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                    //TODO:  Do we need MM Forms if the report is Interims Proficiency???
                    newPage.MultimeasureForms = currentGroup == null || didAssessmentChange || didGradeLevelChange || didScoreSetChange ?
                                                _dbClient.GetFormsForMultimeasureColumn(newPage.ScoreSetId, Convert.ToInt32(ci.NodeId), ci.NodeType, newPage.AssessmentValue, (newPage.GradeLevel as GradeLevel).Level) :
                                                currentPage.MultimeasureForms;
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.MultimeasureForms = currentPage.MultimeasureForms;
                }
            }

            // ----- Custom Data Fields ----- //
            {
                var xmlGroup = newPage.XmlDataFilteringOptions.CustomDataFields;
                if (BuildGroupBasedOnCondition(xmlGroup, newPage))
                {
                    var currentGroup = (CustomFieldGroup)currentPage.GetGroupByType(XMLGroupType.CustomDataFields);
                    if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                    {
                        var refKey = newPage.XmlDisplayOption.DataFilteringOptions.CustomDataFields.globalTypesRef;
                        var exportFileInfoXml = _xmlLoader.Root.GlobalTypes.ExportFileInfo.First(i => i.key == refKey);
                        var newGroup = _mapper.MapCustomDataFields(exportFileInfoXml, currentGroup, updatedGroupType, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }
                    else
                    {
                        newPage.AddGroup(currentGroup);
                    }
                }
            }

            // ----- LOCATIONS, LOCATIONS, LOCATIONS! ----- //
            {
                var selectedNodeIds = new List<int> { Convert.ToInt32(ci.NodeId) };
                string nodeType = ci.NodeType;

                if ((int)updatedGroupType >= (int)XMLGroupType.STATE)
                {
                    //Loop though current locations until "CLASS" or changed group found
                    foreach (var locationGroup in currentPage.GetGroupsOfCategory(OptionsCategory.Locations).Where(g => g.Type != XMLGroupType.Student))
                    {
                        selectedNodeIds = locationGroup.SelectedValues.ConvertAll(int.Parse);
                        newPage.AddGroup(locationGroup);
                        nodeType = (locationGroup as LocationGroup).LocationNodeType;

                        if (locationGroup.Type == XMLGroupType.CLASS || locationGroup.Type == updatedGroupType)
                            break;
                    }
                }

                //EXIT FROM LOOP IF REACHED "CLASS" OR THE "ALL" NODE (-1) IS SELECTED 
                while (nodeType != NodeType.CLASS && selectedNodeIds.Count == 1 && selectedNodeIds[0] != -1)
                {
                    //TODO: Create attribute in XML - something like "Show ALL in Locations" per report bases
                    //TODO:  This is the last parameter in call to _dbClient.GetChildLocations()
                    var reportTypesWithoutAllOption = new List<XMLReportType> { XMLReportType.SP, XMLReportType.SWDS, XMLReportType.CWDS, XMLReportType.WDSR, XMLReportType.CIRR };

                    var childLocations = !newPage.ReportXml.isGradeMultiSelect &&
                                         (newPage.ReportXml.mediaType == XMLReportMediaType.ROI || newPage.XmlDisplayType == XMLReportType.CIRR) ?
                    /* ROI */_dbClient.GetChildLocations(selectedNodeIds[0], nodeType, ci, newPage.AssessmentValue, (newPage.GradeLevel as GradeLevel).Level, newPage.TestAdminValue, newPage.ScoreSetId, !reportTypesWithoutAllOption.Contains(newPage.XmlDisplayType)) :
                    /* PDF */_dbClient.GetChildLocations_Paper(selectedNodeIds[0], nodeType, ci, newPage.AssessmentValue, newPage.Grades as List<Grade>, newPage.TestAdminValue, newPage.ScoreSetId, newPage.XmlDisplayType == XMLReportType.SDE);

                    //Reset next level node
                    selectedNodeIds = new List<int> { -1 };
                    nodeType = childLocations[0].NodeType;

                    //See if there is a previously selected node. Default is All = -1 
                    {
                        var nextGroup = currentPage.Locations.FirstOrDefault(g => g.LocationNodeType == nodeType);

                        //in the following line at the end, e.Id is null when coming from hyperlinks from Group Roster reports
                        if (nextGroup != null && (nextGroup.SelectedValues.Count > 0 || newPage.ReportXml.isGradeMultiSelect))
                            selectedNodeIds = nextGroup.SelectedValues.ConvertAll(int.Parse);

                        //hack for SDE report, which is paper, but has single select locations
                        if (selectedNodeIds.Count == 0 && newPage.ReportXml.reportType == XMLReportType.SDE)
                            selectedNodeIds = new List<int> { -1 };
                    }

                    OptionGroup newGroup;
                    if (!newPage.ReportXml.isGradeMultiSelect || newPage.ReportXml.reportType == XMLReportType.SDE)
                    {
                        newGroup = _mapper.MapLocations(childLocations, selectedNodeIds[0], out var selectedLocation, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                        selectedNodeIds = new List<int> { selectedLocation.Id };
                    }
                    else
                    {
                        newGroup = _mapper.MapLocations_Paper(childLocations, selectedNodeIds, out var selectedLocations, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                        selectedNodeIds = selectedLocations.ConvertAll(e => e.Id);
                    }
                }
            }

            // ----- Students ----- //
            if (!newPage.IsMultimeasure &&
                 newPage.XmlDisplayType != XMLReportType.CWDS &&
                 newPage.XmlDisplayType != XMLReportType.WDSR &&
                 newPage.ReportXml.isStudentLevel && //TODO: Split this XML attribute into 2.  1 - Drill down to class, 2 - Display student
                 newPage.ReportXml.mediaType == XMLReportMediaType.ROI)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.Student);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var classId = newPage.LastSelectedLocationGroup == null ? userData.CurrentCustomerInfo.NodeId.ToString() : newPage.LastSelectedLocationGroup.SelectedValuesString;
                    var students = _dbClient.GetStudents(newPage.ScoreSetId, classId, "CLASS", newPage.AssessmentValue, (newPage.GradeLevel as GradeLevel).Level, newPage.ScoringOptions.AccountabilityFlag);
                    var showAllOption = newPage.XmlDisplayType != XMLReportType.SWDS;
                    var newGroup = _mapper.MapStudents(students, currentGroup, showAllOption, out var selectedStudent, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                    newPage.Student = selectedStudent;
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                    newPage.Student = currentPage.Student;
                }
            }

            // ----- LONGITUDINAL groups ----- //
            //TODO:  We never take any Longitudinal groups from currentPage.  Always recreate new ones.
            if (newPage.XmlDataFilteringOptions.LongitudinalOptions != null)
            {
                XMLLongitudinalOptions xmlLongitudinal = newPage.XmlDataFilteringOptions.LongitudinalOptions;
                var skillReportFlag = newPage.LevelOfAnalysisValue == XMLLevelOfAnalysisType.SD ? 1 : 0;
                var isIowaSelected = false;
                List<LongTestAdminScoreset> testAdmins = null;

                if (xmlLongitudinal.TestAdministrations != null)
                {
                    //bool isStudentLevelReport = context.dataFilteringOptionsXml.Parent.Parent.Parent.Parent.Parent.isStudentLevel;
                    string testFamilyGroupCodes = xmlLongitudinal.TestAdministrations.testFamilyGroupCodes;
                    var battery = newPage.ContentScopeList.First().Battery;
                    int minToSelect = newPage.XmlDataFilteringOptions.LongitudinalOptions.TestAdministrations.minToSelect;
                    int maxToSelect = newPage.XmlDataFilteringOptions.LongitudinalOptions.TestAdministrations.maxToSelect;

                    //Longitudinal Test Administrations w/o subgroup
                    if (!newPage.XmlDataFilteringOptions.LongitudinalOptions.TestAdministrations.showGradesSubgroup)
                    {
                        testAdmins = newPage.Student == null ?
                            _dbClient.GetStudentLongTestAdminScoresetsAll(newPage.ScoreSetId, newPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo), newPage.GetLastSelectedNodeIds(userData.CurrentCustomerInfo)[0], testFamilyGroupCodes, (newPage.GradeLevel as GradeLevel).Level, battery, skillReportFlag, newPage.ScoringOptions.SplitDate, xmlLongitudinal.TestAdministrations.basReportFlag, newPage.ScoringOptions.AccountabilityFlag, newPage.AssessmentCode.ToString(), newPage.IsCovidReport) :
                            _dbClient.GetStudentLongTestAdminScoresets(newPage.ScoreSetId, Convert.ToInt32((newPage.Student as Student).TestInstanceId), testFamilyGroupCodes, battery, skillReportFlag, newPage.ScoringOptions.AccountabilityFlag, newPage.AssessmentCode.ToString(), newPage.IsCovidReport);

                        var currentGroup = currentPage.GetGroupByType(XMLGroupType.LongitudinalTestAdministrations);
                        var newGroup = _mapper.MapLongitudinalTestAdmins_Student(testAdmins, currentGroup, newPage.GetGroupByType(updatedGroupType), minToSelect, maxToSelect, isActuateHyperLink, out isIowaSelected, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }
                    else
                    {
                        //string selectedLongType;
                        {//Longitudinal Types
                            var currentGroup = currentPage.GetGroupByType(XMLGroupType.LongitudinalTypes);
                            var newGroup = _mapper.MapCommonXmlGroup(xmlLongitudinal.LongitudinalTypes, currentGroup, SetInvalidGroup);
                            newPage.AddGroup(newGroup);
                        }
                        {//Longitudinal Test Administrations with subgroup
                            var currentGroup = currentPage.GetGroupByType(XMLGroupType.LongitudinalTestAdministrations);
                            var longitudinalType = newPage.GetSelectedValuesStringOf(XMLGroupType.LongitudinalTypes);
                            testAdmins = _dbClient.GetGroupLongTestAdminScoresets(newPage.ScoreSetId, newPage.GetLastSelectedNodeIds(userData.CurrentCustomerInfo)[0], newPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo), testFamilyGroupCodes, (newPage.GradeLevel as GradeLevel).Level, longitudinalType, battery, skillReportFlag, newPage.ScoringOptions.SplitDate, xmlLongitudinal.TestAdministrations.basReportFlag, newPage.GetSelectedValuesStringOf(XMLGroupType.Assessment), newPage.IsCovidReport);
                            var newGroup = _mapper.MapLongTestAdmins_Group(testAdmins, currentGroup, newPage.GetGroupByType(updatedGroupType), minToSelect, maxToSelect, SetInvalidGroup);
                            newPage.AddGroup(newGroup);
                        }
                    }
                }

                // Growth Feature
                if (xmlLongitudinal.GrowthFeature != null && (xmlLongitudinal.GrowthFeature.scoreType == null || newPage.IsValueSelected(XMLGroupType.Scores, xmlLongitudinal.GrowthFeature.scoreType)))
                {
                    var testAdminsGroup = newPage.GetGroupByType(XMLGroupType.LongitudinalTestAdministrations);

                    {// Growth Start Point Type(s)
                        var currentGroup = currentPage.GetGroupByType(XMLGroupType.LongitudinalGrowthStartPointType);
                        var xmlGrowthStartPoint = xmlLongitudinal.GrowthFeature.GrowthStartPoint;
                        var newGroup = _mapper.MapGrowthStartPointType(xmlGrowthStartPoint, currentGroup, testAdminsGroup, isIowaSelected, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }

                    // Growth Start Point
                    //create a new menu and populate with the selected longitudinal test administrations dates with first last entry as the default(oldest date)
                    if (newPage.GroupHasSelectedOptions(XMLGroupType.LongitudinalGrowthStartPointType) && isIowaSelected)
                    {
                        //One choice of previously selected TestAdministrations
                        var currentGroup = currentPage.GetGroupByType(XMLGroupType.LongitudinalGrowthStartPoint);
                        var newGroup = _mapper.MapGrowthStartPoint(testAdmins, testAdminsGroup, currentGroup, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }

                    {// Growth End Point
                        var currentGroup = currentPage.GetGroupByType(XMLGroupType.LongitudinalGrowthEndPoint);
                        var growthEndPoints = _dbClient.GetGrowthEndPoints(newPage.ScoreSetId, (newPage.GradeLevel as GradeLevel).Level);
                        var newGroup = _mapper.MapGrowthEndPoint(growthEndPoints, currentGroup, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }

                    {//Growth Goal
                        var growthGoalXml = xmlLongitudinal.GrowthFeature.GrowthGoal;
                        var selectedGrowthStartPointTypeValue = newPage.GetSelectedValuesStringOf(XMLGroupType.LongitudinalGrowthStartPointType);

                        //get level from gradelevel
                        string gradeValueString = (newPage.GradeLevel as GradeLevel).GradeText.Split('/')[0].ToUpper().Replace("GRADE", "").Trim();
                        int gradeValue = Convert.ToInt16(Regex.Replace(gradeValueString, "[A-Z]", "0"));

                        var currentGroup = currentPage.GetGroupByType(XMLGroupType.LongitudinalGrowthGoal);
                        var newGroup = _mapper.MapGrowthGoalXml(growthGoalXml, currentGroup, selectedGrowthStartPointTypeValue, newPage.GetSelectedValuesStringOf(XMLGroupType.ContentScope), gradeValue, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }
                }
            }

            // ----- Group Population ----- //
            if (newPage.XmlDataFilteringOptions.GroupPopulation != null)
            {
                newPage.AddGroup(BuildGroupPopulationGroup(newPage, currentPage, newPage.XmlDataFilteringOptions.GroupPopulation, updatedGroupType, userData));
            }

            // ----- Comparison Grouping ----- //
            if (newPage.XmlDataFilteringOptions.ComparisonGrouping != null)
            {
                newPage.AddGroup(BuildGroupPopulationGroup(newPage, currentPage, newPage.XmlDataFilteringOptions.ComparisonGrouping, updatedGroupType, userData));
            }

            // ----- Population Filters ----- //
            if (newPage.XmlDataFilteringOptions.PopulationFilterMenu != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.PopulationFilters);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    if (newPage.XmlDataFilteringOptions.PopulationFilterMenu.subgroupsOnly)
                    {//TODO:  What if subgroupsOnly, but NOT ROI?
                        if (newPage.ReportXml.mediaType == XMLReportMediaType.ROI)
                        {
                            int displayNone = newPage.XmlDataFilteringOptions.PopulationFilterMenu.displayNone;
                            List<Disaggregation> populationFilters = _dbClient.GetSubgroupDisaggregation(newPage.GetLastSelectedNodeIds(userData.CurrentCustomerInfo), newPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo), newPage.AssessmentValue, (newPage.GradeLevel as GradeLevel).Level, newPage.ScoreSetId, displayNone);
                            var newGroup = _mapper.MapSubgroupDisaggDictionary(populationFilters, currentGroup, SetInvalidGroup);
                            newPage.AddGroup(newGroup);
                        }
                    }
                    else
                    {
                        Dictionary<string, List<Disaggregation>> populationFilters = newPage.ReportXml.mediaType == XMLReportMediaType.ROI || newPage.XmlDisplayType == XMLReportType.CIRR ?
                            _dbClient.GetDisaggregation(newPage.ScoringOptions.GroupsetId, newPage.GetLastSelectedNodeIdsString(userData.CurrentCustomerInfo), newPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo), newPage.AssessmentValue, newPage.TestAdminValue, (newPage.GradeLevel as GradeLevel).Level, newPage.ScoreSetId) :
                            _dbClient.GetDisaggregation_Paper(newPage.ScoringOptions.GroupsetId, newPage.GetLastSelectedNodeIdsString(userData.CurrentCustomerInfo), newPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo), newPage.AssessmentValue, newPage.TestAdminValue, (newPage.Grades as List<Grade>), newPage.ScoreSetId, ci);

                        var newGroup = _mapper.MapDisaggDictionary(populationFilters, currentGroup, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- Score Filters ----- //
            if (newPage.XmlDataFilteringOptions.ScoreFilters != null)
            {
                var xmlScoresList = newPage.XmlDataFilteringOptions.ScoreTypes.ScoreTypeName.ToList();
                var filteredScoreOptions = newPage.GetGroupByType(XMLGroupType.Scores).Options.Where(o => o.IsSelected && xmlScoresList.Any(s => s.code == o.Value && s.filterValue != null)).ToList();

                if (filteredScoreOptions.Any())
                {
                    var currentGroup = currentPage.GetGroupByType(XMLGroupType.ScoreFilters);
                    if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                    {
                        var newGroup = _mapper.MapScoreFilters((ScoreFiltersGroup)currentGroup, newPage, xmlScoresList, filteredScoreOptions, pageIndex, SetInvalidGroup);
                        newPage.AddGroup(newGroup);
                    }
                    else
                    {
                        newPage.AddGroup(currentGroup);
                    }
                }
            }

            // ----- Score Warnings ----- //
            if (newPage.XmlDataFilteringOptions.ScoreWarnings != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.ScoreWarnings);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    var scoreWarnings = _typesMapper.Map<List<DbScoreWarning>, List<ScoreWarning>>(_dbClient.GetScoreWarnings(newPage.ScoreSetId, newPage.GetLastSelectedNodeId(userData.CurrentCustomerInfo).ToString(), newPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo), newPage.AssessmentValue, ((GradeLevel)newPage.GradeLevel).Level, newPage.ContentScopeAcronyms, newPage.ScoringOptions.AccountabilityFlag));
                    var changedGroup = updatedGroupType == XMLGroupType._INTERNAL_FIRST_ || updatedGroupType == XMLGroupType._INTERNAL_LAST_ ?
                                        new OptionGroup(updatedGroupType) :
                                        currentPage.GetGroupByType(updatedGroupType);
                    var newGroup = _mapper.MapScoreWarnings(scoreWarnings, newPage.XmlDataFilteringOptions.ScoreWarnings, (ScoreWarningsGroup)currentGroup, changedGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- CogatDifferences ----- //
            if (newPage.XmlDataFilteringOptions.CogatDifferences != null)
            {
                if (BuildGroupBasedOnCondition(newPage.XmlDataFilteringOptions.CogatDifferences, newPage))
                    newPage.AddGroup(BuildCommonXmlGroup(newPage, currentPage, XMLGroupType.CogatDifferences, updatedGroupType));
            }

            // ----- ShowPredictedScores ----- //
            if (newPage.XmlDataFilteringOptions.ShowPredictedScores != null)
            {
                newPage.AddGroup(BuildCommonXmlGroup(newPage, currentPage, XMLGroupType.ShowPredictedScores, updatedGroupType));
            }

            // ----- Report Grouping / Group By ----- //
            if (newPage.XmlDataFilteringOptions.ReportGrouping != null)
            {
                var currentGroup = currentPage.GetGroupByType(XMLGroupType.ReportGrouping);
                if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
                {
                    List<NodeLevel> levels = _dbClient.GetNodeLevels(newPage.ScoreSetId, userData.CurrentCustomerInfo.NodeLevel);
                    var newGroup = _mapper.MapReportGrouping(levels, currentGroup, SetInvalidGroup);
                    newPage.AddGroup(newGroup);
                }
                else
                {
                    newPage.AddGroup(currentGroup);
                }
            }

            // ----- ReportMediaType ----- //
            if (newPage.XmlDataFilteringOptions.ReportMediaType != null)
            {
                newPage.AddGroup(BuildCommonXmlGroup(newPage, currentPage, XMLGroupType.ReportMediaType, updatedGroupType));
            }

            newPage.InvalidGroup = InvalidGroup;

            return newPage;
        }

        public OptionBook SyncUpPagesForMultimeasure(OptionBook book)
        {
            var numTc = book.Pages.Count(p => p.IsValueSelected(XMLGroupType.LevelofAnalysis, "TC"));

            for (int c = 0; c < book.Pages.Count; ++c)
            {
                var page = book.Pages[c];

                page.GetGroupByType(XMLGroupType.DisplayType).IsDisabled = c > 0;

                if (page.GroupExists(XMLGroupType.CompositeCalculationOptions))
                    page.GetGroupByType(XMLGroupType.CompositeCalculationOptions).IsDisabled = numTc > 1;

                if (page.GroupExists(XMLGroupType.ColumnZ))
                    page.GetGroupByType(XMLGroupType.ColumnZ).IsDisabled = c > 0;

                if (page.GroupExists(XMLGroupType.PopulationFilters))
                    page.GetGroupByType(XMLGroupType.PopulationFilters).IsDisabled = c > 0;

                //A consensus has been reached to simply remove all Location groups from pages that are not page one.
                if (c > 0)
                    page.RemoveGroups(page.Locations.Cast<OptionGroup>().ToList());

                if (page.GroupExists(XMLGroupType.ScoreFilters))
                {
                    var scoreFilterRow = ((ScoreFiltersGroup)page.GetGroupByType(XMLGroupType.ScoreFilters)).Rows.First();
                    if (c == 0)
                        scoreFilterRow.Concatenation = ConcatOperatorEnum.None;
                    else if (scoreFilterRow.Concatenation == ConcatOperatorEnum.None)
                    {
                        var prevPageConcatenation = ((ScoreFiltersGroup)book.Pages[c - 1].GetGroupByType(XMLGroupType.ScoreFilters)).Rows.First().Concatenation;
                        scoreFilterRow.Concatenation = prevPageConcatenation == ConcatOperatorEnum.None ? ConcatOperatorEnum.AND : prevPageConcatenation;
                    }
                }
            }

            return book;
        }

        public OptionPage UpdateReportOptions(OptionPage page, string queryString, UserData userData, out OptionGroup topChangedGroup, out string extraQueryParams)
        {
            var isLocationNodePassed = false;
            var productCode = page.AssessmentCode;
            topChangedGroup = null;
            bool isStudentPassed = false;
            extraQueryParams = "";
            var extraParams = new List<string>();

            //queryString.Split('&').ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);
            var keyValuePairs = queryString.Split('&');

            foreach (var keyValuePair in keyValuePairs)
            {
                var parsedPair = ParseKeyValuePair(keyValuePair);
                var pairKey = parsedPair.Key;
                var pairValue = parsedPair.Value;

                //Dmitriy - if this is for location node change when clicking on a link inside Actuate report
                if (pairKey.ToLower() == "nodetype" || pairKey.ToLower() == "nodeid")
                {
                    isLocationNodePassed = true;
                    continue;
                }
                //Dmitriy - extra params that need to be added to query string, but don't go into option groups
                //ST-5199 - additional additional checks for new params which gets passed when the expand icon is clicked in the GMRT Class Word Decoding Skills report
                if (pairKey.ToLower() == "tagsetinstanceid" || pairKey.ToLower() == "studentskillexpandcollapselist" || pairKey.ToLower() == "page" || pairKey.ToLower() == "performancelevel" || pairKey.ToLower() == "cutscorefamilyid")
                {
                    extraParams.Add(keyValuePair);
                    continue;
                }
                //Dmitriy - this is the case when Actuate passes filter value to us, which is not a part of Report Options
                if (pairKey.ToLower() == "subtestfilters")
                {
                    extraParams.Add(keyValuePair);
                    var scoreFilterRow = new ScoreFilterRow(pairValue);
                    if (scoreFilterRow.HasSelection) //TODO: In 4.0 we have an "is valid" kind of check
                    {
                        var filtersGroup = new ScoreFiltersGroup
                        {
                            Category = OptionsCategory.Secondary,
                            InputControl = OptionsInputControl.ScoreFilters,
                            DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ScoreFilters),
                            Rows = new List<ScoreFilterRow> { scoreFilterRow }
                        };

                        page.AddGroup(filtersGroup);
                    }

                    continue;
                }

                //Map Actuate Key to GroupType and GroupName
                var groupName = ActuateKeyToGroupName(pairKey);

                if (groupName.ToUpper() == "GENDERLIST" || groupName.ToUpper() == "ETHNICITYLIST" || groupName.ToUpper() == "ADMINCODELIST" || groupName.ToUpper() == "PROGRAMLIST")
                {
                    groupName = "Population Filters";
                    pairValue = pairValue.ToUpper();
                }

                if (!GroupNameToTypeMapper.TryGetValue(groupName, out var groupType))
                    continue;

                if (groupType == XMLGroupType.DisplayType)
                {
                    var reportXml = Enum.TryParse<XMLReportType>(pairValue, out var reportType) ?
                        _xmlLoader.GetReport(productCode, reportType) :
                        _xmlLoader.GetReportByReportName(productCode, pairValue);

                    //Fail if report is not found
                    if (reportXml == null) return null;

                    //Convert Option Value from DisplayType Name to DisplayType Code
                    pairValue = reportXml.reportType.ToString();
                }

                //Find group; Add new group if not found
                var group = page.GetGroupByType(groupType);
                if (group == null)
                {
                    group = new OptionGroup(groupType);
                    page.AddGroup(group);
                }
                if (topChangedGroup == null || topChangedGroup.TypeCode > group.TypeCode) topChangedGroup = group;

                //Clear all options
                group.Options = new List<Option>();

                //Add selected options
                if (groupType == XMLGroupType.ContentScope)
                {
                    foreach (string contentScopeValue in pairValue.Split(','))
                        group.Options.Add(new Option { Value = contentScopeValue, IsSelected = true });
                }
                else if (groupType == XMLGroupType.Student)
                {
                    isStudentPassed = true;

                    var locationGroups = page.GetGroupsOfCategory(OptionsCategory.Locations);
                    page.RemoveGroups(locationGroups.Where(g => g.Type != XMLGroupType.Student).ToList());

                    //Re-match Locations for a selected student
                    List<Location> locations = _dbClient.GetStudentLocationHierarchy(page.ScoreSetId, userData.CurrentGuid, Int32.Parse(pairValue), page.ScoringOptions.AccountabilityFlag);
                    foreach (var location in locations)
                    {
                        var locationGroup = new LocationGroup(location.NodeType)
                        {
                            LocationNodeType = location.NodeType,
                            Category = OptionsCategory.Locations,
                            Options = new List<Option>
                            {
                                new Option
                                {
                                    Value = location.Id.ToString(),
                                    Text = location.NodeName,
                                    IsSelected = true
                                }
                            },
                        };
                        page.AddGroup(locationGroup);
                    }

                    //Add Student to a Group
                    group.Options.Add(new Option { AltValue = pairValue, IsSelected = true });
                }
                else
                {
                    group.Options.Add(new Option { Value = pairValue, IsSelected = true });
                }
            }

            //Dmitriy - this condition is dealing with case when we get "NodeType=system&NodeID=#####" from Actuate
            //as oppose to the rest of the parameters coming in like this:  "system=######"
            //We also have to make sure NOT to change the selected location in options if the passed location
            //is at customer/guid level.
            var passedNodeType = keyValuePairs.Any(p => p.Contains("NodeType")) ? keyValuePairs.First(p => p.Contains("NodeType")).Split('=')[1] : "";
            if (!isStudentPassed && isLocationNodePassed && !string.Equals(passedNodeType, userData.CurrentCustomerInfo.NodeType, StringComparison.CurrentCultureIgnoreCase))
            {
                var groupName = passedNodeType.ToUpper();
                //NOTE:  I hope this Parse works, since we're now using XMLGroupType everywhere as oppose to a string group name.
                var group = page.GetGroupByType((XMLGroupType)Enum.Parse(typeof(XMLGroupType), groupName));
                group.Options = new List<Option> { new Option { Value = keyValuePairs.First(p => p.Contains("NodeID")).Split('=')[1], IsSelected = true } };

                if (topChangedGroup == null || topChangedGroup.TypeCode > group.TypeCode)
                    topChangedGroup = group;
            }

            extraQueryParams = string.Join("&", extraParams);

            return page;
        }


        private OptionGroup BuildCommonXmlGroup(OptionPage newPage, OptionPage currentPage, XMLGroupType groupType, XMLGroupType updatedGroupType)
        {
            var currentGroup = currentPage.GetGroupByType(groupType);

            if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
            {
                var newGroup = _mapper.MapCommonXmlGroup(newPage.XmlDataFilteringOptions.GetSpecificChild(groupType), currentGroup, SetInvalidGroup);
                return newGroup;
            }

            return currentGroup;
        }

        private OptionGroup BuildGroupPopulationGroup(OptionPage newPage, OptionPage currentPage, XMLReportOptionGroup groupType, XMLGroupType updatedGroupType, UserData userData)
        {
            var currentGroup = currentPage.GetGroupByType(groupType.GroupType);

            if (currentGroup == null || (int)updatedGroupType <= currentGroup.TypeCode)
            {
                var currentLocationNodeType = newPage.LastSelectedLocationGroup?.LocationNodeType;
                var locationNodeType = currentLocationNodeType ?? userData.CurrentCustomerInfo.NodeType;

                var groupPopulations = _dbClient.GetGroupPopulation(newPage.ScoreSetId, Convert.ToInt32(userData.CurrentCustomerInfo.NodeId), locationNodeType);

                var newGroup = _mapper.MapGroupPopulation(groupPopulations, currentGroup, groupType, updatedGroupType, SetInvalidGroup);
                return newGroup;
            }

            return currentGroup;
        }

        private void SetInvalidGroup(OptionGroup group)
        {
            if (InvalidGroup != null)
                return;

            InvalidGroup = group;
            InvalidGroup.DisplayName = _xmlLoader.GetDisplayText(group.Type);
        }

        private bool BuildGroupBasedOnCondition(XMLReportOptionGroup xmlGroup, OptionPage page)
        {
            if (xmlGroup == null || page == null)
                return false;

            var conditionGroup = page.GetGroupByType(xmlGroup.hideConditionGroup);
            var conditionValues = xmlGroup.hideConditionValues.Split(',');
            var reverseCondition = xmlGroup.hideConditionReverse;

            return reverseCondition ? conditionValues.Any(v => conditionGroup.IsValueSelected(v)) : conditionValues.All(v => !conditionGroup.IsValueSelected(v));
        }

        private string GetGradeNumber(GradeLevel gradeLevel)
        {
            return gradeLevel.GradeText.Replace("Grade ", "").Substring(0, 2).Trim();
        }

        private static Dictionary<string, XMLGroupType> GroupNameToTypeMapper => new Dictionary<string, XMLGroupType>
        {

            {"Growth End Point", XMLGroupType.LongitudinalGrowthEndPoint},
            {"Growth Goal", XMLGroupType.LongitudinalGrowthGoal},
            {"Performance Band", XMLGroupType.PerformanceBands},
            {"Composite Calculation Options", XMLGroupType.CompositeCalculationOptions},
            {"Students Coded in Office Use", XMLGroupType.ColumnZ},
            {"Grade/Level", XMLGroupType.GradeLevel},
            {"Display Type", XMLGroupType.DisplayType},
            {"Level of Analysis", XMLGroupType.LevelofAnalysis},
            {"Display Options", XMLGroupType.DisplayOptions},
            {"Skill Domain Classification", XMLGroupType.SkillDomainClassification},
            {"Score(s)", XMLGroupType.Scores},
            {"Assessment", XMLGroupType.Assessment},
            {"Content Scope", XMLGroupType.ContentScope},
            {"Sub Content Scope", XMLGroupType.SubContentScope},
            {"Population Filters", XMLGroupType.PopulationFilters},
            {"Test Administrations", XMLGroupType.LongitudinalTestAdministrations},
            {"Growth Start Point", XMLGroupType.LongitudinalGrowthStartPoint},
            {"Test Administration Date", XMLGroupType.TestAdministrationDate},
            {"STATE", XMLGroupType.STATE},
            {"REGION", XMLGroupType.REGION},
            {"SYSTEM", XMLGroupType.SYSTEM},
            {"DISTRICT", XMLGroupType.DISTRICT},
            {"BUILDING", XMLGroupType.BUILDING},
            {"CLASS", XMLGroupType.CLASS},
            {"Student", XMLGroupType.Student},
        };

        private static string ActuateKeyToGroupName(string actuateValue)
        {
            switch (actuateValue)
            {
                case "ScoreType":
                    return "Score(s)";
                case "StudentID":
                    return "Student";
                case "DisplayType":
                    return "Display Type";
                case "LevelOfAnalysis":
                    return "Level of Analysis";
                case "SubtestList":
                    return "Content Scope";
                default:
                    return actuateValue.Replace('+', ' ');
            }
        }

        private KeyValuePair<string, string> ParseKeyValuePair(string input)
        {
            var indexOfEqual = input.IndexOf('=');
            var key = input.Substring(0, indexOfEqual);
            var val = input.Substring(indexOfEqual + 1, input.Length - indexOfEqual - 1);

            return new KeyValuePair<string, string>(key, val);
        }
    }
}