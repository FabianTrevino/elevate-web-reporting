using System.Xml.Serialization;

namespace DM.WR.Models.Options
{
    public class Option
    {
        public string Text { get; set; }
        public string Value { get; set;}
        public string AltValue { get; set; }
        public bool IsSelected { get; set;}
        [XmlIgnore] public bool IsHidden { get; set; }
        [XmlIgnore] public bool IsDisabled { get; set; }        
    }
}