using System;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoBaseTableOwned : MetaInfoBase
	{
		public MetaInfoBaseTableOwned()
		{
		}

		public MetaInfoTable Table  { get; set; }
	}
}

