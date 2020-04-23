using System;
using System.Xml;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.ServicesLayer.WCF
{
	public class ServicesProxyGenie : GenieBase, IGenie
	{
		public ServicesProxyGenie()
		{
		}

		#region IGenie implementation
		public override void Spell(IModel model)
		{
			base.Spell(model);
			(new CodeGenWCFDomainServicesAdapters(this)).Run();
		}

		public override string Name
		{
			get { return "Genie of WCF Services Proxies"; }
		}
		#endregion
	}
}

