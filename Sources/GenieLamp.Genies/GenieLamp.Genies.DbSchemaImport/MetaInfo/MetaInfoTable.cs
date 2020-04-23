using System;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoTable : MetaInfoBase
	{
		public MetaInfoTable()
		{
			this.Columns = new MetaInfoColumns();
			this.UniqueConstraints = new MetaInfoUniqueConstraints();
			this.ForeignKeys = new MetaInfoForeignKeys();
			this.Indexes = new MetaInfoIndexes();
		}

		public MetaInfoColumns Columns { get; private set; }
		public MetaInfoPrimaryKey PrimaryKey { get; set; }
		public MetaInfoUniqueConstraints UniqueConstraints { get; private set; }
		public MetaInfoForeignKeys ForeignKeys { get; private set; }
		public MetaInfoIndexes Indexes { get; private set; }
	}
}

