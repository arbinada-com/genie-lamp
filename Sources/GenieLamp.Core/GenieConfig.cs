using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core
{
	class GenieConfig : IGenieConfig
	{
		public const string TargetVersionWildcard = "*";
		private GenieLamp lamp;
		private string name;
		private string assemblyName;
		private string typeName;
		private bool active;
		private string outDir;
		private bool singleOutFile = true;
		private string outFileName;
		private string targetVersion;
		private GenieConfigParams configParams;
		private MacroExpander macro;
		
		#region Constructors
		public GenieConfig(GenieLamp lamp, XmlNode node)
		{
			this.lamp = lamp;
			macro = new MacroExpander(lamp.Macro);
			
			name = Utils.Xml.GetAttrValue(node, "name");
			assemblyName = Utils.Xml.GetAttrValue(node, "assembly");
			typeName = Utils.Xml.GetAttrValue(node, "type");
			active = Utils.Xml.GetAttrValue(node, "active", true);
			
			targetVersion = Environment.ExpandEnvironmentVariables(Utils.Xml.GetAttrValue(node, "targetVersion", TargetVersionWildcard));
			macro.SetMacro("%TARGET_VERSION%", targetVersion);

			outDir = macro.Subst(Lamp.ExpandFileName(Utils.Xml.GetAttrValue(node, "outDir")));
			outFileName = macro.Subst(Lamp.Macro.Subst(Utils.Xml.GetAttrValue(node, "outFileName")));
			this.OutFileEncoding = Encoding.GetEncoding(Utils.Xml.GetAttrValue(node, "outFileEncoding", Encoding.UTF8.BodyName));
			this.UpdateDatabase = Utils.Xml.GetAttrValue(node, "updateDatabase", false);

			configParams = new GenieConfigParams(this, lamp.QueryNode(node, "./{0}:Param"));

			this.AssistantConfigs = new GenieAssistantConfigs(this, node);
			this.Assistants = new List<IGenieAssistant>();
			
			if (!Directory.Exists(outDir))
				Directory.CreateDirectory(outDir);
		}
		#endregion

		public GenieLamp Lamp
		{
			get { return lamp; }
		}

		public MacroExpander Macro
		{
			get { return macro; }
		}

		public GenieAssistantConfigs AssistantConfigs { get; private set; }
		public List<IGenieAssistant> Assistants { get; private set; }

		#region IGenieConfig implementation
		public string Name
		{
			get { return this.name; }
		}
		
		public string AssemblyName
		{
			get { return this.assemblyName; }
		}
		
		public string TypeName
		{ 
			get { return this.typeName; }
		}

		public bool Active
		{
			get { return active; }
		}

		public string OutDir
		{
			get { return outDir; }
		}

		public bool SingleOutFile
		{
			get { return singleOutFile; }
		}

		public string OutFileName
		{
			get { return outFileName; }
		}

		public string TargetVersion
		{
			get { return targetVersion; }
		}

		IParamsSimple IGenieConfig.Params
		{
			get { return configParams; }
		}
		
		IMacroExpander IGenieConfig.Macro
		{
			get { return macro; }
		}

		public void NotifyAssistants(string eventName, Metamodel.IMetaObject sender, string text, params object[] args)
		{
			foreach(IGenieAssistant assistant in this.Assistants)
			{
				assistant.HandleEvent(eventName, sender, text, args);
			}
		}

		public Encoding OutFileEncoding	{ get; private set;	}
		public bool UpdateDatabase	{ get; private set;	}

		#endregion
	}
}

