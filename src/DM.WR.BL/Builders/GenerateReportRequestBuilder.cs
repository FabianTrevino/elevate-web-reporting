using DM.WR.Models.BackgroundReport;
using DM.WR.Models.BackgroundReport.Reports;
using DM.WR.Models.Config;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using Newtonsoft.Json;

namespace DM.WR.BL.Builders
{
    public interface IGenerateReportRequestBuilder
    {
        GenerateReportRequest GetReport(string reportTemplate, UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName, string rFormat);
    }

    public class GenerateReportRequestBuilder : IGenerateReportRequestBuilder
    {
        private readonly IOptionPageParser _optionPageParser;
        private readonly ISMIModelBuilder _smiModelBuilder;

        public GenerateReportRequestBuilder(IOptionPageParser optionPageParser, ISMIModelBuilder smiModelBuilder)
        {
            _optionPageParser = optionPageParser;
            _smiModelBuilder = smiModelBuilder;
        }

        public GenerateReportRequest GetReport(string reportTemplate, UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName, string rFormat)
        {
            GenerateReportRequest reportRequest = new GenerateReportRequest();
            switch (reportTemplate)
            {
                case "CatalogExporter":
                    {
                        reportRequest = GetCatalogExporter(userData, optionPage, reportName, loggingId, systemName);
                        break;
                    }
                case "CogAT7ProfileNarrative":
                    {
                        reportRequest = GetCogatProfileNarrativeGenerateReportRequest(userData, optionPage, reportName,loggingId,systemName,rFormat);
                        break;
                    }
                case "CogAT7ListStudentScores":
                    {
                        reportRequest = GetCogatListStudentScoresGenerateReportRequest(userData, optionPage, reportName, loggingId, systemName,rFormat);
                        break;
                    }
                case "CogAT7GroupSummary":
                    {
                        reportRequest = GetCogatGroupSummaryReportRequest(userData, optionPage, reportName, loggingId, systemName,rFormat);
                        break;
                    }
                case "IowaListOfStudentScores":
                    {
                        reportRequest = GetIowaListOfStudentScoresReportRequest(userData, optionPage, reportName, loggingId, systemName,rFormat);
                        break;
                    }
                case "IowaCogatProfileNarrative":
                    {
                        reportRequest = GetIowaProfileNarrativeGenerateReportRequest(userData, optionPage, reportName, loggingId, systemName, rFormat);
                        break;
                    }
                case "IowaGroupPerformanceProfile":
                    {
                        reportRequest = GetIowaGroupPerformanceProfileReportRequest(userData, optionPage, reportName, loggingId, systemName,rFormat);
                        break;
                    }
                case "IowaStudentPerformanceProfile":
                    {
                        reportRequest = GetIowaStudentPerformanceProfile(userData, optionPage, reportName, loggingId, systemName,rFormat);
                        break;
                    }
                case "IowaGroupListSummary":
                    {
                        reportRequest = GetIowaGroupListSummaryReport(userData, optionPage, reportName, loggingId, systemName, rFormat);
                        break;
                    }
                case "IowaGroupItemAnalysis":
                    {
                        reportRequest = GetIowaGroupItemAnalysisReport(userData, optionPage, reportName, loggingId, systemName, rFormat);
                        break;
                    }
                default:
                    {

                        break;
                    }

            }
            return reportRequest;
        }
        private GenerateReportRequest GetCatalogExporter(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName)
        {

            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new CatalogExporter
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                TestFamilyGroupCode = _optionPageParser.GetTestfamilyGroupCode(optionPage),
                ExportFormat = _optionPageParser.GetExportFormat(optionPage),
                ExportFieldFormat = _optionPageParser.GetExportFieldFormat(optionPage),
                ExportHeading = _optionPageParser.GetExportHeading(optionPage),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage),
                SMISkillParameters = _smiModelBuilder.BuildSMISkillParameters(userData.CurrentCustomerInfo, optionPage)
            };
            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment, 
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "CatalogExporter",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };

        }
        private GenerateReportRequest GetIowaGroupListSummaryReport(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName,string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new IowaGroupListSummaryReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                TestFamilyGroupCode = _optionPageParser.GetTestfamilyGroupCode(optionPage),
                TestSet = _optionPageParser.GetTestSet(optionPage),
                ReportGrouping = _optionPageParser.GetReportGrouping(optionPage, userData),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage),

            };
            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment, 
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "IowaGroupListSummary",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };

        }
        private GenerateReportRequest GetIowaGroupItemAnalysisReport(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName,string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new IowaGroupItemAnalysisReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                TestFamilyGroupCode = _optionPageParser.GetTestfamilyGroupCode(optionPage),
                ReportGrouping = _optionPageParser.GetReportGrouping(optionPage, userData),
                GroupPopulation = _optionPageParser.GetGroupPopulation(optionPage),
                SkillsetIDs = _smiModelBuilder.BuildSMISkillParameters(userData.CurrentCustomerInfo, optionPage).SkillsetIDs,
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage),

            };
            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment,
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "IowaGroupItemAnalysis",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };

        }
        private GenerateReportRequest GetIowaListOfStudentScoresReportRequest(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName,string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new IowaListOfStudentScoresReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                TestFamilyGroupCode = _optionPageParser.GetTestfamilyGroupCode(optionPage),
                SuppressProgramLabel = _optionPageParser.GetSuppressProgramLabel(optionPage, userData),
                CogatComposite = _optionPageParser.GetCogatComposite(optionPage),
                CogatDifferences = _optionPageParser.GetCogatDifferences(optionPage),
                ACTScoreGrade = _optionPageParser.GetACTScoreGrade(optionPage),
                SuppressSubtests = _optionPageParser.SuppressSubtests(optionPage),
                RankingDirection = _optionPageParser.GetRankingDirection(optionPage),
                RankingSubtest = _optionPageParser.GetRankingSubtest(optionPage),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage),
            };
            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment, 
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "IowaListOfStudentScores",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };

        }
        private GenerateReportRequest GetIowaStudentPerformanceProfile(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName,string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new IowaStudentPerformanceProfileReportRequest
            {
                LoggingRequestId= loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                TestfamilyGroupCode = _optionPageParser.GetTestfamilyGroupCode(optionPage),
                RFormat = rFormat,
                ReportGrouping = _optionPageParser.GetReportGrouping(optionPage, userData),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                ACTScoreGrade = _optionPageParser.GetACTScoreGrade(optionPage),
                CollegeReadyScoreGrade = _optionPageParser.GetCollegeReadyScoreGrade(optionPage),
                GraphScores = _optionPageParser.GetGraphScores(optionPage),
                UpperGraphType = _optionPageParser.GetUpperGraphType(optionPage),
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISkillParameters = _smiModelBuilder.BuildSMISkillParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
            };
            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment,
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "IowaStudentPerformanceProfile",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };
        }
        private GenerateReportRequest GetIowaGroupPerformanceProfileReportRequest(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName,string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new IowaGroupPerformanceProfileReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName =  systemName,
                CollegeReadyScoreGrade = _optionPageParser.GetCollegeReadyScoreGrade(optionPage),
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage),
                SMISkillParameters = _smiModelBuilder.BuildSMISkillParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                
            };

            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment, 
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "IowaGroupPerformanceProfile",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };
        }
        private GenerateReportRequest GetIowaProfileNarrativeGenerateReportRequest(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName,string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new ProfileNarrativeReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                GraphType = _optionPageParser.GetGraphType(optionPage),
                HomeReporting = _optionPageParser.GetHomeReporting(optionPage),
                TestFamilyGroupCode = _optionPageParser.GetTestfamilyGroupCode(optionPage),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SortbyNodeLevel = _optionPageParser.GetSortByNodelevel(optionPage),
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage)
            };

            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment, 
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "IowaCogatProfileNarrative",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };
        }
        private GenerateReportRequest GetCogatProfileNarrativeGenerateReportRequest(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName, string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new CogatProfileNarrativeReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                ReportGrouping = _optionPageParser.GetReportGrouping(optionPage, userData),
                CogatComposite = _optionPageParser.GetCogatComposite(optionPage),
                SuppressProfile = _optionPageParser.GetSuppressProfile(optionPage),
                GraphType = _optionPageParser.GetGraphType(optionPage),
                HomeReporting = _optionPageParser.GetHomeReporting(optionPage),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage)
            };

            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment,
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "CogAT7ProfileNarrative",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };
        }
        private GenerateReportRequest GetCogatListStudentScoresGenerateReportRequest(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName,string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new CogatListStudentScoresReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                SuppressProgramLabel = _optionPageParser.GetSuppressProgramLabel(optionPage, userData),
                CogatComposite = _optionPageParser.GetCogatComposite(optionPage),
                GraphType = _optionPageParser.GetGraphType(optionPage),
                RankingDirection = _optionPageParser.GetRankingDirection(optionPage),
                RankingSubtest = _optionPageParser.GetRankingSubtest(optionPage),
                RankingScore = _optionPageParser.GetRankingScore(optionPage),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage)

            };
            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment,
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "CogAT7ListStudentScores",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };
        }
        private GenerateReportRequest GetCogatGroupSummaryReportRequest(UserData userData, OptionPage optionPage, string reportName, string loggingId, string systemName, string rFormat)
        {
            var customerScoreOptions = _optionPageParser.GetcustomerDisplayOptions(optionPage);
            var dataRequestObject = new CogatGroupListSummaryReportRequest
            {
                LoggingRequestId = loggingId,
                OutputSystemName = systemName,
                ReportFormat = _optionPageParser.GetReportFormat(optionPage),
                RFormat = rFormat,
                ReportGrouping = _optionPageParser.GetReportGrouping(optionPage, userData),
                CogatComposite = _optionPageParser.GetCogatComposite(optionPage),
                GraphType = _optionPageParser.GetGraphType(optionPage),
                DisaggLabel = _optionPageParser.GetDisAggLabel(optionPage),
                BuildingLabel = customerScoreOptions["BuildingLabel"],
                ClassLabel = customerScoreOptions["ClassLabel"],
                DistrictLabel = customerScoreOptions["DistrictLabel"],
                RegionLabel = customerScoreOptions["RegionLabel"],
                StateLabel = customerScoreOptions["StateLabel"],
                SystemLabel = customerScoreOptions["SystemLabel"],
                SMIBaseParameters = _smiModelBuilder.BuildSMIBaseParameters(userData.CurrentCustomerInfo, optionPage),
                SMISubtestParameters = _smiModelBuilder.BuildSMISubtestParameters(optionPage),
                SMIFilteringParameters = _smiModelBuilder.BuildSMIFilteringParameters(optionPage)


            };
            return new GenerateReportRequest
            {
                Environment = ConfigSettings.Environment,
                Parameters = JsonConvert.SerializeObject(dataRequestObject),
                filename = reportName,
                ReportType = "CogAT7GroupSummary",
                UserID = userData.UserId + "_" + userData.CurrentGuid
            };
        }
    }
}
