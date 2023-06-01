using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DM.UI.Library.Models;
using DM.WR.Models.Xml;

namespace DM.WR.Models.Options
{
    public class OptionGroup
    {
        public OptionGroup() : this(XMLGroupType._INTERNAL_LAST_) { }
        public OptionGroup(string type) : this((XMLGroupType)Enum.Parse(typeof(XMLGroupType), type)) { }
        public OptionGroup(XMLGroupType type) { Type = type; }

        public XMLGroupType Type { get; set; }
        [XmlIgnore] public int TypeCode => (int)Type;
        [XmlIgnore] public string DisplayName { get; set; }
        [XmlIgnore] public OptionsCategory Category { get; set; }
        [XmlIgnore] public OptionsInputControl InputControl { get; set; }
        [XmlIgnore] public int MinToSelect { get; set; } = -1;
        [XmlIgnore] public int MaxToSelect { get; set; } = -1;
        [XmlIgnore] public bool HasSelectAll { get; set; }
        [XmlIgnore] public bool HasSelectNone { get; set; }
        [XmlIgnore] public bool IsHidden { get; set; }
        [XmlIgnore] public bool IsDisabled { get; set; }

        public List<Option> Options { get; set; }

        
        [XmlIgnore]
        public bool IsMultiSelect => InputControl == OptionsInputControl.MultiSelect || InputControl == OptionsInputControl.Checkbox;

        public bool IsValueSelected(object value, bool ignoreCase = false)
        {
            var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return SelectedValues.Any(v => string.Equals(v, value.ToString(), stringComparison));
        }

        public bool IsAltValueSelected(object value, bool ignoreCase = false)
        {
            var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return SelectedAltValues.Any(v => string.Equals(v, value.ToString(), stringComparison));
        }
        
        [XmlIgnore]
        public bool HasSelectedValues => Options.Any(o => o.IsSelected);

        [XmlIgnore]
        public List<string> SelectedValues => !HasSelectedValues ? new List<string>() : Options.Where(o => o.IsSelected).Select(o => o.Value).ToList();

        [XmlIgnore]
        public string SelectedValuesString => string.Join(",", SelectedValues);

        [XmlIgnore]
        public string FirstSelectionText => !HasSelectedValues ? "" : Options.First(o => o.IsSelected).Text;

        [XmlIgnore]
        public List<string> SelectedAltValues => !HasSelectedValues ? new List<string>() : Options.Where(o => o.IsSelected).Select(o => o.AltValue).ToList();
        
        [XmlIgnore]
        public List<string> HiddenValues => Options.Exists(o => o.IsHidden) ? Options.Where(o => o.IsHidden).Select(o => o.Value).ToList() : new List<string>();

        [XmlIgnore]
        public bool IsFromSavedCriteria { get; set; }

        [XmlIgnore]
        public bool IsFromIrm40Xml { get; set; }

        public IEnumerable<DropdownItem> GetDropdownList()
        {
            return Options.Select(o => new DropdownItem
            {
                Text = o.Text,
                Value = o.Value,
                Selected = o.IsSelected,
                Disabled = o.IsDisabled,
                Hidden = o.IsHidden
            });
        }
    }
}