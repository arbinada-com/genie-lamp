using System;
using System.Collections.Generic;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoPrimaryKey : MetaInfoBaseTableOwned
	{
		public List<MetaInfoColumn> Columns { get; private set; }

		public MetaInfoPrimaryKey()
		{
			this.Columns = new List<MetaInfoColumn>();
		}
	}
}

