using System;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Layers
{
	public interface ILayersConfig : IEnumerable<ILayerConfig>
	{
		IPersistenceLayerConfig PersistenceConfig { get; }
		IDomainLayerConfig DomainConfig { get; }
		IServicesLayerConfig ServicesConfig { get; }
	}
	
	public interface ILayerConfig
	{
		bool IsDefined { get; }
		string Name { get; }
		string Subname { get; }
		INamingConvention NamingConvention { get; }
		IParamsSimple Params { get; }
		IParamsSimple LocalizationParams { get; }
		string BaseNamespace { get; }
	}

	/// <summary>
	/// Object to relations mapping strategy
	/// See http://nhforge.org/doc/nh/en/index.html#inheritance
	/// </summary>
	public enum MappingStrategy
	{
		TablePerSubclass, // NHibernate: table per subclass with "joined-subclass"
		TablePerClass // NHibernate: table per concrete class with "union-subclass"
	}

	public interface IDomainLayerConfig : ILayerConfig
	{
		string DomainNamespace { get; }
		string PersistenceNamespace { get; }
		string QueriesNamespace { get; }
		string PatternsNamespace { get; }
		IDomainLayerMethods Methods { get; }
		string GetClassName_QueryParams(bool fullName);
		string GetClassName_SortOrder(bool fullName);
		MappingStrategy MappingStrategy { get; }
	}

	public interface IServicesLayerConfig : ILayerConfig
	{
		string ServicesNamespace { get; }
		string ServicesInterfacesNamespace { get; }
		string ServicesAdaptersNamespace { get; }
		IServicesLayerMethods Methods { get; }
	}

	public enum BooleanValuePersistence
	{
		Native,
		YesNo,
		TrueFalse
	}

	public interface IPersistenceLayerConfig : ILayerConfig
	{
		BooleanValuePersistence BooleanValuePersistence { get; }
	}

	public interface ILayerMethods
	{
		ILayerMethod GetById(IPrimaryId primaryId, IEnvironmentHelper environment);
		ILayerMethod GetByUniqueId(IUniqueId uniqueId, IEnvironmentHelper environment);
		ILayerMethod GetByRelationParent(IRelation relation, IEnvironmentHelper environment);
	}

	public interface IDomainLayerMethods : ILayerMethods
	{
		ILayerMethod Refresh();
		ILayerMethod DeleteById(IPrimaryId primaryId, IEnvironmentHelper environment);
		ILayerMethod GetPage();
		ILayerMethod GetByHQL();
		ILayerMethod GetPageByHQL();
		ILayerMethod GetBySQL();
		ILayerMethod GetPageBySQL();
	}

	public interface IServicesLayerMethods : ILayerMethods
	{
		ILayerMethod GetProxyById(IPrimaryId primaryId, IEnvironmentHelper environment);
		ILayerMethod GetProxyByUniqueId(IUniqueId uniqueId, IEnvironmentHelper environment);
	}

	public interface ILayerMethod
	{
		string Name { get; }
		string Signature { get; }
		string Parameters { get; }
		string Call(params object[] args);
	}
}

