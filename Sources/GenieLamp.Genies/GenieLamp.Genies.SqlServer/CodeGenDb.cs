using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Metamodel.Physical;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.Sql;

namespace GenieLamp.Genies.SqlServer
{
	public class CodeGenDb
	{
		private SqlServerGenie genie;
		private ICodeWriterTransactSql updater;
		private ICodeWriterTransactSql creator;
		private ICodeWriterTransactSql cleaner;
		private IEnvironmentHelper environment;
		private string outFileNameDDLCreate;
		private string outFileNameDDLUpdate;
		private string outFileNameDDLDelete;
		private bool createDatabase = true;
		private string databaseName = String.Empty;
		private bool createSchemas = true;
		private bool useUniqueIndexes = false;

		#region Constructors
		public CodeGenDb(SqlServerGenie owner)
		{
			genie = owner;
			owner.Model.MetaObjects.SetUnprocessedAll();
			environment = owner.Model.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.SqlServer);

			createDatabase = genie.Config.Params.ValueByName("Database.Create", createDatabase);
			databaseName = genie.Config.Params.ParamByName("Database.Name", true).Value;
			createSchemas = genie.Config.Params.ValueByName("Schemas.Create", createSchemas);
			useUniqueIndexes = genie.Config.Params.ValueByName("UniqueIndexConstraint", useUniqueIndexes);

			updater = owner.Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			updater.DefaultOutFileEncoding = owner.Config.OutFileEncoding;
			creator = genie.Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			creator.DefaultOutFileEncoding = owner.Config.OutFileEncoding;
			cleaner = genie.Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			cleaner.DefaultOutFileEncoding = owner.Config.OutFileEncoding;

			outFileNameDDLCreate = Path.Combine(genie.Config.OutDir, "CRE_" + genie.Config.OutFileName);
			outFileNameDDLUpdate = Path.Combine(genie.Config.OutDir, "UPD_" + genie.Config.OutFileName);
			outFileNameDDLDelete = Path.Combine(genie.Config.OutDir, "DEL_" + genie.Config.OutFileName);

		}
		#endregion

		public string OutFileNameDDLUpdate
		{
			get { return outFileNameDDLUpdate; }
		}

		private IModel Model
		{
			get { return genie.Model; }
		}
		
		public void Run()
		{
			BeginScripting();
			
			RunCreation();
			RunCleanup();
			
			EndScripting();
		}

		private void RunCreation()
		{
			ProcessDatabase();
			Model.Lamp.Logger.ProgressStep();
			ProcessSchemas();
			Model.Lamp.Logger.ProgressStep();
			ProcessEntities();
			Model.Lamp.Logger.ProgressStep();
			ProcessRelations();
			Model.Lamp.Logger.ProgressStep();
			ProcessIndexes();
			Model.Lamp.Logger.ProgressStep();
		}

		private void RunCleanup()
		{
			foreach (IRelation r in Model.Relations)
			{
				if (!r.Persistence.Persisted)
					continue;
				cleaner.WriteDropForeignKey(r, environment);
				cleaner.EndBatch();
			}

			cleaner.WriteLine();
			foreach (IEntity entity in Model.Entities)
			{
				if (!entity.Persistence.Persisted)
					continue;
				cleaner.WriteDropTable(entity, environment);
				cleaner.EndBatch();
			}
		}

		private void BeginScripting()
		{
			updater.WriteStdHeader(genie);
			creator.WriteStdHeader(genie);
			cleaner.WriteStdHeader(genie);
		}

		private void EndScripting()
		{
			creator.Save(outFileNameDDLCreate);
			updater.Save(outFileNameDDLUpdate);
			cleaner.Save(outFileNameDDLDelete);

			genie.Config.NotifyAssistants("Finished", null, creator.ToString(true));
		}


		private void WriteUpdate(string condition, ICodeWriterTransactSql sql, bool spExecuteSql = false)
		{
			updater.WriteLine("IF {0}", condition);
			updater.BeginScope();
			if (!spExecuteSql)
				updater.WriteFrom(sql);
			else
				updater.WriteSpExecuteSql(sql.ToConst(sql.ToString(true), false));
			updater.EndScope();
		}

