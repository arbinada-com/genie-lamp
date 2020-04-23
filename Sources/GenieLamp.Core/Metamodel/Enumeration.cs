using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	
	class Enumeration : MetaObjectSchemed, IEnumeration
	{
		private EnumerationItems items = null;
		private Persistence persistence = null;
		private EnumerationItem defaultItem = null;
		
		public Enumeration(Model model, XmlNode node) : base(model, node)
		{
			persistence = new Persistence(this, node);
			items = new EnumerationItems(this, node);
			Model.Types.AddEnumerationType(this);
		}
		
		public EnumerationItems Items {
			get { return items; }
		}

		public void Update()
		{
			persistence.UpdateNames(Schema, Name);
			EnumerationItem foundDefault = null;
			foreach(EnumerationItem item in Items)
			{
				item.Update();
				if (item.IsDefault)
				{
					foundDefault = item;
					break;
				}
			}
			if (foundDefault == null && Items.Count > 0)
				foundDefault = Items.GetByIndex(0);
			if (foundDefault != null)
			{
				this.defaultItem = foundDefault;
				this.defaultItem.IsDefault = true;
			}
		}

		public Persistence Persistence {
			get { return persistence; }
		}
		
		public EnumerationItem DefaultItem {
			get { return this.defaultItem; }
		}

		#region IEnumeration implementation
		IEnumerationItems IEnumeration.Items {
			get { return items; }
		}

		IEnumerationItem IEnumeration.DefaultItem {
			get { return this.defaultItem; }
		}

		IPersistence IPersistent.Persistence {
			get { return persistence; }
		}
		#endregion

		#region IConsistency implementation
		public void Check()
		{
			int defaultCount = 0;
			foreach(EnumerationItem item in Items)
				if (item.IsDefault)
					defaultCount++;

			if (defaultCount == 0)
				throw new GlException("Enumeration has no default item. {0}", this);
			if (defaultCount >  1)
				throw new GlException("Enumeration has multiples default items. {0}", this);
		}
		#endregion

	}
}

