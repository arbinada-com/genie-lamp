using System;
using System.Xml;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core
{
	class GenieAssistantConfig : IGenieAssistantConfig
	{
		#region Constructors
		public GenieAssistantConfig(GenieConfig parentConfig, XmlNode node)
		{
			this.Name = Utils.Xml.GetAttrValue(node, "name");
			this.AssemblyName = Utils.Xml.GetAttrValue(node, "assembly");
			this.TypeName = Utils.Xml.GetAttrValue(node, "type");

			this.ParentConfig = parentConfig;
			this.Params = new ParamsSimple(parentConfig.Lamp.QueryNode(node, "./{0}:Param"), parentConfig.Macro);
		}
		#endregion

		public string Name { get; private set; }
		public string AssemblyName { get; private set; }
		public string TypeName { get; private set; }
		public GenieConfig ParentConfig { get; private set; }
		public ParamsSimple Params { get; private set; }

		#region IGenieAssistantConfig implementation
		IGenieConfig IGenieAssistantConfig.ParentConfig
		{
			get { return this.ParentConfig; }
		}

		IParamsSimple IGenieAssistantConfig.Params
		{
			get { return this.Params; }
		}
		#endregion
	}
}

