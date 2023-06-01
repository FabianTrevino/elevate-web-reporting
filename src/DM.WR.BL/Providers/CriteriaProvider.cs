using System.Collections.Generic;
using System.Linq;
using DM.UI.Library.Models;
using DM.WR.Models.Config;
using DM.WR.BL.Managers;
using DM.WR.Data.Repository;
using DM.WR.Models.Types;
using DM.WR.Models.ViewModels;
using DM.WR.Models.Xml;

namespace DM.WR.BL.Providers
{
    public class CriteriaProvider : ICriteriaProvider
    {
        private readonly IReportCriteriaClient _criteriaClient;
        private readonly IOptionsManager _optionsManager;
        private readonly ICriteriaManager _criteriaManager;

        private readonly UserData _userData;
        private readonly XmlLoader _xmlLoader;

        private readonly CommonProviderFunctions _apiCommon;

        public CriteriaProvider(IReportCriteriaClient criteriaClient, IOptionsManager optionsManager, ICriteriaManager criteriaManager, IUserDataManager userDataManager)
        {
            _criteriaClient = criteriaClient;
            _optionsManager = optionsManager;
            _criteriaManager = criteriaManager;

            _userData = userDataManager.GetUserData();
            _xmlLoader = XmlLoader.GetInstance(ConfigSettings.XmlAbsolutePath);

            _apiCommon = new CommonProviderFunctions();
        }

        public CriteriaPageViewModel BuildViewModel()
        {
            return new CriteriaPageViewModel { IsGuidUser = _apiCommon.IsGuidUser(_userData) };
        }

        public CriteriaTableViewModel BuildTableViewModel(string selectedAssessmentGroupCode, string selectedDisplayType)
        {
            var viewModel = new CriteriaTableViewModel();
            var criteria = _criteriaClient.ReportCriteria_Select(_userData.UserId, _userData.CurrentGuid);

            if (criteria == null || !criteria.Any())
                return viewModel;

            //build Assessments dropdown
            foreach (var assessment in _userData.Assessments)
            {
                if (//5.0 criteria check
                    criteria.All(c => c.AssessmentGroupCode != assessment.TestFamilyGroupCode) &&
                    //4.0 criteria check
                    !criteria.Any(c => c.AssessmentId == assessment.TestFamilyGroupId && c.AssessmentGroupCode == ""))
                    continue;

                bool isSelected = false;

                if (string.IsNullOrEmpty(selectedAssessmentGroupCode)) //very fist time - select the first item
                    isSelected = viewModel.AssessmentsDropdownItems.Count == 0;
                else if (selectedAssessmentGroupCode == assessment.TestFamilyGroupCode) //user selection exists - match to assessment by GroupCode
                    isSelected = true;

                viewModel.AssessmentsDropdownItems.Add(new DropdownItem
                {
                    Text = assessment.TestFamilyDesc,
                    Value = assessment.TestFamilyGroupCode,
                    AltValue = assessment.TestFamilyGroupId.ToString(),
                    Selected = isSelected
                });
            }
            var selectedAssessmentItem = viewModel.AssessmentsDropdownItems.First(i => i.Selected);

            //build Display Types dropdown based on selected Assessment
            var displayTypes = criteria
                .Where(c => c.AssessmentGroupCode == selectedAssessmentItem.Value || c.AssessmentGroupCode == "" && c.AssessmentId.ToString() == selectedAssessmentItem.AltValue)
                .Select(c => c.DisplayType).Distinct().ToList();
            foreach (var displayType in displayTypes)
            {
                var displayTypeXml = _xmlLoader.GetReport(selectedAssessmentItem.Value, displayType);

                var isSelected = string.IsNullOrEmpty(selectedDisplayType) || !displayTypes.Contains(selectedDisplayType) ?
                    viewModel.ReportTypeDropdownItems.Count == 0 :
                    selectedDisplayType == displayType;

                viewModel.ReportTypeDropdownItems.Add(new DropdownItem
                {
                    Text = displayTypeXml.reportName,
                    Value = displayType,
                    Selected = isSelected
                });
            }
            var selecteDisplayTypeItem = viewModel.ReportTypeDropdownItems.First(i => i.Selected);

            //build the list of criteria based on selected Assessment & selected Display Type
            viewModel.CriteriaList = criteria
                .Where(c => (c.AssessmentGroupCode == selectedAssessmentItem.Value || c.AssessmentId.ToString() == selectedAssessmentItem.AltValue && c.AssessmentGroupCode == "") &&
                             c.DisplayType == selecteDisplayTypeItem.Value)
                .Select(c => new CriteriaInfo
                {
                    Id = c.CriteriaId,
                    Name = c.CriteriaName,
                    Summary = c.CriteriaDescription,
                    LastUpdated = c.CreatedDate.ToShortDateString()
                }).ToList();

            return viewModel;
        }

