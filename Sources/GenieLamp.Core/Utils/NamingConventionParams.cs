using System;
using System.Xml;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Utils
{
	class NamingConventionParams : ParamsSimple, IParamsSimple
	{
		private NamingConvention owner;

		#region Constructors
		public NamingConventionParams(NamingConvention owner)
			: base(owner.Macro)
		{
			this.owner = owner;
		}

		public NamingConventionParams(NamingConvention owner, XmlNodeList nodeList)
			: base(nodeList, owner.Macro)
		{
			this.owner = owner;
		}
		#endregion

		public NamingConvention NamingConvention {
			get { return this.owner; }
		}
	}
}

