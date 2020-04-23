using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class MetaObjectNamedCollection<IT, T> : GlNamedCollection<IT, T>, IMetaObjectNamedCollection<IT>, IEnumerable<IT> 
		where T : MetaObject, IT where IT : IMetaObject
	{
		private MetaObjectCollection<IT, T> list;
		
		#region Constructors
		public MetaObjectNamedCollection(Model model) : base()
		{ 
			list = new MetaObjectCollection<IT, T>(model);
		}
		#endregion
		
		public Model Model {
			get { return list.Model; }
		}
				
		public T GetByName(MetaObject metaObject, bool throwNotFound = false)
		{
			return GetByName(metaObject.FullName, throwNotFound);
		}
		
		public T GetByIndex(int index) 
		{
			return list.GetByIndex(index);
		}
		
		public void Add(T metaObject)
		{
			list.Add(metaObject);
			Add(metaObject.FullName, metaObject);
		}
		
		public void AddTop(T o)
		{
			list.AddTop(o);
			Add(o.FullName, o);
		}

		public void Insert(T before, T o)
		{
			int index = list.IndexOf(before);
			base.Add(o.FullName, o);
			if (index < 0)
				list.Add(o);
			else
				list.Insert(index, o);
		}
		
		public override void Clear()
		{
			list.Clear();
			base.Clear();
		}
		
		public bool Remove(T o)
		{
			
			return list.Remove(o) && base.Remove(o.FullName);
		}
		
		public new int Count {
			get { return list.Count; }
		}
		
		public IT this[int index] {
			get { return list[index]; }
		}
		
		public new IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		#region IMetaObjectNamedCollection[IT] implementation
		public IList ToList()
		{
			List<IT> copy = new List<IT>();
			foreach(T o in this.list)
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

