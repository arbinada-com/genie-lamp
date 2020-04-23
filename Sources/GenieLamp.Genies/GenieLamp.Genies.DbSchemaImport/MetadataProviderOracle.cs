using System;
using System.Text;
using System.Data;
using System.Data.Common;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Environments;

namespace GenieLamp.Genies.DbSchemaImport
{
	class MetadataProviderOracle : MetadataProviderBase
	{
		#region Constructors
		public MetadataProviderOracle(DbConnection connection, string providerName, SchemaImporter importer)
			: base(connection, providerName, importer)
		{
			SetEnvironment(TargetEnvironment.OracleDb);
		}
		#endregion

		#region implemented abstract members of GenieLamp.Genies.DbSchemaImport.MetadataProviderBase
		public override string GetGeneratorsSql()
		{
			ISqlStringBuilder sql = Importer.Genie.Lamp.GenieLampUtils.GetSqlStringBuilder();
			sql.Select("SEQUENCE_OWNER AS {0}", SqlColName_Schema);
			sql.Select("SEQUENCE_NAME AS {0}", SqlColName_Name);
			sql.Select("MIN_VALUE AS {0}", SqlColName_GeneratorMinValue);
			sql.Select("MAX_VALUE AS {0}", SqlColName_GeneratorMaxValue);
			sql.Select("INCREMENT_BY AS {0}", SqlColName_GeneratorIncrement);
			sql.Select("{0} AS {1}", (int)GeneratorType.Sequence, SqlColName_GeneratorType);
			sql.From("ALL_SEQUENCES");
			if (!String.IsNullOrEmpty(Importer.FilterSchema))
				sql.WhereAnd("SEQUENCE_OWNER LIKE '{0}' ESCAPE '{1}'", Importer.FilterSchema, Importer.FilterEscape);
			return sql.ToString();
		}


		public override string GetTablesSql()
		{
			ISqlStringBuilder sql = Importer.Genie.Lamp.GenieLampUtils.GetSqlStringBuilder();
			sql.Select("OWNER AS {0}", SqlColName_TableSchema);
			sql.Select("TABLE_NAME AS {0}", SqlColName_TableName);
			sql.From("ALL_TABLES");
			if (!String.IsNullOrEmpty(Importer.FilterSchema))
				sql.WhereAnd("OWNER LIKE '{0}' ESCAPE '{1}'", Importer.FilterSchema, Importer.FilterEscape);
			if (Importer.FilterTables.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("(");
				for(int i = 0; i < Importer.FilterTables.Count; i++)
				{
					sb.AppendFormat("{0}TABLE_NAME LIKE '{1}' ESCAPE '{2}'",
					                 i == 0 ? "" : " OR ",
					                 Importer.FilterTables[i],
					                 Importer.FilterEscape);
				}
				sb.Append(")");
				sql.WhereAnd(sb.ToString());

			}
			return sql.ToString();
		}

		public override string GetColumnsSql(MetaInfoTable table)
		{
			ISqlStringBuilder sql = Importer.Genie.Lamp.GenieLampUtils.GetSqlStringBuilder();
			sql.Select("COLUMN_NAME AS {0}", SqlColName_ColumnName);
			sql.Select("DATA_TYPE AS {0}", SqlColName_ColumnType);
			sql.Select("NVL(DATA_PRECISION, CHAR_LENGTH) AS {0}", SqlColName_ColumnTypeLength);
			sql.Select("NVL(DATA_SCALE, 0) AS {0}", SqlColName_ColumnTypePrecision);
			sql.Select("NULLABLE AS {0}", SqlColName_ColumnNullable);
			sql.Select("COLUMN_ID AS {0}", SqlColName_ColumnPosition);
			sql.From("ALL_TAB_COLUMNS");
			sql.WhereAnd("OWNER='{0}'", table.PersistentSchema);
			sql.WhereAnd("TABLE_NAME='{0}'", table.PersistentName);
			sql.OrderBy("COLUMN_ID ASC");
			return sql.ToString();
		}

		public override string GetPrimaryKeySql(MetaInfoTable table)
		{
			ISqlStringBuilder sql = Importer.Genie.Lamp.GenieLampUtils.GetSqlStringBuilder();
			sql.Select("C.CONSTRAINT_NAME AS {0}", SqlColName_ConstraintName);
			sql.Select("CC.COLUMN_NAME AS {0}", SqlColName_ColumnName);
			sql.Select("CC.POSITION AS {0}", SqlColName_ColumnPosition);
			sql.From("ALL_CONSTRAINTS C");
			sql.From("INNER JOIN ALL_CONS_COLUMNS CC ON C.CONSTRAINT_NAME = CC.CONSTRAINT_NAME");
			sql.WhereAnd("C.OWNER='{0}'", table.PersistentSchema);
			sql.WhereAnd("C.TABLE_NAME='{0}'", table.PersistentName);
			sql.WhereAnd("C.CONSTRAINT_TYPE='P'");
			sql.OrderBy("C.CONSTRAINT_NAME, CC.POSITION ASC");
			return sql.ToString();
		}

