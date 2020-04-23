using System;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoCollection<T> : List<T> where T : MetaInfoBase
	{
		protected Dictionary<string, T> names = new Dictionary<string, T>();
		protected Dictionary<string, T> persistentNames = new Dictionary<string, T>();

		public MetaInfoCollection()
		{
		}

		internal void Reindex()
		{
			persistentNames.Clear();
			names.Clear();
			foreach(T o in this)
			{
				persistentNames.Add(o.PersistentName, o);
				names.Add(o.FullName, o);
			}
		}

		public new void Clear()
		{
			base.Clear();
			Reindex();
		}

		public new void Add(T item)
		{
			AddMetaInfo(item);
		}

		public new bool Contains(T item)
		{
			return FindByPersistentName(item.PersistentSchema, item.PersistentName, false) != null;
		}

		public void AddMetaInfo(T o)
		{
			base.Add(o);
			persistentNames.Add(o.FullPersistentName, o);
		}

		public T FindByName(string name, bool throwException)
		{
			return this.FindByName(String.Empty, name, throwException);
		}

		public T FindByName(string schema, string name, bool throwException)
		{
			T o = null;
			names.TryGetValue(MetaInfoBase.MakeFullName(schema, name), out o);
			if (o == null && throwException)
				throw new GlException("Item with name not found: {0} ({1})", name, typeof(T).Name);
			return o;
		}

		public T FindByPersistentName(string name, bool throwException)
		{
			return this.FindByPersistentName(String.Empty, name, throwException);
		}

		public T FindByPersistentName(string schema, string name, bool throwException)
		{
			T o = null;
			persistentNames.TryGetValue(MetaInfoBase.MakeFullName(schema, name), out o);
			if (o == null && throwException)
				throw new GlException("Item with persistent name not found: {0} ({1})", name, typeof(T).Name);
			return o;
		}

		public bool RemoveByPersistentName(string name, bool throwException)
		{
			return RemoveByPersistentName(String.Empty, name, throwException);
		}

		public bool RemoveByPersistentName(string schema, string name, bool throwException)
		{
			T o = this.FindByPersistentName(schema, name, throwException);
			if (o != null)
			{
				this.Remove(o);
				return true;
			}
			return false;
		}
	}
}