		private void ProcessDatabase()
		{
			ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			if (createDatabase)
			{
				sql.WriteLine("CREATE DATABASE [{0}]", databaseName);
				ISpellHint hint = genie.FindHint("Database", databaseName);
				if (hint != null)
					sql.WriteText(hint.GetText(null));
				sql.WriteSeparator();
				creator.WriteFrom(sql);
				creator.EndBatch();
				creator.WriteLine();
				WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.databases WHERE name = N'{0}')", databaseName), sql);
				genie.Config.NotifyAssistants("CreateDatabase", null, sql.ToString(true));
				updater.EndBatch();
				updater.WriteLine();
			}

			sql.ClearAll();
			sql.WriteLine("USE [{0}]", databaseName);
			sql.EndBatch();
			sql.WriteLine();
			creator.WriteFrom(sql);
			updater.WriteFrom(sql);
		}

		private void ProcessSchemas()
		{
			if (createSchemas)
			{
				List<string> schemas = new List<string>();
				ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
				foreach (ISchema schema in Model.Schemas)
				{
					if (schemas.Contains(schema.Persistence.Name))
					    continue;
					schemas.Add(schema.Persistence.Name);
					sql.ClearAll();
					sql.WriteLine("CREATE SCHEMA [{0}] AUTHORIZATION [dbo]", schema.Persistence.Name);
					ISpellHint hint = genie.FindHint("Schema", schema.Name);
					if (hint != null)
						sql.WriteText(hint.GetText(null));
					WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.schemas WHERE name = N'{0}')", schema.Persistence.Name), sql, true);
					creator.WriteFrom(sql);
					creator.EndBatch();
					creator.WriteLine();
					updater.EndBatch();
					updater.WriteLine();
				}
			}
		}


		void ProcessEntities ()
		{
			foreach (IEntity entity in Model.Entities)
			{
				ProcessEntity(entity);
				updater.WriteLine();
				Model.Lamp.Logger.ProgressStep();
			}
		}

		private void ProcessEntity(IEntity entity)
		{
			if (!entity.Persistence.Persisted)
				return;

			updater.WriteLine(creator.WriteCommentLine("Table of {0}", entity.FullName));

			ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			sql.WriteCreateTable(entity, environment);
			ISpellHint hint = genie.FindHint(entity);
			if (hint != null)
				sql.WriteText(hint.GetText(entity));
			creator.WriteFrom(sql);
			creator.EndBatch();

			WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'{0}'))", entity.Persistence.FullName), sql);

			genie.Config.NotifyAssistants("Create", entity, sql.ToString(true));
			updater.EndBatch();

			IAttributes attributes;
			switch(Model.Lamp.Config.Layers.DomainConfig.MappingStrategy)
			{
			case GenieLamp.Core.Layers.MappingStrategy.TablePerSubclass:
				attributes = entity.GetAttributes(false);
				break;
			case GenieLamp.Core.Layers.MappingStrategy.TablePerClass:
				attributes = entity.GetAttributes(true);
				break;
			default:
				throw new GenieLamp.Core.Exceptions.GlException("Mapping strategy is not supported: '{0}'",
				                                                Model.Lamp.Config.Layers.DomainConfig.MappingStrategy);
			}

			UpdateAttributes(entity, attributes);

			ProcessPrimaryKey(entity);

			ProcessUniqueIds(entity);

