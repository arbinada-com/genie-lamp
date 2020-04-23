using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Layers
{
	class LayersConfig : GlNamedCollection<ILayerConfig, LayerConfig>, ILayersConfig
	{
		private PersistenceLayerConfig persistenceConfig;
		private DomainLayerConfig domainConfig;
		private ServicesLayerConfig servicesConfig;

		public LayersConfig(GenieLampConfig owner, XmlNode node)
			: base()
		{
			XmlNodeList nodeList = owner.Lamp.QueryNode(node, "./{0}:Layer");
			foreach (XmlNode layerNode in nodeList)
			{
				LayerConfig layer = new LayerConfig(owner, layerNode);
				if (layer.FullName.Equals(PersistenceLayerConfig.LayerName, StringComparison.InvariantCultureIgnoreCase))
				{
					persistenceConfig = new PersistenceLayerConfig(owner, layerNode);
				}
				else if (layer.FullName.Equals(DomainLayerConfig.LayerName, StringComparison.InvariantCultureIgnoreCase))
				{
					domainConfig = new DomainLayerConfig(owner, layerNode);
				}
				else if (layer.FullName.Equals(ServicesLayerConfig.LayerName, StringComparison.InvariantCultureIgnoreCase))
				{
					servicesConfig = new ServicesLayerConfig(owner, layerNode);
				}
				this.Add(layer.FullName, layer);
			}
			
			if (persistenceConfig == null)
			{
				owner.Lamp.Logger.Warning(WarningLevel.Medium, "Persistent layer configuration was not found");
				persistenceConfig = new PersistenceLayerConfig(owner);
			}
			if (domainConfig == null)
			{
				owner.Lamp.Logger.Warning(WarningLevel.Medium, "Domain layer configuration was not found");
				domainConfig = new DomainLayerConfig(owner);
			}
			if (servicesConfig == null)
			{
				owner.Lamp.Logger.Warning(WarningLevel.Medium, "Services layer configuration was not found");
				servicesConfig = new ServicesLayerConfig(owner);
			}
		}
		
		public LayerConfig GetByName(string name, string subname, bool throwException = false)
		{
			return GetByName(LayerConfig.MakeFullName(name, subname), throwException);
		}
		
		public PersistenceLayerConfig PersistenceConfig
		{
			get { return persistenceConfig; }
		}
				         
		public DomainLayerConfig DomainConfig
		{
			get { return domainConfig; }
		}

		public ServicesLayerConfig ServicesConfig
		{
			get { return servicesConfig; }
		}

		#region ILayersConfig implementation
		IPersistenceLayerConfig ILayersConfig.PersistenceConfig
		{
			get { return this.persistenceConfig; }
		}

		IDomainLayerConfig ILayersConfig.DomainConfig
		{
			get { return this.domainConfig; }
		}

		IServicesLayerConfig ILayersConfig.ServicesConfig
		{
			get { return this.servicesConfig; }
		}
		#endregion

	}
}