        public string SaveNewCriteria(string criteriaName, string criteriaDescription)
        {
            var criteriaInfo = _criteriaManager.SaveNewCriteria(criteriaName, criteriaDescription, false);

            if (criteriaInfo == null)
                return "A criteria set with the same name already exists.  Please rename this criteria.";

            return string.Empty;
        }

        public string UpdateExistingCriteria(int criteriaId, string name, string summary)
        {
            return _criteriaManager.UpdateExistingCriteria(criteriaId, name, summary);
        }

        public string LoadCriteria(int criteriaId, bool enableEditMode, string criteriaName = null, string criteriaDescription = null, string criteriaDate = null)
        {
            return _criteriaManager.LoadCriteria(criteriaId, enableEditMode, criteriaName, criteriaDescription, criteriaDate);
        }

        public bool DeleteCriteria(int criteriaId)
        {
            return _criteriaManager.DeleteCriteria(criteriaId);
        }

        public ModalModel BuildDeleteCriteriaModal()
        {
            return new ModalModel(SectionColor.Green)
            {
                HeaderText = "Delete Criteria",
                Buttons = new List<Button>
                {
                    new Button(ButtonType.Secondary, ButtonSize.Medium)
                    {
                        Id = "cancel-delete-criteria-button", Text = "Cancel"
                    },
                    new Button(ButtonType.Primary, ButtonSize.Medium)
                    {
                        Id = "yes-delete-criteria-button", Text = "Yes"
                    }
                }
            };
        }

        public ModalModel BuildUnsavedChangesModal()
        {
            return new ModalModel(SectionColor.Green)
            {
                HeaderText = "Unsaved Changes",
                Buttons = new List<Button>
                {
                    new Button(ButtonType.Secondary, ButtonSize.Medium)
                    {
                        Id = "stay-edit-criteria-button", Text = "Stay"
                    },
                    new Button(ButtonType.Primary, ButtonSize.Medium)
                    {
                        Id = "leave-edit-criteria-button", Text = "Leave"
                    }
                }
            };
        }

        public SaveCriteriaModalModel BuildSaveCriteriaModal()
        {
            var page = _optionsManager.GetOptionBook().GetCurrentPage();

            return new SaveCriteriaModalModel(SectionColor.Green)
            {
                HeaderText = "Save Criteria",
                ReportTypeName = page.GetFirstSelectionTextOf(XMLGroupType.DisplayType).Replace("<b>NEW</b> ", ""),
                Buttons = new List<Button>
                {
                    new Button(ButtonType.Secondary, ButtonSize.Medium)
                    {
                        Id = "cancel-save-criteria-button", Text = "Cancel"
                    },
                    new Button(ButtonType.Primary, ButtonSize.Medium)
                    {
                        Id = "save-save-criteria-button", Text = "Save"
                    }
                }
            };
        }

        public SaveCriteriaModalModel BuildRunInBackgroundModal()
        {
            var page = _optionsManager.GetOptionBook().GetCurrentPage();

            return new SaveCriteriaModalModel(SectionColor.Green)
            {
                HeaderText = "Specify a Report Name",
                ReportTypeName = page.GetFirstSelectionTextOf(XMLGroupType.DisplayType).Replace("<b>NEW</b> ", ""),
                Buttons = new List<Button>
                {
                    new Button(ButtonType.Secondary, ButtonSize.Medium)
                    {
                        Id = "cancel-run-in-background-button", Text = "Cancel"
                    },
                    new Button(ButtonType.Primary, ButtonSize.Medium)
                    {
                        Id = "ok-run-in-background-button", Text = "OK"
                    }
                }
            };
        }
    }
}