using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.Config;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using HandyStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace DM.WR.BL.Builders
{
    public class ActuateQueryStringBuilder : IActuateQueryStringBuilder
    {
        private readonly IDbClient _dbClient;

        public ActuateQueryStringBuilder(IDbClient dbClient)
        {
            _dbClient = dbClient;
        }

        public string BuildQueryString(OptionPage page, UserData userData, bool runInBackground, string lastName = null, object extraQueryParams = null)
        {
            var customerInfo = userData.CurrentCustomerInfo;
            var isReportLongitudinal = page.XmlDataFilteringOptions.LongitudinalOptions != null;

            var query = new List<string>
            {
                BuildActuateUrlWithCommonParameters(runInBackground, page.XmlDisplayOption,userData),
                $"AcTemplatePath={ConfigSettings.ActuateVolume}",
                page.ReportXml.isGradeMultiSelect
                    ? $"ListOfGrades={page.GetSelectedValuesStringOf(XMLGroupType.GradePaper)}"
                    : $"GradeLevelList={page.GetSelectedValuesStringOf(XMLGroupType.GradeLevel)}",
                $"HierarchyNodeLevel={page.GetLastSelectedNodeType(customerInfo)}",
                $"HierarchyNodeList={string.Join(",", page.GetLastSelectedNodeIds(customerInfo))}",
                $"GroupSetCode={page.ScoringOptions.GroupsetCode.ToUpper()}",
                $"TestPopulationNodeLevel={page.ScoringOptions.LprNodeLevel}",
                $"TestPopulationNodeList={page.ScoringOptions.LprNodeList}",
                $"RFormat={page.ScoringOptions.RFormat}",
                "OfficeUseList=",
                "OtherInfoList="
            };

            // Word decoding skill reports - pass 2 additional params
            //TODO:  Clean up the if? maybe? beautify?
            if (page.XmlDisplayType == XMLReportType.SWDS || page.XmlDisplayType == XMLReportType.CWDS || page.XmlDisplayType == XMLReportType.WDSR)
            {
                if (page.ReportXml.mediaType == XMLReportMediaType.ROI)
                {
                    if (page.XmlDataFilteringOptions.ContentAreasMenu != null)
                    {
                        if (page.XmlDataFilteringOptions.ContentAreasMenu.showWordSkills)
                            query.Add($"WordskillList={page.ContentScopeAcronyms}");
                    }
                }
                query.Add($"WordskillsetID={page.ScoringOptions.WordskillSetId}");
            }
            else if (page.ReportXml.mediaType == XMLReportMediaType.ROI)
                query.Add($"SubtestList={page.GetSelectedValuesStringOf(XMLGroupType.ContentScope)}");

            if (isReportLongitudinal)
            {
                var scoresetIds = page.GetSelectedAltValuesOf(XMLGroupType.LongitudinalTestAdministrations);
                scoresetIds.Reverse();
                query.Add($"CustomerScoresetIDs={string.Join(",", scoresetIds.Distinct())}");
            }
            else
            {
                query.Add($"CustomerScoresetIDs={page.ScoreSetId}");
            }

            //Population Filters
            if (page.GroupExists(XMLGroupType.PopulationFilters))
            {
                var populationFiltersGroup = page.GetGroupByType(XMLGroupType.PopulationFilters);
                if (populationFiltersGroup.Options.All(o => o is PiledOption))
                {
                    foreach (var pile in populationFiltersGroup.Options.Cast<PiledOption>().GroupBy(o => o.PileKey).ToList())
                    {
                        var selectedValues = pile.Where(o => o.IsSelected).Select(o => o.Value);
                        query.Add($"{pile.Key}=\"{string.Join(",", selectedValues)}\"");
                    }
                }

                //Sub Group Population Filters
                if (page.XmlDataFilteringOptions.PopulationFilterMenu.subgroupsOnly)
                    query.Add($"DisaggSubGroup={page.GetSelectedValuesStringOf(XMLGroupType.PopulationFilters) }");
            }

            if (page.GroupExists(XMLGroupType.AbilityProfile))
                query.Add($"SuppressProfile={page.GetSelectedValuesStringOf(XMLGroupType.AbilityProfile)}");

            if (page.GroupExists(XMLGroupType.CollegeReadiness))
            {
                var selectedValue = page.GetSelectedValuesOf(XMLGroupType.CollegeReadiness).FirstOrDefault();
                var suppress = page.AssessmentCode == XMLProductCodeEnum.CTBS && (page.ReportXml.reportType == XMLReportType.SPP || page.ReportXml.reportType == XMLReportType.GPP);
                var value = suppress ? "0" : selectedValue;

                query.Add($"ACTScoreGrade={value}");
                query.Add($"CollegeReadyScoreGrade={selectedValue}");
            }

            {
                string value = page.GroupExists(XMLGroupType.ColumnZ) ? (page.IncludeColumnZ == 1).ToString() : "";
                query.Add($"ExcludeOUZ={value}");
                query.Add($"ExcludeOUZSubtest={value}");
            }

            if (page.GroupExists(XMLGroupType.SortType))
                query.Add($"SortScore={page.GetSelectedValuesStringOf(XMLGroupType.SortType)}");

            //Passing new param for IPP and GPP paper reports (ST-3839)
            {
                var graphType = page.ReportXml.reportType == XMLReportType.SPP || page.ReportXml.reportType == XMLReportType.GPP ?
                                $"UpperGraphType={page.XmlDisplayOption.code}" :
                                $"GraphType={page.XmlDisplayOption.code}";
                query.Add(graphType);
            }

            //COMPOSITE CALC
            if (page.XmlDataFilteringOptions.CompositeCalcMenu != null)
            {
                if (page.ReportXml.mediaType == XMLReportMediaType.PDF || ((GradeLevel)page.GradeLevel).Battery != "SURVEY")
                {
                    query.Add($"ExcludeMathComputation={page.ExcludeMathComputation == 1}");
                    query.Add($"ExcludeWordAnalysisListening={page.ExcludeEla == 1}");
                }
                else
                {//TODO:  So there is a case when we don't even care what's selected as long as the group exists?
                    query.Add("ExcludeMathComputation=true");
                }
            }

            //Composite Types
            if (page.GroupExists(XMLGroupType.CompositeTypes))
                query.Add($"CogatComposite={page.GetSelectedValuesStringOf(XMLGroupType.CompositeTypes)}");

            //Score(s)  TODO: What a mess!
            string scores = "";
            List<string> selectedScoresIds = page.GetSelectedValuesOf(XMLGroupType.Scores);
            if (selectedScoresIds != null)
            {
                selectedScoresIds.AddRange(page.GetHiddenValuesOf(XMLGroupType.Scores));
                scores = page.XmlDisplayType == XMLReportType.SP || page.XmlDisplayType == XMLReportType.GP ?
                    string.Join("/", selectedScoresIds) :
                    string.Join(",", selectedScoresIds);
            }

            if (!string.IsNullOrEmpty(scores) && (page.XmlDisplayType == XMLReportType.SP || page.XmlDisplayType == XMLReportType.GP))
            {
                string[] scoretype = scores.Split('/');
                string scoregraph = scoretype[0];

                string scorescalecompare = "";
                if (scoretype.Length > 1)
                    scorescalecompare = scoretype[1];

                query.Add("ScoreGraph=" + scoregraph);
                query.Add("ScoreScaleCompare=" + scorescalecompare);
                if (scoretype.Length > 2)
                {
                    if (scoretype[2] == "AP")
                        query.Add("DisplayAbilityScore=Y");
                    else if (scoretype[2] == "SAS")
                        query.Add("DisplaySASScore=Y");
                }
            }
            // Pass scoreability param for all paper background reports and as well as data export
            // This is a hack, in the next release we need to convert all the old datafiltering elements to new global elements so the parameter name is defiend in the xml element
            else if ((page.AssessmentCode == XMLProductCodeEnum.COGAT || page.AssessmentCode == XMLProductCodeEnum.CCAT) &&
                    (page.ReportXml.mediaType == XMLReportMediaType.PDF || page.ReportXml.reportType == XMLReportType.SDE))
            {
                query.Add($"ScoresAbility={scores}");
            }
            else
            {
                query.Add($"Scores={scores}");
            }

            if (page.XmlDisplayType == XMLReportType.SR && page.AssessmentCode == XMLProductCodeEnum.IOWA && scores.Contains("LEXILE"))
                query.Add("LexileScoreDisplay=RANGE");

            // STAAR/GMAS report & predicted roster report
            var staarGmasReportTypes = new[] { XMLReportType.SPS, XMLReportType.SSR, XMLReportType.GPS, XMLReportType.GSR };
            if (staarGmasReportTypes.Contains(page.XmlDisplayType))
            {
                if (page.ScoringOptions.SubtestCutscoreFamilyId != null)
                    query.Add($"CutScoreFamilyID={page.ScoringOptions.SubtestCutscoreFamilyId}");
                if (page.ScoringOptions.PredSubtestGroupType != null)
                    query.Add($"SubtestGroupType={page.ScoringOptions.PredSubtestGroupType}");
            }

            //Graph Scores
            if (page.GroupHasSelectedOptions(XMLGroupType.GraphScores))
                query.Add($"GraphScores={page.GetSelectedValuesStringOf(XMLGroupType.GraphScores)}");

            //GroupPopulation
            if (page.GroupExists(XMLGroupType.GroupPopulation))
                query.Add($"{page.XmlDataFilteringOptions.GroupPopulation.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.GroupPopulation)}");

            //ComparisonGrouping
            if (page.GroupExists(XMLGroupType.ComparisonGrouping))
                query.Add($"{page.XmlDataFilteringOptions.ComparisonGrouping.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.ComparisonGrouping)}");

            //Home Reporting
            if (page.GroupExists(XMLGroupType.HomeReporting))
            {
                query.Add($"Language={page.GetSelectedValuesStringOf(XMLGroupType.HomeReporting)}");
                query.Add($"HomeReporting={page.GetSelectedAltValuesStringOf(XMLGroupType.HomeReporting)}");
            }

            //SkillSet
            if (page.GroupExists(XMLGroupType.SkillDomainClassification))
            {
                string skillSet;
                if (page.XmlDataFilteringOptions.Skillset.getValuesOnSubmit)
                {
                    var dbSkillSets = _dbClient.GetSkillSets(customerInfo.CustomerId.ToString(), page.TestAdminValue, null, page.XmlDisplayOption.reportCode);
                    skillSet = string.Join(",", dbSkillSets.Select(s => s.Id).ToList());
                }
                else if (page.IsGroupHidden(XMLGroupType.SkillDomainClassification))
                {
                    skillSet = page.ScoringOptions.SkillSetId.ToString();
                }
                else
                {
                    skillSet = page.GetSelectedValuesStringOf(XMLGroupType.SkillDomainClassification);
                    query.Add($"SubtestType={((SkillSet)page.SkillSet).SubtestGroupType}");
                }
                query.Add($"SkillsetID={skillSet}");
            }

            if (page.GroupExists(XMLGroupType.SubContentScope))
                query.Add($"SkillIDList={page.GetSelectedValuesStringOf(XMLGroupType.SubContentScope).Replace("'", "")}");

            //Longitudinal
            if (isReportLongitudinal)
            {
                query.Add($"GrowthStartPointType={page.GetSelectedValuesStringOf(XMLGroupType.LongitudinalGrowthStartPointType)}");
                if (page.GroupExists(XMLGroupType.LongitudinalGrowthStartPoint))
                    query.Add($"GrowthStartPoint={page.GetSelectedAltValuesStringOf(XMLGroupType.LongitudinalGrowthStartPoint)}");
                query.Add($"GrowthEndPoint={page.GetSelectedValuesStringOf(XMLGroupType.LongitudinalGrowthEndPoint)}");
                query.Add($"GoalSeason={page.GetFirstSelectionTextOf(XMLGroupType.LongitudinalGrowthEndPoint)}");
                query.Add($"GrowthGoal={page.GetSelectedValuesStringOf(XMLGroupType.LongitudinalGrowthGoal)}");

                if (page.XmlDataFilteringOptions.LongitudinalOptions.TestAdministrations.showGradesSubgroup)
                {
                    query.Add($"LongitudinalType={page.GetSelectedValuesStringOf(XMLGroupType.LongitudinalTypes)}");
                    var selectedOptions = page.GetOptionsOfGroup<Option>(XMLGroupType.LongitudinalTestAdministrations).Where(o => o.IsSelected).Cast<LongitudinalTestAdminOption>();
                    var gradeLevels = selectedOptions.Select(o => o.GradeLevels.Find(gl => gl.IsSelected).Value).Reverse();
                    query.Add($"TestAdminGradeLevelIDs={string.Join(",", gradeLevels)}");
                }
            }

            //get Supress sql  
            {
                var suppressedDimensions = _dbClient.GetDimensionsToSuppress(page.ScoreSetId, page.GetLastSelectedNodeIdsString(customerInfo), page.GetLastSelectedNodeType(customerInfo));
                if (!string.IsNullOrEmpty(suppressedDimensions.SuppressProgramLabels))
                    query.Add($"SuppressProgramLabel={suppressedDimensions.SuppressProgramLabels}");
            }

            //FOR PDF ONLY
            if (page.GroupExists(XMLGroupType.SortDirection))
                query.Add($"RankingDirection={page.GetSelectedValuesStringOf(XMLGroupType.SortDirection)}");

            if (page.GroupExists(XMLGroupType.SortBySubtest))
                query.Add($"RankingSubtest={page.GetSelectedValuesStringOf(XMLGroupType.SortBySubtest)}");

            if (page.GroupExists(XMLGroupType.SortType))
                query.Add($"RankingScore={page.GetSelectedValuesStringOf(XMLGroupType.SortType)}");

            //ReportGrouping param is passed to Group Summary Reports only
            // TODO:  This is already passed to actuate above.  Check with actuate folks.
            if (page.ReportXml.reportType == XMLReportType.GS)
                query.Add($"ReportGrouping={page.GetLastSelectedNodeType(customerInfo)}");

            //need to pass this param so that the order label does not get displayed in the paper reports
            //TODO: Yet, there is no check for report type being paper or not???
            query.Add("OrderNumberLabel=");

            //Performance Bands (Interim Proficiency Profile)
            if (page.GroupExists(XMLGroupType.PerformanceBands))
            {
                var options = page.GetOptionsOfGroup<PerformanceBandOption>(XMLGroupType.PerformanceBands);
                var bands = new List<string>();

                foreach (var option in options)
                {
                    bands.Add($"{option.BandColor.ToString().ToUpper()}^{option.Text}^{option.LowValue}^{option.HighValue}");
                }

                query.Add($"PB1={string.Join("[", bands)}");
            }

            //Score Filters
            if (page.GroupExists(XMLGroupType.ScoreFilters))
            {
                var rows = ((ScoreFiltersGroup)page.GetGroupByType(XMLGroupType.ScoreFilters)).Rows;
                query.Add("SubtestFilters=\"" + string.Join("", rows.Select(r => r.QueryString)).ToUpper().Replace("K.", "(K).").Replace("P.", "(P).").Replace("'", "") + "\"");
            }

            //Report Population Filter
            {
                var reportPopulationFilter = "";

                if ((page.XmlDisplayType == XMLReportType.SP || page.XmlDisplayType == XMLReportType.SWDS)
                    && page.GetSelectedValuesStringOf(XMLGroupType.Student) != "-1")
                {
                    //PM.101-3189 -if it is longitudinal report
                    reportPopulationFilter = page.XmlDataFilteringOptions.LongitudinalOptions != null
                        ? $"\"sdr_student.student_id={page.GetSelectedValuesStringOf(XMLGroupType.Student)}\""
                        : $"\"sdr_testinstance.testinstance_id={page.GetSelectedAltValuesStringOf(XMLGroupType.Student)}\"";
                }
                else
                {
                    if (page.GroupExists(XMLGroupType.ScoreWarnings))
                    {
                        var group = (ScoreWarningsGroup)page.GetGroupByType(XMLGroupType.ScoreWarnings);
                        reportPopulationFilter = group.QueryString;
                    }
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        reportPopulationFilter = string.IsNullOrEmpty(reportPopulationFilter)
                            ? $"sdr_student.last_name like '{lastName.ToUpper()}%25'"
                            : $"sdr_student.last_name like '{lastName.ToUpper()}%25' AND {reportPopulationFilter}";
                    }
                }

                if (!string.IsNullOrEmpty(reportPopulationFilter))
                    query.Add($"ReportPopulationFilter={reportPopulationFilter}");
            }

            {//TODO:  Beautify this logic
                if (page.AssessmentCode == XMLProductCodeEnum.CCAT)
                    query.Add("TestfamilyGroupCode=COGAT");
                else if (page.Assessment.IsIss)
                {
                    query.Add("TestfamilyGroupCode=IOWA");
                    query.Add($"ISSSubtestGroupType={page.AssessmentCode}");
                }
                else if (page.AssessmentCode == XMLProductCodeEnum.CTBS)
                {
                    query.Add("TestfamilyGroupCode=IOWA");
                    query.Add("SuppressACTPredicted=TRUE");

                    if (page.ReportXml.reportType == XMLReportType.LOSS) //SCWO-249
                    {
                        query.Add("ACTScoreGrade=0");  // (ST-4947) 
                        query.Add("CollegeReadyScoreGrade=0"); // (ST-4947) 
                    }
                }
                else
                    query.Add($"TestfamilyGroupCode={page.AssessmentCode}");
            }

            //DATA EXPORT & Custom Data Fields
            if (page.XmlDisplayType == XMLReportType.SDE)
            {
                var ctxt = HttpContext.Current;
                var appPath = ctxt.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;
                var serverUrl = $"{ctxt.Request.Url.GetLeftPart(UriPartial.Authority)}{appPath}/Report/DownloadManager";
                query.Add($"IRMWebLocation={serverUrl}");

                query.Add($"ExportOutputDirectory={HttpUtility.UrlEncode(ConfigSettings.DataExportDirectory)}");

                switch (page.AssessmentCode)
                {
                    case XMLProductCodeEnum.IOWA_INTERIM:
                        query.Add("TestFamily=Interim&RFormat=INTERIM");
                        break;
                    case XMLProductCodeEnum.CCAT:
                        query.Add("TestFamily=COGAT");
                        break;
                    case XMLProductCodeEnum.CTBS:
                        query.Add("TestFamily=IOWA");
                        break;
                    case XMLProductCodeEnum.ISSMATH:
                        query.Add("TestFamily=IOWA");
                        break;
                    case XMLProductCodeEnum.ISSREAD:
                        query.Add("TestFamily=IOWA");
                        break;
                    case XMLProductCodeEnum.ISSSCI:
                        query.Add("TestFamily=IOWA");
                        break;
                    default:
                        query.Add($"TestFamily={page.AssessmentCode}");
                        break;
                }

                //special case for Logramos
                if (page.AssessmentCode == XMLProductCodeEnum.LOGRAMOS)
                {
                    query.Add("RFormat=LOGRAMOS");
                    query.Add("TestSet=Logramos");
                    query.Add("LogoImage=Log2014");
                }

                query.Add("Accountability=true");

                query.Add($"{page.XmlDataFilteringOptions.ExportFormat.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.ExportFormat)}");
                query.Add($"{page.XmlDataFilteringOptions.ExportHeadings.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.ExportHeadings)}");

                if (page.GroupExists(XMLGroupType.CustomDataFields))
                {
                    var group = (CustomFieldGroup)page.GetGroupByType(XMLGroupType.CustomDataFields);
                    var options = group.Options.Cast<CustomFieldOption>().ToList();
                    var itemsList = new List<string>();
                    foreach (var value in group.SelectedValuesOrder)
                    {
                        var option = options.First(o => o.Value == value);

                        var text = option.UserText ?? option.Text;
                        var width = option.UserWidth ?? option.Width;
                        var padding = option.Padding;
                        var d = group.Delimiter;

                        itemsList.Add($"{value}{d}{text}{d}{width}{d}{padding}{d}");
                    }
                    query.Add($"ExportFieldFormat={string.Join(group.Separator, itemsList)}");
                }
            }
            else
            {
                query.Add($"Accountability={page.ScoringOptions.AccountabilityFlag.ToBoolean()}");
            }

            if (page.ReportXml.Parent.productCode == XMLProductCodeEnum.LOGRAMOS && page.ReportXml.mediaType == XMLReportMediaType.PDF)
            {
                query.Add("TestSet=Logramos");
                query.Add("LogoImage=Log2014");
            }

            if (page.GroupExists(XMLGroupType.CogatDifferences))
                query.Add($"{page.XmlDataFilteringOptions.CogatDifferences.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.CogatDifferences)}");

            //ShowPredictedScores
            if (page.GroupExists(XMLGroupType.ShowPredictedScores))
                query.Add($"{page.XmlDataFilteringOptions.ShowPredictedScores.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.ShowPredictedScores)}");

            //Group By
            if (page.GroupExists(XMLGroupType.ReportGrouping))
                query.Add($"{page.XmlDataFilteringOptions.ReportGrouping.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.ReportGrouping)}");

            //ReportMediaType
            if (page.GroupExists(XMLGroupType.ReportMediaType))
                query.Add($"{page.XmlDataFilteringOptions.ReportMediaType.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.ReportMediaType)}");
            else if (page.ReportXml.mediaType == XMLReportMediaType.PDF)
                query.Add("MediaType=PDF");

            //Show Reading Total
            if (page.GroupExists(XMLGroupType.ShowReadingTotal))
            {//TODO: Messy mapping in 4.0.  Make sure this works in 5.0
                var value = page.GetSelectedValuesStringOf(XMLGroupType.ShowReadingTotal);
                //add parameter ONLY when user chose NOT to display Reading Total 
                if (value.Contains("RDGTOTL"))
                    query.Add($"SuppressSubtests='{value}'");
            }

            //Lower Graph Type
            if (page.GroupExists(XMLGroupType.LowerGraphType))
                query.Add($"{page.XmlDataFilteringOptions.LowerGraphType.queryKey}={page.GetSelectedValuesStringOf(XMLGroupType.LowerGraphType)}");

            //We need to pass bit map path of th report if it is in the report element
            if (page.ReportXml.bitmapSubPath != null)
            {
                query.Add($"BitmapPath={ConfigSettings.BitmapPath}{page.ReportXml.bitmapSubPath}");
            }

            if (extraQueryParams != null)
                query.Add(extraQueryParams.ToString());

            //TODO:  Check with Scoring of this param is needed. Currently, we are always sending "Multiple".
            //var popFilterGroupArr = selectedOptions.Where(og => og.Name == "Population Filters");
            //var disaggLabel = (popFilterGroupArr.Count() > 0 && popFilterGroupArr.First().SelectedOptions == null) ? "All Students" : "Multiple";
            //query.Add("DisaggLabel=" + disaggLabel);

            var customerDisplayOptionsString = _dbClient.GetCustomerDisplayOptionsString(page.ScoreSetId);

            if (string.IsNullOrEmpty(customerDisplayOptionsString))
                return string.Join("&", query);

            return $"{string.Join("&", query)}{customerDisplayOptionsString}";
        }

        public string BuildMultimeasureQueryString(OptionBook book, UserData userData, bool runInBackground, string lastName = null)
        {
            var firstPage = book.GetFirstPage();

            var query = new List<string> { BuildActuateUrlWithCommonParameters(runInBackground, firstPage.XmlDisplayOption,userData) };

            var allScoresetIds = new List<string>();
            var scoreFiltersQuery = new List<string>();
            var pageQuery = new List<string>();
            for (int c = 0; c < book.Pages.Count; ++c)
            {
                var page = book.GetPage(c);

                if (page.GroupExists(XMLGroupType.ScoreFilters))
                {
                    var row = ((ScoreFiltersGroup)page.GetGroupByType(XMLGroupType.ScoreFilters)).Rows.First();
                    scoreFiltersQuery.Add(row.MultimeasureQueryString.Replace("__PAGE_NUM__", (c + 1).ToString()));
                }

                var levelOfAnalysis = page.GetSelectedValuesStringOf(XMLGroupType.LevelofAnalysis);

                var contentScopeText = levelOfAnalysis != "SD" ? HttpUtility.UrlEncode(page.GetFirstSelectionTextOf(XMLGroupType.ContentScope)) : "";
                var contentScopeValue = page.GetSelectedValuesStringOf(XMLGroupType.ContentScope).Replace("'", "");
                var contentScope = $"{contentScopeText}Z{contentScopeValue}";

                var skillText = HttpUtility.UrlEncode(page.GetFirstSelectionTextOf(XMLGroupType.SubContentScope).Replace("'", ""));
                var skillSelectedValue = page.GetSelectedValuesStringOf(XMLGroupType.SubContentScope).Replace("'", "");
                var skillValue = string.IsNullOrEmpty(skillSelectedValue) && skillText.ToLower() == "none" ? " " : skillSelectedValue;
                var skill = $"{skillText}Z{skillValue}";

                var assessmentText = page.GetFirstSelectionTextOf(XMLGroupType.Assessment);

                var gradeText = page.GetFirstSelectionTextOf(XMLGroupType.GradeLevel).Replace("Grade ", "").Substring(0, 2).Trim();
                var gradeLevelId = page.GetSelectedValuesStringOf(XMLGroupType.GradeLevel);
                var scoresValue = page.GetSelectedValuesStringOf(XMLGroupType.Scores);

                var testAdminValue = page.GetSelectedValuesStringOf(XMLGroupType.TestAdministrationDate);
                var testAdminDateString = ((List<TestAdmin>)page.TestAdministrations).First(i => i.Id.ToString() == testAdminValue).Date;
                var testAdminDate = Convert.ToDateTime(testAdminDateString).ToString("MM/dd/yyyy");

                var scoresetId = page.ScoreSetId.ToString();
                allScoresetIds.Add(scoresetId);

                var forms = page.MultimeasureForms;

                var slotSkillSetId = levelOfAnalysis == "SD" ? page.GetSelectedValuesStringOf(XMLGroupType.SkillDomainClassification) : "";

                var levelId = page.GetSelectedAltValuesStringOf(XMLGroupType.GradeLevel);

                pageQuery.Add(
                    $"M{c + 1}={contentScope}Z{skill}Z{assessmentText}Z{forms}Z{gradeText}Z{scoresValue}Z{testAdminDate}Z{scoresetId}Z{gradeLevelId}Z{slotSkillSetId}Z{levelId}");
            }

            query.Add(string.Join("&", pageQuery));

            if (scoreFiltersQuery.All(q => !string.IsNullOrEmpty(q)))
            {
                //SubtestFilters="S:NPR:1:<:90|AND|S:NPR:1:>:1"
                //NOTE: (K) & (P) is for GE score when the user types in values like K.1, K.2
                var resultString = string.Join("", scoreFiltersQuery).ToUpper().Replace("K.", "(K).").Replace("P.", "(P).").Replace("'", "");
                query.Add($"SubtestFilters=\"{resultString}\"");
            }

            //--------------------------------------------------------
            //Performance Bands
            //
            //PB1=GREEN^User Category Name^1^20#BLUE^User Category Name^21^30
            //PB2=GREEN^User Category Name^1^20#BLUE^User Category Name^21^30
            //
            //have to send Performance Bands even if no user selection is made
            //--------------------------------------------------------
            var bandsQuery = new List<string>();
            var bandsGroups = book.Pages.Where(p => p.GroupExists(XMLGroupType.PerformanceBands))
                                        .Select(p => p.GetGroupByType(XMLGroupType.PerformanceBands))
                                        .Cast<PerformanceBandGroup>()
                                        .ToList();
            for (int c = 0; c < bandsGroups.Count; ++c)
            {
                var bandsSingleQueries = bandsGroups[c].Options
                                            .Cast<PerformanceBandOption>()
                                            .Select(b => $"{b.BandColor.ToString().ToUpper()}^{b.Text}^{b.LowValue}^{b.HighValue}");
                bandsQuery.Add($"PB{c + 1}={string.Join("[", bandsSingleQueries)}");
            }
            if (bandsQuery.Any())
                query.Add(string.Join("&", bandsQuery));

            //Students Coded, Column Z, OUZ or whatever you call it
            var columnZ = firstPage.GroupExists(XMLGroupType.ColumnZ) ? (firstPage.IncludeColumnZ == 0).ToString() : "";
            query.Add($"ExcludeOUZ={columnZ}");

            //TODO:  Very weird stuff down here....
            //Population Filter
            var disaggValue = firstPage.GroupExists(XMLGroupType.PopulationFilters) ?
                              firstPage.GetGroupByType(XMLGroupType.PopulationFilters).HasSelectedValues ? "All Students" : "Multiple" : "";
            query.Add($"DisaggLabel={disaggValue}");

            //Locations
            string selectedHierarchyLevel;
            string selectedHierarchyId;
            var locationGroups = firstPage.GetGroupsOfCategory(OptionsCategory.Locations);
            var isNodeLevelLocation = locationGroups.Count == 1 && locationGroups.First().SelectedValues.Contains("-1");

            if (isNodeLevelLocation)
            {
                selectedHierarchyLevel = userData.CurrentCustomerInfo.NodeType;
                selectedHierarchyId = userData.CurrentCustomerInfo.NodeId.ToString();
            }
            else
            {
                selectedHierarchyLevel = firstPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo);
                selectedHierarchyId = firstPage.GetLastSelectedNodeId(userData.CurrentCustomerInfo).ToString();
            }
            query.Add($"HierarchyNodeLevel={selectedHierarchyLevel}");
            query.Add($"HierarchyNodeList={selectedHierarchyId}");

            //SkillSetId
            var skillSetIdsList = book.Pages.SelectMany(p => p.GetSelectedValuesOfNonHiddenGroups(XMLGroupType.SkillDomainClassification)).ToList();
            var skillSetIds = skillSetIdsList.Any() ? string.Join(",", skillSetIdsList.Distinct()) : Convert.ToString(firstPage.ScoringOptions.SkillSetId);
            query.Add($"SkillsetID={skillSetIds}");

            //Composite Calculation Options
            string excludeMathComputation = "";
            string excludeWordAnalysisListening = "";
            if (firstPage.GroupExists(XMLGroupType.CompositeCalculationOptions))
            {
                excludeMathComputation = (firstPage.ExcludeMathComputation == 1).ToString();
                excludeWordAnalysisListening = (firstPage.ExcludeEla == 1).ToString();
            }

            query.Add($"ExcludeMathComputation={excludeMathComputation}");
            query.Add($"ExcludeWordAnalysisListening={excludeWordAnalysisListening}");
            query.Add($"TestPopulationNodeLevel={firstPage.ScoringOptions.LprNodeLevel}");
            query.Add($"TestPopulationNodeList={firstPage.ScoringOptions.LprNodeList}");
            query.Add($"CustomerScoresetIDs={string.Join(",", allScoresetIds.Distinct())}");

            var gradeLevels = new List<string>();
            foreach (var page in book.Pages)
                gradeLevels.AddRange(page.GetSelectedValuesOf(XMLGroupType.GradeLevel));
            query.Add($"GradeLevelList={string.Join(",", gradeLevels.Distinct())}");

            query.Add($"AcTemplatePath={ConfigSettings.ActuateVolume}");
            query.Add($"Accountability={Convert.ToBoolean(firstPage.ScoringOptions.AccountabilityFlag)}");

            var supressionLabels = _dbClient.GetCustomerDisplayOptionsString(int.Parse(allScoresetIds.First()));
            query.Add(supressionLabels.TrimStart('&'));

            if (!string.IsNullOrEmpty(lastName))
                query.Add($"ReportPopulationFilter=sdr_student.last_name like '{lastName.ToUpper()}%25'");

            return string.Join("&", query);
        }

        public string BuildBackgroundReportParameters(int criteriaId, string criteriaName, OptionPage page, UserData userData)
        {
            var result = new List<string>
            {
                "&__requesttype=scheduled",
                $"userid={userData.UserId+"_"+userData.CurrentGuid}",
                "password=",
                "showbanner=false",
                $"jobName={WebUtility.UrlEncode(criteriaName)}",
                $"__outputName={WebUtility.UrlEncode(criteriaName)}.ROI",
                "__outputType=ROI",
                "jobType=async",
                $"__headline={criteriaId}"
            };

            if (page.GroupExists(XMLGroupType.ReportMediaType) && page.GetSelectedValuesStringOf(XMLGroupType.ReportMediaType) == "ExcelData")
                result.Add("Export=true");
            else
                result.Add($"__format={page.ReportXml.mediaType}");

            return string.Join("&", result);
        }

        private string BuildActuateUrlWithCommonParameters(bool runInBackground, XMLDisplayOption xmlDisplayOption,UserData userData)
        {
            var actuateCommand = runInBackground ? ConfigSettings.AcCommandSubmit : ConfigSettings.AcCommandExecute;
            
            var query = new List<string>{
                $"{ConfigSettings.AcWebLocation}{actuateCommand}?__executableName={ConfigSettings.ActuateVolume}{xmlDisplayOption.url}",
                $"__progressive={(!runInBackground).ToString().ToLower()}",
                $"__saveOutput={runInBackground.ToString().ToLower()}",
                $"__priority={xmlDisplayOption.priority}",
                "invokesubmit=true",
                "SMVersion=",
                "IrmVersion=5.0"
            };

            //Add admin username and password
            if (!runInBackground)
            {
                query.Add($"userid={userData.UserId + "_" + userData.CurrentGuid}");
                query.Add($"password=");
            }

            return string.Join("&", query);
        }
    }
}