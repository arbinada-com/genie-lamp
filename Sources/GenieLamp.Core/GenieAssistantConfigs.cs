using System;
using System.Xml;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core
{
	class GenieAssistantConfigs : GlCollection<IGenieAssistantConfig, GenieAssistantConfig>
	{
		#region Constructors
		public GenieAssistantConfigs(GenieConfig parent, XmlNode node)
		{
			this.ParentConfig = parent;
			foreach(XmlNode assistantNode in parent.Lamp.QueryNode(node, "./{0}:Assistant"))
			{
				GenieAssistantConfig assistantConfig = new GenieAssistantConfig(parent, assistantNode);
				this.Add(assistantConfig);
			}
		}
		#endregion

		public GenieConfig ParentConfig { get; private set; }
	}
}

