using System;
using System.Collections.Generic;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoUniqueConstraint : MetaInfoBaseTableOwned
	{
		public List<MetaInfoColumn> Columns { get; private set; }

		public MetaInfoUniqueConstraint()
		{
			this.Columns = new List<MetaInfoColumn>();
		}
	}
}

