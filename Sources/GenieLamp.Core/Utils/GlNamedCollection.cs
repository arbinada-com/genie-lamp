using System;
using System.Collections;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Utils
{
	class GlNamedCollection<IT, T> : IEnumerable<IT>
		where T : IT
	{
		protected Dictionary<string, T> dictionary = new Dictionary<string, T>();
		
		public GlNamedCollection()
		{
		}
		
		protected void Add(string name, T o)
		{
			if (dictionary.ContainsKey(name))
				throw new GlException("Item \"{0}\" (Type: {1}) already exists in collection", name, typeof(T).Name);
			dictionary.Add(name, o);
		}
		
		public int Count {
			get { return dictionary.Count; }
		}
		
		public T GetByName(string name, bool throwException = false, object source = null)
		{
			T o;
			dictionary.TryGetValue(name, out o);
			if (o == null && throwException)
				throw new GlException("Item \"{0}\" not found (Type: {1}, Source: {2})", name, typeof(T).Name, source == null ? "" : source.ToString());
			return o; 
		}
		
		public bool Contains(T o) 
		{
			return dictionary.ContainsValue(o);
		}

		public virtual void Clear()
		{
			dictionary.Clear();
		}
		
		public bool Remove(string key)
		{
			return dictionary.Remove(key);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return dictionary.Values.GetEnumerator();
		}
		
		#region IT implementation
		public IT this[string name] {
			get { return GetByName(name, true); }
		}
		#endregion

		#region IEnumerable[IT] implementation
		IEnumerator<IT> IEnumerable<IT>.GetEnumerator()
		{
			return new GlEnumerator<IT, T>(dictionary.Values);
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}
		#endregion

	}
}

