using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class SpellHintParams : ParamsSimple
	{
		private Dictionary<string, ParamSimple> index = new Dictionary<string, ParamSimple>();

		public MetaObject Owner { get; private set; }

		#region Constructors
		public SpellHintParams(MetaObject owner)
			: base(owner.Macro)
		{
			this.Owner = owner;
		}

		public SpellHintParams(MetaObject owner, XmlNodeList nodes)
			: base(nodes, owner.Macro)
		{
			this.Owner = owner;
			Reindex();
		}
		#endregion

		private void Reindex()
		{
			index.Clear();
			foreach(ParamSimple param in this)
			{
				if (String.IsNullOrEmpty(param.Value))
					param.Value = SpellHint.TargetNameWildcard;
				index.Add(MakeFullName(param.Name, param.Value), param);
			}
		}

		private static string MakeFullName(string name, string value)
		{
			return String.Format("{0}.{1}", name, value);
		}

		public Utils.ParamSimple FindParam(string name, string value)
		{
			ParamSimple param = null;
			index.TryGetValue(MakeFullName(name, value), out param);
			return param;
		}
	}
}

