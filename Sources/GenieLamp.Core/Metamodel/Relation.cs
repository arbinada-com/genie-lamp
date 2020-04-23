using System;
using System.Xml;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Metamodel.Physical;

namespace GenieLamp.Core.Metamodel
{
	class Relation : MetaObject, IRelation
	{
		protected string schema;
		protected string entityName;
		protected Entity entity;
		protected bool navigate = true;
		protected string name2;
		protected string schema2;
		protected string entityName2;
		protected Entity entity2;
		protected bool navigate2 = true;
		protected bool required = true;
		protected RelationCascade cascade = RelationCascade.None;
		protected RelationAttributesMatch attrMatch = null;
		protected Cardinality cardinality = Cardinality.R1_M;
		protected Persistence persistence;
		protected ForeignKey foreignKey;
		private bool hasIndexName = false;
		private string indexName = Const.EmptyName;

		
		#region Constructors
		public Relation(Model owner, XmlNode node) : base(owner, node)
		{
			QualName name = new QualName(node, "schema", "entity", Model.DefaultSchema);
			this.schema = name.Schema;
			this.entityName = name.Name;
			QualName name2 = new QualName(node, "schema2", "entity2", Model.DefaultSchema);
			this.schema2 = name2.Schema;
			this.entityName2 = name2.Name;
			this.name2 = Utils.Xml.GetAttrValue(node, "name2", Const.EmptyName);
			this.required = Utils.Xml.GetAttrValue(node, "required", this.required);
			this.navigate = Utils.Xml.GetAttrValue(node, "navigate", this.navigate);
			this.navigate2 = Utils.Xml.GetAttrValue(node, "navigate2", this.navigate2);
			this.cardinality = ParseCardinality(Utils.Xml.GetAttrValue(node, "cardinality"));
			this.cascade = ParseRelationCascade(Utils.Xml.GetAttrValue(node, "cascade"));
			Init();
			this.persistence = new Persistence(this, node);
			this.attrMatch = new RelationAttributesMatch(this, node);
			this.foreignKey = Model.PhysicalModel.ForeignKeys.CreateForeignKey(this);
			this.hasIndexName = Utils.Xml.IsAttrExists(node, "indexName");
			this.indexName = Utils.Xml.GetAttrValue(node, "indexName", this.indexName);

		}
		
		internal Relation(Model owner,
		                  string schema,
		                  string entityName, 
		                  string name,
		                  bool navigate,
		                  string schema2,
		                  string entityName2,
		                  string name2,
		                  bool navigate2,
		                  bool required,
		                  Cardinality cardinality,
		                  Persistence persistence)
			: base(owner, name)
		{
			this.schema = schema;
			this.entityName = entityName;
			this.navigate = navigate;
			this.schema2 = schema2;
			this.entityName2 = entityName2;
			this.name2 = name2;
			this.navigate2 = navigate2;
			this.required = required;
			this.cardinality = cardinality;
			Init();
			this.persistence = new Persistence(this, persistence);
			this.attrMatch = new RelationAttributesMatch(this);
			this.foreignKey = Model.PhysicalModel.ForeignKeys.CreateForeignKey(this);
		}

		private void Init()
		{
			if (cardinality == Cardinality.RM_M)
				Persistence.Persisted = false; // Should be implemented as 2 persisted relation + 1 table

			entity = UpdateEntity(schema, entityName);
			entity2 = UpdateEntity(schema2, entityName2);
		}
		#endregion

		public static RelationCascade ParseRelationCascade(string value)
		{
			switch(value.ToLower())
			{
			case "none":
				return RelationCascade.None;
			case "all":
				return RelationCascade.All;
			case "delete":
				return RelationCascade.Delete;
			case "update":
				return RelationCascade.Update;
			default:
				throw new GlException("Relation cascade value '{0}' is not supported", value);
			}
		}

