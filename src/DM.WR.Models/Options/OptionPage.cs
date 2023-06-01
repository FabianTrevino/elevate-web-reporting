using System;
using System.Collections.Generic;
using System.Linq;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.CogAt;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;

namespace DM.WR.Models.Options
{
    /// <summary>
    /// This was introduced to allow unit tests to be written.  Using 'OptionPage' straight up forces too many dependencies.
    /// </summary>
    public interface IOptionPage
    {
        bool GroupHasSelectedOptions(XMLGroupType type);
        Assessment Assessment { get; set; }

        XMLProductCodeEnum AssessmentCode { get; }

        int ExcludeEla { get; }

        int ExcludeMathComputation { get; }

        List<string> GetHiddenValuesOf(XMLGroupType type);

        bool GroupExists(XMLGroupType type);

        bool IsGroupHidden(XMLGroupType type);

        XMLReport ReportXml { get; }

        int ScoreSetId { get; set; }

        ScoringOptions ScoringOptions { get; set; }

        List<int> GetLastSelectedNodeIds(CustomerInfo custInfo);

        string GetLastSelectedNodeType(CustomerInfo custInfo);

        List<string> GetSelectedValuesOf(XMLGroupType type);

        string GetSelectedValuesStringOf(XMLGroupType type);

        int TestAdminValue { get; }

        XMLDataFilteringOptions XmlDataFilteringOptions { get; }

        XMLDisplayOption XmlDisplayOption { get; }

        XMLReportType XmlDisplayType { get; }
        string GetSelectedAltValuesStringOf(XMLGroupType type);
        OptionGroup GetGroupByType(XMLGroupType type);
        int IncludeColumnZ { get; }
    }

    //TODO:  Clean up this class.  All these typed properties need to be revisited.
    public class OptionPage : IOptionPage
    {
        private readonly List<OptionGroup> _groups;
        private readonly string _xmlPath;

        public OptionPage(string xmlPath) : this(xmlPath, new List<OptionGroup>())
        {
        }

        public OptionPage(string xmlPath, List<OptionGroup> groups)
        {
            _xmlPath = xmlPath;
            _groups = groups;
        }

        public bool IsMultimeasure => GetSelectedValuesOf(XMLGroupType.DisplayType).Contains(XMLReportType.MSR.ToString());
        public bool RunInForeground => XmlDisplayOption.runInForeground;
        public Assessment Assessment { get; set; }
        public int AssessmentValue => Assessment.TestFamilyGroupId;
        public XMLProductCodeEnum AssessmentCode => (XMLProductCodeEnum)Enum.Parse(typeof(XMLProductCodeEnum), Assessment.TestFamilyGroupCode);
        public int TestAdminValue => int.Parse(GetSelectedValuesOf(XMLGroupType.TestAdministrationDate).FirstOrDefault());
        public ScoringOptions ScoringOptions { get; set; }
        public object GradeLevel { get; set; }
        public object Grades { get; set; }
        public string LevelOfAnalysisString => GetSelectedValuesOf(XMLGroupType.LevelofAnalysis).FirstOrDefault();
        public XMLLevelOfAnalysisType LevelOfAnalysisValue { get { Enum.TryParse(LevelOfAnalysisString, out XMLLevelOfAnalysisType result); return result; } }
        public string DisplayOptionValue => GetSelectedValuesOf(XMLGroupType.DisplayOptions).FirstOrDefault();
        public int ScoreSetId { get; set; }
        public object TestAdministrations { get; set; }
        public TestAdmin SelectedTestAdmin => ((List<TestAdmin>)TestAdministrations).FirstOrDefault(ta => ta.Id.ToString() == GetSelectedValuesStringOf(XMLGroupType.TestAdministrationDate));

        public bool IsCovidTestAdmin => SelectedTestAdmin.AllowCovidReportFlag;
        public bool IsCovidReport => IsCovidTestAdmin && (IsValueSelected(XMLGroupType.DisplayType, "EGSR") || IsValueSelected(XMLGroupType.DisplayOptions, "IEGSSA"));

        public object SkillSet { get; set; }
        public string SkillSetValue => GetSelectedValuesOf(XMLGroupType.SkillDomainClassification).Count == 0 ?
            ScoringOptions.SkillSetId.ToString() :
            GetSelectedValuesOf(XMLGroupType.SkillDomainClassification).First();

        public List<ContentScope> ContentScopeList { get; set; }
        public string ContentScopeAcronyms { get { return ContentScopeList == null ? null : string.Join(",", ContentScopeList.Select(e => e.Acronym)); } }

        // public object SubtestFamliy { get; set; }
        public string MultimeasureForms { get; set; }
        public object Student { get; set; }

        public int ExcludeMathComputation
        {
            get
            {
                var options = GetOptionsOfGroup<CheckboxOption>(XMLGroupType.CompositeCalculationOptions);

                if (options == null && Assessment.IsIss)
                    return 0;

                if (options == null)
                    return 1;

                return options[0].IsSelected ? 0 : 1;
            }
        }

        public int ExcludeEla
        {
            get
            {
                var options = GetOptionsOfGroup<CheckboxOption>(XMLGroupType.CompositeCalculationOptions);

                if (options == null)
                    return 1;

                return options[1].IsSelected ? 0 : 1;
            }
        }

