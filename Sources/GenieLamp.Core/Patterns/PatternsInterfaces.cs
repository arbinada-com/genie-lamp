using System;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Patterns
{
	public interface IPatterns : IEnumerable<IPatternConfig>
	{
		IStateVersionPattern StateVersion { get; }
		IRegistryPattern Registry { get; }
		IAuditPattern Audit { get; }
		ILocalizationPattern Localization { get; }
		ISecurityPattern Security { get; }
	}

	public interface IPatternConfig
	{
		string Name { get; }
		IParamsSimple Params { get; }
		/// <summary>
		/// Determine whether pattern should be implementd on persistent layer.
		/// Important note.
		/// Activation of this option break portability within some DBMS because of unsupported fetaures.
		/// I.e. triggers are not supported by SQL Server Compact or SQLite so you cannot use it with this option.
		/// See error messages produced by genies.
		/// </summary>
		/// <value>
		/// <c>true</c> if on persistent layer; otherwise, <c>false</c>.
		/// </value>
		bool OnPersistentLayer { get; }
		bool AppliedTo(IEntity entity);
		bool AppliedToEntityOrSupertypes(IEntity entity);
	}

	public interface IRegistryPattern : IPatternConfig
	{
		IRegistryEntity RegistryEntity { get; }
		ITypesEntity TypesEntity { get; }
		IScalarType PrimaryIdType { get; }
		IRelation GetRelation(IEntity entity);
	}

	public interface IRegistryEntity
	{
		IEntity Entity { get; }
		IRelation TypesRelation { get; }
	}

	public interface ITypesEntity
	{
		IEntity Entity { get; }
		IAttribute FullNameAttribute  { get; }
		IAttribute ShortNameAttribute  { get; }
		IAttribute DescriptionAttribute  { get; }
	}

	public interface IAuditPattern : IPatternConfig
	{
		string CreatedByAttributeName  { get; }
		string CreatedDateAttributeName  { get; }
		string LastModifiedByAttributeName  { get; }
		string LastModifiedDateAttributeName  { get; }
	}

	public interface IStateVersionPattern : IPatternConfig
	{
		string AtributeName { get; }
		IScalarType AttributeType { get; }
		string UnsavedValue { get; }
		bool IsUsed(IAttribute attribute);
	}

	public interface ILocalizationPattern : IPatternConfig
	{
	}

	public interface ISecurityPattern : IPatternConfig
	{
	}

}

