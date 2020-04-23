using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer.WCF
{
	public class CodeGenWCFDomainServicesAdapters : CodeGenBase
	{
		private const string ClassName_ServiceProxyAdapter = "ServiceProxyAdapter";
		private const string ClassName_PersistentProxyAdapter = "PersistentProxyAdapter";
		private const string VarName_DomainObjectProxy = "domainObjectProxy";
		private const string PropertyName_DomainObjectProxy = "Proxy";

		private IEnvironmentHelper servicesEnvironment;

		#region Constructors
		public CodeGenWCFDomainServicesAdapters(ServicesProxyGenie genie)
			: base(genie)
		{
			outFileName = "ClientProxies.cs";
			environment.BaseNamespace = ServicesLayerConfig.ServicesAdaptersNamespace;

			servicesEnvironment = genie.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.CSharp);
			servicesEnvironment.BaseNamespace = ServicesLayerConfig.ServicesNamespace;

			genie.Config.Macro.SetMacro("%ClassName_ServiceProxyAdapter%", ClassName_ServiceProxyAdapter);
			genie.Config.Macro.SetMacro("%ClassName_PersistentProxyAdapter%", ClassName_PersistentProxyAdapter);
		}
		#endregion

		protected override void Write()
		{
			cw.WriteUsing("System.Collections.Generic");
			cw.WriteUsing("System.Linq");
			cw.WriteLine();

			ns.BeginScope(ServicesLayerConfig.ServicesAdaptersNamespace);

			// Utils classes
            cw.WriteText(this.GetType().Assembly,
			             "Templates.WCFClientProxies.cs",
			             genie.Config.Macro);

			WriteServiceProxyAdapter();
			WritePersistentProxyAdapter();
//
//			ProcessEnumerations();
//
			ProcessEntities();

			ns.EndScope();
		}

		private void WriteServiceProxyAdapter()
		{
			cw.WriteLine("public abstract class {0}", ClassName_ServiceProxyAdapter);
			cw.BeginScope();
			cw.WriteLine("private {0} {1};",
			             NamingHelper.ClassName_DomainObjectDTO,
			             VarName_DomainObjectProxy);
			cw.BeginProperty(AccessLevel.Internal,
			                 VirtualisationLevel.None,
			                 NamingHelper.ClassName_DomainObjectDTO,
			                 PropertyName_DomainObjectProxy);
			cw.BeginPropertyGet();
			cw.WriteLine("return {0};", VarName_DomainObjectProxy);
			cw.EndPropertyAccessor();
			cw.BeginPropertySet();
			cw.WriteLine("{0} = value;", VarName_DomainObjectProxy);
			cw.EndPropertyAccessor();
			cw.EndProperty();
			cw.EndScope();
			cw.WriteLine();
		}

		private void WritePersistentProxyAdapter()
		{
			cw.WriteLine("public abstract class {0} : {1}",
			             ClassName_PersistentProxyAdapter,
			             ClassName_ServiceProxyAdapter);
			cw.BeginScope();

			cw.BeginProperty(AccessLevel.Internal,
			                 VirtualisationLevel.New,
			                 NamingHelper.ClassName_PersistentObjectDTO,
			                 PropertyName_DomainObjectProxy);
			cw.BeginPropertyGet();
			cw.WriteLine("return ({0})base.{1};",
			             NamingHelper.ClassName_PersistentObjectDTO,
			             PropertyName_DomainObjectProxy);
			cw.EndPropertyAccessor();
			cw.BeginPropertySet();
			cw.WriteLine("base.{0} = value;", PropertyName_DomainObjectProxy);
			cw.EndPropertyAccessor();
			cw.EndProperty();

			cw.EndScope();
			cw.WriteLine();
		}

		protected override void ProcessEntity(IEntity entity)
		{
			string supertypeName = ClassName_PersistentProxyAdapter;
			if (entity.HasSupertype)
				supertypeName = environment.ToTypeName(entity.Supertype, true);
			else if (!entity.Persistence.Persisted)
				supertypeName = ClassName_ServiceProxyAdapter;
			cw.BeginClass(AccessLevel.Public,
			              true,
			              entity.Name,
			              supertypeName);

			// Proxy accessor
			cw.BeginProperty(AccessLevel.Internal,
			                 VirtualisationLevel.New,
			                 NamingHelper.ToDTOTypeName(entity, servicesEnvironment),
			                 PropertyName_DomainObjectProxy);
			cw.BeginPropertyGet();
			cw.WriteLine("return ({0})base.{1};",
			             NamingHelper.ToDTOTypeName(entity, servicesEnvironment),
			             PropertyName_DomainObjectProxy);
			cw.EndPropertyAccessor();
			cw.BeginPropertySet();
			cw.WriteLine("base.{0} = value;", PropertyName_DomainObjectProxy);
			cw.EndPropertyAccessor();
			cw.EndProperty();
			cw.WriteLine();

			// Constructors
			cw.BeginRegion("Constructors");
			cw.WriteLine("public {0}()", entity.Name);
			cw.BeginScope();
			cw.BeginUsing("{0} client = new {0}()", NamingHelperWCF.GetEntityServiceClient(entity));
			cw.WriteLine("{0} = client.{1};",
			             PropertyName_DomainObjectProxy,
			             NamingHelperWCF.MethodSignature_Create(entity));
			cw.WriteLine("client.Close();");
			cw.EndUsing();
			cw.EndScope();
			cw.WriteLine();

			cw.WriteLine("public {0}({1} source)",
			             entity.Name,
			             NamingHelper.ToDTOTypeName(entity, servicesEnvironment));
			cw.BeginScope();
			cw.WriteLine("if (source == null) throw new NullReferenceException(\"{0}\");",
			             "Source proxy object cannot be null");
			cw.WriteLine("this.{0} = source;", PropertyName_DomainObjectProxy);
			cw.EndScope();

			cw.EndRegion();
			cw.WriteLine();

			// Get by id
			cw.WriteLine("public static {0}{1} {2}",
			             entity.HasSupertype ? "new " : "",
			             entity.Name,
			             ServicesLayerConfig.Methods.GetById(entity.Constraints.PrimaryId, environment).Signature);
			cw.BeginScope();
			cw.BeginUsing("{0} client = new {0}()", NamingHelperWCF.GetEntityServiceClient(entity));
			cw.WriteLine("return new {0}(client.{1}({2}));",
			             entity.Name,
			             ServicesLayerConfig.Methods.GetProxyById(entity.Constraints.PrimaryId, environment).Name,
			             environment.ToArguments(entity.Constraints.PrimaryId.Attributes));
			cw.EndUsing();
			cw.EndScope();
			cw.WriteLine();

			// Get by unique id
			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
				cw.WriteLine("public static {0} {1}",
				             entity.Name,
				             ServicesLayerConfig.Methods.GetByUniqueId(uid, environment).Signature);
				cw.BeginScope();
				cw.BeginUsing("{0} client = new {0}()", NamingHelperWCF.GetEntityServiceClient(entity));
				cw.WriteLine("return new {0}(client.{1}({2}));",
				             entity.Name,
				             ServicesLayerConfig.Methods.GetProxyByUniqueId(uid, environment).Name,
				             environment.ToArguments(uid.Attributes));
				cw.EndUsing();
				cw.EndScope();
				cw.WriteLine();
			}

			ProcessAttributes(entity);

			cw.EndClass();
			cw.WriteLine();
		}

		private void ProcessAttributes(IEntity entity)
		{
			foreach(IAttribute a in entity.Attributes)
			{
				if (a.IsPrimaryId)
				{
					if (!entity.HasSupertype)
					{
						WriteSimpleProperty(a, environment);
					}
				}
				else if (a.HasEnumerationType)
				{
					// No proxy for enumeration so reuse it from services namespace
					WriteSimpleProperty(a, servicesEnvironment);
				}
				else if (a.IsUsedInRelations)
				{
					WriteObjectProperty(a, environment);
				}
				else
				{
					WriteSimpleProperty(a, environment);
				}
			}
			cw.WriteLine();
		}

		private void WriteSimpleProperty(IAttribute attribute, IEnvironmentHelper environment)
		{
			cw.BeginProperty(AccessLevel.Public,
			                 VirtualisationLevel.Virtual,
			                 environment.ToTypeName(attribute, true),
			                 attribute.Name);
			cw.BeginPropertyGet();
			cw.WriteLine("return this.{0}.{1};",
			             PropertyName_DomainObjectProxy,
			             attribute.Name);
			cw.EndPropertyAccessor();
			cw.BeginPropertySet();
			cw.WriteLine("this.{0}.{1} = value;",
			             PropertyName_DomainObjectProxy,
			             attribute.Name);
			cw.EndPropertyAccessor();
			cw.EndProperty();
		}

		private void WriteObjectProperty(IAttribute attribute, IEnvironmentHelper environment)
		{
			IRelation r = attribute.UsedInRelations[0];
			IEntity related = r.Entity;
			if (r.Entity == attribute.Entity)
				related = r.Entity2;

			cw.WriteLine("private {0} {1};",
			             environment.ToTypeName(related, true),
			             environment.ToMemberName(attribute));
			cw.BeginProperty(AccessLevel.Public,
			                 VirtualisationLevel.Virtual,
			                 environment.ToTypeName(related, true),
			                 attribute.Name);
			cw.BeginPropertyGet();
			cw.WriteLine("return this.{0}.{1};",
			             PropertyName_DomainObjectProxy,
			             attribute.Name);
			cw.EndPropertyAccessor();
			cw.BeginPropertySet();
			cw.WriteLine("this.{0}.{1} = value;",
			             PropertyName_DomainObjectProxy,
			             attribute.Name);
			cw.EndPropertyAccessor();
			cw.EndProperty();
		}

	}
}

