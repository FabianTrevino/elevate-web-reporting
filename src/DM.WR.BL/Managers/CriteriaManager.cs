using DM.WR.BL.Builders;
using DM.WR.Models.Config;
using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using HandyStuff;

namespace DM.WR.BL.Managers
{
    public class CriteriaManager : ICriteriaManager
    {
        private static readonly Type[] ExtraSerializationTypes = {
            typeof(CriteriaContainer),
            typeof(Option),
            typeof(CheckboxOption),
            typeof(PiledOption),
            typeof(LongitudinalTestAdminOption ),
            typeof(LocationGroup),
            typeof(PerformanceBandGroup),
            typeof(PerformanceBandOption),
            typeof(XMLBandColorEnum),
            typeof(ScoreFiltersGroup),
            typeof(ScoreWarningsGroup),
            typeof(CustomFieldOption),
            typeof(CustomFieldGroup),
            typeof(XMLGroupType),
        };

        private readonly XmlSerializer _serializer;

        private readonly IReportCriteriaClient _criteriaClient;
        private readonly IOptionsManager _optionsManager;
        private readonly IOptionsBuilder _optionsBuilder;

        private readonly UserData _userData;

        public CriteriaManager(IReportCriteriaClient criteriaClient, IOptionsManager optionsManager, IOptionsBuilder optionsBuilder, IUserDataManager userDataManager)
        {
            _serializer = new XmlSerializer(typeof(CriteriaContainer), ExtraSerializationTypes);

            _criteriaClient = criteriaClient;
            _optionsManager = optionsManager;
            _optionsBuilder = optionsBuilder;

            _userData = userDataManager.GetUserData();
        }

        public string LoadCriteria(int criteriaId, bool enableEditMode, string criteriaName = null, string criteriaDescription = null, string criteriaDate = null)
        {
            var xmlString = _criteriaClient.ReportCriteria_LoadOptions(criteriaId);

            string[] separators = { "<BuildNumber>", "</BuildNumber>" };
            var buildNumber = Convert.ToInt32(xmlString.Split(separators, StringSplitOptions.RemoveEmptyEntries)[1]);
            var isIrm40Xml = buildNumber < 126;
            var container = isIrm40Xml ?
                        new LegacyAdapter().CriteriaXmlToContainerObject(xmlString, buildNumber) :
                        XmlStringToOptions(xmlString);

            var book = _optionsManager.GetOptionBook();
            book.RemoveAllPages();

            OptionGroup invalidGroup = null;

            for (int pageIndex = 0; pageIndex < container.Options.Count; ++pageIndex)
            {
                var optionsList = container.Options[pageIndex];
                optionsList.ForEach(g =>
                {
                    g.IsFromSavedCriteria = true;
                    g.IsFromIrm40Xml = isIrm40Xml;
                    g.Category = g is LocationGroup ? OptionsCategory.Locations : OptionsCategory.Primary;
                });
                var newCurrentPage = new OptionPage(ConfigSettings.XmlAbsolutePath, optionsList);
                var recreatedPage = _optionsBuilder.BuildOptions(newCurrentPage, XMLGroupType._INTERNAL_FIRST_, _userData, pageIndex);
                book.AddPage(recreatedPage);

                if (invalidGroup == null)
                    invalidGroup = _optionsBuilder.InvalidGroup;
            }
            book.CurrentPageIndex = 0;

            if (enableEditMode)
                _optionsManager.EnableCriteriaEditMode(new CriteriaInfo { Id = criteriaId, Name = criteriaName, Summary = criteriaDescription, LastUpdated = criteriaDate });

            _optionsManager.UpdateOptionBook(book);

            return invalidGroup?.DisplayName;
        }

        public CriteriaInfo SaveNewCriteria(string criteriaName, string criteriaDescription, bool runInBackground)
        {
            var book = _optionsManager.GetOptionBook();
            var firstPage = book.GetPage(0);

            var container = new CriteriaContainer
            {
                BuildNumber = ConfigSettings.OptionsCriteriaVersion,
                Options = book.Pages.Select(p => SelectedOptionsOnly(p.GetAllGroups())).ToList()
            };
            string optionsXml = OptionsToXmlString(container);

            ReportCriteria criteria = new ReportCriteria
            {
                DmUserId = _userData.UserId,
                LocationGuid = _userData.CurrentGuid,
                AssessmentId = firstPage.AssessmentValue,
                AssessmentGroupCode = firstPage.AssessmentCode.ToString(),
                DisplayType = runInBackground ? "SR" : firstPage.XmlDisplayType.ToString(),
                CriteriaName = runInBackground ? $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss_ffff}" : criteriaName,
                CriteriaDescription = criteriaDescription,
                OptionsXml = optionsXml,
                SubmitToReportCenter = runInBackground
            };

            ReportCriteria newCriteria = _criteriaClient.ReportCriteria_Insert(criteria);
            if (newCriteria == null)
                return null;

            var criteriaInfo = new CriteriaInfo
            {
                Id = newCriteria.CriteriaId,
                Name = newCriteria.CriteriaName
            };

            _optionsManager.DisableCriteriaEditMode();

            return criteriaInfo;
        }

        public string UpdateExistingCriteria(int criteriaId, string name, string summary)
        {
            var book = _optionsManager.GetOptionBook();
            var container = new CriteriaContainer
            {
                BuildNumber = ConfigSettings.OptionsCriteriaVersion,
                Options = book.Pages.Select(p => SelectedOptionsOnly(p.GetAllGroups())).ToList()
            };
            string optionsXml = OptionsToXmlString(container);

            var reportCriteria = _criteriaClient.ReportCriteria_Update(criteriaId, name, summary, optionsXml);
            if (reportCriteria == null)
                return $"There has been a problem updating criteria '{name}'.";

            _optionsManager.DisableCriteriaEditMode();

            return string.Empty;
        }

        public bool DeleteCriteria(int criteriaId)
        {
            _criteriaClient.ReportCriteria_Delete(new List<int> { criteriaId });
            return true;
        }


        private string OptionsToXmlString(CriteriaContainer container)
        {
            using (var stringWriter = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(stringWriter))
            {
                _serializer.Serialize(writer, container);
                return stringWriter.ToString();
            }
        }

        private CriteriaContainer XmlStringToOptions(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CriteriaContainer), ExtraSerializationTypes);
            using (TextReader reader = new StringReader(xmlString))
            {
                return (CriteriaContainer)serializer.Deserialize(reader);
            }
        }

        private List<OptionGroup> SelectedOptionsOnly(List<OptionGroup> groups)
        {
            var result = new List<OptionGroup>();

            foreach (var group in groups)
            {
                var clonedGroup = group.Copy();

                if (clonedGroup.Type == XMLGroupType.LongitudinalTestAdministrations && clonedGroup.Options.All(o => o is LongitudinalTestAdminOption))
                {
                    var options = clonedGroup.Options.Cast<LongitudinalTestAdminOption>().Where(o => o.IsSelected).ToList();
                    foreach (var testAdmin in options)
                    {
                        testAdmin.GradeLevels = testAdmin.GradeLevels.Where(gl => gl.IsSelected).ToList();
                    }
                    clonedGroup.Options = options.Cast<Option>().ToList();
                }
                else if (clonedGroup.Type != XMLGroupType.ScoreFilters && clonedGroup.Type != XMLGroupType.ScoreWarnings)
                    clonedGroup.Options = clonedGroup.Options.Where(o => o.IsSelected || o is CheckboxOption).ToList();

                result.Add(clonedGroup);
            }

            return result;
        }
    }
}