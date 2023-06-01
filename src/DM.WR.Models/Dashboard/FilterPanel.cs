using System;
using System.Collections.Generic;
using System.Linq;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;

namespace DM.WR.Models.Dashboard
{
    public class FilterPanel
    {
        public FilterPanel()
        {
            _filters = new List<Filter>();
        }

        private readonly List<Filter> _filters;

        public Assessment Assessment { get; set; }
        public int AssessmentValue => Assessment.TestFamilyGroupId;
        public XMLProductCodeEnum AssessmentCode => (XMLProductCodeEnum)Enum.Parse(typeof(XMLProductCodeEnum), Assessment.TestFamilyGroupCode);

        public int TestAdminValue => int.Parse(GetSelectedValuesOf(FilterType.TestAdministrationDate).FirstOrDefault());
        public object TestAdministrations { get; set; }
        public ScoringOptions ScoringOptions { get; set; }
        public int ScoreSetId { get; set; }

        public string GradeId => GroupExists(FilterType.GradeLevel) ? GetSelectedAltValuesStringOf(FilterType.GradeLevel) : "";
        public string Grade => GroupExists(FilterType.GradeLevel) ? GetFirstSelectionTextOf(FilterType.GradeLevel).Split('/')[0].ToLower().Replace("grade", "").Trim().Replace("k", "0") : "";
        public string Level => GroupExists(FilterType.GradeLevel) ? GetSelectedValuesStringOf(FilterType.GradeLevel) : "";
        public object SkillSet { get; set; }

        public List<LocationNode> LocationsPath { get; set; }

        public void AddToLocationsPath(int id, string type, string name)
        {
            AddToLocationsPath(new LocationNode { NodeId = id, NodeType = type, NodeName = name });
        }
        public void AddToLocationsPath(LocationNode node)
        {
            if (LocationsPath == null)
                LocationsPath = new List<LocationNode>();

            LocationsPath.Add(new LocationNode { NodeId = node.NodeId, NodeType = node.NodeType, NodeName = node.NodeName });
        }
        public void SetLocationInLocationsPath(LocationNode node)
        {
            var index = LocationsPath.FindIndex(n => n.NodeId == node.NodeId);

            if (index > -1)
                LocationsPath.RemoveRange(index + 1, LocationsPath.Count - index - 1);
        }
        public int NodeId => LocationsPath != null && LocationsPath.Count > 0 ? LocationsPath.Last().NodeId : -1;
        public string NodeType => LocationsPath != null && LocationsPath.Count > 0 ? LocationsPath.Last().NodeType : "";

        public Filter GetFilterByType(FilterType type)
        {
            return _filters.FirstOrDefault(g => g.Type == type);
        }

        public List<Filter> GetAllFilters()
        {
            return _filters;
        }

        public List<Filter> GetGroupsOfCategory(OptionsCategory category)
        {
            return _filters.Where(g => g.Category == category).ToList();
        }

        public void AddFilter(Filter filter)
        {
            _filters.Add(filter);
        }

        public void RemoveFilter(Filter filter)
        {
            _filters.Remove(filter);
        }

        public void RemoveGroups(List<Filter> groups)
        {
            foreach (var group in groups)
                _filters.Remove(group);
        }

        public bool GroupExists(FilterType type)
        {
            return _filters.Exists(g => g.Type == type);
        }

        public bool GroupHasSelectedOptions(FilterType type)
        {
            return _filters.Exists(g => g.Type == type) && GetFilterByType(type).HasSelectedValues;
        }

        public List<string> GetSelectedValuesOf(FilterType type)
        {
            var group = GetFilterByType(type);
            if (group == null)
                return new List<string>();

            return group.SelectedValues;
        }

        public List<string> GetSelectedAltValuesOf(FilterType type)
        {
            var group = GetFilterByType(type);
            if (group == null)
                return new List<string>();

            return group.SelectedAltValues;
        }

        public bool IsValueSelected(FilterType type, object value)
        {
            var group = GetFilterByType(type);
            if (group == null)
                return false;

            return group.IsValueSelected(value);
        }

        public string GetFirstSelectionTextOf(FilterType type)
        {
            var group = GetFilterByType(type);
            if (group == null)
                return "";

            return group.FirstSelectionText;
        }

        public string GetSelectedValuesStringOf(FilterType type)
        {
            return string.Join(",", GetSelectedValuesOf(type));
        }

        public string GetSelectedAltValuesStringOf(FilterType type)
        {
            return string.Join(",", GetSelectedAltValuesOf(type));
        }
    }
}