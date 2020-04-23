using System;
using System.Xml;

namespace GenieLamp.Core.Utils
{
	class ParamSimple : IParamSimple
	{
		private string name;
		private string paramValue;
		private string text;
		private ParamsSimple owner;

		#region Constructors
		public ParamSimple(ParamsSimple owner, XmlNode node)
		{
			this.owner = owner;
			name = Utils.Xml.GetAttrValue(node, "name");
			paramValue = Utils.Xml.GetAttrValue(node, "value");
			text = Utils.Xml.GetNodeText(node);
		}

		internal ParamSimple(ParamsSimple owner, string name, string value)
		{
			this.owner = owner;
			this.name = name;
			this.paramValue = value;
			this.text = value;
		}
		#endregion

		#region IParamSimple implementation
		public string Name
		{
			get { return name; }
		}

		public string Value
		{
			get { return owner.Macro.Subst(paramValue); }
			internal set { paramValue = value; }
		}

		public bool AsBool
		{
			get { return bool.Parse(Value); }
		}

		public int AsInt
		{
			get { return int.Parse(Value); }
		}

		public string Text
		{
			get { return owner.Macro.Subst(text); }
			internal set { text = value; }
		}
		#endregion
	}
}

