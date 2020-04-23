using System;

using GenieLamp.Core.Metamodel.Physical;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel.Physical
{
	class IndexColumn : IIndexColumn
	{
		#region Constructors
		public IndexColumn(Index owner)
		{
			this.Owner = owner;
		}
		#endregion

		public Index Owner { get; private set; }
		public global::GenieLamp.Core.Metamodel.Attribute Attribute { get; internal set; }
		public ColumnOrder Order { get; internal set; }

		public static ColumnOrder ParseOrder(string orderString)
		{
			switch(orderString.ToLower())
			{
			case "asc": return ColumnOrder.Asc;
			case "desc": return ColumnOrder.Desc;
			}
			throw new GlException("Unknown column order '{0}'", orderString);
		}

		public override string ToString()
		{
			return string.Format("Column (Attribute={0}, Order={1})", Attribute, Order);
		}

		#region IIndexColumn implementation
		IAttribute IIndexColumn.Attribute
		{
			get { return this.Attribute; }
		}
		#endregion
	}
}

