using System;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoForeignKey : MetaInfoBase
	{
		public MetaInfoColumnsMatches ColumnsMatches { get; private set; }

		public MetaInfoForeignKey()
		{
			this.ColumnsMatches = new MetaInfoColumnsMatches();
		}

		public MetaInfoTable Child { get; set; }
		public MetaInfoTable Parent { get; set; }
	}
}

