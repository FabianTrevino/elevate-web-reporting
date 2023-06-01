using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using IRMWeb.Models;
using IRM_Library.Objects;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Diagnostics;

namespace IRMWeb
{
    public static class SerializationManager
    {
        private static readonly Type[] XML_SERIALIZER_TYPES = new Type[]
        {
            typeof(SerializationContainer),
            typeof(object),
            typeof(object[]),
            typeof(ReportOptionGroup),
            typeof(ReportOption),
            typeof(Assessment),
            typeof(ContentArea),
            typeof(List<ContentArea>),
            //typeof(ContentArea[]),
            typeof(SubContentArea),
            typeof(SubContentArea[]),
            typeof(CustomerScoringOptions),
            typeof(GradeLevel),
            typeof(SkillSet),
            typeof(Student),
        };

        [Serializable]
        public class SerializationContainer
        {
            public int BuildNumber;
            public List<List<ReportOptionGroup>> Groups;
        }

        private static List<List<ReportOptionGroup>> SelectedOnly(List<List<ReportOptionGroup>> groups)
        {
            List<List<ReportOptionGroup>> result = new List<List<ReportOptionGroup>>();
            foreach (List<ReportOptionGroup> oldList in groups)
            {
                var newList = new List<ReportOptionGroup>();
                result.Add(newList);
                oldList.ForEach(group => newList.Add(group.Clone(true)));                
            }
            return result;
        }

        private static int GetBuildNumber()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            //int buildNum = assembly.GetName().Version.Build;
            int buildNumF = fvi.FileBuildPart;
            return buildNumF;
        }

        public static string ReportOptionGroups_To_XML(List<List<ReportOptionGroup>> groups, bool selectedOptionsOnly)
        {
            if (selectedOptionsOnly) groups = SelectedOnly(groups);
            DataContractSerializer serializer = new DataContractSerializer(typeof(SerializationContainer), XML_SERIALIZER_TYPES);
            var output = new StringWriter();
            using (var writer = new System.Xml.XmlTextWriter(output) { Formatting = System.Xml.Formatting.Indented })
            {
                serializer.WriteObject(writer, new SerializationContainer() { BuildNumber = GetBuildNumber(), Groups = groups });
                writer.Flush();
                output.Flush();
            }
            return output.GetStringBuilder().ToString();
        }

        public static List<List<ReportOptionGroup>> XML_To_ReportOptionGroups(String xml)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(SerializationContainer), XML_SERIALIZER_TYPES);
            var input = new StringReader(xml);
            using (var reader = new System.Xml.XmlTextReader(input))
            {
                SerializationContainer sc = (SerializationContainer)serializer.ReadObject(reader);
                return sc.Groups;
            }
        }

        public static byte[] ReportOptionGroups_To_Bytes(List<List<ReportOptionGroup>> groups, bool selectedOptionsOnly)
        {
            if (selectedOptionsOnly) groups = SelectedOnly(groups);
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, new SerializationContainer() { BuildNumber = GetBuildNumber(), Groups = groups });
                return stream.ToArray();
            }
        }

        public static List<List<ReportOptionGroup>> Bytes_To_ReportOptionGroups(byte[] bytes)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                SerializationContainer sc = (SerializationContainer)formatter.Deserialize(stream);
                return sc.Groups;
            }
        }
    }
}