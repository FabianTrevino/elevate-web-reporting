using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace DM.WR.Models.Xml
{
    public class XmlLoader
    {
        private static XmlLoader _instance;
        private static readonly object SyncObj = new object();
        private readonly IRMConfig _meta;

        public static XmlLoader GetInstance(string xmlPath)
        {
            if (_instance == null)
            {
                lock (SyncObj)
                {
                    if (_instance == null)
                    {
                        _instance = new XmlLoader(xmlPath);
                    }
                }
            }
            return _instance;
        }

        private XmlLoader(string xmlPath)
        {
            // Read Configuration
            string xmlFile = HttpContext.Current.Request.MapPath(xmlPath);
            if (!File.Exists(xmlFile))
            {
                throw new Exception($"{xmlFile} configuration file could not be found.");
            }

            XmlValidator.Validate(xmlFile);

            // Read The Dictionary File
            using (StreamReader objStreamReader = new StreamReader(xmlFile))
            {
                XmlSerializer x = new XmlSerializer(typeof(IRMConfig));

                //Use pre-generated serializer
                //XmlSerializer x = (XmlSerializer)Activator.CreateInstance("IRMWeb.XmlSerializers", "Microsoft.Xml.Serialization.GeneratedAssembly.IRMConfigSerializer").Unwrap();

                Object o = x.Deserialize(objStreamReader);
                _meta = o as IRMConfig ?? throw new Exception($"Null configuration created from file : {xmlFile}");
            }

            //Set Parent(s) Properties
            {
                foreach(XMLProduct product in _meta.ReportsPerProduct.Product)
                {
                    foreach (XMLReport report in product.Report)
                    {
                        report.Parent = product;
                        report.LevelOfAnalysis.Parent = report;
                        foreach (XMLAnalysisType at in report.LevelOfAnalysis.AnalysisType)
                        {
                            at.Parent = report.LevelOfAnalysis;
                            at.DisplayOptions.Parent = at;
                            foreach(XMLDisplayOption displayOption in at.DisplayOptions.DisplayOption)
                            {
                                displayOption.Parent = at.DisplayOptions;
                                displayOption.DataFilteringOptions.Parent = displayOption;

                                System.Reflection.PropertyInfo[] properties = displayOption.DataFilteringOptions.GetType().GetProperties();
                                foreach(var property in properties)
                                {
                                    SetGroupType(property, displayOption.DataFilteringOptions);
                                }

                                //Dmitriy - 5/16/2018 - if we ever convert all groups to derive from base group, this will be a loop instead
                                //otherwise, just hardcoding for Longitudinal Types group
                                if (displayOption.DataFilteringOptions.LongitudinalOptions?.LongitudinalTypes != null)
                                {
                                    var property = displayOption.DataFilteringOptions.LongitudinalOptions.GetType().GetProperty("LongitudinalTypes");
                                    var obj = displayOption.DataFilteringOptions.LongitudinalOptions;
                                    SetGroupType(property, obj);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetGroupType(System.Reflection.PropertyInfo property, object obj)
        {
            if (!typeof(XMLReportOptionGroup).IsAssignableFrom(property.PropertyType)) return;

            object o = property.GetValue(obj, null);
            if (o == null) return;

            XMLReportOptionGroup group = (XMLReportOptionGroup)o;

            //Substitute reference (if applicable) with a real group
            if (group.keyref != null)
            {
                XMLReportOptionGroup realGroup = _meta.GlobalGroups.FirstOrDefault(e => e.key == group.keyref);
                if (realGroup == null)
                    throw new Exception($"Could not find a global ReportOptionGroup by reference: {group.keyref}");

                if (group.GetType() != realGroup.GetType())
                    throw new Exception($"Wrong Type of the global ReportOptionGroup: {group.keyref}");

                property.SetValue(obj, realGroup, null);

                group = realGroup;
            }

            //Set Group Type
            var propertyInfo = group.GetType().GetProperty("groupType");
            
            if (propertyInfo == null)
                throw new Exception($"Group {group.GetType()} does not have a property groupType specified");
            
            if (propertyInfo.PropertyType != typeof(XMLGroupType))
                throw new Exception($"Group {group.GetType()} property {propertyInfo.PropertyType} type is invalid. Expected XMLGroupType");
            
            group.GroupType = (XMLGroupType)propertyInfo.GetValue(group, null);
        }

        public IRMConfig Root => _meta;

        public XMLReport[] GetReports(XMLProductCodeEnum productCode, string roleId)
        {
            var xmlReports = _meta.ReportsPerProduct.Product.First(e => e.productCode == productCode).Report;

            if(roleId != "-1")
            {
                xmlReports = xmlReports.Where(r => r.accessrole == null || r.accessrole.Split(',').Contains(roleId)).ToArray();
            }

            return xmlReports;
        }

        public XMLAnalysisType GetAnalysisType(XMLReport reportXml, XMLLevelOfAnalysisType selectedAnalysisType)
        {
            return reportXml.LevelOfAnalysis.AnalysisType.Any(el => el.code == selectedAnalysisType) ?
                   reportXml.LevelOfAnalysis.AnalysisType.First(el => el.code == selectedAnalysisType):
                   reportXml.LevelOfAnalysis.AnalysisType.First(el => el.isDefault);
        }

        public XMLDisplayOption GetDisplayOption(XMLAnalysisType analysisTypeXml, string selectedDisplayOption)
        {
            var result = string.IsNullOrEmpty(selectedDisplayOption) ? 
                analysisTypeXml.DisplayOptions.DisplayOption.First(el => el.isDefault) : 
                analysisTypeXml.DisplayOptions.DisplayOption.First(el => el.code == selectedDisplayOption);

            return result ?? analysisTypeXml.DisplayOptions.DisplayOption.First();
        }

        public XMLProduct GetProduct(XMLProductCodeEnum productCodeEnum)
        {
            return _meta.ReportsPerProduct.Product.First(product => product.productCode == productCodeEnum);
        }

        public XMLProduct GetProduct(string productCode)
        {
            var pCode = (XMLProductCodeEnum)Enum.Parse(typeof(XMLProductCodeEnum), productCode);
            return GetProduct(pCode);
        }
    
        public XMLReport GetReport(XMLProductCodeEnum productCode, XMLReportType reportType)
        {
            var productXml = _meta.ReportsPerProduct.Product.First(product => product.productCode == productCode);
            return productXml.Report.FirstOrDefault(report => report.reportType == reportType);
        }
    
        public XMLReport GetReport(string productCode, string reportType)
        {
            var pCode = (XMLProductCodeEnum)Enum.Parse(typeof(XMLProductCodeEnum), productCode);
            var rType = XMLReport.ParseType(reportType);

            return GetReport(pCode, rType);
        }

        public XMLReport GetReportByReportName(XMLProductCodeEnum productCode, string reportName)
        {            
            var productXml = _meta.ReportsPerProduct.Product.First(product => product.productCode == productCode);
            return productXml.Report.FirstOrDefault(report => report.reportName == reportName.Replace("_", " "));
        }

        public XMLInterimPerformanceBands GetInterimPerformanceBands(string score, string content, string grade)
        {
            return _meta.GlobalTypes.InterimPerformanceBands.FirstOrDefault(e => e.score == score && e.content == content && e.grade == grade) ??
                   _meta.GlobalTypes.InterimPerformanceBands.FirstOrDefault(e => e.score == score && e.content == content) ??
                   _meta.GlobalTypes.InterimPerformanceBands.FirstOrDefault(e => e.score == "ALL");
        }

        public string GetDisplayText(XMLGroupType groupType)
        {
            var displayTextNode = _meta.GlobalTypes.DisplayTextMap.FirstOrDefault(n => n.GroupType == groupType);
            if(displayTextNode == null)
                throw new Exception($"Display text was not found for Group Type {groupType}.");

            return displayTextNode.Value;
        }
    }
}