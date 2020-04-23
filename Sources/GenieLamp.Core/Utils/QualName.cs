using System;
using System.Xml;

using GenieLamp.Core;

namespace GenieLamp.Core.Utils
{
	/// <summary>
	/// Fully qualified name
	/// </summary>
	class QualName
	{
		private string schema = Const.EmptyName;
		private string name = Const.EmptyName;
		
		#region Constructors
		public QualName()
		{ }

		public QualName(XmlNode node, string schemaAttrName, string nameAttrName, string defaultSchema)
		{
			string fullName = Utils.Xml.GetAttrValue(node, nameAttrName, Const.EmptyName).Trim();
			name = ExtractName(fullName);
			
			schema = Utils.Xml.GetAttrValue(node, schemaAttrName, Const.EmptyName).Trim();
			if (schema == Const.EmptyName)
				schema = ExtractSchema(fullName);
			if (schema == Const.EmptyName)
				schema = defaultSchema;
		}
		
		public QualName(XmlNode node, string defaultSchema)
			: this(node,
			       "schema",
			       "name",
			       defaultSchema)
		{ }
		
		public QualName(QualName source)
		{
			this.name = source.Name;
			this.schema = source.Schema;
		}

		public QualName(string fullName, string defaultSchema = "")
		{
			schema = ExtractSchema(fullName);
			if (schema == Const.EmptyName)
				schema = defaultSchema;
			name = ExtractName(fullName);
		}
		#endregion

		public static string ExtractSchema(string fullName)
		{
			// Schema.Name
			// .Name
			// Name
			// 01234567890
			int pos = fullName.IndexOf(".");
			if (pos < 1 || fullName.Length == 0)
				return Const.EmptyName;
			return fullName.Substring(0, pos);
		}
		
		public static string ExtractName(string fullName)
		{
			int pos = fullName.IndexOf(".");
			if (pos < 0)
				return fullName;
			if (pos == fullName.Length - 1)
				return Const.EmptyName;
			return fullName.Substring(pos + 1);
		}
		
		public static string MakeFullName(string schema, string name)
		{
			return String.Format("{0}{1}", 
			                     String.IsNullOrEmpty(schema) ? "" : String.Format("{0}.", schema),
			                     name);
		}
		
		public string Name
		{
			get { return this.name; }
		}

		public string Schema
		{
			get { return this.schema; }
		}
		
		public string FullName
		{
			get { return MakeFullName(this.schema, this.name); }
		}
		
		public bool IsEmpty
		{
			get { return this.name == Const.EmptyName; }
		}
		
		public override string ToString()
		{
			return FullName;
		}
	}
}

