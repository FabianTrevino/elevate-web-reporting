using System.Collections.Generic;
using DM.WR.Models.Xml;
using System.Linq;
using System.Xml.Serialization;

namespace DM.WR.Models.Options
{
    public class CustomFieldGroup : OptionGroup
    {
        public CustomFieldGroup() : base(XMLGroupType.CustomDataFields) 
        { 
            SelectedValuesOrder = new List<string>();
        }


        [XmlIgnore] public string Separator { get; set; }
        [XmlIgnore] public string Delimiter { get; set; }
        [XmlIgnore] public int UserTextLength { get; set; }


        public List<string> SelectedValuesOrder { get; set; }

        public string SelectionText
        {
            get
            {
                var groupingTexts = Options.Cast<CustomFieldOption>().ToList().Where(o => o.IsSelected).Select(o => o.GroupingText).Distinct().ToList();

                if (!groupingTexts.Any())
                    return "None selected";

                return string.Join(" ", groupingTexts);
            }
        }
    }
}