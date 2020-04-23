using System;
using System.IO;
using System.Text;

using GenieLamp.Core;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Patterns;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Metamodel.Physical;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.Sql;

namespace GenieLamp.Genies.Sqlite
{
	public class CodeGenDb
	{
		private SqliteGenie genie;
		private bool dbCreate = false;
		private string dbFileName = Const.EmptyName;
		private ICodeWriterSql creator;
		private ICodeWriterSql cleaner;
		private IEnvironmentHelper environment;
		private string outFileNameDDLCreate;
		private string outFileNameDDLDelete;
		private string outFileNameDb;

		#region Constructors
		public CodeGenDb(SqliteGenie owner)
		{
			genie = owner;
			owner.Model.MetaObjects.SetUnprocessedAll();
			environment = owner.Model.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.Sqlite);

			dbCreate = genie.Config.Params.ValueByName("Database.Create", dbCreate);
			dbFileName = genie.Config.Params.ValueByName("Database.FileName",
			                                             String.Format("{0}.db", genie.Model.Lamp.ProjectName));
			creator = genie.Model.Lamp.CodeWritersFactory.CreateCodeWriterSql();
			cleaner = genie.Model.Lamp.CodeWritersFactory.CreateCodeWriterSql();

			outFileNameDDLCreate = Path.Combine(genie.Config.OutDir, "CRE_" + genie.Config.OutFileName);
			outFileNameDDLDelete = Path.Combine(genie.Config.OutDir, "DEL_" + genie.Config.OutFileName);
			outFileNameDb = Path.Combine(genie.Config.OutDir, dbFileName);

			foreach(IPatternConfig pattern in owner.Model.Lamp.Config.Patterns)
			{
				if (pattern.OnPersistentLayer)
					throw new GlException("Pattern '{0}' cannot be implemented on persistent layer", pattern.Name);
			}
		}
		#endregion

		private IModel Model
		{
			get { return genie.Model; }
		}

		public void Run()
		{
			BeginScripting();

			ProcessEntities();
			ProcessIndexes();

			EndScripting();
		}

		private void BeginScripting()
		{
			creator.WriteStdHeader(genie);
			creator.WriteLine();
			creator.BeginComment();
			creator.WriteLine("Use following command to create database");
			creator.WriteLine("sqlite3 \"{0}\" < \"{1}\"",
			                  outFileNameDb,
			                  outFileNameDDLCreate);
			creator.EndComment();
			creator.WriteLine();
			creator.WriteCommentLine("PRAGMA foreign_keys = ON;");
			creator.WriteLine();

			cleaner.WriteStdHeader(genie);
			cleaner.WriteLine();
			cleaner.BeginComment();
			cleaner.WriteLine("Use following command to clean up database");
			cleaner.WriteLine("sqlite3 \"{0}\" < \"{1}\"",
			                  outFileNameDb,
			                  outFileNameDDLDelete);
			cleaner.EndComment();
			cleaner.WriteLine();
		}

		private void EndScripting()
		{
			creator.Save(outFileNameDDLCreate, new UTF8Encoding(false));  // SQLite has problem on UTF-8 with BOM
			
			cleaner.WriteLine("VACUUM");
			cleaner.WriteSeparator();
			cleaner.Save(outFileNameDDLDelete, new UTF8Encoding(false));

			genie.Config.NotifyAssistants("Finished", null, creator.ToString(true));
		}

		#region Entities
		private void ProcessEntities()
		{
			foreach(IEntity entity in Model.Entities)
			{
				ProcessEntity(entity);
			}
		}

		private void ProcessEntity(IEntity entity)
		{
			creator.WriteCommentLine("Table of {0}", entity.FullName);

			ICodeWriterSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterSql();
			sql.WriteLine("CREATE TABLE IF NOT EXISTS {0} (", entity.Persistence.FullName);
			sql.Indent++;
			int count = 1;

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
			foreach(IAttribute attribute in attributes)
			{
				if (attribute.Persistence.Persisted)
				{
					sql.WriteColumnDefinition(attribute, environment);
					if (attribute.IsPrimaryId && entity.Constraints.PrimaryId.HasGenerator)
					{
						sql.Write(" CONSTRAINT {0} PRIMARY KEY AUTOINCREMENT",
						          entity.Constraints.PrimaryId.Persistence.Name);
						entity.Constraints.PrimaryId.Processed = true;
					}

					foreach(IRelation r in entity.Relations)
					{
						if (r.IsChild(entity)
						    && r.Persistence.Persisted
						    && r.AttributesMatch.Count == 1
						    && r.ForeignKey.ChildTableColumns[0].Name == attribute.Name)
						{
							sql.Write(" REFERENCES {0}({1})",
							          r.ForeignKey.ParentTable.Persistence.Name,
							          attribute.Persistence.Name);
						}
					}

					sql.WriteLine(count == attributes.Count ? "" : ",");
				}
				count++;
			}

			if(!entity.Constraints.PrimaryId.Processed)
			{
				sql.WriteLine(", CONSTRAINT {0} PRIMARY KEY ({1})",
				              entity.Constraints.PrimaryId.Persistence.Name,
				              entity.Constraints.PrimaryId.Attributes.ToPersistentNamesString(", "));
			}

			foreach(IUniqueId uc in entity.Constraints.UniqueIds)
			{
				sql.WriteLine(", CONSTRAINT {0} UNIQUE ({1})",
				              uc.Persistence.Name,
				              uc.Attributes.ToPersistentNamesString(", "));
			}

			ProcessEntityRelations(entity, sql);

			sql.Indent--;
			sql.WriteLine(")", entity.Persistence.FullName);
			creator.WriteFrom(sql);
			creator.WriteSeparator();

			cleaner.WriteDropTable(entity, environment);
			cleaner.WriteSeparator();

			genie.Config.NotifyAssistants("Create", entity, sql.ToString(true));
		}
		#endregion

		private void ProcessEntityRelations(IEntity entity, ICodeWriterSql sql)
		{
			foreach(IRelation r in entity.Relations)
			{
				if (r.IsChild(entity) && r.Persistence.Persisted)
				{
					sql.WriteCommentLine(", FOREIGN KEY {0} REFERENCES {1}({2})",
					                     r.ForeignKey.Name,
					                     r.ForeignKey.ParentTable.Persistence.Name,
					                     r.ForeignKey.ChildTableColumns.ToPersistentNamesString(", "));
				}
			}
		}

		private void ProcessIndexes()
		{
		}
	}
}

