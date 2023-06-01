using DM.WR.Data.Repository;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DM.WR.BL.Builders
{
    public interface IOptionPageParser
    {
        int GetACTScoreGrade(IOptionPage optionPage);
        string GetCogatComposite(IOptionPage optionPage);
        int GetCollegeReadyScoreGrade(IOptionPage optionPage);
        string GetGraphType(IOptionPage optionPage);
        bool GetHomeReporting(IOptionPage optionPage);
        string GetRankingDirection(IOptionPage optionPage);
        string GetRankingScore(IOptionPage optionPage);
        string GetRankingSubtest(IOptionPage optionPage);
        string GetReportFormat(IOptionPage optionPage);
        string GetReportGrouping(IOptionPage optionPage, UserData userData);
        bool GetSuppressProfile(IOptionPage optionPage);
        string GetDisAggLabel(IOptionPage optionPage);
        Dictionary<string, string> GetcustomerDisplayOptions(IOptionPage optionPage);
        string GetTestfamilyGroupCode(IOptionPage optionPage);
        string GetGraphScores(IOptionPage optionPage);
        string GetUpperGraphType(IOptionPage optionPage);
        string GetTestSet(IOptionPage optionPage);
        string SuppressSubtests(IOptionPage optionPage);
        string GetSuppressProgramLabel(OptionPage optionPage, UserData userData);
        string GetSortByNodelevel(IOptionPage optionPage);
        string GetExportFormat(IOptionPage optionPage);
        string GetCogatDifferences(IOptionPage optionPage);
        string GetExportFieldFormat(IOptionPage optionPage);
        string GetExportHeading(IOptionPage optionPage);
        string GetGroupPopulation(IOptionPage optionPage);
    }

    public class OptionPageParser : IOptionPageParser
    {
        private readonly IDbClient _dbClient;
        public OptionPageParser(IDbClient dbClient)
        {
            _dbClient = dbClient;
        }

        public string GetGroupPopulation(IOptionPage optionPage)
        {
            string groupPopulation = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.GroupPopulation))
            {
                groupPopulation = optionPage.GetSelectedValuesStringOf(XMLGroupType.GroupPopulation);
            }
            return groupPopulation;
        }
        public string GetExportFieldFormat(IOptionPage optionPage)
        {
            /* var delimiter = "";
            if (optionPage.GroupExists(XMLGroupType.CustomDataFields))
            {
                var group = (CustomFieldGroup)optionPage.GetGroupByType(XMLGroupType.CustomDataFields);
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
                delimiter = string.Join(group.Separator, itemsList);
            }
                return delimiter;*/
            var delimiter = "";
            if (optionPage.GroupExists(XMLGroupType.CustomDataFields))
            {
                var group = (CustomFieldGroup)optionPage.GetGroupByType(XMLGroupType.CustomDataFields);
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
                delimiter = string.Join(group.Separator, itemsList);
            }
            return delimiter;
        }
        public string GetCogatDifferences(IOptionPage optionPage)
        {
            string cogatDiff = optionPage.GetSelectedValuesStringOf(XMLGroupType.CogatDifferences);
            if (cogatDiff == "")
            {
                cogatDiff = null;
            }
            return cogatDiff;
        }
        public string GetExportHeading(IOptionPage optionPage)
        {
            string exportHeading = string.Empty;
            exportHeading = optionPage.GetSelectedValuesStringOf(XMLGroupType.ExportHeadings);
            return exportHeading;
        }
        public string GetExportFormat(IOptionPage optionPage)
        {
            string exportFormat = string.Empty;
            exportFormat = optionPage.GetSelectedValuesStringOf(XMLGroupType.ExportFormat);
            return exportFormat;
        }
        public string SuppressSubtests(IOptionPage optionPage)
        {
            var SuppressSubtests = "";
            if (optionPage.GroupExists(XMLGroupType.ShowReadingTotal))
            {
                var value = optionPage.GetSelectedValuesStringOf(XMLGroupType.ShowReadingTotal);
                if (value.Contains("RDGTOTL"))
                    SuppressSubtests = value;
            }
            return SuppressSubtests;
        }
        public string GetSuppressProgramLabel(OptionPage optionPage, UserData userData)
        {
            var retVal = "";
            var suppressedDimensions = _dbClient.GetDimensionsToSuppress(optionPage.ScoreSetId, optionPage.GetLastSelectedNodeIdsString(userData.CurrentCustomerInfo), optionPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo));
            if (!string.IsNullOrEmpty(suppressedDimensions.SuppressProgramLabels))
            {
                return suppressedDimensions.SuppressProgramLabels;
            }
            return retVal;
        }
        public string GetTestSet(IOptionPage optionPage)
        {
            var testSet = "";
            //var showPredictedScores = XMLGroupType.ShowPredictedScores;

            if (optionPage.GroupExists(XMLGroupType.ShowPredictedScores))
            {
                testSet = optionPage.GetSelectedValuesStringOf(XMLGroupType.ShowPredictedScores);
            }



            return testSet;
        }
        public string GetGraphScores(IOptionPage optionPage)
        {
            var  GraphScores = "";
            if (optionPage.GroupHasSelectedOptions(XMLGroupType.GraphScores))
            {
                GraphScores = optionPage.GetSelectedValuesStringOf(XMLGroupType.GraphScores);
            }
            return GraphScores;
            
        }
        public string GetUpperGraphType(IOptionPage optionPage)
        {
            var UpperGraphType = string.Empty;
            
            if (optionPage.ReportXml.reportType == XMLReportType.SPP || optionPage.ReportXml.reportType == XMLReportType.GPP)
            {
                UpperGraphType = optionPage.XmlDisplayOption.code;
            }
            else
            {
                UpperGraphType = optionPage.XmlDisplayOption.code;
            }
            return UpperGraphType;
        }
        public int GetACTScoreGrade(IOptionPage optionPage)
        {
            if (optionPage.GroupExists(XMLGroupType.CollegeReadiness))
            {
                var suppress = optionPage.AssessmentCode == XMLProductCodeEnum.CTBS &&
                              (optionPage.ReportXml.reportType == XMLReportType.SPP || optionPage.ReportXml.reportType == XMLReportType.GPP);
                var value = suppress ? "0" : optionPage.GetSelectedValuesOf(XMLGroupType.CollegeReadiness).FirstOrDefault();
                return int.Parse(value);
            }

            return 0;
        }

        public int GetCollegeReadyScoreGrade(IOptionPage optionPage)
        {
            if (optionPage.GroupExists(XMLGroupType.CollegeReadiness))
            {
                var selectedValue = optionPage.GetSelectedValuesOf(XMLGroupType.CollegeReadiness).FirstOrDefault();
                return int.Parse(selectedValue);
            }

            return 6;
        }
        public string GetDisAggLabel(IOptionPage optionPage)
        {
            var disaggValue = optionPage.GroupExists(XMLGroupType.PopulationFilters) ?
                              optionPage.GetGroupByType(XMLGroupType.PopulationFilters).HasSelectedValues ? "All Students" : "Multiple" : "Multiple";
            
            return disaggValue;
        }
        public Dictionary<string, string> GetcustomerDisplayOptions(IOptionPage optionPage)
        {
            var customerDisplayOptionsString = _dbClient.GetCustomerDisplayOptionsString(optionPage.ScoreSetId);
            var dict = customerDisplayOptionsString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => split[0], split => split[1]);
            return dict;
        }
        public string GetReportFormat(IOptionPage optionPage)
        {
            var reportFormat = optionPage.AssessmentCode.ToString();
            if (optionPage.AssessmentCode == XMLProductCodeEnum.CCAT)
            {
                reportFormat = "COGAT";
            }
            else if (optionPage.Assessment.IsIss || optionPage.AssessmentCode == XMLProductCodeEnum.CTBS)
            {
                reportFormat = "IOWA";
            }

            return reportFormat;
        }
        public string GetTestfamilyGroupCode (IOptionPage optionPage)
        {

            var testFamilyGroupCode = optionPage.AssessmentCode.ToString();
            if (optionPage.AssessmentCode == XMLProductCodeEnum.CCAT)
            {
                testFamilyGroupCode = "COGAT";
            }
            else if (optionPage.Assessment.IsIss)
            {
                testFamilyGroupCode = "IOWA";
            }
            else if(optionPage.AssessmentCode == XMLProductCodeEnum.CTBS)
            {
                testFamilyGroupCode = "IOWA";
            }
            else
            {
                return testFamilyGroupCode;
            }
            return testFamilyGroupCode;
           
        }
        public string GetSortByNodelevel(IOptionPage optionPage)
        {
            var nodeLevel = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.ReportGrouping))
            {
                if (optionPage.XmlDataFilteringOptions.ReportGrouping.queryKey == "sortbynodelevel")
                {
                    nodeLevel = optionPage.GetSelectedValuesStringOf(XMLGroupType.ReportGrouping);
                }
                
            }
            return nodeLevel;   
        }
        public string GetReportGrouping(IOptionPage optionPage, UserData userData)
        {
            var reportGrouping = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.ReportGrouping))
            {
                reportGrouping = optionPage.GetSelectedValuesStringOf(XMLGroupType.ReportGrouping);
            }
            else if (optionPage.ReportXml.reportType == XMLReportType.GS)
            {
                reportGrouping = optionPage.GetLastSelectedNodeType(userData.CurrentCustomerInfo);
            }
            return reportGrouping;
        }
        public string GetCogatComposite(IOptionPage optionPage)
        {
            string cogatComposite = null;
            if (optionPage.GroupExists(XMLGroupType.CompositeTypes))
            {
                cogatComposite = optionPage.GetSelectedValuesStringOf(XMLGroupType.CompositeTypes);
            }
            return cogatComposite;
        }
        public bool GetSuppressProfile(IOptionPage optionPage)
        {
            bool suppressProfile;
            var suppressProfileString = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.AbilityProfile))
            {
                suppressProfileString = optionPage.GetSelectedValuesStringOf(XMLGroupType.AbilityProfile);
            }
            suppressProfile = bool.Parse(suppressProfileString);
            return suppressProfile;
        }
        public string GetGraphType(IOptionPage optionPage)
        {
            var graphType = string.Empty;
            if (optionPage.ReportXml.reportType == XMLReportType.SPP || optionPage.ReportXml.reportType == XMLReportType.GPP)
            {
                graphType = optionPage.XmlDisplayOption.code;
            }
            else
            {
                graphType = optionPage.XmlDisplayOption.code;
            }
            if (optionPage.GroupExists(XMLGroupType.LowerGraphType))
            {
                graphType = optionPage.GetSelectedValuesStringOf(XMLGroupType.LowerGraphType);
            }
            return graphType;
        }
        public bool GetHomeReporting(IOptionPage optionPage)
        {
            bool homeReporting;
            var homeGroupingString = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.HomeReporting))
            {
                homeGroupingString = optionPage.GetSelectedAltValuesStringOf(XMLGroupType.HomeReporting);
            }
            homeReporting = bool.Parse(homeGroupingString);
            return homeReporting;
        }
        public string GetRankingDirection(IOptionPage optionPage)
        {
            var rankingDirection = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.SortDirection))
            {
                rankingDirection = optionPage.GetSelectedValuesStringOf(XMLGroupType.SortDirection);
            }
            return rankingDirection;
        }
        public string GetRankingSubtest(IOptionPage optionPage)
        {
            var rankingSubtest = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.SortBySubtest))
            {
                rankingSubtest = optionPage.GetSelectedValuesStringOf(XMLGroupType.SortBySubtest);
            }
            return rankingSubtest;
        }
        public string GetRankingScore(IOptionPage optionPage)
        {
            var rankingScore = string.Empty;
            if (optionPage.GroupExists(XMLGroupType.SortType))
            {
                rankingScore = optionPage.GetSelectedValuesStringOf(XMLGroupType.SortType);
            }
            return rankingScore;
        }
    }
}
