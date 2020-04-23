using System;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class EnumerationItems : MetaObjectNamedCollection<IEnumerationItem, EnumerationItem>, IEnumerationItems
	{
		public EnumerationItems(Enumeration owner, XmlNode node) : base(owner.Model)
		{
			int currentValue = 0;
			XmlNodeList itemList = Model.QueryNode(node, "./{0}:Item");
			foreach(XmlNode itemNode in itemList)
			{
				EnumerationItem item = new EnumerationItem(owner, itemNode, currentValue);
				Add(item);
				if (item.Value != currentValue)
					currentValue = item.Value;
				currentValue++;
			}
		}
	}
}

