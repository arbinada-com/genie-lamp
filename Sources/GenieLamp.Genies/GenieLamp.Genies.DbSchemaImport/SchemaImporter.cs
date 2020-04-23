using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Data;
using System.Data.Common;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class SchemaImporter
	{
		private const string CfgParamName_ProviderName = "ProviderName";
		private const string CfgParamName_ConnectionString = "ConnectionString";
		private const string CfgParamName_DefaultSchema = "DefaultSchema";
		private const string CfgParamName_DefaultPersistentSchema = "DefaultPersistentSchema";
		private const string CfgParamName_FilterDb = "Filter.Database";
		private const string CfgParamName_FilterSchema = "Filter.Schema";
		private const string CfgParamName_FilterTables = "Filter.Tables";
		private const string CfgParamName_FilterEscape = "Filter.EscapeSymbol";

		private string outFileName;
		private IDocHelper md;
		private string providerName;
		private string connectionString;
		private MetadataProviderBase metaProvider;

		protected MetaInfoTables Tables { get; private set; }
		protected MetaInfoGenerators Generators { get; private set; }
		protected MetaInfoIndexes Indexes { get; private set; }
		protected string DefaultSchema { get; set; }
		protected string DefaultPersistentSchema { get; set; }

		public DbSchemaImportGenie Genie { get; private set; }
		public List<string> FilterTables { get; private set; }
		public string FilterEscape {get; private set; }
		public string FilterDb { get; private set; }
		public string FilterSchema  {get; private set; }
		public INamingConvention Naming {get; private set; }

		#region Constructors
		public SchemaImporter(DbSchemaImportGenie genie)
		{
			this.Genie = genie;
			genie.Model.MetaObjects.SetUnprocessedAll();
			outFileName = Path.GetFileNameWithoutExtension(genie.Config.OutFileName) + ".xml";

			providerName = genie.Config.Params.ParamByName(CfgParamName_ProviderName, true).Value;
			connectionString = genie.Config.Params.ParamByName(CfgParamName_ConnectionString, true).Value;

			DefaultSchema = genie.Config.Params.ValueByName(CfgParamName_DefaultSchema, "ToChange");
			DefaultPersistentSchema = genie.Config.Params.ValueByName(CfgParamName_DefaultPersistentSchema, "TOCHANGE");

			md = genie.Lamp.GenieLampUtils.Xml.CreateDocHelper("Model");
			md.AddAttribute("xmlns", Model.XmlNamespace);
			md.AddAttribute("defaultSchema", DefaultSchema);
			md.AddAttribute("defaultPersistentSchema", DefaultPersistentSchema);

			FilterDb = null;
			FilterSchema = null;
			FilterDb = genie.Config.Params.ValueByName(CfgParamName_FilterDb, FilterDb);
			FilterSchema = genie.Config.Params.ValueByName(CfgParamName_FilterSchema, FilterSchema);

			FilterEscape = "#";
			FilterEscape = genie.Config.Params.ValueByName(CfgParamName_FilterEscape, FilterEscape);


			FilterTables = new List<string>();
			int count = 1;
			IParamSimple param;
			do
			{
				param = genie.Config.Params.ParamByName(String.Format("{0}{1}", CfgParamName_FilterTables, count++));
				if (param != null)
					FilterTables.Add(param.Value);

			}
			while(param != null);

			if (!PersistenceLayerConfig.IsDefined)
				throw new ApplicationException("Persistence layer configuration is not defined in project");

			this.Naming = genie.Lamp.GenieLampUtils.GetNamingConvention(NamingStyle.CamelCase);

			this.Tables = new MetaInfoTables();
			this.Generators = new MetaInfoGenerators();
			this.Indexes = new MetaInfoIndexes();
		}
		#endregion

		protected ILogger Logger
		{
			get { return Genie.Lamp.Logger; }
		}

		public IPersistenceLayerConfig PersistenceLayerConfig
		{
			get { return Genie.Lamp.Config.Layers.PersistenceConfig; }
		}

		public IModel Model
		{
			get { return Genie.Model; }
		}

		public static void ShowDbProviders()
		{
		    DataTable table = DbProviderFactories.GetFactoryClasses();
		    foreach (DataRow row in table.Rows)
		    {
		        foreach (DataColumn column in table.Columns)
		        {
		            Console.Write("{0} | ", row[column]);
		        }
				Console.WriteLine();
		    }
		}

		public virtual void Run()
		{
			metaProvider = MetadataProviderBase.CreateProvider(providerName, connectionString, this);
			metaProvider.Connect();
			try
			{
				ProcessGenerators();
				ProcessEntities();
				ProcessForeignKeys();
				ProcessIndexes();
			}
			finally
			{
				metaProvider.Close();
			}
			md.Document.Save(String.Format("{0}", Path.Combine(Genie.Config.OutDir, outFileName)));
		}


		private void ProcessGenerators()
		{
			metaProvider.GetGenerators(this.Generators);

			md.SetCurrentToRoot();
			md.CreateComment("Imported generators");
			foreach(MetaInfoGenerator gen in this.Generators)
			{
				gen.Node = md.CreateElement("Generator");
				md.AddAttribute("name", gen.Name);
				md.AddAttribute("type", Enum.GetName(typeof(GeneratorType), gen.Type).ToLower());
				md.AddAttribute("persistentSchema", gen.PersistentSchema);
				md.AddAttribute("persistentName", gen.PersistentName);
				md.AddAttribute("startWith", gen.MinValue);
				md.AddAttribute("increment", gen.Increment);
				md.AddAttribute("maxValue", gen.MaxValue);
				md.Document.DocumentElement.AppendChild(gen.Node);
			}
		}


		private void ProcessEntities()
		{
			metaProvider.GetTables(this.Tables);

			foreach(MetaInfoTable table in Tables)
			{
				md.SetCurrentToRoot();
				md.CreateComment("Entity imported from table {0}.{1}", table.PersistentSchema, table.PersistentName);

				table.Node = md.CreateElement("Entity");
				if (!DefaultSchema.Equals(table.Schema, StringComparison.InvariantCultureIgnoreCase))
					md.AddAttribute("schema", table.Schema);
				md.AddAttribute("name", table.Name);
				if (!DefaultPersistentSchema.Equals(table.PersistentSchema, StringComparison.InvariantCultureIgnoreCase))
					md.AddAttribute("persistentSchema", table.PersistentSchema);
				md.AddAttribute("persistentName", table.PersistentName);
				md.Document.DocumentElement.AppendChild(md.CurrentNode);

				ProcessAttributes(table);

				ProcessPrimaryKey(table);

				ProcessUniqueKeys(table);
			}
		}

		private void ProcessAttributes(MetaInfoTable table)
		{
			metaProvider.GetColumns(table);
			foreach(MetaInfoColumn col in table.Columns)
			{
				md.CreateElement("Attribute");
				md.AddAttribute("name", col.Name);
				md.AddAttribute("type", col.Type.Name);
				if (col.HasLength)
					md.AddAttribute("length", col.Length.ToString());
				if (col.HasPrecision)
					md.AddAttribute("precision", col.Precision.ToString());
				md.AddAttribute("persistentType", col.CompletePersistentTypeName);
				md.AddAttribute("persistentName", col.PersistentName);
				if (col.Nullable)
					md.AddAttribute("required", "false");
				table.Node.AppendChild(md.CurrentNode);
			}
		}

		private void ProcessPrimaryKey(MetaInfoTable table)
		{
			metaProvider.GetPrimaryKey(table);
			if (table.PrimaryKey != null)
			{
				XmlNode pkNode = md.CreateElement("PrimaryId");
				if (table.PrimaryKey.Columns.Count > 1)
					md.AddAttribute("name", table.PrimaryKey.Name);
				md.AddAttribute("persistentName", table.PrimaryKey.PersistentName);
				foreach(MetaInfoColumn col in table.PrimaryKey.Columns)
				{
					md.CreateElement("OnAttribute");
					md.AddAttribute("name", col.Name);
					pkNode.AppendChild(md.CurrentNode);
				}

				// Try to attach generator by name
				if (table.PrimaryKey.Columns.Count == 1)
				{
					IMacroExpander macro = Genie.Config.Macro.CreateChild();
					macro.SetMacro("%TABLE%", table.Name);
					string generatorTemplate = PersistenceLayerConfig.NamingConvention.Params.ValueByName("Generator.Template", "%TABLE%");
					MetaInfoGenerator gen = Generators.FindByName(table.Schema, macro.Subst(generatorTemplate), false);
					if (gen == null)
					{
						macro.SetMacro("%TABLE%", table.PersistentName);
						gen = Generators.FindByPersistentName(table.PersistentSchema, macro.Subst(generatorTemplate), false);
					}
					if (gen != null)
					{
						md.CurrentNode = pkNode;
						md.AddAttribute("generator", gen.Name);
						gen.Node = md.Document.DocumentElement.RemoveChild(gen.Node);
						md.Document.DocumentElement.InsertBefore(gen.Node, table.Node);
					}
				}

				table.Node.AppendChild(pkNode);
			}
		}


		private void ProcessUniqueKeys(MetaInfoTable table)
		{
			metaProvider.GetUniqueKeys(table);
			foreach(MetaInfoUniqueConstraint uc in table.UniqueConstraints)
			{
				XmlNode ucNode = md.CreateElement("UniqueId");
				md.AddAttribute("persistentName", uc.PersistentName);
				foreach(MetaInfoColumn col in uc.Columns)
				{
					md.CreateElement("OnAttribute");
					md.AddAttribute("name", col.Name);
					ucNode.AppendChild(md.CurrentNode);
				}
				table.Node.AppendChild(ucNode);
			}
		}


		private void ProcessForeignKeys()
		{
			metaProvider.GetForeignKeys(this.Tables);
			metaProvider.GetIndexes(this.Indexes, this.Tables);

			foreach(MetaInfoTable table in this.Tables)
			{
				foreach(MetaInfoForeignKey fk in table.ForeignKeys)
				{
					md.SetCurrentToRoot();
					fk.Node = md.CreateElement("Relation");
					XmlNode commentNode = md.CreateComment("Relation imported from FK {0}", fk.FullPersistentName);
					md.AddAttribute("name", fk.Name);
					md.AddAttribute("entity", fk.Child.Name);
					// name2 will be generated by GL
					md.AddAttribute("entity2", fk.Parent.Name);
					md.AddAttribute("cardinality", "M:1");
					md.AddAttribute("persistentName", fk.PersistentName);

					// Try to associate with index
					MetaInfoColumns fkCols = new MetaInfoColumns();
					foreach(MetaInfoColumnsMatch cm in fk.ColumnsMatches)
					{
						fkCols.Add(cm.Child);
					}
					MetaInfoIndex ix = this.Indexes.FindByColumns(fkCols);
					if (ix != null)
					{
						md.AddAttribute("indexName", ix.PersistentName);
						ix.Generate = false;
					}

					foreach(MetaInfoColumnsMatch cm in fk.ColumnsMatches)
					{
						md.CreateElement("AttributeMatch");
						md.AddAttribute("attribute", cm.Child.Name);
						md.AddAttribute("attribute2", cm.Parent.Name);
						fk.Node.AppendChild(md.CurrentNode);
					}

					// Place node after child entity declaration
					if (fk.Child != null)
					{
						md.Document.DocumentElement.InsertAfter(fk.Node, fk.Child.Node);
					}
					else
					{
						md.Document.DocumentElement.AppendChild(commentNode);
						md.Document.DocumentElement.AppendChild(fk.Node);
					}
				}
			}
		}

		private void ProcessIndexes()
		{
			// Mark non-generated indexes
			foreach(MetaInfoTable table in this.Tables)
			{
				if (table.PrimaryKey != null)
				{
					MetaInfoIndex ix = this.Indexes.FindByPersistentName(table.PersistentSchema, table.PrimaryKey.PersistentName, false);
					if (ix != null)
						ix.Generate = false;
				}

				foreach(MetaInfoUniqueConstraint uc in table.UniqueConstraints)
				{
					MetaInfoIndex ix = this.Indexes.FindByPersistentName(uc.PersistentSchema, uc.PersistentName, false);
					if (ix != null)
						ix.Generate = false;
				}
			}


			foreach(MetaInfoIndex ix in this.Indexes)
			{
				if (!ix.Generate)
					continue;
				md.SetCurrentToRoot();
				md.CreateComment("Index imported from {0}", ix.FullPersistentName);
				ix.Node = md.CreateElement("Index");
				md.AddAttribute("name", ix.PersistentName);
				md.AddAttribute("unique", ix.IsUnique ? "true" : "false");
				md.AddAttribute("entitySchema", ix.Table.Schema);
				md.AddAttribute("entityName", ix.Table.Name);
				foreach(MetaInfoColumn col in ix.Columns)
				{
					md.CreateElement("OnAttribute");
					md.AddAttribute("name", col.Name);
					ix.Node.AppendChild(md.CurrentNode);
				}

				md.Document.DocumentElement.AppendChild(ix.Node);
			}
		}

	}
}

