using System;
using System.Data;
using System.Data.Common;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Environments;

namespace GenieLamp.Genies.DbSchemaImport
{
	public abstract class MetadataProviderBase
	{
		protected const string SqlColName_Schema = "OBJECT_SCHEMA";
		protected const string SqlColName_Name = "OBJECT_NAME";
		protected const string SqlColName_TableName = "TABLE_NAME";
		protected const string SqlColName_TableSchema = "TABLE_SCHEMA";
		protected const string SqlColName_ColumnName = "COL_NAME";
		protected const string SqlColName_ColumnType = "DATA_TYPE";
		protected const string SqlColName_ColumnTypeLength = "DATA_LENGTH";
		protected const string SqlColName_ColumnTypePrecision = "DATA_PRECISION";
		protected const string SqlColName_ColumnNullable = "NULLABLE";
		protected const string SqlColName_ColumnPosition = "POSITION";
		protected const string SqlColName_ConstraintName = "CONSTRAINT_NAME";
		protected const string SqlColName_FKTableSchema = "TABLE_SCHEMA";
		protected const string SqlColName_FKTableName = "TABLE_NAME";
		protected const string SqlColName_FKParentTableSchema = "PARENT_TABLE_SCHEMA";
		protected const string SqlColName_FKParentTableName = "PARENT_TABLE_NAME";
		protected const string SqlColName_FKParentColumnName = "PARENT_COL_NAME";
		protected const string SqlColName_IndexOwner = "INDEX_OWNER";
		protected const string SqlColName_IndexName = "INDEX_NAME";
		protected const string SqlColName_IsUnique = "IS_UNIQUE";
		protected const string SqlColName_GeneratorMinValue = "GEN_MIN_VALUE";
		protected const string SqlColName_GeneratorMaxValue = "GEN_MAX_VALUE";
		protected const string SqlColName_GeneratorIncrement = "GEN_INCREMENT";
		protected const string SqlColName_GeneratorType = "GEN_TYPE";

		public DbConnection Connection { get; private set; }
		public string ProviderName { get; private set; }

		public SchemaImporter Importer { get;  private set; }
		protected IEnvironmentHelper Environment { get; private set; }

		#region Constructors
		protected MetadataProviderBase(DbConnection connection, string providerName, SchemaImporter importer)
		{
			this.Connection = connection;
			this.ProviderName = providerName;
			this.Importer = importer;
		}

		public static MetadataProviderBase CreateProvider(string providerName, string connectionString, SchemaImporter importer)
		{
			DbConnection connection;
	        try
	        {
	            DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);
	            connection = factory.CreateConnection();
				connection.ConnectionString = connectionString;
	        }
	        catch (Exception e)
	        {
				throw new GlException(String.Format("Unable to create connection \"{0}\". Verify provider name", providerName), e);
	        }

			MetadataProviderBase metaProvider = null;
			if (connection.GetType().Name.Equals("OracleConnection", StringComparison.InvariantCultureIgnoreCase))
			{
				metaProvider = new MetadataProviderOracle(connection, providerName, importer);
			}
			else
			{
				throw new GlException("Schema import is not supported for data provider {0}", providerName);
			}

			return metaProvider;
		}
		#endregion

		protected ILogger Logger
		{
			get { return Importer.Genie.Lamp.Logger; }
		}

		protected void SetEnvironment(TargetEnvironment target)
		{
			this.Environment = Importer.Genie.Lamp.GenieLampUtils.GetEnvironmentHelper(target);
		}

		public void Connect()
		{
			try
			{
				Connection.Open();
			}
			catch(Exception e)
			{
				throw new GlException("Unable to connect. Verify connection string.", e);
			}
		}

		public void Close()
		{
			if (Connection != null && Connection.State == ConnectionState.Open)
				Connection.Close();
		}

		internal DataTable GetSchema()
		{
			return Connection.GetSchema();
		}

		protected DataTable SelectData(string sql)
		{
			DbCommand cmd = this.Connection.CreateCommand();
			cmd.CommandText = sql.ToString();
			cmd.CommandType = CommandType.Text;
			DataTable dt = new DataTable();
			dt.Load(cmd.ExecuteReader());
			return dt;
		}


		public abstract string GetGeneratorsSql();

