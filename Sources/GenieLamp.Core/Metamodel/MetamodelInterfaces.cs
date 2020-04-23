using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using GenieLamp.Core.CodeWriters;

namespace GenieLamp.Core.Metamodel
{
	public interface IConsistency
	{
		void Check();
	}
	
    public interface IModel : IConsistency
    {
		IGenieLamp Lamp { get; }
		IGenerators Generators { get; }
		ISchemas Schemas { get; }
        IEntities Entities { get; }
		ITypes Types { get; }
		IEnumerations Enumerations { get; }
		IRelations Relations { get; }
		ISpellHints SpellHints { get; }
		string DefaultSchema { get; }
		string XmlNamespace { get; }
		Physical.IPhysicalModel PhysicalModel { get; }
		IMetaObjectCollection<IMetaObject> MetaObjects { get; }
		void Dump(string fileName);

    }
	
	public interface IMetaObject
	{
		string Name { get; }
        string FullName { get; }
		IModel Model { get; }
		IMetaObject Owner { get; }
		bool HasDoc { get; }
		IDoc Doc { get; }
		IParamsSimple SpellHintParams { get; }
		bool Processed { get; set; }
		bool Equals(IMetaObject metaObject);
		bool DefinedInModel { get; }
	}

	public interface ISpellHint : IMetaObject
	{
		string GenieName { get; }
		string TargetVersion { get; }
		string TargetName { get; }
		string TargetType { get; }
		string Text { get; }
		/// <summary>
		/// Returns spell hint text depending on  value of spell hint parameters' source
		/// If no parameters was matched returns Text
		/// </summary>
		/// <returns>
		/// Spell hint text.
		/// </returns>
		/// <param name='source'>
		/// Source metaobject to call spell with hint.
		/// </param>
		string GetText(IMetaObject source);
		/// <summary>
		/// Spell hint properties provided for genie
		/// </summary>
		/// <value>
		/// Spell hint properties collection.
		/// </value>
		IParamsSimple Properties { get; }
	}

	public interface ISpellHints : IMetaObjectCollection<ISpellHint>
	{
		ISpellHint Find(string genieName, string targetVersion, IMetaObject source);
		ISpellHint Find(string genieName, string targetVersion, string targetType, string targetName);
	}

	public interface ISchema : IMetaObject, IPersistent
	{
	}

	public interface ISchemas : IMetaObjectNamedCollection<ISchema>
	{
	}

	public interface IDoc : IMetaObject
	{
		XmlNode TextNode { get; }
		string Text { get; }
		string[] LabelLines { get; }
		string[] TextLines { get; }
		XmlNodeList LabelNodes { get; }
		XmlNodeList TextNodes { get; }
		string GetText(CultureInfo ci = null);
		string GetLabel(CultureInfo ci = null);
	}

	public interface IMetaObjectSchemed : IMetaObject
	{
		string Schema { get; }
	}
	
	public interface IPersistent
	{
		IPersistence Persistence { get; }
	}
	
	public interface IPersistence
	{
		bool Persisted { get; }
		bool SchemaDefined { get; }
		string Schema { get; }
		bool NameDefined { get; }
		string Name { get; }
		bool TypeDefined { get; }
		string TypeName { get; }
		string FullName { get; }
	}
	
	public interface IMetaObjectCollection<IT> : IEnumerable<IT> where IT : IMetaObject
	{
		int Count { get; }
        IT this[int index] { get; }
		IList ToList();
		void SetUnprocessedAll();
	}
	
	public interface IMetaObjectNamedCollection<IT> : IMetaObjectCollection<IT> where IT : IMetaObject
	{
        IT this[string name] { get; }
	}

	#region Generators
	public interface IGenerators : IMetaObjectCollection<IGenerator>
	{ }
	
	public interface IGenerator : IMetaObjectSchemed, IPersistent
	{
		GeneratorType Type { get; }
		int StartWith { get; }
		int Increment { get; }
		bool HasMaxValue { get; }
		decimal MaxValue { get; }
		bool HasMinValue { get; }
		decimal MinValue { get; }
	}
	
	public enum GeneratorType
	{
		Sequence,
		Guid,
		GuidHex
	}
	#endregion
	


	#region Entities
    public interface IEntities :
		IMetaObjectNamedCollection<IEntity>,
		IConsistency
    { }

