using System;
using System.Xml;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Layers
{
	class LayerLocalizationParams : ParamsSimple, IParamsSimple
	{
		private LayerConfig owner;

		#region Constructors
		public LayerLocalizationParams(LayerConfig owner)
			: base(owner.Macro)
		{
			this.owner = owner;
		}

		public LayerLocalizationParams(LayerConfig owner, XmlNodeList nodeList)
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