		public void GetGenerators(MetaInfoGenerators generators)
		{
			generators.Clear();
			foreach(DataRow row in SelectData(GetGeneratorsSql()).Rows)
			{
				MetaInfoGenerator gen = new MetaInfoGenerator();
				gen.PersistentSchema = row[SqlColName_Schema].ToString();
				gen.PersistentName = row[SqlColName_Name].ToString();
				gen.Schema = Importer.Naming.Convert(gen.PersistentSchema);
				gen.Name = Importer.Naming.Convert(gen.PersistentName);
				gen.MinValue = row[SqlColName_GeneratorMinValue].ToString();
				gen.MaxValue = row[SqlColName_GeneratorMaxValue].ToString();
				gen.Increment = row[SqlColName_GeneratorIncrement].ToString();
				gen.Type = (GeneratorType)int.Parse(row[SqlColName_GeneratorType].ToString());

				gen.Schema = Importer.Naming.Convert(gen.PersistentSchema);
				gen.Name = Importer.Naming.Convert(gen.PersistentName);

				generators.Add(gen);

				Logger.TraceLine("Generator {0} imported", gen.FullPersistentName);
			}
			Logger.ProgressStep();
		}


		public abstract string GetTablesSql();

		public virtual void GetTables(MetaInfoTables tables)
		{
			tables.Clear();
			foreach(DataRow row in SelectData(GetTablesSql()).Rows)
			{
				MetaInfoTable table = new MetaInfoTable();
				table.PersistentSchema = row[SqlColName_TableSchema].ToString();
				table.PersistentName = row[SqlColName_TableName].ToString();

				table.Schema = Importer.Naming.Convert(table.PersistentSchema);
				table.Name = Importer.Naming.Convert(table.PersistentName);

				tables.Add(table);

				Logger.TraceLine("Table {0} imported", table.FullPersistentName);
			}
			Logger.ProgressStep();
		}

		public abstract string GetColumnsSql(MetaInfoTable table);

		public virtual void GetColumns(MetaInfoTable table)
		{
			table.Columns.Clear();
			foreach(DataRow row in SelectData(GetColumnsSql(table)).Rows)
			{
				MetaInfoColumn col = new MetaInfoColumn();
				col.Table = table;
				col.PersistentName = row[SqlColName_ColumnName].ToString();
				col.PersistentTypeName = row[SqlColName_ColumnType].ToString();
				if (!row.IsNull(SqlColName_ColumnTypeLength))
				{
					col.PersistentLength = int.Parse(row[SqlColName_ColumnTypeLength].ToString());
					col.Length = col.PersistentLength;
				}
				if (!row.IsNull(SqlColName_ColumnTypePrecision))
					col.Precision = int.Parse(row[SqlColName_ColumnTypePrecision].ToString());
				col.Nullable = row[SqlColName_ColumnNullable].ToString().Equals("Y", StringComparison.InvariantCultureIgnoreCase);

				col.Name = Importer.Naming.Convert(col.PersistentName);
				col.Type = this.Environment.ImportType(col.PersistentTypeName, col);

				table.Columns.Add(col);
			}
			Logger.ProgressStep();
			Logger.TraceLine("Table columns {0} imported", table.FullPersistentName);
		}

		public abstract string GetPrimaryKeySql(MetaInfoTable table);

		public virtual void GetPrimaryKey(MetaInfoTable table)
		{
			table.PrimaryKey = new MetaInfoPrimaryKey();
			foreach(DataRow row in SelectData(GetPrimaryKeySql(table)).Rows)
			{
				if (String.IsNullOrEmpty(table.PrimaryKey.PersistentName))
				{
					table.PrimaryKey.PersistentName = row[SqlColName_ConstraintName].ToString();
					table.PrimaryKey.Name = Importer.Naming.Convert(table.PrimaryKey.PersistentName);
				}
				table.PrimaryKey.Columns.Add(table.Columns.FindByPersistentName(
					row[SqlColName_ColumnName].ToString(), true));
			}
			Logger.ProgressStep();
		}


		public abstract string GetUniqueKeysSql(MetaInfoTable table);