    public interface IEntity : IMetaObjectSchemed, IConsistency, IPersistent
    {
		int Id { get; }
		IEntityType Type { get; }
        IEntity Supertype { get; }
		IEntityAttributes Attributes { get; }
		/// <summary>
		/// Gets entity attributes collection
		/// Equivalent to "Attributes" property but can include all supertypes attributes
		/// </summary>
		/// <returns>
		/// Entity and its supertypes attributes
		/// </returns>
		/// <param name='includeSupertype'>
		/// Include supertypes' atrtributes recursively
		/// </param>
		IAttributes GetAttributes(bool includeSupertype);
		IEntityConstraints Constraints { get; }
		/// <summary>
		/// Shortcut for Constraints.PrimaryId
		/// </summary>
		/// <value>
		/// Returns entity primary identifier or null.
		/// </value>
        IPrimaryId PrimaryId { get; }
		IEntityEventHandlers EventHandlers { get; }
		IEntityOperations Operations { get; }
		bool HasSupertype { get; }
		IRelations Relations { get; }
		/// <summary>
		/// Gets the relations with parent entities.
		/// </summary>
		/// <value>
		/// The parent relations.
		/// </value>
		IRelations Parents { get; }
		/// <summary>
		/// Gets the relations with child entites.
		/// </summary>
		/// <value>
		/// The child relations.
		/// </value>
		IRelations Children { get; }
		IRelations OneToOne { get; }
		IRelations ManyToMany { get; }
	}

	public enum EntityEventHandlerType
	{
		Save,
		Delete,
		Flush,
		Validate
	}

	public interface IEntityEventHandler : IMetaObject
	{
		EntityEventHandlerType Type { get; }
	}
	
	public interface IEntityEventHandlers : IMetaObjectCollection<IEntityEventHandler>
	{
	}

	public interface IEntityOperation : IMetaObject
	{
		IEntity Entity { get; }
		EntityOperationAccess Access { get; }
		IEntityOperationParams Params { get; }
	}

	public enum EntityOperationAccess
	{
		Public,
		Internal
	}

	public interface IEntityOperationParam : IMetaObject
	{
        IType Type { get; }
		ITypeDefinition TypeDefinition { get; }
		bool IsRaw { get; }
		bool IsRef { get; }
		bool IsOut { get; }
	}

	public interface IEntityOperationReturn : IEntityOperationParam
	{
	}

	public interface IEntityOperationParams : IMetaObjectCollection<IEntityOperationParam>
	{
		bool HasReturns { get; }
		IEntityOperationReturn Returns { get; }
	}

	public interface IEntityOperations : IMetaObjectCollection<IEntityOperation>
	{
	}

	public interface IEntityConstraints : IConsistency
	{
        IPrimaryId PrimaryId { get; }
		IUniqueIdConstraints UniqueIds { get; }
	}
	
	public interface IEntityConstraint : IMetaObject, IPersistent
	{
		IAttributes Attributes { get; }
		bool Composite { get; }
		IEntity Entity { get; }
		bool ContainsAttribute(IAttribute attribute);
		bool MatchAttributes(IAttributes attributes);
	}
	
	public interface IPrimaryId : IEntityConstraint, IConsistency
	{ 
		bool HasGenerator { get; }
		IGenerator Generator { get; }
		bool IsAutoincrement { get; }
		Physical.IIndex Index { get; }
	}
	
	public interface IUniqueIdConstraints : IMetaObjectCollection<IUniqueId>
	{ }
	
	public interface IUniqueId : IEntityConstraint, IConsistency
	{ 
		Physical.IIndex Index { get; }
	}

    public interface IAttributes : IMetaObjectNamedCollection<IAttribute>
    { 
		bool Contains(IAttribute attribute);
		/// <summary>
		/// Return attributes names as string separated by given string value
		/// </summary>
		/// <returns>
		/// The names string.
		/// </returns>
		/// <param name='separator'>
		/// Separator.
		/// </param>
		string ToNamesString(string separator);
		/// <summary>
		/// Return attributes names as string separated by given value.
		/// Every name will be prefixed and suffixed by given values.
		/// </summary>
		/// <returns>
		/// The names string.
		/// </returns>
		/// <param name='separator'>
		/// Separator.
		/// </param>
		/// <param name='prefix'>
		/// Prefix.
		/// </param>
		/// <param name='suffix'>
		/// Suffix.
		/// </param>
		string ToNamesString(string separator, string prefix, string suffix);
		string ToArguments();
		string ToPersistentNamesString(string separator);
		int PersistentCount { get; }
		uint GetNamesHash(string prefix);
	}
	
    public interface IEntityAttributes : IAttributes
	{
		IEntity Entity { get; }
	}
	
    public interface IAttribute : IMetaObject, IPersistent
    {
        IType Type { get; }
		ITypeDefinition TypeDefinition { get; }
		bool IsPrimaryId { get; }
		bool ReadOnly { get; }
		IEntity Entity { get; }
		IRelations UsedInRelations { get; }
		bool IsUsedInRelations { get; }
		bool HasEnumerationType { get; }
		/// <summary>
		/// Indicates whether this <see cref="GenieLamp.Core.Metamodel.IAttribute"/> should be processed in relations.
		/// If not, process it as ordinary attribute
		/// </summary>
		/// <value>
		/// <c>true</c>;<c>false</c>.
		/// </value>
		bool ProcessInRelations { get; }
		/// <summary>
		/// Whether attribute was declared explicitely in model
		/// </summary>
		/// <value>
		/// <c>true</c>, <c>false</c>.
		/// </value>
		bool IsDeclared { get; }
		bool IsMigrated { get; }
		IAttributeMigration Migration { get; }
		IRelation GetManyToOne();
		IAttribute GetManyToOneRelatedAttribute();
    }
	
