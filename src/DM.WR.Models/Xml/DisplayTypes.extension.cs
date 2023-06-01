// ReSharper disable InconsistentNaming
using System;

namespace DM.WR.Models.Xml
{
    public partial class XMLReportOptionGroup
    {
        public XMLGroupType GroupType { get; set; }
    }
    public partial class XMLReport { public XMLProduct Parent; }
    public partial class XMLReport { public static XMLReportType ParseType(string strReportType) { return (XMLReportType)Enum.Parse(typeof(XMLReportType), strReportType); } }
    public partial class XMLLevelOfAnalysis { public XMLReport Parent; }
    public partial class XMLAnalysisType { public XMLLevelOfAnalysis Parent; }
    public partial class XMLDisplayOptions { public XMLAnalysisType Parent; }
    public partial class XMLDisplayOption { public XMLDisplayOptions Parent; }
    public partial class XMLDataFilteringOptions
    {
        public XMLDisplayOption Parent;
        public XMLReportOptionGroup GetSpecificChild(XMLGroupType groupType)
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.GetValue(this, null) is XMLReportOptionGroup xmlGroup)

                    // var xmlGroup = (XMLReportOptionGroup) prop.GetValue(this, null);
                    if (xmlGroup?.GroupType == groupType)
                        return xmlGroup;
            }

            return null;
        }
    }
    public partial class XMLInterimPerformanceBands
    {
        public new string Key => $"{score}{content}{grade}";
    }
    public partial class XMLPerformanceBands { public string Key => "default"; }
}