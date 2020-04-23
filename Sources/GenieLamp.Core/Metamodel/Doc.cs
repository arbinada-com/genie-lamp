using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class Doc : MetaObject, IDoc
	{
		private XmlNode textNode;

		#region Constructors
		public Doc(MetaObject owner, XmlNode node)
			: base(owner, node)
		{
			textNode = node;
		}
		#endregion

		/// <summary>
		/// Return corresponding to culture documentation element (label or text).
		/// Fallback to lang name from lang-culture pair
		/// Fallback to default element with no lang specified
		/// </summary>
		/// <returns>
		/// The documentation element text or empty string when not found
		/// </returns>
		/// <param name='nodes'>
		/// Xml nodes of given type (text or labels)
		/// </param>
		/// <param name='ci'>
		/// Culture info. Null for current culture
		/// </param>
		private string GetDocElement(XmlNodeList nodes, CultureInfo ci = null)
		{
			string defaultText = Const.EmptyValue;
			if (ci == null)
				ci = System.Threading.Thread.CurrentThread.CurrentUICulture;
			foreach(XmlNode node in nodes)
			{
				string lang = Utils.Xml.GetAttrValue(node, "lang", Const.EmptyValue);
				if (String.IsNullOrEmpty(lang))
				{
					if (String.IsNullOrEmpty(defaultText))
						defaultText = node.InnerText;
				}
				else
				{
					CultureInfo ci2 = new CultureInfo(lang);
					if (ci.Equals(ci2))
					{
						return node.InnerText;
					}
					else if (ci.Parent != null && ci.Parent.Equals(ci2))
					{
						defaultText = node.InnerText;
					}
				}
			}
			return defaultText;
		}

		private string[] GetLines(XmlNodeList nodes)
		{
			List<string> list = new List<string>();
			foreach(XmlNode node in nodes)
			{
				foreach(string line in Utils.Xml.StripNamespaces(node.OuterXml).Split(new char[] {'\r', '\n'}))
				{
					list.Add(line.Trim());
				}
			}
			return list.ToArray();
		}

		#region IDoc implementation
		public XmlNode TextNode
		{
			get { return textNode; }
		}

		public string Text
		{
			get { return Utils.Xml.StripNamespaces(textNode); }
		}

		public string[] LabelLines
		{
			get { return GetLines(LabelNodes); }
		}

		public string[] TextLines
		{
			get { return GetLines(TextNodes); }
		}

		public XmlNodeList LabelNodes
		{
			get { return Model.QueryNode(textNode, "./{0}:Label"); }
		}

		public XmlNodeList TextNodes
		{
			get { return Model.QueryNode(textNode, "./{0}:Text"); }
		}

		public string GetText(System.Globalization.CultureInfo ci = null)
		{
			return GetDocElement(TextNodes, ci);
		}

		public string GetLabel(System.Globalization.CultureInfo ci = null)
		{
			return GetDocElement(LabelNodes, ci);
		}
		#endregion


	}
}

