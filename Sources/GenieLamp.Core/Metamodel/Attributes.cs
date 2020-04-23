using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace GenieLamp.Core.Metamodel
{
	class AttributesCollection : MetaObjectNamedCollection<IAttribute, Attribute>, IAttributes
	{
		#region Constructors
		public AttributesCollection(Model model)
			: base(model)
		{
		}
		#endregion

		public Attribute GetByPersistentName(string name)
		{
			foreach (Attribute a in this)
				if (a.Persistence.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return a;
			return null;
		}

		#region IAttributes implementation
		public bool Contains(IAttribute attribute)
		{
			IAttribute found = GetByName(attribute.Name);
			return found != null && found.Entity.FullName == attribute.Entity.FullName;
		}

		public string ToNamesString(string separator)
		{
			return ToNamesString(separator, String.Empty, String.Empty);
		}

		public string ToNamesString(string separator, string prefix, string suffix)
		{
			StringBuilder names = new StringBuilder();
			int i = 1;
			foreach (Attribute a in this)
			{
				if (!String.IsNullOrEmpty(prefix))
					names.Append(prefix);
				names.AppendFormat(a.Name);
				if (!String.IsNullOrEmpty(suffix))
					names.Append(suffix);
				if (i++ < this.Count)
					names.Append(separator);
			}
			return names.ToString();
		}

		public string ToArguments()
		{
			return ToNamesString(", ");
		}

		public string ToPersistentNamesString(string separator)
		{
			StringBuilder cols = new StringBuilder();
			int i = 1;
			foreach (Attribute a in this)
			{
				cols.AppendFormat(a.Persistence.Name);
				if (i++ < this.Count)
					cols.Append(separator);
			}
			return cols.ToString();
		}


		public int PersistentCount 
		{ 
			get
			{
				int count = 0;
				foreach (Attribute a in this)
				{
					if (a.Persistence.Persisted)
						count++;
				}
				return count;
			}
		}

		public uint GetNamesHash(string prefix)
		{
			StringBuilder sb = new StringBuilder();
			if (!String.IsNullOrWhiteSpace(prefix))
				sb.AppendLine(prefix);
			foreach (Attribute a in this)
			{
				sb.AppendLine(a.Name);
			}
			return Utils.Cryptography.CalcCrc16(sb.ToString());
		}
		

		#endregion
	}

	class Attributes<TOwner> : AttributesCollection, IAttributes where TOwner : MetaObject
	{
		private TOwner owner;
		
		public Attributes(TOwner owner) : base(owner.Model)
		{ 
			this.owner = owner;
		}
		
		public TOwner Owner
		{
			get { return owner; }
		}
		
		public void Update()
		{
			foreach (Attribute a in this)
				a.Update();
		}
		
		public void Copy<TSource>(Attributes<TSource> source) where TSource : MetaObject
		{
			Clear();
			foreach (Attribute a in source)
			{
				Add(a);
			}
		}
	}
}

