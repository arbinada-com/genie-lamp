using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class MetaObjectCollection<IT, T> : GlCollection<IT, T>, IMetaObjectCollection<IT>, IEnumerable<IT> 
		where T : MetaObject, IT where IT : IMetaObject
	{
		private Model model;
		
		#region Constructors
		public MetaObjectCollection(Model model) : base()
		{ 
			this.model = model;
		}
		#endregion
		
		public Model Model
		{
			get { return model; }
		}
				
		#region IMetaObjectCollection[IT] implementation
		public IList ToList()
		{
			List<IT> copy = new List<IT>();
			foreach (T o in this.list)
				copy.Add(o);
			return copy;
		}

		public void SetUnprocessedAll()
		{
			foreach (T o in this.list)
				o.Processed = false;
		}
		#endregion
	}
}

