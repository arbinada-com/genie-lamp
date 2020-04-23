using System;
using System.IO;

using GenieLamp.Core;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;
using GenieLamp.Core.Patterns;

namespace GenieLamp.Genies.NHibernate
{
	public abstract class CodeGenBase
	{
		protected NHibernateGenie genie;
		protected ICodeWriterCSharp cw;
		protected IDocHelper map;
		protected IEnvironmentHelper environment;
		protected string targetAssemblyName;
		protected string outFileName;
		protected ICSharpNamespaceWriter nswriter;

		public CodeGenBase(NHibernateGenie genie)
		{
			this.genie = genie;
			genie.Model.MetaObjects.SetUnprocessedAll();

			if (!genie.Lamp.Config.Layers.DomainConfig.IsDefined)
				throw new ApplicationException("Domain layer configuration is not defined in project");
			if(DomainLayerConfig.BaseNamespace == Const.EmptyName)
				throw new ApplicationException("Domain layer configuration: base namespace is not defined");

			string defaultBaseNamespace = Path.GetFileNameWithoutExtension(genie.Config.OutFileName);
			targetAssemblyName = genie.Config.Params.ParamByName("TargetAssemblyName", defaultBaseNamespace).Value;
			environment = genie.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.CSharp);
			environment.BaseNamespace = DomainLayerConfig.BaseNamespace;

			genie.Config.Macro.SetMacro("%TARGET_ASSEMBLY_NAME%", targetAssemblyName);
			genie.Config.Macro.SetMacro("%BASE_NAMESPACE%", DomainLayerConfig.BaseNamespace);
			genie.Config.Macro.SetMacro("%DOMAIN_NAMESPACE%", DomainLayerConfig.DomainNamespace);
			genie.Config.Macro.SetMacro("%PERSISTENCE_NAMESPACE%", DomainLayerConfig.PersistenceNamespace);
			genie.Config.Macro.SetMacro("%QUERING_NAMESPACE%", DomainLayerConfig.QueriesNamespace);
			genie.Config.Macro.SetMacro("%PATTERNS_NAMESPACE%", DomainLayerConfig.PatternsNamespace);
		}

		protected ILogger Logger
		{
			get { return genie.Lamp.Logger; }
		}

		public IModel Model
		{
			get { return genie.Model; }
		}

		public IPersistenceLayerConfig PersistenceLayerConfig
		{
			get { return genie.Lamp.Config.Layers.PersistenceConfig; }
		}

		public IDomainLayerConfig DomainLayerConfig
		{
			get { return genie.Lamp.Config.Layers.DomainConfig; }
		}

		public IPatterns Patterns
		{
			get { return genie.Lamp.Config.Patterns; }
		}

		public virtual void Run()
		{
			environment.BaseNamespace = DomainLayerConfig.DomainNamespace;

			cw = genie.Model.Lamp.CodeWritersFactory.CreateCodeWriterCSharp();
			cw.WriteStdHeader(genie);

			nswriter = cw.CreateNamespaceWriter();

			InternalRun();

			cw.Save(Path.Combine(genie.Config.OutDir, outFileName));
		}

		protected abstract void InternalRun();


		public static string RelationCascadeToNHValue(RelationCascade cascade)
		{
			switch (cascade)
			{
			case RelationCascade.None:
				return "none";
			case RelationCascade.Delete:
				return "delete";
			case RelationCascade.Update:
				return "save-update";
			case RelationCascade.All:
				return "all";
			default:
				throw new GlException("Relation cascade '{0}' is not supported by genie",
				                      Enum.GetName(typeof(RelationCascade), cascade));
			}
		}
	}
}

