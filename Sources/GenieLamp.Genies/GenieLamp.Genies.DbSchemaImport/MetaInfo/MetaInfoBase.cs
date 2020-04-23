using System;
using System.Xml;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoBase
	{
		public MetaInfoBase()
		{
			this.Generate = true;
		}
		
		public string PersistentSchema { get; set; }
		public string PersistentName { get; set; }
		public string Schema { get; set; }
		public string Name { get; set; }
		public XmlNode Node { get; set; }
		public bool Generate { get; set; }

		public string FullPersistentName
		{
			get { return MakeFullName(PersistentSchema, PersistentName); }
		}

		public string FullName
		{
			get { return MakeFullName(Schema, Name); }
		}

		public static string MakeFullName(string schema, string name)
		{
			return String.Format("{0}{1}", String.IsNullOrEmpty(schema) ? "" : schema + ".", name);
		}

	}
}

