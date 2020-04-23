using System;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class Enumerations : MetaObjectNamedCollection<IEnumeration, Enumeration>, IEnumerations
	{
		public Enumerations(Model model) : base(model)
		{
		}
		
		public void AddList(XmlNodeList list)
		{
			foreach (XmlNode node in list)
			{
				Enumeration en = new Enumeration(Model, node);
				Add(en);
			}
		}

		public void Update()
		{
			foreach (Enumeration enumeration in this)
				enumeration.Update();
		}
		
		#region IConsistency implementation
		public void Check()
		{
			foreach (Enumeration enumeration in this)
				enumeration.Check();
		}
		#endregion
	
	}
}

