using DM.UI.Library.Models;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.Config;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DM.WR.BL.Builders
{
    public class OptionsMapper
    {
        private readonly XmlLoader _xmlLoader;
        private readonly BuildersHelper _helper;

        private DateTime _covidChopOffDate = DateTime.Parse("12/31/2020");

        public OptionsMapper()
        {
            _xmlLoader = XmlLoader.GetInstance(ConfigSettings.XmlAbsolutePath);
            _helper = new BuildersHelper();
        }

        public OptionGroup MapAssessments(List<Assessment> assessments, OptionGroup currentGroup, out Assessment selectedAssessment, Action<OptionGroup> setInvalidGroup)
        {
            selectedAssessment = null;
            var applyDefaultSelection = currentGroup == null;

            var options = new List<Option>();
            for (int c = 0; c < assessments.Count; ++c)
            {
                var assessment = assessments[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentGroup.IsFromIrm40Xml ? currentGroup.IsValueSelected(assessment.TestFamilyGroupId) || currentGroup.IsValueSelected($"{assessment.TestFamilyGroupId}{assessment.TestFamilyGroupCode}") : currentGroup.IsValueSelected(assessment.TestFamilyGroupCode);
                var reportOption = new Option
                {
                    Value = assessment.TestFamilyGroupCode,
                    Text = assessment.TestFamilyDesc,
                    IsSelected = applySelection
                };

                if (applySelection)
                    selectedAssessment = assessment;

                options.Add(reportOption);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;
                selectedAssessment = assessments.FirstOrDefault();

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.Assessment)
            {
                Category = OptionsCategory.Primary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.Assessment),
                Options = options
            };
        }

        public OptionGroup MapTestAdmins(List<TestAdmin> testAdmins, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            for (int c = 0; c < testAdmins.Count; ++c)
            {
                var testAdmin = testAdmins[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentGroup.IsValueSelected(testAdmin.Id);
                var option = new Option
                {
                    Value = testAdmin.Id.ToString(),
                    Text = $"{Convert.ToDateTime(testAdmin.Date):MM/dd/yyyy} - {testAdmin.Name}",
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.TestAdministrationDate)
            {
                Category = OptionsCategory.Primary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.TestAdministrationDate),
                Options = options
            };
        }

        public OptionGroup MapDisplayTypes(XMLReport[] displayTypes, OptionGroup currentGroup, ScoringOptions scoringOptions, TestAdmin selectedTestAdmin, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            var isWebReportingLite = !string.IsNullOrEmpty(scoringOptions.PrintOnlyReports);
            var availableLiteReports = isWebReportingLite ? scoringOptions.PrintOnlyReports.Split(',').ToList() : new List<string>();

            foreach (var displayType in displayTypes)
            {
                if (displayType.showIfItemsAvailable && !scoringOptions.HasItemsFlag)
                    continue;

                if (displayType.showIfSubtestGroupType.Length > 0 && !displayType.showIfSubtestGroupType.Contains(scoringOptions.PredSubtestGroupType))
                    continue;

                var testEventDate = DateTime.Parse(selectedTestAdmin.Date);
                if (displayType.reportType == XMLReportType.EGSR && (!selectedTestAdmin.AllowCovidReportFlag || testEventDate.Date > _covidChopOffDate.Date))
                    continue;

                if (ConfigSettings.IsWebReportingLiteFeatureEnabled)  //TODO: Remove this line when this feature goes live.
                    if (isWebReportingLite && !availableLiteReports.Contains(displayType.reportType.ToString())) 
                    { 
                        continue; 
                    }
                        

                var applySelection = applyDefaultSelection ? displayType.isDefault : currentGroup.IsValueSelected(displayType.reportType);
                var option = new Option
                {
                    Value = displayType.reportType.ToString(),
                    Text = WebUtility.HtmlDecode(displayType.reportName),
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.DisplayType)
            {
                Category = OptionsCategory.Primary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.DisplayType),
                Options = options
            };
        }

        public OptionGroup MapGradeLevels(List<GradeLevel> gradeLevels, OptionGroup currentGroup, out GradeLevel selectedGradeLevel, Action<OptionGroup> setInvalidGroup)
        {
            selectedGradeLevel = null;
            var options = new List<Option>();
            bool applyDefaultSelection = currentGroup == null;

            for (int c = 0; c < gradeLevels.Count; ++c)
            {
                var gradeLevel = gradeLevels[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentGroup.IsValueSelected(gradeLevel.Level);
                var option = new Option
                {
                    Value = gradeLevel.Level.ToString(),
                    AltValue = gradeLevel.Grade,
                    Text = gradeLevel.GradeText,
                    IsSelected = applySelection
                };
                options.Add(option);

                if (applySelection)
                    selectedGradeLevel = gradeLevel;
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;
                selectedGradeLevel = gradeLevels.First();

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.GradeLevel)
            {
                Category = OptionsCategory.Primary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.GradeLevel),
                Options = options
            };
        }

        public OptionGroup MapGradePaper(List<Grade> grades, OptionGroup currentGroup, out List<Grade> selectedGrades, Action<OptionGroup> setInvalidGroup)
        {
            selectedGrades = new List<Grade>();
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            for (int c = 0; c < grades.Count; ++c)
            {
                var grade = grades[c];
                var applySelection = applyDefaultSelection ? c == 0 : currentGroup.IsValueSelected(grade.GradeId);
                var option = new Option
                {
                    Value = grade.GradeId,
                    Text = grade.GradeText,
                    IsSelected = applySelection
                };

                if (applySelection)
                    selectedGrades.Add(grade);

                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;
                selectedGrades.Add(grades.First());

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.GradePaper)
            {
                Category = OptionsCategory.Primary,
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.GradePaper),
                Options = options,
                MinToSelect = 1,//TODO:  Should this come from XML???
                MaxToSelect = grades.Count,
                HasSelectAll = true,
                HasSelectNone = true
            };
        }

        public OptionGroup MapLevelOfAnalysis(XMLLevelOfAnalysis levelOfAnalysis, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var analysisType in levelOfAnalysis.AnalysisType)
            {
                var applySelection = applyDefaultSelection ? analysisType.isDefault : currentGroup.IsValueSelected(analysisType.code);
                var option = new Option
                {
                    Value = analysisType.code.ToString(),
                    Text = analysisType.text,
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.LevelofAnalysis)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LevelofAnalysis),
                Options = options
            };
        }

        public OptionGroup MapDisplayOptions(XMLDisplayOptions displayOptions, OptionGroup currentGroup, List<string> selectedGradeValues, TestAdmin selectedTestAdmin, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var displayOption in displayOptions.DisplayOption)
            {
                if (selectedGradeValues.Any(gradeValue => displayOption.hideIfGrade.Contains(gradeValue)))
                    continue;

                var testEventDate = DateTime.Parse(selectedTestAdmin.Date);
                if (displayOption.code == "IEGSSA" && (!selectedTestAdmin.AllowCovidReportFlag || testEventDate.Date > _covidChopOffDate.Date))
                    continue;

                var applySelection = applyDefaultSelection ? displayOption.isDefault : currentGroup.IsValueSelected(displayOption.code);
                var option = new Option
                {
                    Value = displayOption.code,
                    Text = displayOption.text,
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.DisplayOptions)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.DisplayOptions),
                Options = options
            };
        }

        public OptionGroup MapSkillSets(List<SkillSet> skillSets, OptionGroup currentGroup, XMLReportOptionGroup_Skillset xmlSkillSet, ScoringOptions scoringOptions, out SkillSet selectedSkillSet, Action<OptionGroup> setInvalidGroup)
        {
            selectedSkillSet = null;
            var options = new List<Option>();

            if (!xmlSkillSet.show)
            {
                options.Add(new Option
                {
                    Value = scoringOptions.SkillSetId.ToString(),
                    IsSelected = true
                });
            }
            else
            {
                var hideSkillsetId = xmlSkillSet.excludeOptionId;
                var applyDefaultSelection = currentGroup == null;

                foreach (var skillSet in skillSets.Where(sk => sk.Id != hideSkillsetId))
                {
                    var applySelection = applyDefaultSelection ? skillSet.IsDefaultSkill : currentGroup.IsValueSelected(skillSet.Id);

                    var option = new Option
                    {
                        Value = skillSet.Id,
                        Text = skillSet.Description,
                        IsSelected = applySelection
                    };

                    if (applySelection)
                        selectedSkillSet = skillSet;

                    options.Add(option);
                }

                if (!options.Any(o => o.IsSelected))
                {
                    options.First().IsSelected = true;
                    selectedSkillSet = skillSets.First(sk => sk.Id == options.First().Value);

                    if (IsFromSavedCriteria(currentGroup))
                        setInvalidGroup(currentGroup);
                }
            }

            return new OptionGroup(XMLGroupType.SkillDomainClassification)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.SkillDomainClassification),
                Options = options,
                IsHidden = !xmlSkillSet.show
            };
        }

        public OptionGroup MapLowerGraphType(XMLReportOptionGroup_LowerGraphType lowerGraphType, OptionGroup currentGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var lowerGraph in lowerGraphType.Option)
            {
                var applySelection = applyDefaultSelection ? lowerGraph.isDefault : currentGroup.IsValueSelected(lowerGraph.id);
                var option = new Option
                {
                    Value = lowerGraph.id,
                    Text = lowerGraph.text,
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
                options.First(o => o.Value == lowerGraphType.Option.First(gt => gt.isDefault).id).IsSelected = true;

            return new OptionGroup(XMLGroupType.LowerGraphType)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LowerGraphType),
                Options = options
            };
        }

        public OptionGroup MapGraphScoreTypesForMultiSelect(XMLScoreTypes scoreTypes, OptionGroup currentGroup, ScoringOptions scoringOptions, SkillSet selectedSkillSet, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var scoreType in scoreTypes.ScoreTypeName)
            {
                var values = scoreType.code.Split('/'); //check against Scoring Options if we need to skip this Score

                if (values.Any(scoringOptions.HideScore))
                    continue;

                var applySelection = applyDefaultSelection ?
                                     scoreType.isDefault :
                                     !currentGroup.IsHidden && options.Count(o => o.IsSelected) <= scoreTypes.maxToSelect && currentGroup.IsValueSelected(scoreType.code);

                options.Add(new Option
                {
                    Value = scoreType.code,
                    Text = scoreType.text,
                    IsHidden = scoreType.hide,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.GraphScores)
            {
                Category = OptionsCategory.Secondary,
                InputControl = scoreTypes.isMultiselect ? OptionsInputControl.MultiSelect : OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.GraphScores),
                Options = options,
                MinToSelect = scoreTypes.minToSelect,
                MaxToSelect = scoreTypes.maxToSelect,
                HasSelectNone = scoreTypes.isMultiselect
            };
        }

        public OptionGroup MapGraphScoreTypesForSingleSelect(XMLScoreTypes scoreTypes, OptionGroup currentGroup, ScoringOptions scoringOptions, SkillSet selectedSkillSet, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var scoreType in scoreTypes.ScoreTypeName)
            {
                var values = scoreType.code.Split('/'); //check against Scoring Options if we need to skip this Score

                if (values.Any(scoringOptions.HideScore))
                    continue;

                var applySelection = applyDefaultSelection ? scoreType.isDefault : currentGroup.IsValueSelected(scoreType.code);

                options.Add(new Option
                {
                    Value = scoreType.code,
                    Text = scoreType.text,
                    IsHidden = scoreType.hide,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.GraphScores)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.GraphScores),
                Options = options
            };
        }

        public OptionGroup MapScoreTypes(XMLReportOptionGroup_XMLScoreTypes scoreTypes, OptionGroup currentGroup, ScoringOptions scoringOptions, string avoidScoreType, SkillSet selectedSkillSet, bool hideGroup, bool isCovid, Action<OptionGroup> setInvalidGroup)
        {//TODO:  Split into single select and multi select functions.
            List<string> prevSelectedIds = currentGroup == null ? new List<string>() : currentGroup.SelectedValues;
            var options = new List<Option>();
            var defaultOptions = new List<Option>();
            var selectedOptions = new List<Option>();

            var numSelected = 0;

            foreach (var scoreType in scoreTypes.ScoreTypeName.Where(st => st.code != avoidScoreType))
            {
                if (avoidScoreType == scoreType.code) continue;

                //Dmitriy - next 3 lines check the XML score types against database values for this particular report/customer/etc.
                //if database says to skip this option, we're skipping it even though it is defined in XML
                //TODO:  Dmitriy - add alt value to XML for alt scores.  Do NOT use v.Replace - not good.
                var scoreTypeValues = scoreType.code.Split('/');
                bool skipThisOption = scoreTypeValues.Any(v =>
                                                        scoringOptions.DisplayFlags.Keys.Contains(v.Replace("{0}", "")) &&
                                                        !scoringOptions.DisplayFlags[v.Replace("{0}", "")]);
                if (skipThisOption) continue;

                //PM.101-3624
                //Web Reporting - Interims Roster - Remove ISS-D, ISS-T and eSS score options when ELA CCR anchor Stds are selected
                skipThisOption = selectedSkillSet != null && selectedSkillSet.Id == scoreType.hideIfSkillID;
                if (skipThisOption) continue;

                var scoreTypeCode = CreateAlternativeScoreValue(scoreType.code, scoringOptions.AlternativeNormYear);
                var scoreLabel = CreateAlternativeScoreLabel(scoreType.text, scoringOptions.AlternativeNormYear);

                var option = new Option
                {
                    Value = scoreTypeCode,
                    Text = scoreLabel,
                    IsHidden = scoreType.hide,
                };

                if (scoreType.isDefault && !scoreType.hide) defaultOptions.Add(option);

                if (!scoreType.hide && (scoreTypes.isMultiselect || selectedOptions.Count == 0) && prevSelectedIds.Exists(id => id == option.Value) && numSelected < scoreTypes.maxToSelect && !currentGroup.IsHidden)
                {
                    option.IsSelected = true;
                    selectedOptions.Add(option);
                    ++numSelected;
                }

                options.Add(option);
            }

            if (selectedOptions.Count == 0 && defaultOptions.Count > 0)
            {
                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);

                if (scoreTypes.isMultiselect)
                {
                    foreach (var o in defaultOptions) { o.IsSelected = true; selectedOptions.Add(o); }
                }
                else
                {
                    defaultOptions[0].IsSelected = true;
                    selectedOptions.Add(defaultOptions[0]);
                }
            }

            var maxToSelect = 1;
            if (scoreTypes.isMultiselect)
                maxToSelect = options.Count;
            if (scoreTypes.maxToSelect < maxToSelect)
                maxToSelect = scoreTypes.maxToSelect;

            if (isCovid)
                options.ForEach(o => o.IsSelected = true);

            return new OptionGroup(XMLGroupType.Scores)
            {
                Category = OptionsCategory.Secondary,
                InputControl = scoreTypes.isMultiselect ? OptionsInputControl.MultiSelect : OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.Scores),
                Options = options,
                MinToSelect = scoreTypes.minToSelect,
                MaxToSelect = maxToSelect,
                HasSelectNone = scoreTypes.isMultiselect,
                IsHidden = hideGroup
            };
        }

        public OptionGroup MapExportTemplate(XMLDFExportTemplateMenu exportTemplate, OptionGroup currentGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var templateOption in exportTemplate.TemplateOption)
            {
                var applySelection = applyDefaultSelection ? templateOption.isDefault : currentGroup.IsValueSelected(templateOption.code);
                var option = new Option
                {
                    Value = templateOption.code,
                    Text = templateOption.text,
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            return new OptionGroup(XMLGroupType.ExportTemplate)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ExportTemplate),
                Options = options
            };
        }

        public OptionGroup MapExportFormat(XMLReportOptionGroup_ExportFormat exportFormat, OptionGroup currentGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var exportOption in exportFormat.Option)
            {
                var applySelection = applyDefaultSelection ? exportOption.isDefault : currentGroup.IsValueSelected(exportOption.id);
                var option = new Option//TODO:  Add to options on this line.  Do for all functions.
                {
                    Value = exportOption.id,
                    Text = exportOption.text,
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;
            }

            return new OptionGroup(XMLGroupType.ExportFormat)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ExportFormat),
                Options = options
            };
        }

        public OptionGroup MapExportHeadings(XMLReportOptionGroup_ExportHeadings exportHeadings, OptionGroup exportFormatGroup, OptionGroup currentGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var headingOption in exportHeadings.Option)
            {//TODO:  Get rid of hardcoded values and use boolean instead of string "true"
                if (headingOption.id == "true" && exportFormatGroup.SelectedValues.Contains("none"))
                    continue;

                var applySelection = applyDefaultSelection ? headingOption.isDefault : currentGroup.IsValueSelected(headingOption.id);
                options.Add(new Option
                {
                    Value = headingOption.id,
                    Text = headingOption.text,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
                options.First().IsSelected = true;

            return new OptionGroup(XMLGroupType.ExportHeadings)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ExportHeadings),
                Options = options
            };
        }

        public CustomFieldGroup MapCustomDataFields(XMLExportFileInfo exportFileInfoXml, CustomFieldGroup currentGroup, XMLGroupType updatedType, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<CustomFieldOption>();
            var applySelection = currentGroup != null && currentGroup.Options.Any(i => i.IsSelected);
            var selectedOptions = applySelection ? currentGroup.Options.Cast<CustomFieldOption>().Where(i => i.IsSelected).ToList() : new List<CustomFieldOption>();

            foreach (var grouping in exportFileInfoXml.ParentItem)
                foreach (var item in grouping.Item)
                {
                    var applyDefaultSelection = (currentGroup == null || updatedType == XMLGroupType.Assessment) && !item.manualSelection;
                    var isSelected = applyDefaultSelection || selectedOptions.Any(o => o.Value == item.value);
                    var userText = selectedOptions.FirstOrDefault(o => o.Value == item.value)?.UserText;
                    var userWidth = selectedOptions.FirstOrDefault(o => o.Value == item.value)?.UserWidth;

                    options.Add(new CustomFieldOption
                    {
                        Value = item.value,
                        Text = item.text,
                        Width = item.width,
                        GroupingValue = grouping.value,
                        GroupingText = grouping.text,
                        Padding = item.padding,
                        ManualSelection = item.manualSelection,

                        IsSelected = isSelected,
                        UserText = isSelected && userText == item.text ? null : userText,
                        UserWidth = isSelected && userWidth == item.width ? null : userWidth
                    });
                }

            if (selectedOptions.Count != options.Count(o => o.IsSelected) && IsFromSavedCriteria(currentGroup))
                setInvalidGroup(currentGroup);

            return new CustomFieldGroup
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.DataExportCustomDataFields,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.CustomDataFields),
                Options = options.Cast<Option>().ToList(),
                Separator = exportFileInfoXml.separator,
                Delimiter = exportFileInfoXml.delimiter,
                UserTextLength = exportFileInfoXml.userTextLength,
                SelectedValuesOrder = currentGroup == null || updatedType == XMLGroupType.Assessment ? new List<string>() : currentGroup.SelectedValuesOrder
            };
        }

        public OptionGroup MapAbilityProfile(OptionGroup currentGroup, XMLDFAbilityProfile xmlGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            var abilityProfiles = new Dictionary<string, string> { { "Yes", "false" }, { "No", "true" } };
            foreach (var profile in abilityProfiles)
            {
                var applySelection = applyDefaultSelection ? xmlGroup.defaultValue && profile.Value == "false" : currentGroup.IsValueSelected(profile.Value);
                options.Add(new Option
                {
                    Value = profile.Value,
                    Text = profile.Key,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected) && IsFromSavedCriteria(currentGroup))
                setInvalidGroup(currentGroup);

            return new OptionGroup(XMLGroupType.AbilityProfile)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.AbilityProfile),
                Options = options,
                IsHidden = xmlGroup.hide
            };
        }

        public OptionGroup MapColumnZ(OptionGroup currentGroup, ScoringOptions scoringOptions, XMLDFExcludeZMenu xmlExcludeZ, Action<OptionGroup> setInvalidGroup)
        {
            var applyDefaultSelection = currentGroup == null;
            var applySelection = applyDefaultSelection ? scoringOptions.DefaultAdmz == 0 : currentGroup.Options.First().IsSelected;

            var options = new List<Option>
            {
                new CheckboxOption
                {
                    Text = "Include Students Coded in Office Use",
                    IsSelected = applySelection
                }
            };

            return new OptionGroup(XMLGroupType.ColumnZ)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.Checkbox,
                DisplayName = "",
                Options = options,
                IsHidden = xmlExcludeZ.hide
            };
        }

        public OptionGroup MapMathComputation(OptionGroup currentGroup, OptionPage newPage, XMLGroupType updatedGroupType, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();

            {
                var specialCondition = newPage.AssessmentCode == XMLProductCodeEnum.IOWA &&
                                       newPage.XmlDisplayType == XMLReportType.GP &&
                                       updatedGroupType == XMLGroupType.DisplayOptions &&
                                       newPage.GetSelectedValuesOf(updatedGroupType).Contains("EGS");
                var persistUserSelection = currentGroup != null && currentGroup.Options[0].IsSelected;
                var defaultSelection = currentGroup == null && newPage.ScoringOptions.ExcludeMathcompDefault == 0;

                options.Add(new CheckboxOption
                {
                    Text = "Include Math Computation in Math Total",
                    IsSelected = specialCondition || persistUserSelection || defaultSelection
                });
            }

            {
                var persistUserSelection = currentGroup != null && currentGroup.Options[1].IsSelected;
                var defaultSelection = currentGroup == null && newPage.ScoringOptions.ElaTotal == 0;

                options.Add(new CheckboxOption
                {
                    Text = "Include Extended ELA Total in Core and Complete Composite Calculation",
                    IsSelected = persistUserSelection || defaultSelection
                });
            }

            return new OptionGroup(XMLGroupType.CompositeCalculationOptions)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.Checkbox,
                DisplayName = "",
                Options = options
            };
        }

        public OptionGroup MapHomeReporting(XMLHomeReporting xmlHomeReporting, OptionGroup currentGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var language in xmlHomeReporting.Language)
            {
                var applySelection = applyDefaultSelection ? language.isDefault : currentGroup.IsValueSelected(language.value);
                options.Add(new Option
                {
                    Value = language.value,
                    AltValue = language.isHomeReporting,
                    Text = language.text,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;
            }

            return new OptionGroup(XMLGroupType.HomeReporting)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.HomeReporting),
                Options = options
            };
        }

        //TODO:  Same functionality as in MapCommonXmlGroup
        public OptionGroup MapSortDirections(XMLSortDirections sortDirections, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var sortDirection in sortDirections.SortDirection)
            {
                var applySelection = applyDefaultSelection ? sortDirection.isDefault : currentGroup.IsValueSelected(sortDirection.code);
                options.Add(new Option
                {
                    Value = sortDirection.code,
                    Text = sortDirection.text,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.SortDirection)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.SortDirection),
                Options = options
            };
        }

        public OptionGroup MapContentAreasForSingleSelect(List<DbContentScope> contentAreas, OptionGroup currentGroup, XMLProductCodeEnum selectedAssessmentCode, out List<DbContentScope> selectedContentAreas, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            selectedContentAreas = new List<DbContentScope>();

            foreach (var contentArea in contentAreas)
            {
                var applySelection = applyDefaultSelection ? contentArea.IsDefault : currentGroup.IsValueSelected(contentArea.Acronym);

                options.Add(new Option
                {
                    Value = contentArea.Acronym,
                    Text = contentArea.SubtestName,
                    IsSelected = applySelection && !options.Any(o => o.IsSelected)
                });

                if (applySelection)
                    selectedContentAreas.Add(contentArea);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;
                selectedContentAreas.Add(contentAreas.First());

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.ContentScope)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ContentScope),
                Options = options
            };
        }

        public OptionGroup MapContentAreasForMultiSelect(List<DbContentScope> contentAreas, OptionGroup currentGroup, XMLProductCodeEnum selectedAssessmentCode, out List<DbContentScope> selectedContentAreas, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            selectedContentAreas = new List<DbContentScope>();

            foreach (var contentArea in contentAreas)
            {
                var applySelection = applyDefaultSelection ? contentArea.IsDefault : currentGroup.IsValueSelected(contentArea.Acronym);

                options.Add(new Option
                {
                    Value = contentArea.Acronym,
                    Text = contentArea.SubtestName,
                    IsSelected = applySelection
                });

                if (applySelection)
                    selectedContentAreas.Add(contentArea);
            }

            if (!applyDefaultSelection && options.Count(o => o.IsSelected) != currentGroup.SelectedValues.Count)
            {
                options.First().IsSelected = true;
                selectedContentAreas.Add(contentAreas.First());

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.ContentScope)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ContentScope),
                Options = options,
                HasSelectAll = true,
                HasSelectNone = true,
                MinToSelect = 1,
                MaxToSelect = options.Count
            };
        }

        public OptionGroup MapCompositeTypes(List<CompositeType> compositeTypes, XMLDFCompositeTypesMenu xmlCompositeTypes, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {//TODO:  Revisit, possibly split Mult Vs. Single select
            int selectedCount = 0;
            var selectedValue = currentGroup == null ? new List<string>() : currentGroup.SelectedValues;
            var options = new List<Option>();
            foreach (var compositeType in compositeTypes)
            {
                var option = new Option
                {
                    Value = compositeType.Acronym,
                    Text = compositeType.SubtestName
                };

                if (selectedValue.Contains(compositeType.Acronym))
                {
                    option.IsSelected = true;
                    ++selectedCount;
                }
                options.Add(option);
            }

            if (selectedCount == 0)
            {
                string selected = compositeTypes.Find(e => e.DefaultSelectedFlag).Acronym;
                options.Find(e => e.Value == selected).IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return xmlCompositeTypes.isMultiselect ?
                new OptionGroup(XMLGroupType.CompositeTypes)
                {
                    Category = OptionsCategory.Secondary,
                    InputControl = OptionsInputControl.MultiSelect,
                    DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.CompositeTypes),
                    Options = options,
                    HasSelectAll = true,
                    HasSelectNone = true,
                    MinToSelect = 1,
                    MaxToSelect = options.Count
                } :
                new OptionGroup(XMLGroupType.CompositeTypes)
                {
                    Category = OptionsCategory.Secondary,
                    InputControl = OptionsInputControl.SingleSelect,
                    DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.CompositeTypes),
                    Options = options,
                };
        }

        public OptionGroup MapSubContentAreaList(List<SubContentArea> subContentAreas, OptionGroup currentGroup, bool disableParentSkill, bool disableCognitiveskill, bool isMultiselect, Action<OptionGroup> setInvalidGroup)
        {//TODO .... this whole function is just a big OMG...
            var options = new List<Option>();
            var selectedValues = currentGroup == null ? new List<string>() : currentGroup.SelectedValues;
            int selectedCount = 0;
            foreach (var subContentArea in subContentAreas)
            {
                if (disableParentSkill && subContentArea.ParentFlag == "1")
                {
                    continue;
                }
                if (disableCognitiveskill && subContentArea.CognitiveSkillFlag == "1")
                {
                    continue;
                }

                var option = new Option
                {
                    Value = subContentArea.SkillId.Trim(),
                    Text = subContentArea.SkillName,
                    AltValue = subContentArea.ParentFlag
                };

                if ((isMultiselect || selectedCount == 0) && selectedValues.Contains(option.Value))
                {
                    option.IsSelected = true;
                    ++selectedCount;
                }
                options.Add(option);

            }

            //Apply defaults if nothing selected
            if (selectedCount == 0)
            {
                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);

                if (isMultiselect) foreach (var o in options) o.IsSelected = true; //Select All
                else options[0].IsSelected = true; //Select First
            }
            else if (selectedCount != selectedValues.Count && IsFromSavedCriteria(currentGroup))
                setInvalidGroup(currentGroup);
            else
                foreach (string selectedId in selectedValues)
                    if (!options.Exists(e => e.IsSelected && e.Value == selectedId) && IsFromSavedCriteria(currentGroup))
                    {
                        setInvalidGroup(currentGroup);
                        break;
                    }

            return new OptionGroup(XMLGroupType.SubContentScope)
            {
                Category = OptionsCategory.Secondary,
                InputControl = isMultiselect ? OptionsInputControl.MultiSelect : OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.SubContentScope),
                Options = options,
                //OriginalObject = subContentAreas,
                HasSelectAll = isMultiselect,
                HasSelectNone = isMultiselect,
                MinToSelect = 1,
                MaxToSelect = options.Count
            };
        }

        public OptionGroup MapSortBySubtest(List<SubtestFamily> subtestFamilies, OptionGroup currentGroup, out SubtestFamily selectedSubtestFamily, Action<OptionGroup> setInvalidGroup)
        {
            selectedSubtestFamily = null;
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var family in subtestFamilies)
            {
                var value = family.SubtestFamilyId.Replace("'", "");
                var applySelection = applyDefaultSelection ? family.DefaultSelectedFlag : currentGroup.IsValueSelected(value);
                options.Add(new Option
                {
                    Value = value,
                    Text = family.SubtestName,
                    IsSelected = applySelection
                });

                if (applySelection)
                    selectedSubtestFamily = family;
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;
                selectedSubtestFamily = subtestFamilies.First();

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.SortBySubtest)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.SortBySubtest),
                Options = options
            };
        }

        public OptionGroup MapPerformanceBands(XMLPerformanceBands xmlBands, PerformanceBandGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<PerformanceBandOption>();
            var newBandKey = xmlBands is XMLInterimPerformanceBands bands ? bands.Key : xmlBands.Key;
            //The only time currentGroup.BandKey is null is when the group is loaded from IRM 4.0 saved criteria.
            //Therefore, have to treat it as a "persist to user selections".
            var applyDefaults = currentGroup == null ||
                                currentGroup.BandKey != null && currentGroup.BandKey != newBandKey;

            foreach (var band in xmlBands.Band)
            {
                var currentOption = applyDefaults ? null : currentGroup.Options.Cast<PerformanceBandOption>().FirstOrDefault(o => o.BandColor == band.color);

                options.Add(new PerformanceBandOption
                {
                    Text = applyDefaults ? band.name : currentOption.Text,
                    BandColor = band.color,
                    LowValue = applyDefaults ? band.low : currentOption.LowValue,
                    HighValue = applyDefaults ? band.high : currentOption.HighValue,
                    IsSelected = true
                });
            }

            //TODO:  Check if (IsFromSavedCriteria(currentGroup))... But then, do we really care for this group?

            return new PerformanceBandGroup
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.MultimeasurePerformanceBands,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.PerformanceBands),
                Options = options.Cast<Option>().ToList(),
                BandKey = newBandKey
            };
        }

        public LocationGroup MapLocations(List<Location> locations, int selectedId, out Location selectedLocation, Action<OptionGroup> setInvalidGroup)
        {
            selectedLocation = null;
            var options = new List<Option>();

            foreach (var location in locations)
            {
                var applySelection = location.Id == selectedId;
                options.Add(new Option
                {
                    Value = location.Id.ToString(),
                    Text = location.NodeName,
                    IsSelected = applySelection
                });

                if (applySelection)
                    selectedLocation = location;
            }

            var group = new LocationGroup(locations[0].NodeType)
            {
                LocationNodeType = locations[0].NodeType,
                Category = OptionsCategory.Locations,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = locations[0].NodeTypeDisplay.ToUpper(),
                Options = options
            };

            if (selectedLocation != null)
                return group;

            //setInvalidGroup(group);

            //IF NOT STUDENT REPORT
            //THE "ALL" NODE IS THE FIRST ONE WHICH HAS "-1"ID
            options.First().IsSelected = true;
            selectedLocation = locations.First();

            return group;
        }

        public LocationGroup MapLocations_Paper(List<Location> locations, List<int> selectedIds, out List<Location> selectedLocations, Action<OptionGroup> setInvalidGroup)
        {
            selectedLocations = new List<Location>();
            var options = new List<Option>();

            foreach (var location in locations)
            {
                var applySelection = selectedIds.Contains(location.Id);
                options.Add(new Option
                {
                    Value = location.Id.ToString(),
                    Text = location.NodeName,
                    IsSelected = applySelection
                });

                if (applySelection)
                    selectedLocations.Add(location);
            }

            var group = new LocationGroup(locations[0].NodeType)
            {
                LocationNodeType = locations[0].NodeType,
                Category = OptionsCategory.Locations,
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = locations[0].NodeTypeDisplay.ToUpper(),
                Options = options,
                HasSelectAll = true,
                HasSelectNone = true,
                MinToSelect = 0,
                MaxToSelect = locations.Count
            };

            if (IsFromSavedCriteria(group) && (selectedLocations.Count != selectedIds.Count || selectedLocations.Any(l => !selectedIds.Contains(l.Id))))
                setInvalidGroup(group);

            return group;
        }

        public OptionGroup MapStudents(List<Student> students, OptionGroup currentGroup, bool showAllOption, out Student selectedStudent, Action<OptionGroup> setInvalidGroup)
        {
            selectedStudent = null;
            var persistSelection = currentGroup != null;
            var options = new List<Option>();

            if (showAllOption)
            {
                options.Add(new Option
                {
                    Value = "-1",
                    AltValue = "",
                    Text = "All",
                    IsSelected = persistSelection && currentGroup.IsValueSelected("-1")
                });
            }

            foreach (var student in students)
            {
                //TestInstanceId is used when report is switching from Roster to Profile by clicking inside Actuate report
                //Id in this case is null, so we have to go by TestInstanceId
                var applySelection = persistSelection && (currentGroup.IsValueSelected(student.Id) || currentGroup.SelectedAltValues.Contains(student.TestInstanceId));

                options.Add(new Option
                {
                    Value = student.Id,
                    AltValue = student.TestInstanceId,
                    Text = student.Name,
                    IsSelected = applySelection
                });

                if (applySelection)
                    selectedStudent = student;
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First(o => o.Value != "-1").IsSelected = true;
                selectedStudent = students.First();

                if (persistSelection && currentGroup.IsFromSavedCriteria)
                    setInvalidGroup(currentGroup);
            }

            return new LocationGroup(XMLGroupType.Student)
            {
                LocationNodeType = "STUDENT",
                Category = OptionsCategory.Locations,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.Student),
                Options = options
            };
        }

        public OptionGroup MapGroupPopulation(List<GroupPopulation> groupPopulations, OptionGroup currentGroup, XMLReportOptionGroup xmlGroup, XMLGroupType updatedGroupType, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelections = currentGroup == null || (int)updatedGroupType < (int)XMLGroupType.GroupPopulation && !currentGroup.IsFromSavedCriteria;

            foreach (var gp in groupPopulations)
            {
                var isSelected = applyDefaultSelections ? gp.IsDefault : currentGroup.IsValueSelected(gp.NodeValue, currentGroup.IsFromIrm40Xml);
                options.Add(new Option
                {
                    Value = gp.NodeValue,
                    Text = gp.NodeType,
                    IsSelected = isSelected
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(xmlGroup.GroupType)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = _xmlLoader.GetDisplayText(xmlGroup.GroupType),
                Options = options,
                MinToSelect = xmlGroup.minToSelect,
                MaxToSelect = xmlGroup.maxToSelect
            };
        }

        public OptionGroup MapDisaggDictionary(Dictionary<string, List<Disaggregation>> populationFilters, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<PiledOption>();

            foreach (var pile in populationFilters)
            {
                var pileKey = _helper.CreatePopulationFiltersPileKey(pile.Key);
                var pileLabel = _helper.CreatePopulationFiltersPileLabel(pile.Key);

                foreach (var item in pile.Value)
                {
                    var applySelection = currentGroup != null && currentGroup.IsValueSelected(item.GroupKey);
                    options.Add(new PiledOption
                    {
                        Value = item.GroupKey,
                        Text = item.GroupValue,
                        IsSelected = applySelection,
                        PileKey = pileKey,
                        PileLabel = pileLabel
                    });
                }
            }

            var newSelectedValues = options.Where(o => o.IsSelected).Select(o => o.Value);
            if (IsFromSavedCriteria(currentGroup) && currentGroup != null && currentGroup.SelectedValues.Any(s => !newSelectedValues.Contains(s)))
                setInvalidGroup(currentGroup);

            return new OptionGroup(XMLGroupType.PopulationFilters)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.PiledSingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.PopulationFilters),
                Options = options.Cast<Option>().ToList()
            };
        }

        public OptionGroup MapSubgroupDisaggDictionary(List<Disaggregation> populationFilters, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null || currentGroup.InputControl is OptionsInputControl.PiledSingleSelect;

            foreach (var item in populationFilters)
            {
                var applySelection = applyDefaultSelection ? item.DefaultFlag == 1 : currentGroup.IsValueSelected(item.GroupValue);
                options.Add(new Option
                {
                    Value = item.GroupValue,
                    Text = item.GroupLabel,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.PopulationFilters)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.PopulationFilters),
                Options = options
            };
        }

        public OptionGroup MapLongitudinalTestAdmins_Student(List<LongTestAdminScoreset> testAdmins, OptionGroup currentGroup, OptionGroup updatedGroup, int minToSelect, int maxToSelect, bool isActuateHyperlink, out bool isIowaSelected, Action<OptionGroup> setInvalidGroup)
        {
            isIowaSelected = false;
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null || ResetLongitudinalTestAdminsToDefault(currentGroup, updatedGroup, isActuateHyperlink);

            for (int c = 0; c < testAdmins.Count; ++c)
            {
                var testAdmin = testAdmins[c];
                //Default selection = from 0 to MaxToSelct
                //per 4.0 implementation - completely ignoring IsDefaultTestGrade property of LongTestAdminScoreset
                var applySelection = applyDefaultSelection ? c < maxToSelect : currentGroup.IsValueSelected(testAdmin.TestAdminGradeLevelId);

                options.Add(new Option
                {
                    Value = testAdmin.TestAdminGradeLevelId,
                    AltValue = testAdmin.CustomerScoresetId.ToString(),
                    Text = testAdmin.ScoresetDesc,
                    IsSelected = applySelection, //Dmitriy - the first option in this group is always selected and disabled per business rules
                    IsDisabled = c == 0
                });

                //this "iowa" is used to determine if Growth Start Point group is displayed
                if (!isIowaSelected && applySelection && (testAdmin.Battery == "COMPLETE" || testAdmin.Battery == "SURVEY" || testAdmin.Battery == "CORE"))
                    isIowaSelected = true;
            }

            if (IsFromSavedCriteria(currentGroup) && !applyDefaultSelection && currentGroup.SelectedValues.Count != options.Count(o => o.IsSelected))
                setInvalidGroup(currentGroup);

            return new OptionGroup(XMLGroupType.LongitudinalTestAdministrations)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.MultiSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LongitudinalTestAdministrations),
                Options = options,
                MinToSelect = minToSelect,
                MaxToSelect = maxToSelect
            };
        }

        private bool ResetLongitudinalTestAdminsToDefault(OptionGroup currentGroup, OptionGroup updatedGroup, bool isActuateHyperlink)
        {
            if (isActuateHyperlink) return false;

            var groupTypes = new List<XMLGroupType>
            {
                XMLGroupType.Assessment,
                XMLGroupType.TestAdministrationDate,
                XMLGroupType.GradeLevel,
                XMLGroupType.DisplayType,
                XMLGroupType.LevelofAnalysis,
                XMLGroupType.DisplayOptions,
                XMLGroupType.LongitudinalTypes,
                XMLGroupType.ContentScope
            };

            var updatedGroupType = updatedGroup?.Type ?? XMLGroupType._INTERNAL_FIRST_;

            return currentGroup == null || groupTypes.Contains(updatedGroupType) || updatedGroup is LocationGroup;
        }

        private double GetLongitudinalTestAdminGradeNumber(LongTestAdminScoreset testAdmin)
        {
            var gmrtLevelsMap = new Dictionary<string, string>
            {
                {"PR", "01" },
                {"BR", "02" },
                {"AR", "25" },
            };

            string[] array = testAdmin.ScoresetDesc.Split(new[] { "evel" }, StringSplitOptions.None);

            var whole = "";
            var dec = "";
            if (array.Length > 0)
            {
                var wholeArray = array[0].Split(new[] { "rade" }, StringSplitOptions.None);
                var gmrtHandle = wholeArray.Length > 1 ? wholeArray[1] : wholeArray[0];
                whole = gmrtHandle.ToUpper().Contains("PHS") ? "15" : Regex.Replace(gmrtHandle, @"[^\d]", "");
            }

            if (array.Length > 1)
            {
                MatchCollection col = Regex.Matches(array[1], @"(PR|BR|AR)|[0-9]+");
                dec = gmrtLevelsMap.ContainsKey(col[0].Value) ? gmrtLevelsMap[col[0].Value] : col[0].Value;
                if (col.Count > 1) dec += '.' + col[1].Value;
            }

            if (string.IsNullOrEmpty(whole))
                whole = "0";

            Double iWhole = Double.Parse(whole);
            Double iDecimal = Double.Parse(dec) * 0.01;

            return iWhole + iDecimal;
        }

        private List<string> GetSelectedGradeLevelValues(OptionGroup group)
        {
            if (group?.Options == null || group.Options.Any(o => !(o is LongitudinalTestAdminOption)))
                return new List<string>();

            return group.Options.Cast<LongitudinalTestAdminOption>().SelectMany(o => o.GradeLevels.Where(gl => gl.IsSelected).Select(gl => gl.Value)).ToList();
        }

        public OptionGroup MapLongTestAdmins_Group(List<LongTestAdminScoreset> testAdmins, OptionGroup currentGroup, OptionGroup changedGroup, int minToSelect, int maxToSelect, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<LongitudinalTestAdminOption>();
            var currentParentOption = new LongitudinalTestAdminOption();
            var selectedGradeLevelValues = GetSelectedGradeLevelValues(currentGroup);

            var resetToDefaultSelections = ResetLongitudinalTestAdminsToDefault(currentGroup, changedGroup, false);
            bool isMaxSelectionsReached = false;

            for (int c = 0; c < testAdmins.Count; ++c)
            {
                var testAdmin = testAdmins[c];
                var parentText = testAdmin.ScoresetDesc.Substring(0, testAdmin.ScoresetDesc.IndexOf("Grade", StringComparison.Ordinal) - 1);
                var childText = testAdmin.ScoresetDesc.Substring(testAdmin.ScoresetDesc.IndexOf("Grade", StringComparison.Ordinal), testAdmin.ScoresetDesc.Length - testAdmin.ScoresetDesc.IndexOf("Grade", StringComparison.Ordinal));
                var gradeNumber = GetLongitudinalTestAdminGradeNumber(testAdmin);

                var applyDefaultSelections = resetToDefaultSelections && testAdmin.IsDefaultTestGrade && !isMaxSelectionsReached;
                var persistUserSelections = !resetToDefaultSelections && selectedGradeLevelValues.Contains(testAdmin.TestGradeId) && !isMaxSelectionsReached;
                var applySelection = applyDefaultSelections || persistUserSelections;

                //Dmitriy - Starting a new parent group (checkbox)
                if (c == 0 || testAdmins[c - 1].CustomerScoresetId != testAdmin.CustomerScoresetId)
                {
                    var option = new LongitudinalTestAdminOption
                    {
                        Value = testAdmin.TestAdminGradeLevelId,
                        AltValue = testAdmin.CustomerScoresetId.ToString(),
                        Text = parentText,
                        IsSelected = c == 0 || applySelection,
                        IsDisabled = c == 0,
                        GradeLevels = new List<Option> //every parent Test Admin gets first sub group made out of itself
                            {
                                new Option
                                {
                                    Value = testAdmin.TestGradeId,
                                    Text = childText,
                                    AltValue = gradeNumber.ToString(CultureInfo.InvariantCulture),
                                    IsSelected = c == 0 || applySelection,
                                    IsDisabled = c == 0,
                                }
                            }
                    };

                    options.Add(option);
                    currentParentOption = option;
                }
                else//Dmitriy - If Scoreset Id is the same as the previous Test Admin - build a sub (radio) group
                {
                    var subOption = new Option
                    {
                        Value = testAdmin.TestGradeId,
                        Text = childText,
                        AltValue = gradeNumber.ToString(CultureInfo.InvariantCulture),
                    };

                    if (applySelection)
                    {
                        subOption.IsSelected = true;
                        currentParentOption.IsSelected = true;
                    }

                    currentParentOption.GradeLevels.Add(subOption);
                }

                isMaxSelectionsReached = options.Count(o => o.IsSelected) == maxToSelect;
            }

            //Disable Logic - Only disable when maximum number of selections was made
            if (isMaxSelectionsReached)
            {
                foreach (var parentOption in options.Where(o => !o.IsSelected))
                {
                    parentOption.IsDisabled = true;
                }
            }

            if (IsFromSavedCriteria(currentGroup) && currentGroup.SelectedValues.Count != options.Count(o => o.IsSelected))
                setInvalidGroup(currentGroup);

            return new OptionGroup(XMLGroupType.LongitudinalTestAdministrations)
            {

                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.LongitudinalTestAdministrations,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LongitudinalTestAdministrations),
                Options = options.Cast<Option>().ToList(),
                MinToSelect = minToSelect,
                MaxToSelect = maxToSelect
            };
        }

        public OptionGroup MapGrowthStartPointType(XMLGrowthStartPoint growthStartPoint, OptionGroup currentGroup, OptionGroup testAdminsGroup, bool isIowaSelected, Action<OptionGroup> setInvalidGroup)
        {
            //Dmitriy - creating options, getting user selected Ids, usual procedure
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var xmlOption in growthStartPoint.GrowthStartPointOption)
            {
                if (xmlOption.code == "TestAdmin" && !isIowaSelected) //Dmitriy - if the user has no IOWAs selected, remove the first option
                    continue;

                var applySelection = applyDefaultSelection ? xmlOption.isDefault : currentGroup.IsValueSelected(xmlOption.code);

                options.Add(new Option
                {
                    Value = xmlOption.code,
                    Text = xmlOption.text,
                    IsSelected = applySelection
                });
            }

            if (options.Count == 1 && !options.First().IsSelected) //in case we are left with only 1 option after removing options in the loop above
                options.First().IsSelected = true;

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.LongitudinalGrowthStartPointType)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LongitudinalGrowthStartPointType),
                Options = options
            };
        }

        public OptionGroup MapGrowthStartPoint(List<LongTestAdminScoreset> testAdmins, OptionGroup testAdminsGroup, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            var goodList = new List<string> { "COMPLETE", "SURVEY", "CORE" };

            var filteredTestAdminOptions = new List<Option>();
            foreach (var selectedTestAdminOption in testAdminsGroup.Options.Where(o => o.IsSelected))
            {
                var testAdmin = testAdmins.First(ta => ta.TestAdminGradeLevelId == selectedTestAdminOption.Value);

                if (goodList.Contains(testAdmin.Battery))
                    filteredTestAdminOptions.Add(selectedTestAdminOption);
            }

            var optionsCount = filteredTestAdminOptions.Count;

            for (int c = 0; c < optionsCount; ++c)
            {
                var testAdminOption = filteredTestAdminOptions[c];

                var applySelection = applyDefaultSelection ? c == optionsCount - 1 : currentGroup.IsValueSelected(testAdminOption.Value);

                options.Add(new Option
                {
                    Value = testAdminOption.Value,
                    AltValue = testAdminOption.AltValue,
                    Text = testAdminOption.Text,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);

                if (options.Any())
                    options.Last().IsSelected = true;
            }

            return new OptionGroup(XMLGroupType.LongitudinalGrowthStartPoint)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LongitudinalGrowthStartPoint),
                Options = options
            };
        }

        public OptionGroup MapGrowthEndPoint(List<GrowthEndPoint> growthEndPoints, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            for (int c = 0; c < growthEndPoints.Count; ++c)
            {
                var growthEndPoint = growthEndPoints[c];

                var applySelection = applyDefaultSelection ? c == 0 : currentGroup.IsValueSelected(growthEndPoint.ExpectedGrowthParameters);
                options.Add(new Option
                {
                    Value = growthEndPoint.ExpectedGrowthParameters,
                    Text = growthEndPoint.ExpectedGrowthDesc,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.LongitudinalGrowthEndPoint)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LongitudinalGrowthEndPoint),
                Options = options
            };
        }
        public OptionGroup MapGrowthGoalXml(XMLGrowthGoal xmlGrowthGoal, OptionGroup currentGroup, string selectedGrowthStartPointTypeValue, string contentScopeValuesString, int gradeValue, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var xmlOption in xmlGrowthGoal.GrowthGoalOption)
            {
                bool skipFlg = false;
                if (xmlOption.ContentAreaIncludeCondition != null)
                {
                    var inclusiveValues = xmlOption.ContentAreaIncludeCondition.Split(',');
                    // TODO: Dmitriy - 5/15/2018 (6.0 initial work) - Where did the below comments come from?  Do we still need to do something here?
                    // MERGED FROM PROD BRANCH
                    // As this feature is intended to pertain to grades 6 and above, 
                    // would we have a way to suppress it as an option if the Grade/Level selection is Level 11 or below?
                    skipFlg = inclusiveValues.Any(contentScopeValuesString.Contains) && gradeValue > 5;
                }

                if (xmlOption.ContentAreaIncludeCondition != null && !skipFlg
                    ||
                    xmlOption.GrowthStartPointExcludeCondition != null && xmlOption.GrowthStartPointExcludeCondition == selectedGrowthStartPointTypeValue)
                    continue;

                var applySelection = applyDefaultSelection ? xmlOption.isDefault : currentGroup.IsValueSelected(xmlOption.code);
                options.Add(new Option
                {
                    Value = xmlOption.code,
                    Text = xmlOption.text,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.LongitudinalGrowthGoal)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.LongitudinalGrowthGoal),
                Options = options
            };
        }

        public ScoreFiltersGroup MapScoreFilters(ScoreFiltersGroup currentGroup, OptionPage newPage, List<XMLScoreTypeName> xmlScoresList, List<Option> filteredScoreOptions, int pageIndex, Action<OptionGroup> setInvalidGroup)
        {
            //TODO:  Not sure when to set Group Invalid...

            //  Score filters:
            //  1. Take all SELECTED scores from Scores Group (from New Page).
            //  2. Remove scores without @filterValue (from XML).
            //  3. For each score 
            //   a. If Score is ISST -> Take all SELECTED items from Content Scope.
            //   b. All other scores -> Take all SELECTED items from Subcontent Scope (if exists) otherwise from Content Scope

            //  Persist user selection if:
            //  1. SELECTED Score in the Current Group exists in the New Group.
            //  2. SELECTED Content Area in the Current Group exists in the New Group.
            //  3. Current Group has a Value.

            var contentAreas = new Dictionary<string, List<DropdownItem>>();

            if (filteredScoreOptions.Any(s => s.Value == "ISST"))
                contentAreas.Add("ISST", newPage.GetGroupByType(XMLGroupType.ContentScope).Options.Where(o => o.IsSelected).Select(o => new DropdownItem { Text = o.Text, Value = o.Value }).ToList());

            var contentAreaGroup = newPage.GetGroupByType(XMLGroupType.SubContentScope) ?? newPage.GetGroupByType(XMLGroupType.ContentScope);
            contentAreas.Add("regular", contentAreaGroup.Options.Where(o => o.IsSelected).Select(o => new DropdownItem { Text = o.Text, Value = o.Value }).ToList());

            var scores = new List<ScoreFiltersScore>();
            foreach (var newScoreOption in filteredScoreOptions)
            {
                scores.Add(new ScoreFiltersScore
                {
                    FilterValue = xmlScoresList.First(s => s.code == newScoreOption.Value).filterValue,
                    ScoreValue = CreateAlternativeScoreValue(newScoreOption.Value, newPage.ScoringOptions.AlternativeNormYear),
                    ScoreText = CreateAlternativeScoreLabel(newScoreOption.Text, newPage.ScoringOptions.AlternativeNormYear),
                    ContentAreaKey = newScoreOption.Value == "ISST" ? "ISST" : "regular"
                });
            }

            var newGroup = new ScoreFiltersGroup
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.ScoreFilters,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ScoreFilters),
            };
            newGroup.SetScores(scores);
            newGroup.SetContentAreas(contentAreas);

            var numRows = newPage.IsMultimeasure ? 1 : 3;
            var groupExists = currentGroup != null;
            var scoresValues = scores.Select(s => s.ScoreValue).ToList();
            for (int c = 0; c < numRows; ++c)
            {
                var persistSelection = groupExists &&
                                       currentGroup.Rows.Count > c &&
                                       currentGroup.Rows[c].HasSelection &&
                                       scoresValues.Contains(currentGroup.Rows[c].ScoreValue) &&
                                       contentAreaGroup.IsValueSelected(currentGroup.Rows[c].ContentAreaValue);

                var row = new ScoreFilterRow
                {
                    Concatenation = c == 0 && pageIndex == 0 ? ConcatOperatorEnum.None : persistSelection ? currentGroup.Rows[c].Concatenation : ConcatOperatorEnum.AND,
                    FilterValue = persistSelection ? currentGroup.Rows[c].FilterValue : scores.First().FilterValue,
                    ScoreValue = persistSelection ? currentGroup.Rows[c].ScoreValue : scores.First().ScoreValue,
                    //ContentAreaValue = building below (don't know ContentAreaKey at this moment)
                    ComparisonOperator = persistSelection ? currentGroup.Rows[c].ComparisonOperator : newGroup.GetComparisonOperators().First().Value,
                    Value = persistSelection ? currentGroup.Rows[c].Value : ""
                };

                row.ContentAreaKey = scores.First(s => s.FilterValue == row.FilterValue).ContentAreaKey;
                row.ContentAreaValue = persistSelection ? currentGroup.Rows[c].ContentAreaValue : contentAreas[row.ContentAreaKey].First().Value;

                newGroup.Rows.Add(row);
            }

            return newGroup;
        }

        public ScoreWarningsGroup MapScoreWarnings(List<ScoreWarning> scoreWarnings, XMLScoreWarnings xmlScoreWarnings, ScoreWarningsGroup currentGroup, OptionGroup changedGroup, Action<OptionGroup> setInvalidGroup)
        {
            //---------------------------------------------------------------------------------------------------
            //Unfortunately, we did not save SequenceNumbers nor Switch values in IRM 4.0 options criteria XML.
            //Therefore, the only proper way to map to 5.0 objects is to manually go through each row.
            //---------------------------------------------------------------------------------------------------
            if (currentGroup != null && currentGroup.IsFromIrm40Xml)
            {
                if (currentGroup.Rows.Count != scoreWarnings.Count)
                    setInvalidGroup(currentGroup);

                for (int c = 0; c < currentGroup.Rows.Count; ++c)
                {
                    var row = currentGroup.Rows[c];
                    var scoreWarning = scoreWarnings[c];

                    row.SequenceNumber = scoreWarning.DisplaySeq;

                    if (scoreWarning.IncludeFilter == row.Value)
                        row.Switch = ScoreWarningsFilterSwitchEnum.Include;
                    else if (scoreWarning.ExcludeFilter == row.Value)
                        row.Switch = ScoreWarningsFilterSwitchEnum.Exclude;
                }
            }
            //---------------------------------------------------------------------------------------------------

            var rows = new List<ScoreWarningRow>();
            var resetAllSelections = currentGroup == null
                                     || currentGroup.IsFromIrm40Xml
                                     || changedGroup.Type == XMLGroupType.Assessment
                                     || changedGroup.Type == XMLGroupType.TestAdministrationDate
                                     || changedGroup.Type == XMLGroupType.GradeLevel
                                     || changedGroup is LocationGroup;

            for (int c = 0; c < xmlScoreWarnings.numRows; ++c)
            {
                var persistSelection = !resetAllSelections && currentGroup.Rows.Count > c && currentGroup.Rows[c].HasSelection;

                rows.Add(new ScoreWarningRow
                {
                    Concatenation = persistSelection ? currentGroup.Rows[c].Concatenation : c == 0 ? ConcatOperatorEnum.None : ConcatOperatorEnum.AND,
                    SequenceNumber = persistSelection ? currentGroup.Rows[c].SequenceNumber : "",
                    Switch = persistSelection ? currentGroup.Rows[c].Switch : ScoreWarningsFilterSwitchEnum.Include
                });
            }

            return new ScoreWarningsGroup
            {
                ScoreWarningObjects = scoreWarnings,
                Rows = rows,
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.ScoreWarnings,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ScoreWarnings),
            };
        }
        public OptionGroup MapReportGrouping(List<NodeLevel> levels, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            for (int c = 0; c < levels.Count; ++c)
            {
                var level = levels[c];
                var applySelection = applyDefaultSelection ? c == levels.Count - 1 : currentGroup.IsValueSelected(level.NodeType);

                options.Add(new Option
                {
                    Value = level.NodeType,
                    Text = level.NodeDescription,
                    IsSelected = applySelection
                });
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.Last().IsSelected = true;

                if (!applyDefaultSelection && IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(XMLGroupType.ReportGrouping)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(XMLGroupType.ReportGrouping),
                Options = options
            };
        }

        public OptionGroup MapCommonXmlGroup(XMLReportOptionGroup xmlGroup, OptionGroup currentGroup, Action<OptionGroup> setInvalidGroup)
        {
            var options = new List<Option>();
            var applyDefaultSelection = currentGroup == null;

            foreach (var xmlOption in xmlGroup.Option)
            {
                var xmlValue = xmlOption.value ?? xmlOption.id;//TODO:  Id must really go!!!
                var applySelection = applyDefaultSelection ? xmlOption.isDefault : currentGroup.IsValueSelected(xmlValue);
                var option = new Option
                {
                    Value = xmlValue,
                    Text = xmlOption.text,
                    IsSelected = applySelection
                };
                options.Add(option);
            }

            if (!options.Any(o => o.IsSelected))
            {
                options.First().IsSelected = true;

                if (IsFromSavedCriteria(currentGroup))
                    setInvalidGroup(currentGroup);
            }

            return new OptionGroup(xmlGroup.GroupType)
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.SingleSelect,
                DisplayName = _xmlLoader.GetDisplayText(xmlGroup.GroupType),
                Options = options
            };
        }

        private string CreateAlternativeScoreValue(string scoreTypeCode, string alternativeNormYear)
        {
            if (alternativeNormYear == null)
                return string.Format(scoreTypeCode, "");

            var lastTwoDigitsOfYear = alternativeNormYear.Substring(2, 2); //getting last 2 digits from a 4 digit year string
            return string.Format(scoreTypeCode, lastTwoDigitsOfYear);
        }

        private string CreateAlternativeScoreLabel(string scoreLabel, string alternativeNormYear)
        {
            if (alternativeNormYear == null)
                return string.Format(scoreLabel, "");

            return string.Format(scoreLabel, alternativeNormYear);
        }

        private bool IsFromSavedCriteria(OptionGroup group)
        {
            return group != null && group.IsFromSavedCriteria;
        }
    }
}