using System;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class EnumerationItem : MetaObject, IEnumerationItem
	{
		private int itemValue;
		private bool hasValue;
		private bool isDefault = false;
		private Persistence persistence = null;
		
		public EnumerationItem(Enumeration owner, XmlNode node, int currentValue) : base(owner, node)
		{
			hasValue = Utils.Xml.IsAttrExists(node, "value");
			itemValue = Utils.Xml.GetAttrValue(node, "value", currentValue);
			isDefault = Utils.Xml.GetAttrValue(node, "default", isDefault);
			persistence = new Persistence(owner, node);
		}

		public void Update()
		{
			persistence.UpdateNames(Const.EmptyName, Name);
		}

		public Persistence Persistence {
			get { return persistence; }
		}
		
		#region IEnumerationItem implementation
		public int Value {
			get { return itemValue; }
		}

		public bool HasValue {
			get { return hasValue; }
		}

		public bool IsDefault {
			get { return this.isDefault; }
			internal set { isDefault = value; }
		}

		IPersistence IPersistent.Persistence {
			get { return persistence; }
		}
		#endregion


	}
}

