// Genie Lamp Core (1.1.4798.27721)
// ServiceStack services genie (1.0.4798.27724)
// Starter application (1.1.4798.27722)
// This file was automatically generated at 2013-03-14 16:56:47
// Do not modify it manually.

using System;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;

namespace GenieLamp.Examples.QuickStart.Services
{
	public partial class CustomCredentialsAuthProvider : CredentialsAuthProvider { }
	
	public class DomainServicesHost : AppHostHttpListenerBase
	{
		public DomainServicesHost()
			: base("ServiceStack Services", typeof(DomainServicesHost).Assembly)
		{
		}
		
		public override void Configure(Funq.Container container)
		{
			Plugins.Add(new AuthFeature(() => new AuthUserSession(), new AuthProvider[]
			{
				new CustomCredentialsAuthProvider()
			}
			));
			
			container.Register<ICacheClient>(new MemoryCacheClient() { FlushOnDispose = false });
		}
	}
}

