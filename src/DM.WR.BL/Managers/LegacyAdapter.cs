using AutoMapper;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using IRMWeb;
using IRMWeb.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using LegacyOption = IRMWeb.Models.ReportOption;
using LegacyOptionGroup = IRMWeb.Models.ReportOptionGroup;

namespace DM.WR.BL.Managers
{
    public class LegacyAdapter
    {
        public CriteriaContainer CriteriaXmlToContainerObject(string xml, int buildNumber)
        {
            var legacyBook = SerializationManager.XML_To_ReportOptionGroups(xml);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LegacyOptionGroup, OptionGroup>()
                    .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.TypeEnum))
                    .ForMember(dest => dest.DisplayName, opts => opts.Ignore())
                    .ForMember(dest => dest.Category, opts => opts.Ignore())
                    .ForMember(dest => dest.InputControl, opts => opts.MapFrom(src => InputControlMapper(src)))
                    .ForMember(dest => dest.MinToSelect, opts => opts.Ignore())
                    .ForMember(dest => dest.MaxToSelect, opts => opts.Ignore())
                    .ForMember(dest => dest.HasSelectAll, opts => opts.Ignore())
                    .ForMember(dest => dest.HasSelectNone, opts => opts.Ignore())
                    .ForMember(dest => dest.IsHidden, opts => opts.Ignore())
                    .ForMember(dest => dest.IsDisabled, opts => opts.Ignore())
                    ;
                cfg.CreateMap<LegacyOptionGroup, LocationGroup>()
                    .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.TypeEnum))
                    .ForMember(dest => dest.DisplayName, opts => opts.Ignore())
                    .ForMember(dest => dest.Category, opts => opts.MapFrom(src => OptionsCategory.Locations))
                    .ForMember(dest => dest.InputControl, opts => opts.Ignore())
                    .ForMember(dest => dest.MinToSelect, opts => opts.Ignore())
                    .ForMember(dest => dest.MaxToSelect, opts => opts.Ignore())
                    .ForMember(dest => dest.HasSelectAll, opts => opts.Ignore())
                    .ForMember(dest => dest.HasSelectNone, opts => opts.Ignore())
                    .ForMember(dest => dest.IsHidden, opts => opts.Ignore())
                    .ForMember(dest => dest.IsDisabled, opts => opts.Ignore())
                    .ForMember(dest => dest.LocationNodeType, opts => opts.MapFrom(src => src.TypeEnum))
                    ;
                cfg.CreateMap<LegacyOption, Option>()
                    .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.Label))
                    .ForMember(dest => dest.Value, opts => opts.MapFrom(src => src.Id))
                    .ForMember(dest => dest.IsSelected, opts => opts.MapFrom(src => src.IsSelected))
                    ;
            });
            IMapper mapper = config.CreateMapper();

            var newBook = new List<List<OptionGroup>>();

            foreach (var legacyPage in legacyBook)
            {
                var newPage = new List<OptionGroup>();

                foreach (var legacyGroup in legacyPage)
                {
                    switch (legacyGroup.TypeEnum)
                    {
                        case XMLReportOptionGroupType.MultimesuarePerformanceBand:
                            newPage.Add(MapPerformanceBands(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.LongitudinalTestAdministrations:
                            newPage.Add(MapLongitudinalTestAdmins(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.ColumnZ:
                            newPage.Add(MapColumnZ(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.CompositeCalculationOptions:
                            newPage.Add(MapCompositeCalculation(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.ScoreFilters:
                            newPage.Add(MapScoreFilters(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.ScoreWarnings:
                            newPage.Add(MapScoreWarnings(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.DataSelection:
                            newPage.Add(MapCustomFields(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.CollegeReadiness:
                            newPage.Add(MapCollegeReadiness(legacyGroup));
                            break;
                        case XMLReportOptionGroupType.ShowReadingTotal:
                            newPage.Add(MapReadingTotal(legacyGroup));
                            break;
                        default:
                            var group = legacyGroup.Category == "locations" ?
                                        mapper.Map<LocationGroup>(legacyGroup) :
                                        mapper.Map<OptionGroup>(legacyGroup);
                            newPage.Add(group);
                            break;
                    }
                }

                newBook.Add(newPage);
            }

            return new CriteriaContainer { BuildNumber = buildNumber, Options = newBook };
        }


        private OptionGroup MapCollegeReadiness(LegacyOptionGroup legacyGroup)
        {
            var options = new List<Option>();

            foreach (var legacyOption in legacyGroup.Options)
            {
                options.Add(new Option
                {
                    Value = legacyOption.Id.ToLower() == "true" ? "6" : "0",
                    Text = legacyOption.Label,
                    IsSelected = legacyOption.IsSelected
                });
            }

            return new OptionGroup(XMLGroupType.CollegeReadiness)
            {
                Category = OptionsCategory.Secondary,
                Options = options
            };
        }

        private OptionGroup MapReadingTotal(LegacyOptionGroup legacyGroup)
        {
            var options = new List<Option>();

            foreach (var legacyOption in legacyGroup.Options)
            {
                options.Add(new Option
                {
                    Value = legacyOption.Id.ToLower() == "false" ? "RDGTOTL" : "true",
                    Text = legacyOption.Label,
                    IsSelected = legacyOption.IsSelected
                });
            }

            return new OptionGroup(XMLGroupType.ShowReadingTotal)
            {
                Category = OptionsCategory.Secondary,
                Options = options
            };
        }

        private OptionGroup MapColumnZ(LegacyOptionGroup legacyGroup)
        {
            return new OptionGroup(XMLGroupType.ColumnZ)
            {
                Category = OptionsCategory.Secondary,
                Options = MapToCheckboxes(legacyGroup.Options).ToList()
            };
        }

        private OptionGroup MapCompositeCalculation(LegacyOptionGroup legacyGroup)
        {
            return new OptionGroup(XMLGroupType.CompositeCalculationOptions)
            {
                Category = OptionsCategory.Secondary,
                Options = MapToCheckboxes(legacyGroup.Options).ToList()
            };
        }

        private IEnumerable<Option> MapToCheckboxes(List<LegacyOption> legacyOptions)
        {
            foreach (var legacyOption in legacyOptions)
            {
                yield return new CheckboxOption { IsSelected = legacyOption.Value.ToLower().StartsWith("include") };
            }
        }

        private PerformanceBandGroup MapPerformanceBands(LegacyOptionGroup legacyGroup)
        {
            //Unfortunately, there is absolutelty nothing in the IRM 4.0 XML that could tell us what band color an option is.
            //Therefore, we have to rely on the order of colors in the XMLBandColorEnum generated from DisplayTypes.xml
            if (legacyGroup.Options.Count != Enum.GetNames(typeof(XMLBandColorEnum)).Length)
                throw new Exception("Legacy Adapter :: Could not map legacy 'Performance Bands Group' due to the number of options in IRM 4.0 XML mismatching the number of items in the XMLBandColorEnum.");

            var newGroup = new PerformanceBandGroup
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.MultimeasurePerformanceBands,
                Options = new List<Option>()
            };

            var enumColors = Enum.GetValues(typeof(XMLBandColorEnum)).Cast<XMLBandColorEnum>().ToList();

            for (int c = 0; c < Enum.GetNames(typeof(XMLBandColorEnum)).Length; ++c)
            {
                var legacyOption = legacyGroup.Options[c];

                var values = legacyOption.Value.Split('^');
                var text = legacyOption.Other.Split('|')[0];

                newGroup.Options.Add(new PerformanceBandOption
                {
                    Text = text,
                    BandColor = enumColors[c],
                    LowValue = values[1],
                    HighValue = values[2],
                    IsSelected = true
                });
            }

            return newGroup;
        }

        private OptionGroup MapLongitudinalTestAdmins(LegacyOptionGroup legacyGroup)
        {
            var newGroup = new OptionGroup(XMLGroupType.LongitudinalTestAdministrations)
            {
                InputControl = OptionsInputControl.LongitudinalTestAdministrations,
                Options = new List<Option>()
            };

            foreach (var testAdmin in legacyGroup.Options)
            {
                if (!testAdmin.IsSelected)
                    continue;

                if (testAdmin.OptionGroup == null) //non-nested Longitudinal (no GradeLevels)
                {
                    newGroup.Options.Add(new Option
                    {
                        Text = testAdmin.Label,
                        Value = testAdmin.Id,
                        IsSelected = true
                    });
                }
                else //nested Longitudinal with GradeLevels
                {
                    if (testAdmin.OptionGroup.Options.All(gl => !gl.IsSelected))
                        continue;

                    var gradeLevels = testAdmin.OptionGroup.Options
                        .Where(o => o.IsSelected)
                        .Select(gl => new Option
                        {
                            Text = gl.Label,
                            Value = gl.Id,
                            IsSelected = true
                        })
                        .ToList();

                    newGroup.Options.Add(new LongitudinalTestAdminOption
                    {
                        Text = testAdmin.Label,
                        Value = testAdmin.Id,
                        GradeLevels = gradeLevels,
                        IsSelected = true
                    });
                }
            }

            return newGroup;
        }

        private ScoreFiltersGroup MapScoreFilters(LegacyOptionGroup legacyGroup)
        {
            var newGroup = new ScoreFiltersGroup
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.ScoreFilters,
                Options = new List<Option>(),
                Rows = new List<ScoreFilterRow>()
            };

            foreach (var legacyOption in legacyGroup.Options)
            {
                var split = legacyOption.Value.Split('~');

                var hasConcatenation = legacyOption.Value.Contains("OP:");

                var concatenation = hasConcatenation ? split[0].Split(':')[1] == "|AND|" ? ConcatOperatorEnum.AND : ConcatOperatorEnum.OR : ConcatOperatorEnum.None;
                var filterValue = split.First(t => t.Contains("LOA")).Split(':')[1];
                var scoreValue = split.First(t => t.Contains("SCR")).Split(':')[1];
                var contentAreaValue = split.First(t => t.Contains("CS")).Split(':')[1];
                var comparisonOperator = split.First(t => t.Contains("COND")).Split(':')[1];
                var value = split.First(t => t.Contains("VAL")).Split(':')[1];

                newGroup.Rows.Add(new ScoreFilterRow
                {
                    Concatenation = concatenation,
                    FilterValue = filterValue,
                    ScoreValue = scoreValue,
                    ContentAreaValue = contentAreaValue,
                    ComparisonOperator = comparisonOperator,
                    Value = value
                });
            }

            return newGroup;
        }

        private ScoreWarningsGroup MapScoreWarnings(LegacyOptionGroup legacyGroup)
        {
            var newGroup = new ScoreWarningsGroup
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.ScoreWarnings,
                Options = new List<Option>(),
                Rows = new List<ScoreWarningRow>()
            };

            foreach (var legacyOption in legacyGroup.Options)
            {
                if (legacyOption.Value == "NONE")
                    continue;

                var concatenation = legacyOption.Other != null ? legacyOption.Other == "AND" ? ConcatOperatorEnum.AND : ConcatOperatorEnum.OR : ConcatOperatorEnum.None;

                newGroup.Rows.Add(new ScoreWarningRow
                {
                    Concatenation = concatenation,
                    Value = legacyOption.Value
                });
            }

            return newGroup;
        }

        private CustomFieldGroup MapCustomFields(LegacyOptionGroup legacyGroup)
        {
            var newGroup = new CustomFieldGroup
            {
                Category = OptionsCategory.Secondary,
                InputControl = OptionsInputControl.DataExportCustomDataFields,
                Options = new List<Option>()
            };

            foreach (var legacyValue in legacyGroup.Options.Select(o => o.Value))
            {
                var split = legacyValue.Split('|');

                newGroup.SelectedValuesOrder.Add(split[0]);

                newGroup.Options.Add(new CustomFieldOption
                {
                    IsSelected = true,
                    Value = split[0],
                    UserText = split[1],
                    UserWidth = Convert.ToInt32(split[2])
                });
            }

            return newGroup;
        }

        private OptionsInputControl InputControlMapper(LegacyOptionGroup legacyGroup)
        {
            if (legacyGroup.Category == "checkbox" && legacyGroup.TypeEnum != XMLReportOptionGroupType.ColumnZ && legacyGroup.TypeEnum != XMLReportOptionGroupType.CompositeCalculationOptions ||
                legacyGroup.TypeEnum == XMLReportOptionGroupType.CompositeTypes && legacyGroup.Options.Count(o => o.IsSelected) > 1)
                return OptionsInputControl.MultiSelect;

            return OptionsInputControl.SingleSelect;
        }
    }
}