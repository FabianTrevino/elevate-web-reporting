using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;
using DM.UI.Library.Models;
using DM.WR.Models.Options;

namespace DM.WR.Models.CogAt
{
    public class Filter
    {
        public Filter() : this(FilterType._INTERNAL_LAST_) { }
        public Filter(string type) : this((FilterType)Enum.Parse(typeof(FilterType), type)) { }
        public Filter(FilterType type) { Type = type; }

        public FilterType Type { get; set; }
        [XmlIgnore] public int TypeCode => (int)Type;
        [XmlIgnore] public string DisplayName { get; set; }
        [XmlIgnore] public OptionsCategory Category { get; set; }
        [XmlIgnore] public OptionsInputControl InputControl { get; set; }
        [XmlIgnore] public int MinToSelect { get; set; } = -1;
        [XmlIgnore] public int MaxToSelect { get; set; } = -1;
        [XmlIgnore] public bool HasSelectAll { get; set; }
        [XmlIgnore] public bool HasSelectNone { get; set; }
        [XmlIgnore] public bool IsLocation { get; set; }
        [XmlIgnore] public bool IsHidden { get; set; }

        public List<FilterItem> Items { get; set; }

        public bool IsValueSelected(object value)
        {
            return SelectedValues.Contains(value.ToString());
        }

        public bool IsAltValueSelected(object value)
        {
            return SelectedAltValues.Contains(value.ToString());
        }

        [XmlIgnore]
        public bool HasSelectedValues
        {
            get
            {
                return Items != null && Items.Any(i => i.IsSelected);
            }
        }

        [XmlIgnore]
        public List<string> SelectedValues
        {
            get
            {
                return !HasSelectedValues ? new List<string>() : Items.Where(i => i.IsSelected).Select(i => i.Value).ToList();
            }
        }

        [XmlIgnore]
        public string FirstSelectionText
        {
            get
            {
                return !HasSelectedValues ? "" : Items.First(i => i.IsSelected).Text;
            }
        }

        [XmlIgnore]
        public List<string> SelectedAltValues
        {
            get
            {
                return !HasSelectedValues ? new List<string>() : Items.Where(i => i.IsSelected).Select(i => i.AltValue).ToList();
            }
        }
        [XmlIgnore]
        public List<int> SelectedTestGroupIds
        {
            get
            {
                return !HasSelectedValues ? new List<int>() : Items.Where(i => i.IsSelected).Select(i => i.TestGroupId).ToList();
            }
        }

        [XmlIgnore]
        public List<string> SelectedDistrictIds
        {
            get
            {
                return !HasSelectedValues ? new List<string>() : Items.Where(i => i.IsSelected).Select(i => i.DistrictIds).ToList();
            }
        }

        [XmlIgnore]
        public List<string> SelectedTexts
        {
            get
            {
                return !HasSelectedValues ? new List<string>() : Items.Where(i => i.IsSelected).Select(i => i.Text).ToList();
            }
        }

        [XmlIgnore]
        public string SelectedValuesString => string.Join(",", SelectedValues);

        [XmlIgnore]
        public string SelectedAltValuesString => string.Join(",", SelectedAltValues);

        public IEnumerable<DropdownItem> GetDropdownList()
        {
            return Items.Select(i => new DropdownItem
            {
                Text = i.Text,
                Value = i.Value,
                AltValue = i.AltValue,
                Selected = i.IsSelected
            });
        }

        public IEnumerable<DropdownItem> GetGroupDropdownList()
        {
            var piledItems = Items.Cast<PiledFilterItem>();

            return piledItems.Select(i => new DropdownItem
            {
                Text = i.Text,
                Value = i.Value,
                AltValue = i.AltValue,
                Selected = i.IsSelected,
                Group = new DropdownGroup { Name = i.PileLabel }
            });
        }

        public IEnumerable<DropdownItem> GetGroupLocationDropdownList()
        {
            var piledItems = Items.Cast<LocationFilterItem>();

            return piledItems.Select(i => new DropdownItem
            {
                Text = i.Text,
                Value = i.Value,
                AltValue = i.AltValue,
                Selected = i.IsSelected,
                Group = new DropdownGroup { GroupId = i.ParentLocationId, Name = i.ParentLocationName }
            });
        }
    }
}