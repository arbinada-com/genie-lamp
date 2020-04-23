using System;
using System.Xml;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Layers
{
	class LayerConfigParams : ParamsSimple, IParamsSimple
	{
		private LayerConfig owner;

		#region Constructors
		public LayerConfigParams(LayerConfig owner)
			: base(owner.Macro)
		{
			this.owner = owner;
		}

		public LayerConfigParams(LayerConfig owner, XmlNodeList nodeList)
			: base(nodeList, owner.Macro)
		{
			this.owner = owner;
		}
		#endregion

		public LayerConfig LayerConfig
		{
			get { return this.owner; }
		}
	}
}

