using System;
using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Layers
{
	class ServicesLayerConfig : LayerConfig, IServicesLayerConfig
	{
		public const string LayerName = "Services";
		public const string SubnamespaceServices = "Services";
		public const string SubnamespaceServicesAdapters = SubnamespaceServices + ".Adapters";
		public const string SubnamespaceServicesInterfaces = SubnamespaceServices + ".Interfaces";

		#region Constructors
		internal ServicesLayerConfig(GenieLampConfig owner)
			: base(owner, LayerName)
		{
			if (BaseNamespace == Const.EmptyName)
				throw new GlException("Services layer configuration: base namespace is empty or parameter is not defined.");
		}

		public ServicesLayerConfig(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
		}
		#endregion

		#region IServicesLayerConfig implementation
		public string ServicesNamespace
		{
			get	{ return String.Format("{0}.{1}", BaseNamespace, SubnamespaceServices); }
		}

		public string ServicesAdaptersNamespace
		{
			get	{ return String.Format("{0}.{1}", BaseNamespace, SubnamespaceServicesAdapters); }
		}

		public string ServicesInterfacesNamespace
		{
			get	{ return String.Format("{0}.{1}", BaseNamespace, SubnamespaceServicesInterfaces); }
		}

		public IServicesLayerMethods Methods
		{
			get { return new ServicesLayerMethods(); }
		}
		#endregion

		class ServicesLayerMethods : LayerConfig.LayerMethods, IServicesLayerMethods
		{
			#region ILayerMethods implementation
			public ILayerMethod GetProxyById(IPrimaryId primaryId, IEnvironmentHelper environment)
			{
				return new LayerMethod(String.Format("Get{0}ById", primaryId.Entity.Name),
				                       environment.ToParams(primaryId.Attributes));
			}

			public ILayerMethod GetProxyByUniqueId(IUniqueId uniqueId, IEnvironmentHelper environment)
			{
				return new LayerMethod(String.Format("Get{0}By{1}", uniqueId.Entity.Name, uniqueId.Attributes.ToNamesString("")),
				                       environment.ToParams(uniqueId.Attributes));
			}
			#endregion


		}

	}
}

