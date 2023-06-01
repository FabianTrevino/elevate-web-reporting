using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace IRMWeb.Models
{
    [Serializable]
    [DataContract]
    public class ReportOption
    {
        [DataMember(Name="_Id")]public string Id { get; set;} 
        [DataMember(Name="_Type")]public string Type { get; set;}
        [DataMember(Name="_Value")]public string Value { get; set;}
        [DataMember(Name="_Other")]public string Other { get; set;}
        [DataMember(Name="_IsSelected")]public bool IsSelected { get; set;}
        public bool IsHidden { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }
        [DataMember]public string Label { get; set; }
        public bool BreaksBusinessLogic { get; set; }
        [DataMember(Name="_OptionGroup")]public ReportOptionGroup OptionGroup { get; set; }

        public ReportOption Clone(bool selectedOptionsOnly = false)
        {
            ReportOption result =  new ReportOption();
            result.Id = this.Id;
            result.Type = this.Type;
            result.Name = this.Name;
            result.Value = this.Value;
            result.IsSelected = this.IsSelected;
            result.IsHidden = this.IsHidden;
            result.IsDisabled = this.IsDisabled;
            result.Label = this.Label;
            result.Other = this.Other;
            result.OptionGroup = this.OptionGroup == null ? null : this.OptionGroup.Clone(selectedOptionsOnly);
            return result;
        }
    }
}