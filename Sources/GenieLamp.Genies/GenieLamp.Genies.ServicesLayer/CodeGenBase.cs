using System;
using System.IO;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer
{
	public abstract class CodeGenBase
	{
		protected GenieBase genie;
		protected IEnvironmentHelper environment;
		protected IEnvironmentHelper domainEnvironment;
		protected ICodeWriterCSharp cw;
		protected ICSharpNamespaceWriter ns;
		protected string outFileName;
		protected IEnvironmentHelper interfacesEnvironment;
		protected IEnvironmentHelper dtoEnvironment;


		#region Constructors
		public CodeGenBase(GenieBase genie)
		{
			this.genie = genie;
			genie.Model.MetaObjects.SetUnprocessedAll();

			if (!DomainLayerConfig.IsDefined)
				throw new ApplicationException("Domain layer configuration is not defined in project");
			if (!ServicesLayerConfig.IsDefined)
				throw new ApplicationException("Services layer configuration is not defined in project");
			if(ServicesLayerConfig.BaseNamespace == Const.EmptyName)
				throw new ApplicationException("Services layer configuration: base namespace is not defined");

			environment = genie.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.CSharp);
			environment.BaseNamespace = ServicesLayerConfig.ServicesNamespace;

			domainEnvironment = genie.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.CSharp);
			domainEnvironment.BaseNamespace = DomainLayerConfig.DomainNamespace;

			interfacesEnvironment = genie.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.CSharp);
			interfacesEnvironment.BaseNamespace = ServicesLayerConfig.ServicesInterfacesNamespace;

			dtoEnvironment = genie.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.CSharp);
			dtoEnvironment.BaseNamespace = ServicesLayerConfig.ServicesInterfacesNamespace;
			
			genie.Config.Macro.SetMacro("%BASE_NAMESPACE%", ServicesLayerConfig.BaseNamespace);
			genie.Config.Macro.SetMacro("%NAMESPACE_SERVICES%", environment.BaseNamespace);
			genie.Config.Macro.SetMacro("%NAMESPACE_SERVICES_INTERFACES%", interfacesEnvironment.BaseNamespace);
			genie.Config.Macro.SetMacro("%NAMESPACE_SERVICES_DTO%", dtoEnvironment.BaseNamespace);
			genie.Config.Macro.SetMacro("%NAMESPACE_DOMAIN%", DomainLayerConfig.DomainNamespace);
			genie.Config.Macro.SetMacro("%NAMESPACE_PERSISTENCE%", DomainLayerConfig.PersistenceNamespace);

			genie.Config.Macro.SetMacro("%ClassName_PersistentObjectDTO%", NamingHelper.ClassName_PersistentObjectDTO);
			genie.Config.Macro.SetMacro("%ClassName_PersistentObjectDTOAdapter%", NamingHelper.ClassName_PersistentObjectDTOAdapter);
			genie.Config.Macro.SetMacro("%ClassName_DomainObjectDTO%", NamingHelper.ClassName_DomainObjectDTO);
			genie.Config.Macro.SetMacro("%ClassName_QueryParams%", NamingHelper.ClassName_ServicesQueryParams);
			genie.Config.Macro.SetMacro("%ClassName_QueryParam%", NamingHelper.ClassName_ServicesQueryParam);
			//genie.Config.Macro.SetMacro("%ClassName_UnitOfWork%", NamingHelper.ClassName_UnitOfWorkDTO);
			genie.Config.Macro.SetMacro("%ClassName_UnitOfWorkDTO%", NamingHelper.ClassName_UnitOfWorkDTO);
			genie.Config.Macro.SetMacro("%ClassName_UnitOfWorkDomain%", NamingHelper.ClassName_UnitOfWorkDomain);
			genie.Config.Macro.SetMacro("%ClassName_UnitOfWorkDTOAdapter%", NamingHelper.ClassName_UnitOfWorkDTOAdapter);

			genie.Config.Macro.SetMacro("%ServiceName_Persistence%", NamingHelper.ServiceName_Persistence);

			genie.Config.Macro.SetMacro("%PropertyName_InternalObjectId%", NamingHelper.PropertyName_InternalObjectId);
			genie.Config.Macro.SetMacro("%PropertyName_UnitOfWorkDTO%", NamingHelper.PropertyName_UnitOfWorkDTO);
			genie.Config.Macro.SetMacro("%PropertyName_AdapterDTO%", NamingHelper.PropertyName_AdapterDTO);
		}
		#endregion

		protected ILogger Logger
		{
			get { return genie.Lamp.Logger; }
		}

		public IDomainLayerConfig DomainLayerConfig
		{
			get { return genie.Lamp.Config.Layers.DomainConfig; }
		}

		public IServicesLayerConfig ServicesLayerConfig
		{
			get { return genie.Lamp.Config.Layers.ServicesConfig; }
		}

		public IModel Model
		{
			get { return genie.Model; }
		}

		public virtual void Run()
		{
			cw = Model.Lamp.CodeWritersFactory.CreateCodeWriterCSharp();
			cw.WriteStdHeader(genie);
			cw.WriteUsing("System");
			ns = cw.CreateNamespaceWriter();

			Write();

			cw.Save(Path.Combine(genie.Config.OutDir, outFileName));
		}

		protected abstract void Write();

		protected virtual void ProcessEntities()
		{
			cw.BeginRegion("Entities");
			ICSharpNamespaceWriter nswriter = cw.CreateNamespaceWriter();
			foreach(IEntity entity in Model.Entities)
			{
				nswriter.BeginOrContinueScope(entity.Schema);
				ProcessEntity(entity);
				cw.WriteLine();
			}
			nswriter.EndScope();
			cw.EndRegion();
			cw.WriteLine();
		}

		protected abstract void ProcessEntity(IEntity entity);
	}
}

