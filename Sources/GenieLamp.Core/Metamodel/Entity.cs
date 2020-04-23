using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class Entity : MetaObjectSchemed, IEntity
	{
		private Entity supertype = null;
		private EntityAttributes attributes = null;
		private EntityConstraints constraints = null;
		private Relations relations = null;
		private Relations parents = null;
		private Relations children = null;
		private Relations oneToOne = null;
		private Relations manyToMany = null;
		private Persistence persistence;
		private EntityType type = null;
		private QualName supertypeName;
		private int id;

		public EntityEventHandlers EventHandlers { get; private set; }
		public EntityOperations Operations { get; private set; }
		
		#region Constructors
		public Entity(Model model, string schema, string name)
			: base(model, schema, name)
		{
			Init();
			persistence = new Persistence(this);
		}

		public Entity(Model model, XmlNode node) : base(model, node)
		{
			Init();

			supertypeName = new QualName(node, "supertypeSchema", "supertype", this.Schema);
			if (!supertypeName.IsEmpty)
			{
				Type t = Model.Types.FindType(supertypeName);
				if (t == null)
					throw new GlException("Supertype \"{0}\" not found. Entity: {1}", supertypeName.FullName, FullName);
				supertype = (t as EntityType).Entity;
			}
			
			Model.Generators.AddList(this, Model.QueryNode(node, "./{0}:Generator"));
			
			persistence = new Persistence(this, node);
			if (!Persistence.SchemaDefined)
			{
				this.Persistence.Schema = Model.DefaultPersistentSchema;
				this.Persistence.LockSchema();
			}
			attributes = new EntityAttributes(this, node);
			constraints = new EntityConstraints(this, node);

			this.EventHandlers = new EntityEventHandlers(this, node);
			this.Operations = new EntityOperations(this, node);

			Model.PhysicalModel.Indexes.AddList(this, Model.QueryNode(node, "./{0}:Index"));
		}

		private void Init()
		{
			type = Model.Types.AddEntityType(this);
			relations = new Relations(Model);
			parents = new Relations(Model);
			children = new Relations(Model);
			oneToOne = new Relations(Model);
			manyToMany = new Relations(Model);
			attributes = new EntityAttributes(this);
			constraints = new EntityConstraints(this);
			this.EventHandlers = new EntityEventHandlers(this);
			this.Operations = new EntityOperations(this);
		}
		#endregion
		
		
		public Entity Supertype
		{
			get { return this.supertype; }
		}

		public EntityAttributes Attributes
		{
			get { return attributes; }
		}
		
		public EntityConstraints Constraints
		{
			get { return constraints; }
		}
		
		public Persistence Persistence
		{
			get { return persistence; }
		}
		
		public Relations Relations
		{
			get { return this.relations; }
		}

		public Relations Parents
		{
			get { return this.parents; }
		}

		public Relations Children
		{
			get { return this.children; }
		}

		public Relations OneToOne
		{
			get { return this.oneToOne; }
		}

		public Relations ManyToMany
		{
			get { return this.manyToMany; }
		}

		public Metamodel.Attribute AddAttribute(string name, ScalarType type)
		{
			Metamodel.Attribute a = new Metamodel.Attribute(this, name, type);
			this.Attributes.Add(a);
			return a;
		}

		public IAttributes GetAttributes(bool includeSupertype)
		{
			AttributesCollection collection = new AttributesCollection(Model);
			if (includeSupertype)
			{
				foreach(Attribute a in PrimaryId.Attributes)
					collection.Add(a);
				IterateToSupertypes(this, delegate(Entity entity) {
					foreach(Attribute a in entity.Attributes)
					{
						if (collection.GetByName(a.Name) == null) // Omit primary id in subtypes
							collection.Add(a);
					}
				});
			}
			else
			{
				foreach(Attribute a in this.Attributes)
					collection.Add(a);
			}
			return collection;
		}

		public delegate void IterateToSupertypesDelegate(Entity entity);

		public void IterateToSupertypes(Entity entity, IterateToSupertypesDelegate iteratorDelegate)
		{
			if (entity.Supertype != null)
				IterateToSupertypes(entity.Supertype, iteratorDelegate);
			iteratorDelegate(entity);
		}

		/// <summary>
		/// Creates the generator metaobject if it doesn't exists for primaryid that is simple and defined with autoincremental option
		/// </summary>
		private void CreateGenerator()
		{
			if (PrimaryId == null || !PrimaryId.IsAutoincrement)
				return;
			Generator gen = new Generator(this);
			if (Model.Generators.GetByName(gen.FullName) == null)
			{
				if (PrimaryId.Attribute.Type is ScalarType)
				{
					ScalarType t = (PrimaryId.Attribute.Type as ScalarType);
					if (PrimaryId.Attribute.HasMaxValue)
					{
						gen.MaxValue = PrimaryId.Attribute.MaxValue;
						gen.HasMaxValue = true;
					}
					else if (t.MaxValue.HasValue)
					{
						gen.MaxValue = t.MaxValue.Value;
						gen.HasMaxValue = true;
					}
				}
				PrimaryId.Generator = gen;
				Model.Generators.Add(gen);
			}
		}

		public void Update()
		{
			Macro.SetMacroTable(Name);
			string tableNameTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName("Table.Template", Name).Value;
			string tableName = Macro.Subst(Macro.Subst(tableNameTemplate));
			persistence.UpdateNames(Schema, tableName);

			Macro.SetMacroTable(persistence.Name);
			Attributes.Update();
			Constraints.Update();

			CreateGenerator();
			
			// Establish links with relations
			relations.Clear();
			parents.Clear();
			children.Clear();
			oneToOne.Clear();
			manyToMany.Clear();
			
			foreach (Relation relation in Model.Relations)
			{
				if (relation.ContainsEntity(this))
				{
					relations.Add(relation);
					switch (relation.Cardinality)
					{
					case Cardinality.R1_1:
						oneToOne.Add(relation);
						break;
					case Cardinality.RM_M:
						manyToMany.Add(relation);
						break;
					case Cardinality.R1_M:
						if (relation.Entity.Equals(this))
							children.Add(relation);
						else
							parents.Add(relation);
						break;
					case Cardinality.RM_1:
						if (relation.Entity.Equals(this))
							parents.Add(relation);
						else
							children.Add(relation);
						break;
					}
				}
			}
		}
		
		
		#region IConsistency implementation
		public void Check()
		{
			foreach (Attribute a in Attributes)
				a.Check();

			Constraints.Check();
			
			if (relations.Count == 0)
				Logger.Warning(WarningLevel.Medium, "Entity \"{0}\" has no relations", this.FullName);

			Dictionary<string, Attribute> allAttributes = new Dictionary<string, Attribute>();
			CheckAttributesUniqueness(this, allAttributes);

			CheckRelationNames();

			// Duplicate names in supertypes check
			AttributesCollection collection = new AttributesCollection(Model);
			IterateToSupertypes(this, delegate(Entity entity) {
				foreach(Attribute a in entity.Attributes)
				{
					Attribute duplicate = collection.GetByName(a.Name);
					if (duplicate == null)
					{
						collection.Add(a);
					}
					else
					{
						if (!a.IsPrimaryId)
						throw new GlException("Attribute name '{0}' is already used in supertype '{1}'. Entity: {2}",
						                      a.Name, duplicate.Entity.FullName, entity.FullName);
					}
				}
			});

			// Note that it is not possible to use identity with union-subclasses mapping strategy
			if (this.HasSupertype && Lamp.Config.Layers.DomainConfig.MappingStrategy == Layers.MappingStrategy.TablePerClass)
			{
				IterateToSupertypes(this.Supertype, delegate(Entity entity) {
					if (entity.PrimaryId.HasGenerator)
						throw new GlException(
							"Entity '{0}': primary id autoincrement/generator is not allowed with union-subclasses mapping strategy." +
							"Use TablePerSubclass strategy or 'uuid' type column",
							entity.FullName);
				});
			}
		}

		private void CheckAttributesUniqueness(Entity entity, Dictionary<string, Attribute> allAttributes)
		{
			if (entity.HasSupertype)
				CheckAttributesUniqueness(entity.Supertype, allAttributes);
			foreach (Attribute a in entity.Attributes)
			{
				if (a.IsMigrated && entity.Constraints.PrimaryId.ContainsAttribute(a))
					continue;
				Attribute parentAttr;
				if (allAttributes.TryGetValue(a.Name, out parentAttr))
					throw new GlException("Duplcate attribute name \"{0}\". Entity: {1}. This name already used in supertype \"{2}\"",
					                      a.Name, a.Entity.FullName, parentAttr.Entity.FullName);
				allAttributes.Add(a.Name, a);
			}
		}

		private void CheckRelationNames()
		{
			Dictionary<string, Relation> verified = new Dictionary<string, Relation>();
			foreach(Relation r in this.Relations)
			{
				string name = r.ParentName;
				if (r.IsChild(this))
					name = r.ChildName;
				if (!verified.ContainsKey(name))
					verified.Add(name, r);
				else
					throw new GlException("Duplcate relation name \"{0}\" for entity: {1}. Relation: {2}",
					                      name, FullName, r);
			}
		}
		#endregion


		public EntityType Type
		{
			get { return this.type; }
		}


		public PrimaryId PrimaryId
		{
			get { return Constraints != null ? Constraints.PrimaryId : null; }
		}
		
	
		#region IEntity implementation
		IEntity IEntity.Supertype
		{
			get { return this.supertype; }
		}

		IEntityAttributes IEntity.Attributes
		{
			get { return this.attributes; }
		}

		IEntityConstraints IEntity.Constraints
		{
			get { return this.constraints; }
		}

		public bool HasSupertype
		{
			get { return this.supertype != null; }
		}

		IRelations IEntity.Relations
		{
			get { return this.relations; }
		}

		IRelations IEntity.Parents
		{
			get { return this.parents; }
		}

		IRelations IEntity.Children
		{
			get { return this.children; }
		}

		IRelations IEntity.OneToOne
		{
			get { return this.oneToOne; }
		}

		IRelations IEntity.ManyToMany
		{
			get { return this.manyToMany; }
		}
		
		IPersistence IPersistent.Persistence
		{
			get { return this.persistence; }
		}

		IEntityType IEntity.Type
		{
			get { return this.type; }
		}

		public int Id
		{
			get { return this.id; }
			internal set { this.id = value; }
		}

		IEntityEventHandlers IEntity.EventHandlers
		{
			get { return this.EventHandlers; }
		}

		IEntityOperations IEntity.Operations
		{
			get { return this.Operations; }
		}

		IPrimaryId IEntity.PrimaryId
		{
			get	{ return this.PrimaryId; }
		}
		#endregion
















	}
}

