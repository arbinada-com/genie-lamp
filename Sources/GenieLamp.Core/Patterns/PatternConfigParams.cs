using System;
using System.Xml;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Patterns
{
	class PatternConfigParams : ParamsSimple, IParamsSimple
	{
		private PatternConfig owner;

		#region Constructors
		public PatternConfigParams(PatternConfig owner)
			: base(owner.Macro)
		{
			this.owner = owner;
		}

		public PatternConfigParams(PatternConfig owner, XmlNodeList nodeList)
			: base(nodeList, owner.Macro)
		{
			this.owner = owner;
		}
		#endregion

		public PatternConfig PatternConfig
		{
			get { return this.owner; }
		}
	}
}

