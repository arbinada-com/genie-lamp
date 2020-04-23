using System;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class ServicesGenie : GenieBase
	{
		public ServicesGenie()
			: base()
		{
		}

		public override void Spell(GenieLamp.Core.Metamodel.IModel model)
		{
			base.Spell(model);
			(new CodeGenStackDomainServices(this)).Run();
			(new CodeGenStackServicesHost(this)).Run();
		}

		public override string Name
		{
			get { return "ServiceStack services genie"; }
		}

	}
}