		public void GetUniqueKeys(MetaInfoTable table)
		{
			table.UniqueConstraints.Clear();
			foreach(DataRow row in SelectData(GetUniqueKeysSql(table)).Rows)
			{
				string persistentSchema = row[SqlColName_Schema].ToString();
				string persistentName = row[SqlColName_ConstraintName].ToString();
				MetaInfoUniqueConstraint uc = table.UniqueConstraints.FindByPersistentName(persistentSchema, persistentName, false);
				if (uc == null)
				{
					uc = new MetaInfoUniqueConstraint();
					uc.PersistentSchema = persistentSchema;
					uc.PersistentName = persistentName;
					table.UniqueConstraints.Add(uc);
				}
				uc.Columns.Add(table.Columns.FindByPersistentName(row[SqlColName_ColumnName].ToString(), true));
			}
			Logger.ProgressStep();
		}

		public abstract string GetIndexesSql();

		public void GetIndexes(MetaInfoIndexes indexes, MetaInfoTables tables)
		{
			//table.Indexes.Clear();
			foreach(DataRow row in SelectData(GetIndexesSql()).Rows)
			{
				string persistentSchema = row[SqlColName_IndexOwner].ToString();
				string persistentName = row[SqlColName_IndexName].ToString();
				bool isUnique = int.Parse(row[SqlColName_IsUnique].ToString()) == 1 ? true : false;
				MetaInfoIndex ix = indexes.FindByPersistentName(persistentSchema, persistentName, false);
				bool indexExists = ix != null;
				if (!indexExists)
				{
					ix = new MetaInfoIndex();
					ix.PersistentSchema = persistentSchema;
					ix.PersistentName = persistentName;
					ix.IsUnique = isUnique;
				}
				string tableSchema = row[SqlColName_TableSchema].ToString();
				string tableName = row[SqlColName_TableName].ToString();
				MetaInfoTable table = tables.FindByPersistentName(tableSchema, tableName, true);
				if (table != null)
				{
					ix.Table = table;
					ix.Columns.Add(table.Columns.FindByPersistentName(row[SqlColName_ColumnName].ToString(), true));
					if (!indexExists)
					{
						table.Indexes.Add(ix);
						indexes.Add(ix);
					}
				}
				Logger.ProgressStep();
			}
		}
		

		public abstract string GetForeignKeysSql(MetaInfoTable tabel);

		public virtual void GetForeignKeys(MetaInfoTables tables)
		{
			foreach(MetaInfoTable table in tables)
			{
				table.ForeignKeys.Clear();
				foreach(DataRow row in SelectData(GetForeignKeysSql(table)).Rows)
				{
					string persistentSchema = row[SqlColName_FKTableSchema].ToString();
					string persistentName = row[SqlColName_ConstraintName].ToString();
					MetaInfoForeignKey fk = table.ForeignKeys.FindByPersistentName(persistentSchema, persistentName, false);
					if (fk == null)
					{
						string msg = String.Format("Importing FK: {0}...", MetaInfoBase.MakeFullName(persistentSchema, persistentName));
						fk = new MetaInfoForeignKey();
						fk.PersistentSchema = persistentSchema;
						fk.PersistentName = persistentName;
						fk.Child = table;
						string parentTableSchema = row[SqlColName_FKParentTableSchema].ToString();
						string parentTableName = row[SqlColName_FKParentTableName].ToString();
						fk.Parent = tables.FindByPersistentName(parentTableSchema, parentTableName, false);
						if (fk.Parent != null)
						{
							table.ForeignKeys.Add(fk);
							fk.Name = fk.Parent.Name;
							Logger.TraceLine("{0}OK", msg);
						}
						else
							Logger.TraceLine("{0}Ignored (parent table {1} was not imported)", msg, MetaInfoBase.MakeFullName(parentTableSchema, parentTableName));
					}

					if (fk.Child != null && fk.Parent != null)
					{
						MetaInfoColumnsMatch cm = new MetaInfoColumnsMatch();
						cm.Child = fk.Child.Columns.FindByPersistentName(row[SqlColName_ColumnName].ToString(), true);
						cm.Parent = fk.Parent.Columns.FindByPersistentName(row[SqlColName_FKParentColumnName].ToString(), true);
						fk.ColumnsMatches.Add(cm);
					}
					Logger.ProgressStep();
				}

				// Renaming duplicates in FK names
				foreach(MetaInfoForeignKey fk in table.ForeignKeys)
				{
					int count = 1;
					foreach(MetaInfoForeignKey fk2 in table.ForeignKeys)
					{
						if (fk != fk2 && fk.Name == fk2.Name)
							fk2.Name = String.Format("{0}{1}", fk.Name, count++);
					}
				}
			}
		}
	}
}

