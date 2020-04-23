using System;
using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Layers
{
	class LayerConfig : ILayerConfig
	{
		private GenieLampConfig owner;
		private string name;
		private string subname;
		private NamingConvention namingConvention;
		private LayerConfigParams layerConfigParams;
		private MacroExpander macro;
		bool isDefined = false;

		#region Constructors
		internal LayerConfig(GenieLampConfig owner, string name)
		{
			Init(owner, name);
		}

		public LayerConfig(GenieLampConfig owner, XmlNode node)
		{
			Init(owner, Utils.Xml.GetAttrValue(node, "name"));
			isDefined = true;
			this.subname = Utils.Xml.GetAttrValue(node, "subname", Const.EmptyName);

			XmlNodeList namingConvNodes = owner.Lamp.QueryNode(node, "./{0}:NamingConvention");
			if (namingConvNodes.Count == 0)
				namingConvention = new NamingConvention(this);
			else
				namingConvention = new NamingConvention(this, namingConvNodes[0]);
			layerConfigParams = new LayerConfigParams(this, owner.Lamp.QueryNode(node, "./{0}:Param"));
			this.LocalizationParams = new LayerLocalizationParams(this, owner.Lamp.QueryNode(node, "./{0}:Localization/{0}:Param"));
		}

		private void Init(GenieLampConfig owner, string name)
		{
			this.owner = owner;
			this.name = name;
			namingConvention = new NamingConvention(this);
			layerConfigParams = new LayerConfigParams(this);
			macro = new MacroExpander(owner.Macro);
			this.Keywords = new LayerKeywords();
			this.LocalizationParams = new LayerLocalizationParams(this);
		}
		#endregion
		
		public LayerKeywords Keywords { get; protected set; }
		public LayerLocalizationParams LocalizationParams { get; private set; }

		public GenieLampConfig GenieLampConfig
		{
			get { return owner; }
		}

		public static string MakeFullName(string name, string subname)
		{
			return String.Format("{0}{1}", name, subname.Equals(Const.EmptyValue) ? Const.EmptyValue : "." + subname);
		}
		
		public string FullName
		{
			get { return MakeFullName(name, subname); }
		}
		
		public NamingConvention NamingConvention
		{
			get { return namingConvention; }
		}
		
		public MacroExpander Macro
		{
			get { return macro; }
		}

		public ParamsSimple Params
		{
			get { return this.layerConfigParams; }
		}

		#region ILayerConfig implementation
		public string Name
		{
			get { return name; }
		}
	
		public string Subname
		{
			get { return subname; }
		}
	
		INamingConvention ILayerConfig.NamingConvention
		{
			get { return this.namingConvention; }
		}
	
		IParamsSimple ILayerConfig.Params
		{
			get { return this.layerConfigParams; }
		}

		public string BaseNamespace
		{
			get { return this.Params.ValueByName("BaseNamespace", this.GenieLampConfig.Lamp.ProjectName); }
		}

		public bool IsDefined
		{
			get { return this.isDefined; }
		}

		IParamsSimple ILayerConfig.LocalizationParams
		{
			get { return this.LocalizationParams; }
		}
		#endregion


		public class LayerMethods : ILayerMethods
		{
			#region ILayerMethods implementation
			public ILayerMethod GetById(IPrimaryId primaryId, IEnvironmentHelper environment)
			{
				return new LayerMethod("GetById", environment.ToParams(primaryId.Attributes));
			}

			public ILayerMethod GetByUniqueId(IUniqueId uniqueId, IEnvironmentHelper environment)
			{
				return new LayerMethod(String.Format("GetBy{0}", uniqueId.Attributes.ToNamesString("")),
				                       environment.ToParams(uniqueId.Attributes));
			}

			public ILayerMethod GetByRelationParent(IRelation relation, IEnvironmentHelper environment)
			{
				return new LayerMethod(String.Format("GetCollectionBy{0}", relation.ChildAttributes.ToNamesString("")),
				                       environment.ToParams(relation.ChildAttributes));
			}
			#endregion
		}
	}
}

