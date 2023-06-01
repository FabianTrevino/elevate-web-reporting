using DM.WR.Data.Repository;
using DM.WR.Models.Options;
using DM.WR.Models.SMI;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using HandyStuff;
using System.Linq;

namespace DM.WR.BL.Builders
{
    public interface ISMIModelBuilder
    {
        SMIBaseParameters BuildSMIBaseParameters(CustomerInfo customerInfo, IOptionPage optionPage);

        SMIFilteringParameters BuildSMIFilteringParameters(IOptionPage optionPage);

        SMISkillParameters BuildSMISkillParameters(CustomerInfo customerInfo, IOptionPage optionPage);

        SMISubtestParameters BuildSMISubtestParameters(IOptionPage optionPage);
    }

    public class SMIModelBuilder : ISMIModelBuilder
    {
        private readonly IDbClient _dbClient;

        public SMIModelBuilder(IDbClient dbClient)
        {
            _dbClient = dbClient;
        }

        public SMIBaseParameters BuildSMIBaseParameters(CustomerInfo customerInfo, IOptionPage optionPage)
        {
            var language = 3;
            var excludeOUZ = bool.Parse(optionPage.GroupExists(XMLGroupType.ColumnZ) ? (optionPage.IncludeColumnZ == 1).ToString() : "false");
            var excludeSubtest = bool.Parse(optionPage.GroupExists(XMLGroupType.ColumnZ) ? (optionPage.IncludeColumnZ == 1).ToString() : "false");
            var retLanguage = optionPage.GetSelectedValuesStringOf(XMLGroupType.HomeReporting);
            if (optionPage.GroupExists(XMLGroupType.HomeReporting))
            {
                if (retLanguage == "English")
                {
                    language = 3;
                }
                if (retLanguage == "Spanish")
                {
                    language = 2;
                }
                if (retLanguage == "ascoded")
                {
                    language = 3;
                }
            }
            return new SMIBaseParameters
            {
                Accountability = optionPage.ScoringOptions.AccountabilityFlag.ToBoolean(),
                CustomerScoresetIDs = optionPage.ScoreSetId.ToString(),

                Grades = optionPage.GetSelectedValuesStringOf(XMLGroupType.GradePaper),
                ExcludeMathComputation = optionPage.ExcludeMathComputation == 1 ? ExcludeType.Exclude : ExcludeType.Include,
                ExcludeWordAnalysisListening = optionPage.ExcludeEla == 1 ? ExcludeType.Exclude : ExcludeType.Include,
                ExcludeOUZ = excludeOUZ,
                ExcludeOUZSubtest= excludeSubtest,
                Language = language,

                ReportPopulationNodeIDs = string.Join(",", optionPage.GetLastSelectedNodeIds(customerInfo)),
                ReportPopulationNodeType = optionPage.GetLastSelectedNodeType(customerInfo),

                TestPopulationNodeIDs = optionPage.ScoringOptions.LprNodeList,
                TestPopulationNodeType = optionPage.ScoringOptions.LprNodeLevel
            };
        }

        public SMIFilteringParameters BuildSMIFilteringParameters(IOptionPage optionPage)
        {
            var GenderListValue = "";
            var EthnicityListValue = "";
            var ProgramListValue = "";
            var AdminCodeListValue = "";
            var OfficeUseListValue = "";
            var OtherInfoListValue = "";
            var AdminValueListValue = "";
            if (optionPage.GroupExists(XMLGroupType.PopulationFilters))
            {
                var populationFiltersGroup = optionPage.GetGroupByType(XMLGroupType.PopulationFilters);
                if (populationFiltersGroup.Options.All(o => o is PiledOption))
                {
                    foreach (var pile in populationFiltersGroup.Options.Cast<PiledOption>().GroupBy(o => o.PileKey).ToList())
                    {
                        var selectedValues = pile.Where(o => o.IsSelected).Select(o => o.Value);
                        if (pile.Key == "GenderList")
                        {
                            GenderListValue = string.Join(",", selectedValues);
                        }
                        if (pile.Key == "EthnicityList")
                        {
                            EthnicityListValue = string.Join(",", selectedValues);
                        }
                        if (pile.Key == "ProgramList")
                        {
                            ProgramListValue = string.Join(",", selectedValues);
                        }
                        if (pile.Key == "AdminCodeList")
                        {
                            AdminCodeListValue = string.Join(",", selectedValues);
                        }
                        if (pile.Key == "OfficeUseList")
                        {
                            OfficeUseListValue = string.Join(",", selectedValues);
                        }
                        if (pile.Key == "OtherInfoList")
                        {
                            OtherInfoListValue = string.Join(",", selectedValues);
                        }
                        if (pile.Key == "AdminValueList")
                        {
                            AdminValueListValue = string.Join(",", selectedValues);
                        }

                    }
                }

            }
           
            return new SMIFilteringParameters
            {
                GroupSetCode = optionPage.ScoringOptions.GroupsetCode.ToUpper(),
                GenderList = GenderListValue,
                EthnicityList = EthnicityListValue,
                ProgramList = ProgramListValue,
                AdminCodeList = AdminCodeListValue,
                OfficeUseList = OfficeUseListValue,
                OtherInfoList = OtherInfoListValue,
                AdminValueList = AdminValueListValue
            };
        }

        public SMISkillParameters BuildSMISkillParameters(CustomerInfo customerInfo, IOptionPage optionPage)
        {
            var skilSetIds = string.Empty;


            if (optionPage.GroupExists(XMLGroupType.SkillDomainClassification))
            {
                if (optionPage.XmlDataFilteringOptions.Skillset.getValuesOnSubmit)
                {
                    var dbSkillSets = _dbClient.GetSkillSets(customerInfo.CustomerId.ToString(), optionPage.TestAdminValue, null, optionPage.XmlDisplayOption.reportCode);
                    skilSetIds = string.Join(",", dbSkillSets.Select(s => s.Id));
                }
                else if (optionPage.IsGroupHidden(XMLGroupType.SkillDomainClassification))
                {
                    skilSetIds = optionPage.ScoringOptions.SkillSetId.ToString();
                }
                else
                {
                    skilSetIds = optionPage.GetSelectedValuesStringOf(XMLGroupType.SkillDomainClassification);
                }
            }

            return new SMISkillParameters
            {
                SkillsetIDs = skilSetIds
            };
        }

        public SMISubtestParameters BuildSMISubtestParameters(IOptionPage optionPage)
        {
            const string RDGTOTL = "RDGTOTL";
            var result = new SMISubtestParameters
            {
                SubtestScoresList = GetSubtestScoresList(optionPage)
            };

            if (optionPage.GroupExists(XMLGroupType.ShowReadingTotal))
            {
                var value = optionPage.GetSelectedValuesStringOf(XMLGroupType.ShowReadingTotal);
                if (value.Contains(RDGTOTL))
                {
                    result.SubtestFamiliesToSuppress = $"'{RDGTOTL}'";
                }
            }

            return result;
        }

        private string GetSubtestScoresList(IOptionPage optionPage)
        {
            var selectedScoresIds = optionPage.GetSelectedValuesOf(XMLGroupType.Scores);
            if (selectedScoresIds == null)
            {
                return string.Empty;
            }

            selectedScoresIds.AddRange(optionPage.GetHiddenValuesOf(XMLGroupType.Scores));
            return optionPage.XmlDisplayType == XMLReportType.SP || optionPage.XmlDisplayType == XMLReportType.GP ?
                    string.Join("/", selectedScoresIds) :
                    string.Join(",", selectedScoresIds);
        }
    }
}
