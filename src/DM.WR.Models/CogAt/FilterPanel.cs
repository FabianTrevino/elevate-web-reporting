using DM.WR.Data.Repository.Types;
using DM.WR.Models.Types;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DM.WR.Models.CogAt
{
    public class FilterPanel
    {
        private readonly List<Filter> _filters;

        public FilterPanel()
        {
            _filters = new List<Filter>();
        }

        public Assessment Assessment { get; set; }
        public int AssessmentValue => Assessment.TestFamilyGroupId;
        public string AssessmentCode => Assessment.TestFamilyGroupCode;

        public int TestAdminId => FilterExists(FilterType.TestEvent) ? int.Parse(GetSelectedAltValuesOf(FilterType.TestEvent).First()) : -1;
        public List<TestAdmin> TestAdmins { get; set; }
        public TestAdmin SelectedTestAdmin => TestAdmins.FirstOrDefault(ta => $"{ta.Id}_{ta.NodeId}" == GetSelectedValuesStringOf(FilterType.TestEvent));
        public ScoringOptions ScoringOptions { get; set; }
        public int ScoreSetId { get; set; }
        public string GradeId => FilterExists(FilterType.Grade) ? GetSelectedAltValuesStringOf(FilterType.Grade) : "";
        public string Grade => FilterExists(FilterType.Grade) ? GetFirstSelectionTextOf(FilterType.Grade).Split('/')[0].ToLower().Replace("grade", "").Trim().Replace("k", "0") : "";
        public int GradeLevel => FilterExists(FilterType.Grade) ? int.Parse(GetSelectedValuesStringOf(FilterType.Grade)) : -1;
        public List<int> GroupId => FilterExists(FilterType.Grade) ? GetSlelectedTestGroupIds(FilterType.Grade) : new List<int>();

        public List<string> DistrictIds => FilterExists(FilterType.Building) ? GetSlelectedDistrictIds(FilterType.Grade) : new List<string>();

        public object SkillSet { get; set; }

        public List<string> ContentIds => FilterExists(FilterType.Content) ? GetSelectedAltValuesOf(FilterType.Content) : null;

        public bool isRetry { get; set; }
        public List<string> Content => FilterExists(FilterType.Content) ? GetSelectedValuesOf(FilterType.Content) : null;

        public string Battery => GetSelectedAltValuesStringOf(FilterType.Grade);

        public List<ScoreWarning> ScoreWarnings { get; set; }

        public Filter FirstLocationFilter => _filters.FirstOrDefault(f => f.IsLocation);
        public Filter LastLocationFilter => _filters.LastOrDefault(f => f.IsLocation);

        public string PopulationGenderValue => GetPopulationFiltersValue(PileKey.GenderList);
        public string PopulationEthnicityValue => GetPopulationFiltersValue(PileKey.EthnicityList);
        public string PopulationProgramValue => GetPopulationFiltersValue(PileKey.ProgramList);
        public string PopulationAdminValue => GetPopulationFiltersValue(PileKey.AdminValueList);
        public string PopulationOfficeUseValue => GetPopulationFiltersValue(PileKey.OfficeUseList);
        public string PopulationTestAdminCodeValue => GetPopulationFiltersValue(PileKey.TestAdminCodeList);


        public Dictionary<string, string> PopulationList => GetPopulationFiltersValueAsList(PileKey.AdminValueList);




        public List<Filter> LocationFilters => _filters.Any(f => f.IsLocation) ? _filters.FindAll(f => f.IsLocation) : new List<Filter>();
        public bool HasStudentsFilter => _filters.Any(f => f.Type == FilterType.Student);

        public Filter GetFilterByType(FilterType type)
        {
            return _filters.FirstOrDefault(g => g.Type == type);
        }

        public List<Filter> GetAllFilters()
        {
            return _filters;
        }

        public void AddFilter(Filter filter)
        {
            _filters.Add(filter);
        }

        public void RemoveLocationFilters()
        {
            _filters.RemoveAll(f => f.IsLocation);
        }

        public bool FilterExists(FilterType type)
        {
            return _filters.Exists(g => g.Type == type);
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
        public List<int> GetSlelectedTestGroupIds(FilterType type)
        {
            var group = GetFilterByType(type);
            if (group == null)
                return new List<int>();

            return group.SelectedTestGroupIds;
        }
        public List<string> GetSlelectedDistrictIds(FilterType type)
        {
            var group = GetFilterByType(type);
            if (group == null)
                return new List<string>();

            return group.SelectedDistrictIds;
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

        private string GetPopulationFiltersValue(string pileKey)
        {
            if (!FilterExists(FilterType.PopulationFilters))
                return "";

            var selectedItem = GetFilterByType(FilterType.PopulationFilters).Items.Cast<PiledFilterItem>().FirstOrDefault(i => i.PileKey == pileKey && i.IsSelected);

            return selectedItem != null ? selectedItem.Value : "";
        }

        private Dictionary<string,string> GetPopulationFiltersValueAsList(string pileKey)
        {
            Dictionary<string, string> vs = new Dictionary<string, string> { };

            if (!FilterExists(FilterType.PopulationFilters))
                return vs;
            //var items = filterPanel.GetFilterByType(FilterType.PopulationFilters).Items.Where(i => i.IsSelected).Cast<PiledFilterItem>().GroupBy(i => i.PileKey);

            var selectedItem = GetFilterByType(FilterType.PopulationFilters).Items.Where(i => i.IsSelected).Cast<PiledFilterItem>().GroupBy(i => pileKey);

            foreach (var item in selectedItem)
            {
                foreach (var popitem in item)
                {
                    var si = popitem.Value;
                    var st = popitem.Text;
                    vs.Add(si,st);
                }
            }
            return selectedItem != null ? vs : vs;
        }
    }
}