		public static Cardinality ParseCardinality(string value)
		{
			switch(value.ToUpper())
			{
			case "1:1":
				return Cardinality.R1_1;
			case "1:M":
				return Cardinality.R1_M;
			case "M:1":
				return Cardinality.RM_1;
			case "M:M":
				return Cardinality.RM_M;
			default:
				throw new GlException("Invalid cardinlaity value: {0}", value);
			}
		}
		
		public static string CardinalityToString(Cardinality cardinality)
		{
			switch (cardinality)
			{
			case Cardinality.R1_1:
				return "1:1";
			case Cardinality.RM_1:
				return "M:1";
			case Cardinality.R1_M:
				return "1:M";
			case Cardinality.RM_M:
				return "M:M";
			default:
				return Enum.GetName(typeof(Cardinality), cardinality);	
			}
		}
		
		public static string SideToNameSuffix(RelationSide side)
		{
			switch (side)
			{
			case RelationSide.Left:
				return "";
			case RelationSide.Right:
				return "2";
			default:
				return Enum.GetName(typeof(RelationSide), side);	
			}
		}
		
		private void MakeName(RelationSide side, bool warnDuplicateDetected = true)
		{
			bool isListName =
				(side == RelationSide.Right && cardinality == Cardinality.RM_1) ||
				(side == RelationSide.Left && cardinality == Cardinality.R1_M) || 
				cardinality == Cardinality.RM_M;
			string suffix = isListName ? "List" : "";
			Entity nameSource = side == RelationSide.Left ? Entity2 : Entity;
			string root = nameSource.Name;
			string newName = String.Format("{0}{1}", root, suffix);
			bool generated = false;
			int counter = 1;
			while (Model.Relations.RelationExists(Entity, Entity2, newName, "*") ||
			      Model.Relations.RelationExists(Entity, Entity2, "*", newName))
			{
				generated = true;
				newName = String.Format("{0}{1}{2}", root, counter++, suffix);
			}
			
			if (side == RelationSide.Left)
				name = newName;
			else
				name2 = newName;
			
			bool navigated = (side == RelationSide.Left && Navigate) || (side == RelationSide.Right && Navigate2);
			if (generated && navigated && warnDuplicateDetected)
				Logger.Warning(WarningLevel.High, 
				               "Relation name{0} \"{1}\" was generated for navigation. Specify name explicitely to avoid it. {2}", 
				               SideToNameSuffix(side), newName, this);
		}

		public virtual void Update()
		{
			if (name == Const.EmptyName)
				MakeName(RelationSide.Left);
			if (name2 == Const.EmptyName)
				MakeName(RelationSide.Right);
			
			AttributesMatch.Update();

			Persistence.UpdateNames(Const.EmptyName, foreignKey.Name);
		}
		
		private Entity UpdateEntity(string entSchema, string entName)
		{
			Entity result = Model.Entities.GetByName(QualName.MakeFullName(entSchema, entName));
			if (result == null)
				throw new GlException("Specified entity not found: {0}. {1}",
				                      QualName.MakeFullName(entSchema, entName),
				                      this);
			return result;
		}
		
		
		#region IConsistency implementation
		public void Check()
		{
			if (AttributesMatch.Count == 0 && Cardinality != Cardinality.RM_M)
				throw new GlException("Attributes match not found. {0}", this);
			
			foreach (RelationAttributeMatch am in AttributesMatch)
				am.Check();
		}
		#endregion
		
		public override string ToString()
		{
			return String.Format("{0} ({1}-{2}): <{3}>--{4}--<{5}>",
			                     GetType().Name,
			                     Name,
			                     Name2,
			                     QualName.MakeFullName(schema, entityName),
			                     CardinalityToString(cardinality),
			                     QualName.MakeFullName(schema2, entityName2));
		}
		
		public bool ContainsEntity(Entity e)
		{
			return entity == e || entity2 == e;
		}
		
		public Entity Entity
		{
			get { return entity; }
		}
		
		public Entity Entity2
		{
			get { return entity2; }
		}

		public RelationAttributesMatch AttributesMatch
		{
			get { return attrMatch; }
		}

