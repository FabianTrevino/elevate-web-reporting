using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.Models.CogAt.elevate
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ContentScope
    {
        public string acronym { get; set; }
        public string battery { get; set; }
        public string subtestName { get; set; }
        public bool isDefault { get; set; }
    }

    public class Grade
    {
        public int level { get; set; }
        public string name { get; set; }
        public string gradeText { get; set; }
        public string battery { get; set; }
        public List<ContentScope> contentScope { get; set; }
    }

    public class Class
    {
        public int id { get; set; }
        public string nodeType { get; set; }
        public string nodeName { get; set; }
        public string nodeTypeDisplay { get; set; }
    }

    public class Building
    {
        public List<Class> classes { get; set; }
        public int id { get; set; }
        public string nodeType { get; set; }
        public string nodeName { get; set; }
        public string nodeTypeDisplay { get; set; }
    }

    public class District
    {
        public List<Building> buildings { get; set; }
        public int id { get; set; }
        public string nodeType { get; set; }
        public string nodeName { get; set; }
        public string nodeTypeDisplay { get; set; }
    }

    public class FiltersModel
    {
        public string name { get; set; }
        public string date { get; set; }
        public int id { get; set; }
        public int nodeId { get; set; }
        public string nodeType { get; set; }
        public string nodeName { get; set; }
        public string nodeGuid { get; set; }
        public int customerId { get; set; }
        public object userId { get; set; }
        public bool allowCovidReportFlag { get; set; }
        public List<Grade> grades { get; set; }
        public List<District> districts { get; set; }
    }

}