        public int IncludeColumnZ
        {
            get
            {
                var options = GetOptionsOfGroup<CheckboxOption>(XMLGroupType.ColumnZ);
                return !options[0].IsSelected ? 1 : 0;
            }
        }

        //Locations
        public LocationGroup LastSelectedLocationGroup
        {
            get
            {
                return Locations.LastOrDefault(g => g.LocationNodeType != "STUDENT" && g.SelectedValues.Count > 0 && g.SelectedValues.First() != "-1");
            }
        }
        public string GetLastSelectedNodeType(CustomerInfo custInfo)
        {
            return LastSelectedLocationGroup == null ? custInfo.NodeType : LastSelectedLocationGroup.LocationNodeType;
        }
        public int GetLastSelectedNodeId(CustomerInfo custInfo)
        {
            return LastSelectedLocationGroup == null ? Convert.ToInt32((custInfo.NodeId)) : Convert.ToInt32(LastSelectedLocationGroup.SelectedValues.First());
        }
        public List<int> GetLastSelectedNodeIds(CustomerInfo custInfo)
        {
            if (LastSelectedLocationGroup == null) return new List<int> { Convert.ToInt32((custInfo.NodeId))};
            return LastSelectedLocationGroup.SelectedValues.ConvertAll(int.Parse);
        }
        public string GetLastSelectedNodeIdsString(CustomerInfo custInfo)
        {
            return string.Join(",", GetLastSelectedNodeIds(custInfo));
        }

        // ----- XML Elements ----- //
        private XMLReport _reportXml;
        public XMLReport ReportXml => _reportXml ?? (_reportXml = XmlLoader.GetInstance(_xmlPath).GetReport(AssessmentCode, XmlDisplayType));

        public XMLAnalysisType XmlAnalysisType => XmlLoader.GetInstance(_xmlPath).GetAnalysisType(ReportXml, LevelOfAnalysisValue);
        public XMLReportType XmlDisplayType => XMLReport.ParseType(GetSelectedValuesOf(XMLGroupType.DisplayType).First());
        public XMLDisplayOption XmlDisplayOption => XmlLoader.GetInstance(_xmlPath).GetDisplayOption(XmlAnalysisType, DisplayOptionValue);
        public XMLDataFilteringOptions XmlDataFilteringOptions => XmlDisplayOption.DataFilteringOptions;

        public OptionGroup InvalidGroup { get; set; }

        public OptionGroup GetGroupByType(XMLGroupType type)
        {
            return _groups.FirstOrDefault(g => g.Type == type);
        }

        public List<OptionGroup> GetAllGroups()
        {
            return _groups;
        }

        public List<OptionGroup> GetGroupsOfCategory(OptionsCategory category)
        {
            return _groups.Where(g => g.Category == category).ToList();
        }

        public List<LocationGroup> Locations => GetGroupsOfCategory(OptionsCategory.Locations).Cast<LocationGroup>().ToList();

        public void AddGroup(OptionGroup group)
        {
            _groups.Add(group);
        }

        public void RemoveGroup(OptionGroup group)
        {
            _groups.Remove(group);
        }

        public void RemoveGroups(List<OptionGroup> groups)
        {
            foreach (var group in groups)
                _groups.Remove(group);
        }

        public bool GroupExists(XMLGroupType type)
        {
            return _groups.Exists(g => g.Type == type);
        }

        public bool IsGroupHidden(XMLGroupType type)
        {
            return _groups.Exists(g => g.Type == type) && GetGroupByType(type).IsHidden;
        }

        public bool GroupHasSelectedOptions(XMLGroupType type)
        {
            return _groups.Exists(g => g.Type == type) && GetGroupByType(type).HasSelectedValues;
        }

        public List<string> GetSelectedValuesOf(XMLGroupType type)
        {
            var group = GetGroupByType(type);
            if (group == null)
                return new List<string>();

            return group.SelectedValues;
        }

        public List<string> GetSelectedValuesOfNonHiddenGroups(XMLGroupType type)
        {
            var group = GetGroupByType(type);
            if (group == null || group.IsHidden)
                return new List<string>();

            return group.SelectedValues;
        }

        public List<string> GetSelectedAltValuesOf(XMLGroupType type)
        {
            var group = GetGroupByType(type);
            if (group == null)
                return new List<string>();

            return group.SelectedAltValues;
        }

        public bool IsValueSelected(XMLGroupType type, object value)
        {
            var group = GetGroupByType(type);
            if (group == null)
                return false;

            return group.IsValueSelected(value);
        }

        public string GetFirstSelectionTextOf(XMLGroupType type)
        {
            var group = GetGroupByType(type);
            if (group == null)
                return "";

            return group.FirstSelectionText;
        }

        public List<string> GetHiddenValuesOf(XMLGroupType type)
        {
            var group = GetGroupByType(type);
            if (group == null)
                return new List<string>();

            return group.HiddenValues;
        }

        public string GetSelectedValuesStringOf(XMLGroupType type)
        {
            return string.Join(",", GetSelectedValuesOf(type));
        }

        public string GetSelectedAltValuesStringOf(XMLGroupType type)
        {
            return string.Join(",", GetSelectedAltValuesOf(type));
        }

        public List<T> GetOptionsOfGroup<T>(XMLGroupType type)
        {
            var group = GetGroupByType(type);
            return group?.Options.Cast<T>().ToList();
        }
    }
}