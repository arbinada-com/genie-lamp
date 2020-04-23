using System;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core
{
	/// <summary>
	/// Genie lamp singleton class
	/// </summary>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
    public class GenieLamp : IGenieLamp
    {
		public const string GLProjectNamespaceName = "gl";
		public const string GLModelNamespaceName = "glm";
		
        private Genies genies = null;
        private Model model = null;
		private CodeWritersFactory codeWritersFactory = null;
        private GenieLampConfig config = null;
		private ILogger logger = null;
		private IGenieLampSpellConfig spellConfig;
		private string projectName;
		private string projectVersion;
		private string projectFileName;
		private string projectDirectory;
		private string xmlNamespace;
		private MacroExpander macro = new MacroExpander();

		#region Constructors
        private GenieLamp(IGenieLampSpellConfig spellConfig, ILogger logger)
		{
			this.logger = logger;
			this.spellConfig = spellConfig;
		}

		public static GenieLamp Instance { get; private set; }

		public static GenieLamp CreateGenieLamp(IGenieLampSpellConfig spellConfig, ILogger logger)
		{
			if (Instance == null)
				Instance = new GenieLamp(spellConfig, logger);
			return Instance;
		}
		#endregion
		
        public void Init()
        {
			projectFileName = Path.GetFullPath(System.Environment.ExpandEnvironmentVariables(spellConfig.FileName));
			projectDirectory = Path.GetDirectoryName(projectFileName);
			
			macro.SetMacro("%PROJECT_DIR%", projectDirectory);
			macro.SetMacro("%PROJECT_FILENAME%", projectFileName);
			
			codeWritersFactory = new CodeWritersFactory(this);
			
			GenieLampLoader loader = new GenieLampLoader(this);
			XmlDocument projectDoc = loader.LoadFile(ProjectFileName, GLProjectNamespaceName, "GenieLamp.Core.XMLSchema.GenieLamp.xsd");
			xmlNamespace = loader.XmlNamespace;
			
			projectName = Utils.Xml.GetAttrValue(projectDoc.DocumentElement, "project");
			macro.SetMacro("%PROJECT_NAME%", projectName);
			projectVersion = Utils.Xml.GetAttrValue(projectDoc.DocumentElement, "version");
			macro.SetMacro("%PROJECT_VERSION%", projectVersion);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(projectDoc.NameTable);
            nsmgr.AddNamespace(GLProjectNamespaceName, xmlNamespace);
			
			config = new GenieLampConfig(this, projectDoc.SelectSingleNode(String.Format("//{0}:Configuration", GLProjectNamespaceName), nsmgr));

            genies = new Genies(this, projectDoc.SelectNodes(String.Format("//{0}:Genie", GLProjectNamespaceName), nsmgr));
			
            model = new Model(this);
			XmlNodeList modelFileNodes = projectDoc.SelectNodes(String.Format("//{0}:ImportModel", GLProjectNamespaceName), nsmgr);
			foreach(XmlNode importModelNode in modelFileNodes)
			{
				model.AddModelDoc(ExpandFileName(Utils.Xml.GetAttrValue(importModelNode, "fileName"), ProjectDirectory));
			}
			model.Update();
			model.Check();

			config.Update();
			model.ImplementPatterns();
        }
		
        public void Spell()
        {
            logger.TraceLine("Spell started");
            foreach (IGenie genie in genies)
            {
				if (!genie.Config.Active)
					continue;

                logger.Trace(String.Format("{0} is working...", genie.Name));
				logger.Echo = false;
                try
                {
                    genie.Spell(model);
					if (genie is IGenieOfPersistence && genie.Config.UpdateDatabase)
					{
						logger.Echo = true;
						(genie as IGenieOfPersistence).UpdateDatabase();
					}
					logger.Echo = true;
                    logger.TraceLine("done");
                }
                catch (Exception e)
                {
					logger.Echo = true;
                    logger.TraceLine("FAILED");
                    throw new Exception(e.Message, e);
                }
            }
            logger.TraceLine("All done");
#if DEBUG
			DumpModel();
#endif
        }
		
		public void DumpModel()
		{
			if (Model != null)
				Model.Dump(Path.Combine(ProjectDirectory, "model_dump.txt"));
		}
		
		public XmlNodeList QueryNode(XmlNode lampNode, string query)
		{
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(lampNode.OwnerDocument.NameTable);
			nsmgr.AddNamespace(GenieLamp.GLModelNamespaceName, xmlNamespace);
			return lampNode.SelectNodes(String.Format(query, GenieLamp.GLModelNamespaceName), nsmgr);
		}
		
		public string ExpandFileName(string fileName)
		{
			return ExpandFileName(fileName, System.Environment.CurrentDirectory);
		}
		
		public string ExpandFileName(string fileName, string currentDirectory)
		{
			string newName = macro.Subst(fileName);
			string currDir = System.Environment.CurrentDirectory;
			try
			{
				System.Environment.CurrentDirectory = currentDirectory;
				newName = Path.GetFullPath(newName); // Relative to current directory
			}
			finally
			{
				System.Environment.CurrentDirectory = currDir;
			}
			return newName;	
		}
		
		public string GetTitle()
		{
			return "Genie Lamp Core";
		}
		
        internal GenieLampConfig Config
		{
            get { return config; }
        }

		internal MacroExpander Macro
		{
			get { return macro; }
		}
		
        internal Model Model
		{
            get { return this.model; }
        }

        #region IGenieLamp implementation
		public Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}
			
        IGenieLampConfig IGenieLamp.Config
		{
            get { return this.config; }
        }
		
        public IGenies Genies
		{
            get { return this.genies; }
        }

		public ILogger Logger
		{
			get { return this.logger; }
		}
		
        IModel IGenieLamp.Model
		{
            get { return this.model; }
        }

		public ICodeWritersFactory CodeWritersFactory
		{
			get { return this.codeWritersFactory; }
		}
		
		public string ProjectName
		{
			get { return this.projectName; }
		}		
		
		public string ProjectVersion
		{
			get { return this.projectVersion; }
		}

		public string ProjectFileName
		{
			get { return projectFileName; }
		}

		public string ProjectDirectory
		{
			get { return projectDirectory; }
		}

		IMacroExpander IGenieLamp.Macro
		{
			get { return macro; }
		}

		public IGenieLampUtils GenieLampUtils
		{
			get { return new GenieLampUtils(this); }
		}
		#endregion
    }
}

