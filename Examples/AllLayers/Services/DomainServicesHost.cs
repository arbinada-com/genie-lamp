// Genie Lamp Core (1.1.4594.29523)
// ServiceStack services genie (1.0.4594.29525)
// Starter application (1.1.4594.29524)
// This file was automatically generated at 2012-07-30 16:36:49
// Do not modify it manually.

using System;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;

namespace Arbinada.GenieLamp.Warehouse.Services
{
	public class DomainServicesHost : AppHostHttpListenerBase
	{
		public DomainServicesHost()
			: base("Warehouse Services", typeof(DomainServicesHost).Assembly)
		{
		}
		
		public override void Configure(Funq.Container container)
		{
		}
	}
}

