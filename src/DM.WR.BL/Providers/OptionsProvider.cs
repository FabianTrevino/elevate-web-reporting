using System;
using System.Collections.Generic;
using System.Linq;
using DM.UI.Library.Models;
using DM.WR.BL.Builders;
using DM.WR.BL.Managers;
using DM.WR.Models.Config;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.ViewModels;
using DM.WR.Models.Xml;
using DM.WR.ServiceClient;

namespace DM.WR.BL.Providers
{
    public class OptionsProvider : IOptionsProvider
    {
        private readonly IUserDataManager _userDataManager;
        private readonly IOptionsManager _optionsManager;
        private readonly IOptionsBuilder _optionsBuilder;
        private readonly IActuateServiceClient _actuateServiceClient;
        private readonly UserData _userData;

        private readonly CommonProviderFunctions _commonFunctions;

        public OptionsProvider(IUserDataManager userDataManager, IOptionsManager optionsManager, IOptionsBuilder optionsBuilder, IActuateServiceClient actuateServiceClient)
        {
            _userDataManager = userDataManager;
            _optionsManager = optionsManager;
            _optionsBuilder = optionsBuilder;
            _actuateServiceClient = actuateServiceClient;
            _userData = userDataManager.GetUserData();

            _commonFunctions = new CommonProviderFunctions();
        }

        public OptionsPageViewModel BuildDefaultModel()
        {
            var book = _optionsManager.GetOptionBook();
            _actuateServiceClient.CreateUser(_userData.UserId, _userData.ActuateUserId, ConfigSettings.AcBpVolume, ConfigSettings.AcBpUserName, ConfigSettings.AcBpPassword, out string error);
            return new OptionsPageViewModel
            {
                CurrentLocationName = _userData.CurrentCustomerInfo.NodeName,
                IsMultiLocation = _userData.CustomerInfoList.Count > 1,
                IsGuidUser = _commonFunctions.IsGuidUser(_userData),

                IsInEditCriteriaMode = book.Criteria != null,
                CriteriaId = book.Criteria?.Id ?? -1,
                CriteriaName = book.Criteria?.Name,
                CriteriaDescription = book.Criteria?.Summary,
                CriteriaDate = book.Criteria?.LastUpdated
            };
        }

        public LocationChangeModalModel BuildLocationChangeModalModel()
        {
            var locationsDropdown = new List<DropdownItem>();
            foreach (var customerInfo in _userData.CustomerInfoList)
            {
                locationsDropdown.Add(new DropdownItem
                {
                    Text = customerInfo.NodeName,
                    Value = customerInfo.NodeId.ToString(),
                    Selected = customerInfo.Guid == _userData.CurrentGuid
                });
            }

            return new LocationChangeModalModel
            {
                HeaderText = "Report Criteria",
                Label = "REPORT CRITERIA SET",
                Locations = locationsDropdown,
                Buttons = new List<Button>
                 {
                     new Button(ButtonType.Secondary, ButtonSize.Medium)
                     {
                        Text = "Cancel",
                        Id = "location-change-cancel-button"
                     },
                     new Button(ButtonType.Primary, ButtonSize.Medium)
                     {
                        Text = "Apply",
                        Id = "location-change-apply-button"
                     }
                 }
            };
        }

        public void SwitchLocation(int nodeId)
        {
            _userDataManager.ChangeLocation(nodeId);
            _actuateServiceClient.CreateUser(_userData.UserId, _userData.ActuateUserId, ConfigSettings.AcBpVolume, ConfigSettings.AcBpUserName, ConfigSettings.AcBpPassword, out string error);
            _optionsManager.DeleteOptionBook();
        }

        public OptionsViewModel GetOptions()
        {
            var book = _optionsManager.GetOptionBook();
            var page = book.GetCurrentPage();

            return new OptionsViewModel
            {
                Groups = page.GetAllGroups(),
                LocationName = _userData.CurrentCustomerInfo.NodeName,
                RunInForeground = page.RunInForeground,
                IsMultimeasure = page.IsMultimeasure,
                CurrentPage = book.CurrentPageIndex + 1,
                TotalPages = book.PagesCount,
                InvalidGroup = page.InvalidGroup
            };
        }

        public OptionGroup GetGroup(string groupTypeNumber)
        {
            var book = _optionsManager.GetOptionBook();
            var currentPage = book.GetCurrentPage();
            Enum.TryParse(groupTypeNumber, out XMLGroupType groupType);

            return currentPage.GetGroupByType(groupType);
        }

