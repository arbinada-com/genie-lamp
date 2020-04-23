using System;

using GenieLamp.Core.Metamodel.Physical;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel.Physical
{
	class IndexColumns : GlCollection<IIndexColumn, IndexColumn>, IIndexColumns
	{
		#region Constructors
		public IndexColumns(Index owner)
		{
			this.Owner = owner;
		}
		#endregion

		public Index Owner { get; private set; }

		internal void CopyAttributes<TOwner>(Attributes<TOwner> source) where TOwner : MetaObject
		{
			Clear();
			foreach (Attribute a in source)
			{
				IndexColumn col = new IndexColumn(Owner)
				{
					Attribute = a
				};
				Add(col);
			}
		}
		
		#region IIndexColumns implementation

		public global::GenieLamp.Core.Metamodel.IAttributes Attributes
		{
			get
			{
				global::GenieLamp.Core.Metamodel.Attributes<Index> attributes = new global::GenieLamp.Core.Metamodel.Attributes<Index>(Owner);
				foreach(IndexColumn col in this)
				{
					attributes.Add(col.Attribute);
				}
				return attributes;
			}
		}

		#endregion
	}
}