		public Persistence Persistence
		{
			get { return persistence; }
		}
		
		public ForeignKey ForeignKey
		{
			get	{ return this.foreignKey; }
		}

		public Entity ParentEntity
		{
			get { return ParentSide == RelationSide.Left ? Entity : Entity2; }
		}

		public Entity ChildEntity
		{
			get { return ParentSide == RelationSide.Left ? Entity2 : Entity; }
		}

		public RelationAttributes ParentAttributes
		{
			get
			{
				return ParentSide == RelationSide.Left ? this.AttributesMatch.Attributes : this.AttributesMatch.Attributes2;
			}
		}

		public RelationAttributes ChildAttributes
		{
			get
			{
				return ParentSide == RelationSide.Left ? this.AttributesMatch.Attributes2 : this.AttributesMatch.Attributes;
			}
		}

		#region IRelation implementation
		public RelationCascade Cascade
		{
			get { return this.cascade; }
			internal set { cascade = value; }
		}

		public Cardinality Cardinality
		{
			get { return cardinality; }
		}

		IEntity IRelation.Entity
		{
			get { return this.entity; }
		}

		public bool Navigate
		{
			get { return navigate; }
		}

		public string Name2
		{
			get { return name2; }
		}

		IEntity IRelation.Entity2
		{
			get { return this.entity2; }
		}

		public bool Navigate2
		{
			get { return navigate2; }
		}

		public bool Required
		{
			get { return required; }
		}

		IRelationAttributesMatch IRelation.AttributesMatch
		{
			get { return attrMatch; }
		}

		IForeignKey IRelation.ForeignKey
		{
			get	{ return this.foreignKey; }
		}

		public bool HasIndexName 
		{ 
			get { return hasIndexName; }
		}

		public string IndexName 
		{ 
			get { return indexName; }
		}

		public RelationSide ChildSide
		{
			get { return this.ParentSide == RelationSide.Left ? RelationSide.Right : RelationSide.Left; }
		}

		public RelationSide ParentSide
		{
			get
			{
				switch(this.Cardinality)
				{
				case Cardinality.R1_1:
					if (this is SubtypeRelation ||
					    (AttributesMatch.Count > 0 && AttributesMatch[0].Attribute.IsPrimaryId))
						return RelationSide.Left;
					else
						return RelationSide.Right;
				case Cardinality.R1_M:
					return RelationSide.Left;
				case Cardinality.RM_1:
					return RelationSide.Right;
				default:
					return RelationSide.None;
				}
			}
		}

		IEntity IRelation.ParentEntity
		{
			get { return this.ParentEntity; }
		}

		IEntity IRelation.ChildEntity
		{
			get { return this.ChildEntity; }
		}

		IAttributes IRelation.ParentAttributes
		{
			get { return this.ParentAttributes; }
		}

		IAttributes IRelation.ChildAttributes
		{
			get { return this.ChildAttributes; }
		}

		public bool ParentNavigate
		{
			get { return ParentSide == RelationSide.Left ? this.Navigate : this.Navigate2; }
		}

		public bool ChildNavigate
		{
			get { return ParentSide == RelationSide.Left ? this.Navigate2 : this.Navigate; }
		}

		public string ParentName
		{
			get { return ParentSide == RelationSide.Left ? this.Name : this.Name2; }
		}

		public string ChildName
		{
			get { return ParentSide == RelationSide.Left ? this.Name2 : this.Name; }
		}

		public bool IsParent(IEntity entity)
		{
			return ParentSide == RelationSide.Left ? this.Entity.Equals(entity) : this.Entity2.Equals(entity);
		}

		public bool IsChild(IEntity entity)
		{
			return ParentSide == RelationSide.Right ? this.Entity.Equals(entity) : this.Entity2.Equals(entity);
		}
		#endregion

		#region IPersistent
		IPersistence IPersistent.Persistence
		{
			get { return persistence; }
		}

		#endregion
	}
}