        public void UpdateOptions(string groupTypeNumber, List<string> values)
        {
            var book = _optionsManager.GetOptionBook();
            var currentPage = book.GetCurrentPage();
            Enum.TryParse(groupTypeNumber, out XMLGroupType groupType);
            var currentGroup = currentPage.GetGroupByType(groupType);

            if (currentGroup.InputControl == OptionsInputControl.Checkbox)
            {
                if (currentGroup.Options.Count != values.Count)
                    throw new Exception("Options Manager :: Number of options does not match.");

                for (int c = 0; c < currentGroup.Options.Count; ++c)
                    currentGroup.Options[c].IsSelected = bool.Parse(values[c]);
            }
            else if (currentGroup.InputControl == OptionsInputControl.MultimeasurePerformanceBands)
            {
                foreach (var bandString in values)
                {
                    //bandString format:   [color:bandName,lowValue,highValue]
                    var splittedBandString = bandString.Split(':');
                    var splittedValues = splittedBandString[1].Split(',');

                    var option = currentGroup.Options.Cast<PerformanceBandOption>().First(o => o.BandColor.ToString() == splittedBandString[0]);
                    option.Text = splittedValues[0];
                    option.LowValue = splittedValues[1];
                    option.HighValue = splittedValues[2];
                }
            }
            else if (currentGroup.InputControl == OptionsInputControl.DataExportCustomDataFields)
            {
                var group = (CustomFieldGroup)currentGroup;
                group.SelectedValuesOrder = new List<string>();

                var options = group.Options.Cast<CustomFieldOption>().ToList();
                foreach (var option in options)
                {
                    option.IsSelected = false;
                    option.UserText = null;
                    option.UserWidth = null;
                }

                if (values != null)
                    foreach (var customItem in values)
                    {
                        var splitted = customItem.Split('~');

                        group.SelectedValuesOrder.Add(splitted[0]);

                        var option = options.First(o => o.Value == splitted[0]);
                        option.IsSelected = true;
                        option.UserText = option.Text == splitted[1] ? null : splitted[1];
                        option.UserWidth = option.Width == Convert.ToInt32(splitted[2]) ? (int?) null : Convert.ToInt32(splitted[2]);
                    }
            }
            else if (currentGroup.InputControl == OptionsInputControl.LongitudinalTestAdministrations)
            {
                var newSelectionDictionary = values.ToDictionary(v => v.Split(':')[0], v => v.Split(':')[1]);

                var options = currentGroup.Options.Cast<LongitudinalTestAdminOption>().ToList();
                options.ForEach(o =>
                {
                    var gradeLevelValue = newSelectionDictionary.ContainsKey(o.Value) ? newSelectionDictionary[o.Value] : null;

                    o.IsSelected = gradeLevelValue != null;
                    o.GradeLevels.ForEach(gl => gl.IsSelected = gl.Value == gradeLevelValue);
                });
            }
            else if (currentGroup.InputControl == OptionsInputControl.ScoreFilters)
            {
                var filtersGroup = (ScoreFiltersGroup)currentGroup;
                filtersGroup.Rows.Clear();

                foreach (var row in values)
                {
                    filtersGroup.Rows.Add(new ScoreFilterRow(row));
                }
            }
            else if (currentGroup.InputControl == OptionsInputControl.ScoreWarnings)
            {
                var warningsGroup = (ScoreWarningsGroup)currentGroup;
                warningsGroup.Rows.Clear();

                foreach (var selections in values)
                {
                    warningsGroup.Rows.Add(new ScoreWarningRow(selections));
                }
            }
            else
            {
                currentGroup.Options.ForEach(o => o.IsSelected = false);

                if (values != null)
                    currentGroup.Options.Where(o => values.Contains(o.Value)).ToList().ForEach(o => o.IsSelected = true);
            }

            var newOptionsPage = _optionsBuilder.BuildOptions(currentPage, groupType, _userData, book.CurrentPageIndex);
            book.UpdateCurrentPage(newOptionsPage);

            if (newOptionsPage.IsMultimeasure)
                book = _optionsBuilder.SyncUpPagesForMultimeasure(book);

            _optionsManager.UpdateOptionBook(book);
        }

        public void ResetOptions()
        {
            _optionsManager.DeleteOptionBook();
        }

        public void AddMultimeasureColumn()
        {
            _optionsManager.AddOptionPage();
        }

        public void GoToMultimeasureColumn(int pageNumber)
        {
            _optionsManager.FlipPage(pageNumber);
        }

        public void RemoveCurrentMultimeasureColumn()
        {
            _optionsManager.RemoveOptionPage();
        }

        public void ClearCriteriaEditMode()
        {
            _optionsManager.DisableCriteriaEditMode();
        }
    }
}