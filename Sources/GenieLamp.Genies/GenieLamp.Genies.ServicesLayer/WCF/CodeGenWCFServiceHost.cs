using System;
using System.IO;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer.WCF
{
	public class CodeGenWCFServiceHost : CodeGenBase
	{
		public const string ClassName_ServicesHost = "DomainServicesHost";

		#region Constructors
		public CodeGenWCFServiceHost(ServicesGenie genie)
			: base(genie)
		{
			outFileName = String.Format("{0}.cs", ClassName_ServicesHost);
		}
		#endregion

		protected override void Write()
		{
			cw.WriteUsing("System.ServiceModel");
			cw.WriteUsing("System.ServiceModel.Description");
			cw.WriteUsing("System.ServiceModel.Channels");
			cw.WriteLine();

			ns.BeginScope(ServicesLayerConfig.ServicesNamespace);
			cw.BeginComment();
			cw.WriteLine("How to setup security: http://msdn.microsoft.com/en-us/library/ms733768.aspx");
			cw.WriteLine("If you are running on Windows Vista, Windows Server 2008 R2 or Windows 7, use the Netsh.exe tool");
			cw.WriteLine("Example: netsh http add urlacl url=http://+:8080/MyUri user=DOMAIN\\user");
			cw.EndComment();

			cw.BeginClass(AccessLevel.Public, false, ClassName_ServicesHost, null);
			cw.WriteLine("private ServiceHost host;");
			cw.WriteLine();
			cw.WriteLine("public void Start()");
			cw.BeginScope();
            cw.WriteLine("host = new ServiceHost(typeof(DomainServices));");
            cw.WriteLine("Console.WriteLine(\"Service started at: {0}\", host.BaseAddresses[0]);");
            cw.WriteLine("WSHttpBinding binding = new WSHttpBinding();");
            cw.WriteLine("host.AddServiceEndpoint(typeof({0}), binding, \"\");",
			             NamingHelperWCF.IntfName_PersistenceService);
			foreach(IEntity entity in Model.Entities)
			{
	            cw.WriteLine("host.AddServiceEndpoint(typeof({0}), binding, \"\");",
				             NamingHelperWCF.GetEntityServiceName(entity));
			}
            cw.WriteLine("host.Open();");
            cw.WriteLine("Console.WriteLine(\"The service is ready\");");
			cw.EndScope();
			cw.WriteLine();

			cw.WriteLine("public void Stop()");
			cw.BeginScope();
            cw.WriteLine("host.Close();");
            cw.WriteLine("Console.WriteLine(\"The service is down\");");
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

