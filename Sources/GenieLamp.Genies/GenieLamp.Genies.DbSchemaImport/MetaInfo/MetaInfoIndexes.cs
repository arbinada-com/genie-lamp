using System;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoIndexes : MetaInfoCollection<MetaInfoIndex>
	{
		public MetaInfoIndexes()
		{
		}

		public MetaInfoIndex FindByColumns(MetaInfoColumns cols)
		{
			foreach(MetaInfoIndex ix in this)
			{
				if (ix.Columns.Count == cols.Count)
				{
					bool colsMatched = true;
					for (int i = 0; i < cols.Count; i++)
					{
						if (ix.Columns[i].Table.FullPersistentName != cols[i].Table.FullPersistentName ||
							ix.Columns[i].Name != cols[i].Name)
						{
							colsMatched = false;
							break;
						}
					}
					if (colsMatched)
						return ix;
				}
			}
			return null;
		}
	}
}

