using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Patterns
{
	class RegistryPattern : PatternConfig, IRegistryPattern
	{
		public const string NodeName = "Registry";

		private string registryEntityName = "EntityRegistry";
		private string typesEntityName = "EntityType";
		private string primaryIdTypeName = "uuid";
		private string typesEntityPidTypeName = "smallint";
		private Dictionary<string, Relation> relations = new Dictionary<string, Relation>();

		#region Constructors
		public RegistryPattern(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
			registryEntityName = Params.ValueByName("RegistryEntity.Name", registryEntityName);
			typesEntityName = Params.ValueByName("TypesEntity.Name", typesEntityName);
			primaryIdTypeName = Params.ValueByName("PrimaryId.Type", primaryIdTypeName);
			typesEntityPidTypeName = Params.ValueByName("TypesEntity.PrimaryId.Type", typesEntityPidTypeName);

			if (String.IsNullOrEmpty(Schema))
				throw new GlException("Registry pattern requires schema specification");
		}
		#endregion

		public string RegistryEntityName
		{
			get { return this.registryEntityName; }
		}

		public string TypesEntityName
		{
			get { return this.typesEntityName; }
		}

		public RegistryEntity RegistryEntity { get; private set; }
		public TypesEntity TypesEntity { get; private set; }
		public ScalarType PrimaryIdType { get; private set; }
		public ScalarType TypesEntityPidType { get; private set; }

		public override void Prepare()
		{
			if (this.PrimaryIdType == null)
				this.PrimaryIdType = GetPidType(primaryIdTypeName);
			if (this.TypesEntityPidType == null)
				this.TypesEntityPidType = GetPidType(typesEntityPidTypeName);

			if (this.TypesEntity == null)
				this.TypesEntity = new TypesEntity(this);

			if (this.RegistryEntity == null)
				this.RegistryEntity = new RegistryEntity(this);
		}

		private ScalarType GetPidType(string pidTypeName)
		{
				Metamodel.Type t = Model.Types.FindType(new QualName(pidTypeName));
				if (t == null)
					throw new GlException("Registry pattern: invalid primary id type name: {0}", pidTypeName);
				if (!(t is ScalarType))
					throw new GlException("Registry pattern: primary id type is not scalar: {0}", pidTypeName);
				return t as ScalarType;
		}

		public override void Update()
		{
			base.Update();
			foreach(PatternConfig pattern in owner.Patterns)
			{
				pattern.PatternApply.Exclude(this.RegistryEntity.Entity);
				pattern.PatternApply.Exclude(this.TypesEntity.Entity);
			}
		}

		public override void Implement()
		{
			foreach(Entity entity in Model.Entities)
			{
				if (this.AppliedTo(entity))
				{
					string attrName = String.Format("{0}Id", this.RegistryEntity.Entity.Name);
					if (entity.Attributes.GetByName(attrName) != null)
						throw new GlException(
							"Cannot apply registry pattern to entity '{0}'. Attribute '{1}' already exists. Rename it or exclude entity from pattern",
							entity.FullName,
							attrName);

					Metamodel.Attribute registryAttr = entity.AddAttribute(
						attrName,
						this.RegistryEntity.Entity.PrimaryId.Attribute.Type as ScalarType);
					registryAttr.TypeDefinition.Required = false;

					UniqueId uid = new UniqueId(entity, registryAttr.Name);
					uid.Attributes.Add(registryAttr);
					entity.Constraints.UniqueIds.Add(uid);

					Relation r = new Relation(Model,
					                          entity.Schema,
					                          entity.Name,
					                          String.Empty,
					                          true,
					                          this.RegistryEntity.Entity.Schema,
					                          this.RegistryEntity.Entity.Name,
					                          String.Empty,
					                          false,
					                          true,
					                          Cardinality.R1_1,
					                          entity.Persistence);
					r.AttributesMatch.Add(new RelationAttributeMatch(r, registryAttr, this.RegistryEntity.Entity.PrimaryId.Attribute));
					relations.Add(entity.FullName, r);
					Model.Relations.Add(r);
				}
			}
		}


		internal Entity CreateEntity(string name, ScalarType primaryIdType)
		{
			Entity entity = new Entity(Model, Schema, name);
			if (PersistentSchemaDefined)
			{
				entity.Persistence.Schema = PersistentSchema;
				entity.Persistence.SchemaDefined = true;
			}

			Metamodel.Attribute a = entity.AddAttribute("Id", primaryIdType);
			a.TypeDefinition.Required = true;
			if (PrimaryIdType.IsIntegerValueType)
				a.IsAutoincrement = true;
			entity.Constraints.PrimaryId = new PrimaryId(entity, a);
			return entity;
		}

		#region IRegistryPattern implementation
		public override bool AppliedTo(IEntity entity)
		{
			return !entity.HasSupertype && entity.Persistence.Persisted && base.AppliedTo(entity);
		}

		IRegistryEntity IRegistryPattern.RegistryEntity
		{
			get { return this.RegistryEntity; }
		}

		ITypesEntity IRegistryPattern.TypesEntity
		{
			get { return this.TypesEntity; }
		}

		IScalarType IRegistryPattern.PrimaryIdType
		{
			get { return this.PrimaryIdType; }
		}

		IRelation IRegistryPattern.GetRelation(IEntity entity)
		{
			Relation r;
			relations.TryGetValue(entity.FullName, out r);
			if (r == null)
				throw new GlException("No relation was found. Check that registry pattern is applied to entity '{0}", entity.FullName);
			return r;
		}
		#endregion
	}

	/// <summary>
	/// Registry entity implementation
	/// </summary>
	class RegistryEntity : IRegistryEntity
	{
		public Entity Entity { get; internal set; }
		public Metamodel.Attribute EntityTypeAttribute { get; private set; }
		public Relation TypesRelation { get; private set; }

		public RegistryEntity(RegistryPattern owner)
		{
			if (owner.TypesEntity.Entity == null)
				throw new GlException("TypesEntity.Entity is null");

			this.Entity = owner.CreateEntity(owner.RegistryEntityName, owner.PrimaryIdType);

			Metamodel.Attribute typesPid = owner.TypesEntity.Entity.PrimaryId.Attribute;
			this.EntityTypeAttribute  = new Metamodel.Attribute(
				this.Entity,
				String.Format("EntityType{0}", typesPid.Name),
				typesPid.Type as ScalarType);
			this.EntityTypeAttribute.Migrate(typesPid);
			this.Entity.Attributes.Add(this.EntityTypeAttribute);

			owner.Model.Entities.Add(this.Entity);

			this.TypesRelation = new Relation(
				owner.Model,
				this.Entity.Schema,
				this.Entity.Name,
				String.Empty,
				true,
				owner.TypesEntity.Entity.Schema,
				owner.TypesEntity.Entity.Name,
				String.Empty,
				false,
				true,
				Cardinality.RM_1,
				this.Entity.Persistence);
			this.TypesRelation.AttributesMatch.Add(
				new RelationAttributeMatch(this.TypesRelation,
			                           this.EntityTypeAttribute,
			                           owner.TypesEntity.Entity.PrimaryId.Attribute));
			owner.Model.Relations.Add(this.TypesRelation);
		}

		#region ITypesEntity implementation
		IEntity IRegistryEntity.Entity
		{
			get { return this.Entity; }
		}

		IRelation IRegistryEntity.TypesRelation
		{
			get { return this.TypesRelation; }
		}
		#endregion
	}

	/// <summary>
	/// Types metadata entity implementation
	/// </summary>
	class TypesEntity : ITypesEntity
	{
		public Entity Entity { get; internal set; }
		public Metamodel.Attribute FullNameAttribute { get; internal set; }
		public Metamodel.Attribute ShortNameAttribute { get; internal set; }
		public Metamodel.Attribute DescriptionAttribute { get; internal set; }


		public TypesEntity(RegistryPattern owner)
		{
			this.Entity = owner.CreateEntity(owner.TypesEntityName, owner.TypesEntityPidType);

			ScalarType fullNameType = (ScalarType)owner.Model.Types.GetByName("string");
			fullNameType.TypeDefinition.Length = 255;
			fullNameType.TypeDefinition.Required = true;
			this.FullNameAttribute = this.Entity.AddAttribute("FullName", fullNameType);
			ScalarType shortNameType = (ScalarType)owner.Model.Types.GetByName("string");
			shortNameType.TypeDefinition.Length = 64;
			fullNameType.TypeDefinition.Required = true;
			this.ShortNameAttribute = this.Entity.AddAttribute("ShortName", fullNameType);
			ScalarType descriptionType = (ScalarType)owner.Model.Types.GetByName("string");
			descriptionType.TypeDefinition.Length = 1000;
			descriptionType.TypeDefinition.Required = false;
			this.DescriptionAttribute = this.Entity.AddAttribute("Description", fullNameType);

			UniqueId uid = new UniqueId(this.Entity, this.FullNameAttribute.Name);
			uid.Attributes.Add(this.FullNameAttribute);
			this.Entity.Constraints.UniqueIds.Add(uid);

			owner.Model.Entities.Add(this.Entity);
		}

		#region ITypesEntity implementation
		IEntity ITypesEntity.Entity
		{
			get { return this.Entity; }
		}

		IAttribute ITypesEntity.FullNameAttribute
		{
			get { return this.FullNameAttribute; }
		}

		IAttribute ITypesEntity.ShortNameAttribute
		{
			get { return this.ShortNameAttribute; }
		}

		IAttribute ITypesEntity.DescriptionAttribute
		{
			get { return this.DescriptionAttribute; }
		}
		#endregion
	}
}

