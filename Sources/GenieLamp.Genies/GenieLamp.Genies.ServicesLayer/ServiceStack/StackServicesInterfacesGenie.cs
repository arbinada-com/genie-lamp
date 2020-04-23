using System;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class ServicesInterfacesGenie : GenieBase
	{
		public ServicesInterfacesGenie()
			: base()
		{
		}

		public override void Spell(GenieLamp.Core.Metamodel.IModel model)
		{
			base.Spell(model);
			(new CodeGenStackDTO(this)).Run();
			(new CodeGenStackServicesInterfaces(this)).Run();
			(new CodeGenDomainDTOAdapters(this)).Run();
		}

		public override string Name
		{
			get { return "ServiceStack services interfaces genie"; }
		}

	}
}