    public interface IAttributeMigration
	{
		IEntity RelatedEntity { get; }
		string Name { get; }
		IAttribute Source { get; }
	}
		
	public enum CollectionType
	{
		None,
		Array,
		List
	}

	public interface ITypeDefinition
	{
		bool HasLength { get; }
		int Length { get; }
		bool HasPrecision { get; }
		int Precision { get; }
		bool HasFixed { get; }
		bool Fixed { get; }
		bool HasRequired { get; }
		bool Required { get; }
		bool HasDefault { get; }
		string Default { get; }
		CollectionType CollectionType { get; }
	}

	public interface IAttributeTypeDefinitionImported : ITypeDefinition
	{
		new int Length { get; set; }
		new int Precision { get; set; }
		new bool Fixed { get; set; }
		new bool Required { get; set; }
		new string Default { get; set; }
	}
	#endregion
	
	#region Types
    public interface ITypes : IMetaObjectNamedCollection<IType>
    { }
	
	public enum BaseType
	{
		TypeVoid,
		TypeByte,
		TypeCurrency,
		TypeDecimal,
        TypeFloat,
        TypeInt,
		// Date & time types
        TypeDate,
        TypeTime,
        TypeDateTime,
		// Character types
        TypeString, // unicode string
        TypeAnsiString, // single byte string
        TypeChar,  // unicode char
        TypeAnsiChar,
		// Other types
        TypeBool,
        TypeBinary,
        TypeUuid
	}
	
	public interface IType : IMetaObjectSchemed
	{ }
	
	public interface IScalarType : IType
	{
		BaseType BaseType { get; }
		ITypeDefinition TypeDefinition { get; }
		bool IsIntegerValueType { get; }
	}
	
	public interface IEntityType : IType
	{
		IEntity Entity { get; }
	}
	
	public interface IEnumerationType : IType
	{
		IEnumeration Enumeration { get; }
		int Length { get; }
	}
	
	
	public interface IEnumerations : 
		IMetaObjectNamedCollection<IEnumeration>,
		IConsistency
	{ }
	
	public interface IEnumeration : 
		IMetaObjectSchemed, 
		IConsistency,
		IPersistent
	{
		IEnumerationItems Items { get; }
		IEnumerationItem DefaultItem { get; }
	}
	
	public interface IEnumerationItems : 
		IMetaObjectNamedCollection<IEnumerationItem>
	{ }
	
	public interface IEnumerationItem : IMetaObject, IPersistent
	{
		bool HasValue { get; }
        int Value { get; }
		bool IsDefault { get; }
	}
	#endregion
	
	#region Relations
	public enum Cardinality
	{
        R1_1,
        R1_M,
        RM_1,
        RM_M
	}
	
	public enum RelationSide
	{
		None,
		Left,
		Right
	}

	public interface IRelations : 
		IMetaObjectCollection<IRelation>,
		IConsistency
	{ }
	
	public interface IRelation : 
		IMetaObject,
		IPersistent,
		IConsistency
	{
		IEntity Entity { get; }
		bool Navigate { get; }
		string Name2  { get; }
		IEntity Entity2 { get; }
		bool Navigate2 { get; }
		Cardinality Cardinality { get; }
		bool Required { get; }
		RelationCascade Cascade { get; }
		IRelationAttributesMatch AttributesMatch { get; }
		Physical.IForeignKey ForeignKey { get; }
		bool HasIndexName { get; }
		string IndexName { get; }
		//
		RelationSide ParentSide { get; }
		RelationSide ChildSide { get; }
		IEntity ParentEntity { get; }
		IEntity ChildEntity { get; }
		IAttributes ParentAttributes { get; }
		IAttributes ChildAttributes { get; }
		bool ParentNavigate { get; }
		bool ChildNavigate { get; }
		string ParentName { get; }
		string ChildName { get; }
		bool IsParent(IEntity entity);
		bool IsChild(IEntity entity);
	}
	
	public interface ISubtypeRelation : IRelation
	{ }

	public enum RelationCascade
	{
		None,
		All,
		Delete,
		Update
	}
	
	public interface IRelationAttributesMatch : IMetaObjectCollection<IRelationAttributeMatch>
	{
		bool ContainsAttribute(IAttribute attribute);
		IAttributes Attributes { get; }
		IAttributes Attributes2 { get; }
	}
	
	public interface IRelationAttributeMatch : 
		IMetaObject, 
		IConsistency
	{
		IAttribute Attribute { get; }
		IAttribute Attribute2 { get; }
	}
	#endregion
}

