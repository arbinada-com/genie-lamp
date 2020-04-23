using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.ServicesLayer.WCF
{
	public class ServicesGenie : GenieBase, IGenie
	{
		public ServicesGenie()
		{
		}

		#region IGenie implementation
		public override void Spell(IModel model)
		{
			base.Spell(model);
			(new CodeGenDTO(this)).Run();
			(new CodeGenWCFServiceHost(this)).Run();
			(new CodeGenWCFDomainServices(this)).Run();
			(new CodeGenWCFDomainContracts(this)).Run();
		}

		public override string Name
		{
			get { return "Genie of WCF Services"; }
		}
		#endregion
	}
}

