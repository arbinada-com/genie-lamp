using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer
{
	public class CodeGenStackServicesHost : CodeGenBase
	{
		public CodeGenStackServicesHost(GenieBase genie)
			: base(genie)
		{
			outFileName = "DomainServicesHost.cs";
		}

		protected override void Write()
		{
			cw.WriteUsing("ServiceStack.ServiceHost");
			cw.WriteUsing("ServiceStack.WebHost.Endpoints");
			cw.WriteUsing("ServiceStack.ServiceInterface");
			cw.WriteUsing("ServiceStack.ServiceInterface.Auth");
			cw.WriteUsing("ServiceStack.CacheAccess");
			cw.WriteUsing("ServiceStack.CacheAccess.Providers");
			cw.WriteLine();

			ns.BeginScope(ServicesLayerConfig.ServicesNamespace);

			cw.WriteLine("public partial class CustomCredentialsAuthProvider : CredentialsAuthProvider { }");
			cw.WriteLine();

			cw.BeginClass(AccessLevel.Public,
			              false,
			              "DomainServicesHost",
			              "AppHostHttpListenerBase",
			              Const.EmptyName);
			cw.WriteLine("public DomainServicesHost()");
			cw.Indent++;
			cw.WriteLine(": base(\"{0}\", typeof(DomainServicesHost).Assembly)",
			             genie.Config.Name);
			cw.Indent--;
			cw.BeginScope();
			cw.EndScope();
			cw.WriteLine();

	        cw.WriteLine("public override void Configure(Funq.Container container)");
			cw.BeginScope();
			cw.WriteLine("Plugins.Add(new AuthFeature(() => new AuthUserSession(), new AuthProvider[]");
			cw.BeginScope();
			cw.WriteLine("new CustomCredentialsAuthProvider()");
			cw.EndScope();
			cw.WriteLine("));");
			cw.WriteLine();
			cw.WriteLine("container.Register<ICacheClient>(new MemoryCacheClient() { FlushOnDispose = false });");
			cw.EndScope();

			cw.EndClass();

			ns.EndScope();
		}

		#region implemented abstract members of GenieLamp.Genies.ServicesLayer.CodeGenBase
		protected override void ProcessEntity(IEntity entity)
		{
		}
		#endregion
	}
}

