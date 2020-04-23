using System;
using System.Xml;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class Attribute : MetaObject, IAttribute, IConsistency
	{
		private Persistence persistence;
		private QualName typeName;
		private Type attrType = null;
		private TypeDefinition typeDefinition = null;
		private bool readOnly = false;
		private bool primaryId = false;
		private bool uniqueId = false;
		private QualName generatorName = new QualName();
		private AttributeMigration migration;
		private bool isDeclared = false;
		private bool isAutoincrement = false;
		private bool hasMaxValue = false;
		private decimal maxValue = 0;

		#region Constructors
		public Attribute(Entity owner, XmlNode node)
			: base(owner, node)
		{
			isDeclared = true;
			typeName = new QualName(node, "typeSchema", "type", owner.Schema);
			SetType();
			if (attrType is ScalarType)
			{
				typeDefinition = new TypeDefinition(node, (attrType as ScalarType).TypeDefinition);
			}
			else
				typeDefinition = new TypeDefinition(node);
			
			persistence = new Persistence(this, node);

			primaryId = Utils.Xml.GetAttrValue(node, "primaryid", primaryId);
			uniqueId = Utils.Xml.GetAttrValue(node, "uniqueid", uniqueId);
			readOnly = Utils.Xml.GetAttrValue(node, "readonly", readOnly);
			isAutoincrement = Utils.Xml.GetAttrValue(node, "autoincrement", isAutoincrement);
			hasMaxValue = Utils.Xml.IsAttrExists(node, "maxValue");
			maxValue = Utils.Xml.GetAttrValue(node, "maxValue", maxValue);
			
			generatorName = new QualName(node, "generatorSchema", "generator", owner.Schema);
			if (!generatorName.IsEmpty && Model.Generators.GetByName(generatorName.FullName) == null)
				throw new GlException("Generator \"{0}\" not found. {1}", generatorName, this);
		}
		
		public Attribute(Entity owner, Attribute source)
			: base(owner)
		{
			this.name = source.name;
			this.typeName = new QualName(source.TypeName);
			attrType = source.Type;
			readOnly = source.ReadOnly;
			typeDefinition = new TypeDefinition(source.TypeDefinition);
			persistence = new Persistence(this,
			                              source.Persistence.Persisted,
			                              this.Entity.Persistence.SchemaDefined,
			                              this.Entity.Persistence.Schema,
			                              source.Persistence.NameDefined,
			                              source.Persistence.Name,
			                              false,
			                              Const.EmptyName);
		}
		
		public Attribute(Entity owner, string name, ScalarType type)
			: base(owner)
		{
			this.name = name;
			this.isDeclared = false;
			this.typeName = new QualName(type.FullName, owner.Schema);
			attrType = type;
			typeDefinition = new TypeDefinition(type.TypeDefinition);
			persistence = new Persistence(this,
			                              true,
			                              false,
			                              owner.Persistence.Schema,
			                              false,
			                              name,
			                              false,
			                              Const.EmptyName);
		}

		private void SetType()
		{
			attrType = Model.Types.FindType(typeName);
			if (attrType == null)
				throw new GlException("Type \"{0}\" not found. Entity: {1}. Attribute: {2}",
				              typeName, Entity.FullName, Name);
		}
		#endregion
		
		
		public override string ToString()
		{
			return String.Format("{0}(Type:{1}{2})",
			                     base.ToString(),
			                     Type.FullName,
			                     IsMigrated ? String.Format("({0})", Migration.ToString()) : "");
		}

		public void Update()
		{
		}

		#region IConsistency
		public void Check()
		{
			if (this.Type is EntityType)
				throw new GlException("Attribute of entity type found. {0}. {1}", this, this.Entity);
		}
		#endregion

		public void Migrate(Attribute source)
		{
			this.migration = new AttributeMigration(this, source);
		}

		internal QualName GeneratorName
		{
			get { return this.generatorName; }
		}
		
		internal QualName TypeName
		{
			get { return this.typeName; }
		}
		
		public Type Type
		{
			get { return this.attrType; }
		}
		
		public Entity Entity
		{
			get { return Owner as Entity; }
		}
		
		public bool HasEnumerationType
		{
			get { return attrType is EnumerationType; }
		}
		
		public bool HasEntityType
		{
			get { return attrType is EntityType; }
		}
		
		public Persistence Persistence
		{
			get { return persistence; }
		}
		
		public TypeDefinition TypeDefinition
		{
			get { return typeDefinition; }
		}

		public bool IsAutoincrement
		{
			get { return isAutoincrement; }
			internal set { isAutoincrement = value; }
		}

		public bool HasMaxValue
		{
			get { return hasMaxValue; }
		}

		public decimal MaxValue
		{
			get { return this.maxValue; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GenieLamp.Core.Metamodel.Attribute"/> has "primaryid" XML attribute defined in model.
		/// </summary>
		/// <value>
		/// <c>true</c>, <c>false</c>.
		/// </value>
		public bool PrimaryId
		{
			get { return primaryId; }
			internal set { this.primaryId = value; }
		}

		/// <summary>
		/// Indicates whether this attribute is a primary identifier or is a part of primary id.
		/// </summary>
		/// <value>
		/// <c>true</c>, <c>false</c>.
		/// </value>
		public bool IsPrimaryId
		{
			get
			{
				return (this.primaryId ||
				        (Entity.Constraints != null
				 		&& Entity.Constraints.PrimaryId != null
				 		&& Entity.Constraints.PrimaryId.Attributes.Contains(this)));
			}
		}

		public bool UniqueId
		{
			get { return uniqueId; }
		}

		public AttributeMigration Migration
		{
			get { return this.migration; }
		}

		#region IAttribute implementation
		IType IAttribute.Type
		{
			get { return this.attrType; }
		}

		IPersistence IPersistent.Persistence
		{
			get { return persistence; }
		}
		
		ITypeDefinition IAttribute.TypeDefinition
		{
			get { return typeDefinition; }
		}

		bool IAttribute.IsPrimaryId
		{
			get
			{
				return this.IsPrimaryId;
			}
		}

		public bool ReadOnly
		{
			get { return readOnly; }
		}

		IEntity IAttribute.Entity
		{
			get { return this.Owner as IEntity; }
		}

		public bool IsUsedInRelations
		{
			get { return UsedInRelations.Count > 0; }
		}

		public IRelations UsedInRelations
		{
			get
			{
				Relations usedInRelations = new Relations(Model);
				foreach (Relation r in Entity.Relations)
				{
					foreach (RelationAttributeMatch am in r.AttributesMatch)
					{
						if (am.Attribute == this || am.Attribute2 == this)
						{
							usedInRelations.Add(r);
						}
					}
				}
				return usedInRelations;
			}
		}

		bool IAttribute.HasEnumerationType
		{
			get { return this.HasEnumerationType; }
		}

		public bool ProcessInRelations
		{
			get
			{
				return
					(IsUsedInRelations && (!IsPrimaryId || (this.Entity.Constraints.PrimaryId != null && this.Entity.Constraints.PrimaryId.Composite)))
					|| (IsPrimaryId && this.Entity.HasSupertype);
			}
		}

		public bool IsDeclared
		{
			get { return this.isDeclared; }
		}

		public bool IsMigrated
		{
			get { return this.migration != null; }
		}

		IAttributeMigration IAttribute.Migration
		{
			get { return this.migration; }
		}

		public Relation GetManyToOne()
		{
			foreach(Relation r in Entity.Relations)
			{
				if (r.Cardinality == Cardinality.RM_1 && r.ChildAttributes.Contains(this))
					return r;
			}
			return null;
		}

		IRelation IAttribute.GetManyToOne()
		{
			return this.GetManyToOne();
		}

		public IAttribute GetManyToOneRelatedAttribute()
		{
			Relation r = this.GetManyToOne();
			if (r == null)
				return null;
			foreach(RelationAttributeMatch am in r.AttributesMatch)
			{
				if (this == (r.ChildSide == RelationSide.Left ? am.Attribute : am.Attribute2))
					return (r.ParentSide == RelationSide.Left ? am.Attribute : am.Attribute2);
			}
			throw new GlException("Many-to-one relation does not contain atribute {0}. Relation: {1}", this.ToString(), r);
		}
		#endregion




	}
}

