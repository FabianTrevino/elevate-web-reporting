using System;
using System.Xml;
using System.Xml.Schema;

namespace DM.WR.Models.Xml
{
    public class XmlValidator
    {
        public static void Validate(string xmlFileName)
        {
            void ValidationEventHandler(object sender, ValidationEventArgs e)
            {
                switch (e.Severity)
                {
                    case XmlSeverityType.Error:
                        throw new Exception(e.Message);
                    case XmlSeverityType.Warning:
                        throw new Exception(e.Message);
                }
            }

            XmlReaderSettings settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationEventHandler += ValidationEventHandler;


            using (var reader = XmlReader.Create(xmlFileName, settings))
            {
                while (reader.Read())
                {
                }
            }
        }
    }
}