		public override string GetUniqueKeysSql(MetaInfoTable table)
		{
			ISqlStringBuilder sql = Importer.Genie.Lamp.GenieLampUtils.GetSqlStringBuilder();
			sql.Select("C.OWNER AS {0}", SqlColName_Schema);
			sql.Select("C.CONSTRAINT_NAME AS {0}", SqlColName_ConstraintName);
			sql.Select("CC.POSITION AS {0}", SqlColName_ColumnPosition);
			sql.Select("C.INDEX_OWNER AS {0}", SqlColName_IndexOwner);
			sql.Select("C.INDEX_NAME AS {0}", SqlColName_IndexName);
			sql.Select("CC.COLUMN_NAME AS {0}", SqlColName_ColumnName);
			sql.From("ALL_CONSTRAINTS C");
			sql.From("INNER JOIN ALL_CONS_COLUMNS CC ON C.OWNER = CC.OWNER AND C.CONSTRAINT_NAME = CC.CONSTRAINT_NAME");
			sql.WhereAnd("C.OWNER='{0}'", table.PersistentSchema);
			sql.WhereAnd("C.TABLE_NAME='{0}'", table.PersistentName);
			sql.WhereAnd("C.CONSTRAINT_TYPE='U'");
			sql.UnionAll();
			sql.Select("I.OWNER AS {0}", SqlColName_Schema);
			sql.Select("I.INDEX_NAME AS {0}", SqlColName_ConstraintName);
			sql.Select("IC.COLUMN_POSITION AS {0}", SqlColName_ColumnPosition);
			sql.Select("I.OWNER AS {0}", SqlColName_IndexOwner);
			sql.Select("I.INDEX_NAME AS {0}", SqlColName_IndexName);
			sql.Select("IC.COLUMN_NAME AS {0}", SqlColName_ColumnName);
			sql.From("ALL_INDEXES I");
			sql.From("INNER JOIN ALL_IND_COLUMNS IC ON I.OWNER = IC.INDEX_OWNER AND I.INDEX_NAME = IC.INDEX_NAME");
			sql.From("LEFT OUTER JOIN ALL_CONSTRAINTS C ON I.OWNER = C.OWNER AND I.INDEX_NAME = C.INDEX_NAME");
			sql.WhereAnd("I.OWNER='{0}'", table.PersistentSchema);
			sql.WhereAnd("I.TABLE_NAME='{0}'", table.PersistentName);
			sql.WhereAnd("I.UNIQUENESS = 'UNIQUE'");
			sql.WhereAnd("C.INDEX_NAME IS NULL");
			sql.OrderBy("1 ASC, 2 ASC, 3 ASC");
			return sql.ToString();
		}


		public override string GetIndexesSql()
		{
			ISqlStringBuilder sql = Importer.Genie.Lamp.GenieLampUtils.GetSqlStringBuilder();
			sql.Select("I.OWNER AS {0}", SqlColName_IndexOwner);
			sql.Select("I.INDEX_NAME AS {0}", SqlColName_IndexName);
			sql.Select("IC.COLUMN_POSITION AS {0}", SqlColName_ColumnPosition);
			sql.Select("IC.COLUMN_NAME AS {0}", SqlColName_ColumnName);
			sql.Select("I.TABLE_OWNER AS {0}", SqlColName_TableSchema);
			sql.Select("I.TABLE_NAME AS {0}", SqlColName_TableName);
			sql.Select("CASE I.UNIQUENESS WHEN 'UNIQUE' THEN 1 ELSE 0 END AS {0}", SqlColName_IsUnique);
			sql.From("ALL_INDEXES I");
			sql.From("INNER JOIN ALL_IND_COLUMNS IC ON I.OWNER = IC.INDEX_OWNER AND I.INDEX_NAME = IC.INDEX_NAME");
			if (!String.IsNullOrEmpty(Importer.FilterSchema))
				sql.WhereAnd("I.TABLE_OWNER LIKE '{0}' ESCAPE '{1}'", Importer.FilterSchema, Importer.FilterEscape);
			sql.WhereAnd("I.INDEX_NAME IS NOT NULL");
			sql.OrderBy("1 ASC, 2 ASC, 3 ASC");
			return sql.ToString();
		}


		public override string GetForeignKeysSql(MetaInfoTable table)
		{
			ISqlStringBuilder sql = Importer.Genie.Lamp.GenieLampUtils.GetSqlStringBuilder();
			sql.Select("C.CONSTRAINT_NAME AS {0}", SqlColName_ConstraintName);
			sql.Select("C.OWNER AS {0}", SqlColName_FKTableSchema);
			sql.Select("C.TABLE_NAME AS {0}", SqlColName_FKTableName);
			sql.Select("CC.COLUMN_NAME AS {0}", SqlColName_ColumnName);
			sql.Select("CC.POSITION AS {0}", SqlColName_ColumnPosition);
			sql.Select("C2.OWNER AS {0}", SqlColName_FKParentTableSchema);
			sql.Select("C2.TABLE_NAME AS {0}", SqlColName_FKParentTableName);
			sql.Select("CC2.COLUMN_NAME AS {0}", SqlColName_FKParentColumnName);
			sql.From("ALL_CONSTRAINTS C");
			sql.From("INNER JOIN ALL_CONS_COLUMNS CC ON C.OWNER = CC.OWNER AND C.CONSTRAINT_NAME = CC.CONSTRAINT_NAME");
			sql.From("INNER JOIN ALL_CONSTRAINTS C2 ON C.R_OWNER = C2.OWNER AND C.R_CONSTRAINT_NAME = C2.CONSTRAINT_NAME");
			sql.From("INNER JOIN ALL_CONS_COLUMNS CC2 ON C2.OWNER = CC2.OWNER AND C2.CONSTRAINT_NAME = CC2.CONSTRAINT_NAME AND CC.POSITION = CC2.POSITION");
			sql.WhereAnd("C.OWNER='{0}'", table.PersistentSchema);
			sql.WhereAnd("C.TABLE_NAME='{0}'", table.PersistentName);
			sql.WhereAnd("C.CONSTRAINT_TYPE='R'");
			sql.OrderBy("C.OWNER, C.TABLE_NAME, C.CONSTRAINT_NAME, CC.POSITION ASC");
			return sql.ToString();
		}
		#endregion

	}
}

