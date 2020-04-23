using System;
using System.Collections.Generic;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoIndex : MetaInfoBaseTableOwned
	{
		public List<MetaInfoColumn> Columns { get; private set; }

		public MetaInfoIndex()
		{
			this.Columns = new List<MetaInfoColumn>();
			this.IsUnique = false;
		}

		public bool IsUnique { get; set; }
	}
}

