using System;
using System.Collections.Generic;
using IRMWeb.BL;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Linq;

namespace IRMWeb.Models
{

    [Serializable]
    public class ReportOptionGroup
    {
        public ReportOptionGroup()
            : this(XMLReportOptionGroupType._INTERNAL_LAST_)
        {
        }

        public ReportOptionGroup(string type, bool isChanged = false)
            : this((XMLReportOptionGroupType)Enum.Parse(typeof(XMLReportOptionGroupType), type))
        {
        }
        public ReportOptionGroup(XMLReportOptionGroupType type, bool isChanged = false)
        {
            this.TypeCode = (int)type;
            this.IsChanged = isChanged;
        }

        private string _Id; public string Id { get { return _Id; } set { _Id = value; } }
        private string _Category; public string Category { get { return _Category; } set { _Category = value; } }
        private string _Name; public string Name { get { return _Name; } set { _Name = value; } }
        
        [DefaultValue(null)]
        private string _TypeName;

        [NonSerialized]
        private int _TypeCode;
        public int TypeCode { get { return _TypeCode; } set { _TypeCode = value; } }

        public XMLReportOptionGroupType TypeEnum { get { return (XMLReportOptionGroupType)TypeCode; } }
        [NonSerialized]private bool _IsChanged; public bool IsChanged { get { return _IsChanged; } set { _IsChanged = value; } }
        [NonSerialized]private string _DisplayName; public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; } }
        [NonSerialized]private int _MinToSelect = -1; public int MinToSelect { get { return _MinToSelect; } set { _MinToSelect = value; } }
        [NonSerialized]private int _MaxToSelect = -1; public int MaxToSelect { get { return _MaxToSelect; } set { _MaxToSelect = value; } }
        [NonSerialized]private bool _HasSelectAll; public bool HasSelectAll { get { return _HasSelectAll; } set { _HasSelectAll = value; } }
        [NonSerialized]private bool _HasSelectNone; public bool HasSelectNone { get { return _HasSelectNone; } set { _HasSelectNone = value; } }
        [NonSerialized]private bool _IsHidden; public bool IsHidden { get { return _IsHidden; } set { _IsHidden = value; } }
        [NonSerialized]private bool _IsDisabled; public bool IsDisabled { get { return _IsDisabled; } set { _IsDisabled = value; } }
        [NonSerialized]private bool _HasSubGroups; public bool HasSubGroups { get { return _HasSubGroups; } set { _HasSubGroups = value; } }
        [NonSerialized]private object _OriginalObject; public object OriginalObject { get { return _OriginalObject; } set { _OriginalObject = value; } }
        [NonSerialized]private object _CachedValue; public object CachedValue { get { return _CachedValue; } set { _CachedValue = value; } }
        private List<ReportOption> _Options; public List<ReportOption> Options { get { return _Options; } set { _Options = value; } }
        public List<ReportOption> SelectedOptions { get { return Options.FindAll(e => e.IsSelected); } }

        public ReportOptionGroup Clone(bool selectedOptionsOnly = false)
        {
            ReportOptionGroup result = new ReportOptionGroup(this.TypeEnum);
            result.Id = this.Id;
            result.Category = this.Category;
            result.Name = this.Name;
            result.TypeCode = this.TypeCode;
            result.DisplayName = this.DisplayName;
            result.MinToSelect = this.MinToSelect;
            result.MaxToSelect = this.MaxToSelect;
            result.HasSelectAll = this.HasSelectAll;
            result.HasSelectNone = this.HasSelectNone;
            result.IsHidden = this.IsHidden;
            result.IsDisabled = this.IsDisabled;
            result.HasSubGroups = this.HasSubGroups;
            result.OriginalObject = this.OriginalObject;
            result.CachedValue = this.CachedValue;
            result.Options = new List<ReportOption>();
            this.Options.ForEach(e => { if (!selectedOptionsOnly || e.IsSelected) result.Options.Add(e.Clone(false)); });
            return result;
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            this._TypeName = ((XMLReportOptionGroupType)this._TypeCode).ToString();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            this.TypeCode = (int)(XMLReportOptionGroupType)Enum.Parse(typeof(XMLReportOptionGroupType), this._TypeName);
        }

        public string GetSelectedOptionValue()
        {
            if (this.SelectedOptions.Count > 0)
            {
                return this.SelectedOptions[0].Value;
            }
            return "";
        }

        public string GetSelectedOptionId()
        {
            if (this.SelectedOptions.Count > 0)
            {
                return this.SelectedOptions[0].Id;
            }
            return "";
        }

        public List<string> GetSelectedOptionIds()
        {
            if (this.SelectedOptions.Count > 0)
            {
                return this.SelectedOptions.Select(o => o.Id).ToList();
            }
            return new List<string>();
        }

        public  List<string> GetSelectedOptionValues()
        {
            if (this.SelectedOptions.Count > 0)
            {
                return this.SelectedOptions.Select(o => o.Value).ToList();
            }
            return new List<string>();
        }

        public string GetSelectedOtherValue()
        {
            if (this.SelectedOptions.Count > 0)
            {
                return this.SelectedOptions[0].Other;
            }
            return "";
        }

    }
}