//			creator.WriteLine();
//			updater.WriteLine();
//			WriteComment(CommentTarget.Table, entity.Persistence.FullName, entity.Doc);
//
//			foreach (IAttribute a in attributes)
//			{
//				if (a.Persistence.Persisted)
//				{
//					WriteComment(CommentTarget.Column,
//					             String.Format("{0}.{1}", entity.Persistence.FullName, a.Persistence.Name),
//					             a.Doc);
//				}
//			}
		}

		void UpdateAttributes (IEntity entity, IAttributes attributes)
		{
			foreach (IAttribute a in attributes)
			{
				if (a.Persistence.Persisted)
				{
					ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
					sql.WriteLine("ALTER TABLE {0} ADD ", entity.Persistence.FullName);
					sql.Indent++;
					sql.WriteLine("{0} {1} NULL {2}{3}",
					              a.Persistence.Name,
					              environment.ToTypeName(a, false),
					              a.TypeDefinition.HasDefault ? "DEFAULT " + environment.ToDefaultValue(a) : "",
					              sql.Separator);
					sql.Indent--;
					if (a.TypeDefinition.Required && a.TypeDefinition.HasDefault)
					{
						sql.WriteSpExecuteSql("UPDATE {0} SET {1} = {2}{3}",
						                      entity.Persistence.FullName,
						                      a.Persistence.Name,
						                      environment.ToDefaultValue(a),
						                      sql.Separator);

						sql.WriteLine("ALTER TABLE {0} ALTER COLUMN {1} {2} NOT NULL{3}",
						              entity.Persistence.FullName,
						              a.Persistence.Name,
						              environment.ToTypeName(a, false),
						              sql.Separator);
					}

					WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'{0}') AND name = '{1}')",
					                          entity.Persistence.FullName, a.Persistence.Name),
					            sql);

					genie.Config.NotifyAssistants("Update", a, sql.ToString(true));
					updater.EndBatch();
				}
			}
			updater.WriteLine();
		}


		private void ProcessPrimaryKey(IEntity entity)
		{
			ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			sql.WriteCreatePrimaryKey(entity.PrimaryId, environment);
			ISpellHint hint = genie.FindHint(entity.PrimaryId);
			if (hint != null)
				sql.WriteText(hint.GetText(entity.PrimaryId));
			creator.WriteFrom(sql);

			WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'{0}') AND is_primary_key = 1)",
			                          entity.Persistence.FullName),
			            sql);

			genie.Config.NotifyAssistants("Create", entity.PrimaryId, sql.ToString(true));

			creator.EndBatch();
			creator.WriteLine();
			updater.EndBatch();
			updater.WriteLine();
		}


		private void ProcessUniqueIds(IEntity entity)
		{
			ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
				sql.ClearAll();
				if (!useUniqueIndexes)
				{
					sql.WriteCreateUniqueConstraint(uid, environment);
					creator.WriteFrom(sql);

					WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'{0}') AND is_unique_constraint = 1 AND is_primary_key = 0 AND name = N'{1}')",
					                          entity.Persistence.FullName,
					                          uid.Persistence.Name),
					            sql);
					creator.EndBatch();
					uid.Index.Processed = true;

					genie.Config.NotifyAssistants("Create", uid, sql.ToString(true));

					creator.WriteLine();
					updater.EndBatch();
					updater.WriteLine();
				}
			}
		}


		void ProcessRelations ()
		{
			foreach (IRelation r in Model.Relations)
			{
				if (!r.Persistence.Persisted)
					continue;
				if (r is ISubtypeRelation)
					creator.WriteLine(updater.WriteCommentLine(
						"Subtyping reference: {0} is a {1}", 
						r.ForeignKey.ChildTable.FullName,
						r.ForeignKey.ParentTable.FullName));
				
				ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
				sql.WriteCreateForeignKey(r, environment);
				creator.WriteFrom(sql);
				creator.WriteSeparator();
				
				WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name = N'{0}')",
				                          r.ForeignKey.Name),
				            sql);
				updater.EndBatch();

				genie.Config.NotifyAssistants("Create", r, sql.ToString());
			}
		}


		void ProcessIndexes ()
		{
			ICodeWriterTransactSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterTransactSql();
			foreach (IIndex index in Model.PhysicalModel.Indexes)
			{
				if (!index.Processed && index.Generate)
				{
					sql.ClearAll();
					sql.WriteCreateIndex(index, environment);
					ISpellHint hint = genie.FindHint(index);
					if (hint != null)
						sql.WriteText(hint.GetText(index));
					creator.WriteFrom(sql);
					WriteUpdate(String.Format("NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'{0}') AND name = N'{1}')",
					                          index.Entity.Persistence.FullName,
					                          index.Name),
					            sql);
					creator.EndBatch();
					updater.EndBatch();
					creator.WriteLine();
					updater.WriteLine();

					genie.Config.NotifyAssistants("Create", index, sql.ToString(true));
				}
			}
		}
	}
}

