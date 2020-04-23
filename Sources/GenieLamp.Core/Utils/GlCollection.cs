using System;
using System.Collections.Generic;

namespace GenieLamp.Core.Utils
{
	class GlCollection<IT, T> : IList<IT>
		where T : IT
	{
		protected List<T> list = new List<T>();
		
		public GlCollection()
		{
		}
		
		public virtual void Add(T o)
		{
			list.Add(o);
		}
		
		public void AddTop(T o)
		{
			list.Insert(0, o);
		}
		
		public void Insert(int index, T o)
		{
			list.Insert(index, o);
		}
		
		public int IndexOf(T o)
		{
			return list.IndexOf(o);
		}


		public bool Contains(T o)
		{
			return list.Contains(o);
		}
		
		public void Clear()
		{
			list.Clear();
		}
				
		public int Count 
		{
			get { return list.Count; }
		}
		
		public IT this[int index] 
		{
			get { return list[index]; }
			set { list[index] = (T)value; }
		}
		
		public T GetByIndex(int index) 
		{
			return list[index];
		}
		
		public bool Remove(T o)
		{
			return list.Remove(o);
		}

		#region IList implementation

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		int IList<IT>.IndexOf(IT o)
		{
			return this.IndexOf((T)o);
		}

		void IList<IT>.Insert(int index, IT o)
		{
			this.Insert(index, (T)o);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}
		#endregion

		#region ICollection implementation
		void ICollection<IT>.Add(IT o)
		{
			this.Add((T)o);
		}

		bool ICollection<IT>.Contains(IT o)
		{
			return this.Contains((T)o);
		}

		void ICollection<IT>.CopyTo(IT[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		bool ICollection<IT>.Remove(IT o)
		{
			return this.Remove((T)o);
		}
		#endregion


		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
		
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		#region IEnumerable[IT] implementation
		IEnumerator<IT> IEnumerable<IT>.GetEnumerator()
		{
			return new GlEnumerator<IT, T>(list); 
		}
		#endregion

//		#region IEnumerable implementation
//		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//		{
//			return list.GetEnumerator();
//		}
//		#endregion
		
	}
	
}

