using System.Xml.Serialization;

namespace DM.WR.Models.Options
{
    public class CustomFieldOption : Option
    {
        public int Width { get; set; }
        [XmlIgnore] public string GroupingValue { get; set; }
        [XmlIgnore] public string GroupingText { get; set; }
        [XmlIgnore] public int Padding { get; set; }
        [XmlIgnore] public bool ManualSelection { get; set; }
        public int? UserWidth { get; set; }
        public string UserText { get; set; }
    }
}