using System;
using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Layers
{
	class DomainLayerConfig : LayerConfig, IDomainLayerConfig
	{
		public const string LayerName = "Domain";
		public const string SubnamespaceDomain = "Domain";
		public const string SubnamespacePersistence = "Persistence";
		public const string SubnamespaceQueries = "Queries";
		public const string SubnamespacePatterns = "Patterns";
		public const string ClassName_QueryParams = "DomainQueryParams";
		public const string ClassName_SortOrder = "SortOrder";

		private MappingStrategy mappingStrategy = MappingStrategy.TablePerSubclass;

		#region Constructors
		internal DomainLayerConfig(GenieLampConfig owner)
			: base(owner, LayerName)
		{
			if (BaseNamespace == Const.EmptyName)
				throw new GlException("Domain layer configuration: base namespace is empty or parameter is not defined.");
		}

		public DomainLayerConfig(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
			mappingStrategy = ToMappingStrategy(Params.ValueByName("MappingStrategy", Const.EmptyName));
		}
		#endregion

		public static MappingStrategy ToMappingStrategy(string name)
		{
			if (name.Equals("TablePerClass", StringComparison.InvariantCultureIgnoreCase))
			    return MappingStrategy.TablePerClass;
			return MappingStrategy.TablePerSubclass;
		}

		#region IDomainLayerConfig implementation
		public MappingStrategy MappingStrategy
		{
			get { return this.mappingStrategy; }
		}

		public string DomainNamespace
		{
			get	{ return String.Format("{0}.{1}", BaseNamespace, SubnamespaceDomain); }
		}

		public string PersistenceNamespace
		{
			get	{ return String.Format("{0}.{1}", BaseNamespace, SubnamespacePersistence); }
		}

		public string QueriesNamespace
		{
			get { return String.Format("{0}.{1}", BaseNamespace, SubnamespaceQueries); }
		}

		public string PatternsNamespace
		{
			get { return String.Format("{0}.{1}", BaseNamespace, SubnamespacePatterns); }
		}

		public IDomainLayerMethods Methods
		{
			get { return new DomainLayerMethods(this); }
		}

		public string GetClassName_QueryParams(bool fullName)
		{
			return QualName.MakeFullName(fullName ? this.QueriesNamespace : Const.EmptyName, ClassName_QueryParams);
		}

		public string GetClassName_SortOrder(bool fullName)
		{
			return QualName.MakeFullName(fullName ? this.QueriesNamespace : Const.EmptyName, ClassName_SortOrder);
		}
		#endregion






		class DomainLayerMethods : LayerConfig.LayerMethods, IDomainLayerMethods
		{
			private DomainLayerConfig owner;

			public DomainLayerMethods(DomainLayerConfig owner)
			{
				this.owner = owner;
			}

			#region IDomainLayerMethods implementation
			public ILayerMethod Refresh()
			{
				return new LayerMethod("Refresh", Const.EmptyName);
			}

			public ILayerMethod DeleteById(IPrimaryId primaryId, IEnvironmentHelper environment)
			{
				return new LayerMethod("DeleteById", environment.ToParams(primaryId.Attributes));
			}

			public ILayerMethod GetPage()
			{
				return new LayerMethod(
					"GetPage",
					String.Format("int page, int pageSize, {0}.{1}[] sortOrders",
				              owner.QueriesNamespace,
				              DomainLayerConfig.ClassName_SortOrder));
			}

			public ILayerMethod GetByHQL()
			{
				return new LayerMethod(
					"GetByHQL",
					String.Format("string hql, {0}.{1} hqlParams, int firstResult = 0, int maxResult = 100",
				              owner.QueriesNamespace,
				              DomainLayerConfig.ClassName_QueryParams));
			}

			public ILayerMethod GetBySQL()
			{
				return new LayerMethod("GetBySQL", "string sql, int firstResult = 0, int maxResult = 100");
			}

			public ILayerMethod GetPageByHQL()
			{
				return new LayerMethod(
					"GetPageByHQL",
					String.Format("string hql, {0}.{1} hqlParams, int page = 0, int pageSize = 20",
				              owner.QueriesNamespace,
				              DomainLayerConfig.ClassName_QueryParams));
			}

			public ILayerMethod GetPageBySQL()
			{
				return new LayerMethod("GetPageBySQL", "string sql, int page = 0, int pageSize = 20");
			}
			#endregion

		}

	}
}

