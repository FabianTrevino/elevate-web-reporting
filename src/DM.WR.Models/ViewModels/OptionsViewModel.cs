using DM.WR.Models.Options;
using System.Collections.Generic;
using System.Linq;

namespace DM.WR.Models.ViewModels
{
    public class OptionsViewModel
    {
        public List<OptionGroup> Groups { get; set; }

        public List<OptionGroup> ShownGroups { get { return Groups.Where(g => !g.IsHidden).ToList(); } }
        public List<OptionGroup> HiddenGroups { get { return Groups.Where(g => g.IsHidden).ToList(); } }

        public List<OptionGroup> PrimaryGroups { get { return ShownGroups.Where(g => g.Category == OptionsCategory.Primary).ToList(); } }
        public List<OptionGroup> PrimaryGroupsLeft => PrimaryGroups.GetRange(0, PrimaryLeftRowCount);
        public List<OptionGroup> PrimaryGroupsRight => PrimaryGroups.GetRange(PrimaryLeftRowCount, PrimaryGroups.Count - PrimaryLeftRowCount);

        public List<OptionGroup> SecondaryAndLocationsGroups { get { return ShownGroups.Where(g => g.Category == OptionsCategory.Secondary || g.Category == OptionsCategory.Locations).ToList(); } }
        public List<OptionGroup> SecondaryAndLocationsGroupsLeft => SecondaryAndLocationsGroups.GetRange(0, SecondaryAndLocationsLeftRowCount);
        public List<OptionGroup> SecondaryAndLocationsGroupsRight => SecondaryAndLocationsGroups.GetRange(SecondaryAndLocationsLeftRowCount, SecondaryAndLocationsGroups.Count - SecondaryAndLocationsLeftRowCount);

        public string LocationName { get; set; }
        public bool RunInForeground { get; set; }
        public bool IsMultimeasure { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public OptionGroup InvalidGroup { get; set; }

        private int PrimaryLeftRowCount => RowCount(OptionsCategory.Primary);
        private int SecondaryAndLocationsLeftRowCount { get { return RowCount(ShownGroups.Count(g => g.Category == OptionsCategory.Secondary || g.Category == OptionsCategory.Locations)); } }
        private int RowCount(OptionsCategory category)
        {
            var numGroupsInCategory = ShownGroups.Count(g => g.Category == category);
            return RowCount(numGroupsInCategory);
        }
        private int RowCount(int numGroupsInCategory)
        {
            int half = numGroupsInCategory / 2;
            var modulus = numGroupsInCategory % 2;
            return half + modulus;
        }
    }
}