using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.Metamodel.Physical;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class Model : IModel
	{
		private GenieLamp lamp = null;
		private string defaultSchema = Const.EmptyName;
		private string defaultPersistentSchema = Const.EmptyName;
		private Entities entities = null;
		private Generators generators = null;
		private Enumerations enumerations = null;
		private Relations relations = null;
		private SpellHints spellHints = null;
		private Types types = null;
		private string xmlNamespace;
		private MetaObjectCollection<IMetaObject, MetaObject> metaObjects;
		private Schemas schemas;
		
		public PhysicalModel PhysicalModel { get; private set; }

		#region Constructors
		public Model(GenieLamp lamp)
		{
			this.lamp = lamp;
			metaObjects = new MetaObjectCollection<IMetaObject, MetaObject>(this);
			entities = new Entities(this);
			types = new Types(this);
			enumerations = new Enumerations(this);
			relations = new Relations(this);
			generators = new Generators(this);
			this.PhysicalModel = new PhysicalModel(this);
			schemas = new Schemas(this);
			spellHints = new SpellHints(this);
		}
		#endregion

		public MetaObjectCollection<IMetaObject, MetaObject> MetaObjects
		{
			get { return metaObjects; }
		}

		public GenieLamp Lamp
		{
			get { return lamp; }
		}
		
		public ILogger Logger
		{
			get { return lamp.Logger; }
		}

		public string DefaultSchema
		{
			get { return this.defaultSchema; }
		}

		public string DefaultPersistentSchema
		{
			get { return this.defaultPersistentSchema; }
		}

		public Entities Entities
		{
			get{ return entities; }
		}
		
		public Enumerations Enumerations
		{
			get { return enumerations; }
		}
		
		public Relations Relations
		{
			get{ return relations; }
		}
		
		public Types Types
		{
			get { return types; }
		}
		
		public Generators Generators
		{
			get { return generators; }
		}

		public SpellHints SpellHints
		{
			get { return this.spellHints; }
		}

		public void AddModelDoc(string fileName)
		{
			lamp.Logger.TraceLine("Loading model file: {0}", fileName);
			GenieLampLoader loader = new GenieLampLoader(lamp);
			XmlDocument doc = loader.LoadFile(fileName, GenieLamp.GLModelNamespaceName, "GenieLamp.Core.XMLSchema.GenieLampModel.xsd");
			xmlNamespace = loader.XmlNamespace;
			
			// Default schema is used only in current file scope
			defaultSchema = Utils.Xml.GetAttrValue(doc.DocumentElement, "defaultSchema", Const.EmptyName);
			defaultPersistentSchema = Lamp.Config.Layers.PersistenceConfig.NamingConvention.Convert(
				Utils.Xml.GetAttrValue(doc.DocumentElement, "defaultPersistentSchema", Const.EmptyName));
			
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace(GenieLamp.GLModelNamespaceName, XmlNamespace);
			
			types.AddList(doc.SelectNodes(String.Format("/{0}:Model/{0}:Type", GenieLamp.GLModelNamespaceName), nsmgr));
			Lamp.Config.Patterns.Prepare();

			enumerations.AddList(doc.SelectNodes(String.Format("/{0}:Model/{0}:Enumeration", GenieLamp.GLModelNamespaceName), nsmgr));
			generators.AddList(doc.SelectNodes(String.Format("/{0}:Model/{0}:Generator", GenieLamp.GLModelNamespaceName), nsmgr));
			entities.AddList(doc.SelectNodes(String.Format("/{0}:Model/{0}:Entity", GenieLamp.GLModelNamespaceName), nsmgr));
			relations.AddList(doc.SelectNodes(String.Format("/{0}:Model/{0}:Relation", GenieLamp.GLModelNamespaceName), nsmgr));
			this.PhysicalModel.Indexes.AddList(doc.SelectNodes(String.Format("/{0}:Model/{0}:Index", GenieLamp.GLModelNamespaceName), nsmgr));
			spellHints.AddList(doc.SelectNodes(String.Format("/{0}:Model//{0}:SpellHint", GenieLamp.GLModelNamespaceName), nsmgr));
		}
		
		private void InternalUpdate()
		{
			this.enumerations.Update();
			this.generators.Update();
			this.entities.Update();
			this.relations.Update();
			this.PhysicalModel.Update();
			this.schemas.Update();
		}
		
		public void Update()
		{
			Relations.CreateSubtypingRelations();
			InternalUpdate();
			Relations.CreateImplicitesRelations();
			InternalUpdate();
			Lamp.Config.Patterns.Update();
		}

		public void ImplementPatterns()
		{
			Lamp.Config.Patterns.Implement();
			InternalUpdate();
			Check();
		}

		#region IConsistency implementation
		public void Check()
		{
			Enumerations.Check();
			Entities.Check();
			Relations.Check();
			this.PhysicalModel.Check();
		}
		#endregion
		
		
		public XmlNodeList QueryNode(XmlNode modelNode, string query)
		{
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(modelNode.OwnerDocument.NameTable);
			nsmgr.AddNamespace(GenieLamp.GLModelNamespaceName, XmlNamespace);
			return modelNode.SelectNodes(String.Format(query, GenieLamp.GLModelNamespaceName), nsmgr);
		}
		
		#region IModel implementation
		public string XmlNamespace
		{
			get { return xmlNamespace; }
		}
		
		IEntities IModel.Entities
		{
			get { return this.entities; }
		}

		ITypes IModel.Types
		{
			get { return this.types; }
		}

		IEnumerations IModel.Enumerations
		{
			get { return this.enumerations; }
		}

		IRelations IModel.Relations
		{
			get { return relations; }
		}

		IGenieLamp IModel.Lamp
		{
			get { return this.Lamp; }
		}

		IGenerators IModel.Generators
		{
			get { return generators; }
		}

		Physical.IPhysicalModel IModel.PhysicalModel
		{
			get	{ return this.PhysicalModel; }
		}

		IMetaObjectCollection<IMetaObject> IModel.MetaObjects
		{
			get { return metaObjects; }
		}

		ISchemas IModel.Schemas
		{
			get { return this.schemas; }
		}

		public void Dump(string fileName)
		{
			CodeWriterText cw = new CodeWriterText(Lamp);

			cw.WriteLine("Entities:");
			cw.BeginScope();
			foreach (Entity en in Entities)
			{
				cw.WriteLine("{0} {1}", en.ToString(), en.Persistence.ToString());
				cw.BeginScope();
				cw.WriteLine("Attributes:");
				cw.BeginScope();
				foreach (Attribute a in en.Attributes)
				{
					cw.WriteLine("{0} {1}", a.ToString(), a.Persistence.ToString());
				}
				cw.EndScope();
				if (en.Constraints.PrimaryId != null)
				{
					cw.WriteLine("PrimaryId: {0}", en.Constraints.PrimaryId.Persistence);
					cw.BeginScope();
					foreach (Attribute a in en.Constraints.PrimaryId.Attributes)
						cw.WriteLine(a.ToString());
					cw.EndScope();
				}
				else
					cw.WriteLine("PrimaryId: not defined");
				if (en.Constraints.UniqueIds.Count > 0)
				{
					foreach (UniqueId uid in en.Constraints.UniqueIds)
					{
						cw.WriteLine("UniqueId: {0}", uid.Persistence);
						cw.BeginScope();
						foreach (Attribute a in uid.Attributes)
							cw.WriteLine(a.ToString());
						cw.EndScope();
					}
				}
				cw.EndScope();
			}
			cw.EndScope();

			cw.WriteLine();
			cw.WriteLine("Relations:");
			cw.BeginScope();
			foreach (Relation r in Relations)
			{
				cw.WriteLine("{0}(Nav:{1}, Nav2:{2}) {3}", r.ToString(), r.Navigate, r.Navigate2, r.Persistence);
				cw.BeginScope();
				foreach (RelationAttributeMatch am in r.AttributesMatch)
				{
					cw.WriteLine(am.ToString());
				}
				cw.WriteLine("Foreign key: {0}", r.ForeignKey);
				cw.BeginScope();
				foreach (Attribute a in r.ForeignKey.ChildTableColumns)
				{
					cw.WriteLine(a.ToString());
				}
				if (r.ForeignKey.Index != null)
				{
					cw.WriteLine("Index: {0}", r.ForeignKey.Index.ToString());
					cw.BeginScope();
					foreach (IndexColumn col in r.ForeignKey.Index.Columns)
					{
						cw.WriteLine(col.ToString());
					}
					cw.EndScope();
				}
				cw.EndScope();
				cw.EndScope();
			}
			cw.EndScope();

			cw.WriteLine();
			cw.WriteLine("Enumerations:");
			cw.BeginScope();
			foreach (Enumeration em in Enumerations)
			{
				cw.WriteLine("{0} {1}", em.ToString(), em.Persistence.ToString());
				cw.BeginScope();
				foreach (EnumerationItem item in em.Items)
				{
					cw.WriteLine(item.ToString());
				}
				cw.EndScope();
			}
			cw.EndScope();
			
			cw.WriteLine();
			cw.WriteLine("Generators:");
			cw.BeginScope();
			foreach (Generator gen in Generators)
			{
				cw.WriteLine("{0} {1}", gen.ToString(), gen.Persistence.ToString());
			}
			cw.EndScope();

			cw.WriteLine();
			cw.WriteLine("Types:");
			cw.BeginScope();
			foreach (Type t in Types)
			{
				cw.WriteLine("{0}", t.ToString());
			}
			cw.EndScope();
			
			cw.WriteLine();
			cw.WriteLine("Indexes:");
			cw.BeginScope();
			foreach (Index i in this.PhysicalModel.Indexes)
			{
				cw.WriteLine("{0}", i.ToString());
				cw.BeginScope();
				foreach (IndexColumn col in i.Columns)
					cw.WriteLine("{0}", col.ToString());
				cw.EndScope();
			}
			cw.EndScope();

			cw.Save(fileName);
		}

		ISpellHints IModel.SpellHints
		{
			get { return this.SpellHints; }
		}
		#endregion
	}
}

