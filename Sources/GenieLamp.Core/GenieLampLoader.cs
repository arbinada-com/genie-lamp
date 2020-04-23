using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core
{
	class GenieLampLoader
	{
		private GenieLamp lamp;
		private string xmlNamespace = String.Empty;
		private bool validationErrorOccurred = false;
		
        public string XmlNamespace
		{
			get { return xmlNamespace; }
		}

		
		public GenieLampLoader(GenieLamp lamp)
		{
			this.lamp = lamp;
		}
		
		public XmlDocument LoadFile(string fileName, string namespaceName, string xmlSchemaResourceName)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreWhitespace = true;
            settings.ValidationType = ValidationType.Schema;
			settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
			
			Assembly assembly = Assembly.GetExecutingAssembly();
			/*XmlReaderSettings schemaSettings = new XmlReaderSettings();
			schemaSettings.IgnoreWhitespace = true;
			schemaSettings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);*/
			ClearValidationErrors();
			using (Stream stream = assembly.GetManifestResourceStream(xmlSchemaResourceName))
			using (XmlReader reader = XmlReader.Create(stream/*, schemaSettings*/))
			{
				XmlSchema schema = XmlSchema.Read(
			        reader, 
			        new ValidationEventHandler(ValidationEventHandler));
	            settings.Schemas.Add(schema);
				
				xmlNamespace = String.Empty;
				foreach(XmlQualifiedName xmlQName in schema.Namespaces.ToArray())
				{
					if (xmlQName.Name.Equals(namespaceName))
					{
						xmlNamespace = xmlQName.Namespace;
						break;
					}
				}
				if (xmlNamespace.Equals(String.Empty))
					throw new GlException("Namespace reference \"{0}\" not found in file {1}", namespaceName, fileName);
			}
			if (ValidationErrorOccurred)
				throw new GlException("Error validating schema \"{0}\" ({1})", namespaceName, xmlNamespace);
			
            XmlDocument document = new XmlDocument();
			ClearValidationErrors();
			using(XmlReader reader = XmlReader.Create(fileName, settings))
			{
            	document.Load(reader);
			}
			if (ValidationErrorOccurred)
				throw new GlException("Error validating file: {0}", fileName);
			return document;
		}
		
		private void ClearValidationErrors()
		{
			validationErrorOccurred = false;
		}
		
		private bool ValidationErrorOccurred
		{
			get { return validationErrorOccurred; }
		}
		
		
        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
			if (sender != null)
			{
				IXmlLineInfo info = (sender as XmlReader) as IXmlLineInfo;
	            lamp.Logger.TraceLine("{0} (Ln {1}, Pos {2}): {3}", e.Severity.ToString(), info.LineNumber, info.LinePosition, e.Message);
			}
			else
			{
	            lamp.Logger.TraceLine("{0}: {1}", e.Severity.ToString(), e.Message);
			}
			validationErrorOccurred = e.Severity == XmlSeverityType.Error;
//            switch (e.Severity)
//            {
//                case XmlSeverityType.Error:
//                    logger.TraceLine("Error ({0}, {1}): {2}", e.Severity.ToString(), info.LineNumber, info.LinePosition, e.Message);
//                    break;
//                case XmlSeverityType.Warning:
//                    logger.TraceLine("Warning {0}", e.Message);
//                    break;
//            }

        }
	}
}

