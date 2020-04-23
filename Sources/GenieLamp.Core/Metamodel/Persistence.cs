using System;
using System.Xml;

using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class Persistence : IPersistence
	{
		private MetaObject owner;
		private bool persisted = true;
		private bool schemaDefined = false;
		private bool schemaLocked = false;
		private string schema = Const.EmptyName;
		private bool nameDefined = false;
		private bool nameLocked = false;
		private string name = Const.EmptyName;
		private bool typeDefined = false;
		private string typeName = Const.EmptyName;
		
		#region Constructors
		internal Persistence(MetaObject owner)
		{ 
			this.owner = owner;
		}
		
		public Persistence(MetaObject owner, XmlNode node) : this(owner)
		{
			if (owner is IPersistent && (owner as IPersistent).Persistence != null)
			{
				IPersistence p = (owner as IPersistent).Persistence;
				this.schema = p.Schema;
				this.name = p.Name;
			}
			else
			{
				this.schema = owner.Model.DefaultPersistentSchema;
				this.name = owner.Name;
			}
				
			Init(Utils.Xml.GetAttrValue(node, "persisted", this.persisted),
			     Utils.Xml.IsAttrExists(node, "persistentSchema"),
			     Utils.Xml.GetAttrValue(node, "persistentSchema", this.schema),
			     Utils.Xml.IsAttrExists(node, "persistentName"),
			     Utils.Xml.GetAttrValue(node, "persistentName", owner.Name),
			     Utils.Xml.IsAttrExists(node, "persistentType"),
			     Utils.Xml.GetAttrValue(node, "persistentType", Const.EmptyName)
			     );
		}
		
		public Persistence(MetaObject owner,
		                   bool persisted,
		                   bool schemaDefined,
		                   string schema,
		                   bool nameDefined,
		                   string name,
		                   bool typeDefined,
		                   string typeName)
			: this(owner)
		{
			Init(persisted,
			     schemaDefined, 
			     schema,
			     nameDefined,
			     name,
			     typeDefined,
			     typeName
			     );
		}
		
		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name='owner'>
		/// Owner.
		/// </param>
		/// <param name='source'>
		/// Source.
		/// </param>
		internal Persistence(MetaObject owner, Persistence source) : this(owner)
		{
			if (source == null)
				throw new GlException("Cannot init copy of persistence object: source is null. {0}", owner);
			
			Init(source.Persisted,
			     source.SchemaDefined,
			     source.Schema,
			     source.NameDefined,
			     source.Name,
			     source.TypeDefined,
			     source.TypeName
			     );
		}
		
		private void Init(bool persisted,
		                  bool schemaDefined,
		                  string schema,
		                  bool nameDefined,
		                  string name,
		                  bool typeDefined,
		                  string typeName)
		{
			this.persisted = persisted;
			this.schemaDefined = schemaDefined;
			this.schema = schema;
			this.nameDefined = nameDefined;
			this.name = CheckName(name);
			this.typeDefined = typeDefined;
			this.typeName = typeName;

			UpdateNames(schema, name);
		}
		#endregion
		
		public string CheckName(string persistentName)
		{
			if (owner == null)
				return persistentName;
			return this.owner.Lamp.Config.Layers.PersistenceConfig.NamingConvention.TruncateName(
				persistentName,
				(owner is Entity || owner is Attribute || owner is Enumeration),
				owner);
		}
		
		public void UpdateNames(string sourceSchema, string sourceName)
		{
			this.Schema = SchemaDefined ? this.Schema : sourceSchema;
			this.Name = NameDefined ? this.Name : sourceName;

			string oldName = this.Schema;
			this.Schema = owner.Model.Lamp.Config.Layers.PersistenceConfig.Keywords.CheckAndModify(this.Schema);
			WarnIfNameChanged(oldName, this.Schema, "schema");
			oldName = this.Name;
			this.Name = owner.Model.Lamp.Config.Layers.PersistenceConfig.Keywords.CheckAndModify(this.Name);
			WarnIfNameChanged(oldName, this.Name, "name");

			if (owner.Model.Lamp.Config.Layers.PersistenceConfig.Keywords.Contains(this.Name))
			{
				this.Name = "GL_" + this.Name;
			}

			// Apply naming convention only when schema or name are defined explicitly
			this.Schema = owner.Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Convert(this.Schema);
			this.Name = owner.Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Convert(this.Name);
		}

		private string ChangeKeywordName(string name)
		{
			return "GL_" + name;
		}

		private void WarnIfNameChanged(string oldName, string newName, string nameKind)
		{
			if (oldName != newName)
			{
				owner.Logger.Warning(WarningLevel.High,
				                     "Persistent {0} \"{1}\" is a keyword and was changed to \"{2}\". Owner: {3}\n" +
				                     "Change this {0} in model to avoid implicit renaming",
				                     nameKind,
				                     oldName,
				                     newName,
				                     owner.ToString());
			}
		}
		
		public override string ToString()
		{
			return String.Format("{0}(Schema: {1}({2}), Name: {3}({4}), Type: {5}({6}))", 
			                     this.GetType().Name,
			                     this.Schema, this.SchemaDefined,
			                     this.Name, this.NameDefined,
			                     this.TypeName, this.TypeDefined);
		}
		
		internal void LockSchema()
		{
			this.schemaLocked = true;
		}
		
		internal void LockName()
		{
			this.nameLocked = true;
		}
		
		internal bool NameLocked
		{
			get { return this.nameLocked; }
		}
		
		internal bool SchemaLocked
		{
			get { return this.schemaLocked; }
		}
		
		#region IPersistence implementation
		public bool Persisted
		{
			get { return persisted; }
			internal set { persisted = value; }
		}

		public bool SchemaDefined
		{
			get { return schemaDefined; }
			internal set { schemaDefined = value; }
		}

		public string Schema
		{
			get { return schema; }
			internal set
			{
				if (!schemaLocked) 
					schema = value; 
			}
		}

		public bool NameDefined
		{
			get { return nameDefined; }
		}

		public string Name
		{
			get { return name; }
			internal set
			{ 
				if (!nameLocked) 
					name = CheckName(value); 
			}
		}

		public bool TypeDefined
		{
			get { return typeDefined; }
		}

		public string TypeName
		{
			get { return typeName; }
			internal set { typeName = value; }
		}

		public string FullName
		{
			get { return QualName.MakeFullName(schema, name); }
		}
		#endregion
	}
}

