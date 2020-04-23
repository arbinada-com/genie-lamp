using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Metamodel.Physical;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.Sql;

namespace GenieLamp.Genies.Oracle
{
	public class CodeGenDb
	{
		private const string VarNameFoundCount = "found";
		private const string VarNameSql = "sqlstr";
		private OracleGenie genie;
		private ICodeWriterPlSql updater;
		private ICodeWriterPlSql creator;
		private ICodeWriterPlSql cleaner;
		private IEnvironmentHelper environment;
		private bool createSchema = true;
		private string schemaPassword = "M@sterKey";
		private string schemaDefaultTablespace = Const.EmptyValue;
		private string schemaTempTablespace = Const.EmptyValue;
		private bool schemaGrantDba = true;
		private bool useUniqueIndexes = false;
		private string outFileNameDDLCreate;
		private string outFileNameDDLUpdate;
		private string outFileNameDDLDelete;
		
		enum CommentTarget
		{
			Table,
			Column
		}
		
		#region Constructors
		public CodeGenDb(OracleGenie owner)
		{
			genie = owner;
			owner.Model.MetaObjects.SetUnprocessedAll();
			environment = owner.Model.Lamp.GenieLampUtils.GetEnvironmentHelper(TargetEnvironment.OracleDb);
			
			createSchema = genie.Config.Params.ValueByName("CreateSchema", createSchema);
			schemaPassword = genie.Config.Params.ValueByName("Schema.Password", schemaPassword);
			schemaDefaultTablespace = genie.Config.Params.ValueByName("Schema.DefaultTablespace", schemaDefaultTablespace);
			schemaTempTablespace = genie.Config.Params.ValueByName("Schema.TempTablespace", schemaTempTablespace);
			schemaGrantDba = genie.Config.Params.ValueByName("Schema.GrantDba", schemaGrantDba);
			useUniqueIndexes = genie.Config.Params.ValueByName("UniqueIndexConstraint", useUniqueIndexes);
			
			updater = owner.Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			updater.DefaultOutFileEncoding = owner.Config.OutFileEncoding;
			creator = genie.Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			creator.DefaultOutFileEncoding = owner.Config.OutFileEncoding;
			cleaner = genie.Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			cleaner.DefaultOutFileEncoding = owner.Config.OutFileEncoding;

			outFileNameDDLCreate = Path.Combine(genie.Config.OutDir, "CRE_" + genie.Config.OutFileName);
			outFileNameDDLUpdate = Path.Combine(genie.Config.OutDir, "UPD_" + genie.Config.OutFileName);
			outFileNameDDLDelete = Path.Combine(genie.Config.OutDir, "DEL_" + genie.Config.OutFileName);
			
		}
		#endregion

		public string UpdateScriptFileName
		{
			get { return this.outFileNameDDLUpdate; }
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
			ProcessSchemas();
			ProcessSequences();
			ProcessEntities();
			ProcessRelations();
			ProcessIndexes();
		}
		
		private void RunCleanup()
		{
			foreach (IRelation r in Model.Relations)
			{
				if (r.Persistence.Persisted)
				{
					cleaner.WriteDropForeignKey(r, environment);
					cleaner.WriteSeparator();
				}
			}
			
			cleaner.WriteLine();
			foreach (IEntity en in Model.Entities)
			{
				if (en.Persistence.Persisted)
				{
					cleaner.WriteDropTable(en, environment);
					cleaner.WriteSeparator();
				}
			}
			
			cleaner.WriteLine();
			foreach (IGenerator gen in Model.Generators)
			{
				cleaner.WriteDropSequence(gen, environment);
				cleaner.WriteSeparator();
			}
		}
		
		private void BeginScripting()
		{
			updater.WriteStdHeader(genie);
			creator.WriteStdHeader(genie);			
			cleaner.WriteStdHeader(genie);			

			updater.WriteLine("SET SERVEROUTPUT ON;");
			updater.WriteLine("WHENEVER SQLERROR EXIT FAILURE ROLLBACK;");
			updater.WriteLine("WHENEVER OSERROR EXIT FAILURE ROLLBACK;");
			updater.WriteLine("DECLARE");
			updater.Indent++;
			updater.DeclareVariable(VarNameFoundCount, "INTEGER");
			updater.DeclareVariable(VarNameSql, "VARCHAR2(32000)");
			updater.Indent--;
			updater.BeginScope();
		}

		private void EndScripting()
		{
			updater.WriteLine("COMMIT;");
			updater.Indent--;
			updater.WriteLine("EXCEPTION");
			updater.Indent++;
  			updater.WriteLine("WHEN OTHERS THEN");
			updater.Indent++;
			updater.WriteLine("DBMS_OUTPUT.PUT_LINE('SQLCODE: ' || SQLCODE); ");
			updater.WriteLine("DBMS_OUTPUT.PUT_LINE('SQL: ' || {0});", VarNameSql);
			updater.WriteLine("RAISE;");
			updater.Indent--;
			updater.EndScope();

			updater.Save(outFileNameDDLUpdate);
			creator.Save(outFileNameDDLCreate);
			cleaner.Save(outFileNameDDLDelete);

			genie.Config.NotifyAssistants("Finished", null, creator.ToString(true));
		}
		
		private void WriteExecImmediatWhenNotExists(string fromSource, string whereCondition, ICodeWriterPlSql sql)
		{
			updater.WriteLine("SELECT count(*) INTO {0} FROM {1} WHERE ROWNUM=1 AND {2};",
			                 VarNameFoundCount, fromSource, whereCondition);
			updater.If("{0} <> 1", VarNameFoundCount);
			updater.WriteSetVar(VarNameSql, sql);
			updater.WriteExecImmediatVariable(VarNameSql);
			updater.EndIf();
		}

		private void WriteExecImmediat(ICodeWriterPlSql sql)
		{
			updater.WriteSetVar(VarNameSql, sql);
			updater.WriteExecImmediatVariable(VarNameSql);
		}
		
		private void WriteExecImmediat(string sql)
		{
			updater.WriteSetVar(VarNameSql, sql);
			updater.WriteExecImmediatVariable(VarNameSql);
		}

		private void WriteComment(CommentTarget target, string targetName, IDoc doc)
		{
			if (doc != null && doc.Text.Length > 0)
			{
				ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
				sql.Write("COMMENT ON ");
				switch (target)
				{
				case CommentTarget.Table:
					sql.Write("TABLE ");
					break;
				default:
					sql.Write("COLUMN ");
					break;
				}
				sql.Write("{0} IS {1}", targetName, sql.ToConst(doc.Text));
				genie.Config.NotifyAssistants("Doc", doc, sql.ToString(true));
				WriteExecImmediat(sql);
				creator.WriteFrom(sql);
				creator.WriteSeparator();

			}
		}
		
		private void ProcessSchemas()
		{
			List<string> schemas = new List<string>();
			foreach (IEntity entity in Model.Entities)
			{
				if (!schemas.Contains(entity.Persistence.Schema))
				{
					schemas.Add(entity.Persistence.Schema);
				}
			}
			
			if (createSchema)
			{
				foreach (string schemaName in schemas)
				{
					WriteSchema(schemaName);
					updater.WriteLine();
				}
			}
			
			string sql = String.Format("ALTER SESSION SET current_schema={0}", schemas[0]);
			WriteExecImmediat(sql);
			creator.WriteLine(sql);
			creator.WriteSeparator();
		}
		
		private void WriteSchema(string name)
		{
			updater.WriteLine("SELECT count(*) INTO {0} FROM ALL_USERS WHERE USERNAME='{1}' AND ROWNUM=1;",
			                 VarNameFoundCount, name);
			updater.If("{0} <> 1", VarNameFoundCount);
			
			ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			sql.WriteLine("CREATE USER {0}", name);
			sql.Indent++;
			sql.WriteLine("IDENTIFIED BY {0}", schemaPassword);
			sql.WriteLine("DEFAULT TABLESPACE {0}", schemaDefaultTablespace);
			sql.WriteLine("TEMPORARY TABLESPACE {0}", schemaTempTablespace);
			sql.WriteLine("ACCOUNT UNLOCK");
			WriteExecImmediat(sql);
			creator.WriteFrom(sql);
			creator.WriteSeparator();
			
			if (schemaGrantDba)
			{
				WriteExecImmediat(creator.WriteLine("GRANT DBA TO {0}", name));
				creator.WriteSeparator();
			}
			
			WriteExecImmediat(creator.WriteLine("GRANT SELECT ANY TABLE TO {0}", name));
			creator.WriteSeparator();
			WriteExecImmediat(creator.WriteLine("GRANT UPDATE ANY TABLE TO {0}", name));
			creator.WriteSeparator();
			WriteExecImmediat(creator.WriteLine("GRANT INSERT ANY TABLE TO {0}", name));
			creator.WriteSeparator();
			WriteExecImmediat(creator.WriteLine("GRANT DELETE ANY TABLE TO {0}", name));
			creator.WriteSeparator();
			WriteExecImmediat(creator.WriteLine("GRANT EXECUTE ANY TABLE TO {0}", name));
			creator.WriteSeparator();
			WriteExecImmediat(creator.WriteLine("GRANT SELECT ANY TABLE TO {0}", name));
			creator.WriteSeparator();

			updater.EndIf();
		}
		
		#region Sequences
		private void ProcessSequences()
		{
			ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			
			foreach (IGenerator gen in Model.Generators)
			{
				if (gen.Owner is IEntity && !(gen.Owner as IEntity).Persistence.Persisted)
					continue;
				if (gen.Type == GeneratorType.Sequence)
				{
					sql.ClearAll();
					sql.WriteCreateSequence(gen, environment);
					ISpellHint hint = genie.FindHint(gen);
					if (hint != null)
						sql.WriteText(hint.GetText(gen));
					creator.WriteFrom(sql);
					creator.WriteSeparator();

					genie.Config.NotifyAssistants("Create", gen, sql.ToString(true));

					WriteExecImmediatWhenNotExists(
						"ALL_SEQUENCES", 
						String.Format("SEQUENCE_OWNER='{0}' AND SEQUENCE_NAME='{1}'", gen.Persistence.Schema, gen.Persistence.Name), 
						sql);
					updater.WriteLine();
				}
				
			}
		}
		#endregion

		
		#region Entities
		private void ProcessEntities()
		{
			foreach (IEntity entity in Model.Entities)
			{
				if (entity.Persistence.Persisted)
					ProcessEntity(entity);
				updater.WriteLine();
				Model.Lamp.Logger.ProgressStep();
			}
		}
		
		private void ProcessEntity(IEntity entity)
		{
			
			updater.WriteLine(creator.WriteCommentLine("Table of {0}", entity.FullName));

			ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			sql.WriteCreateTable(entity, environment);
			ISpellHint hint = genie.FindHint(entity);
			if (hint != null)
				sql.WriteText(hint.GetText(entity));
			creator.WriteFrom(sql);
			creator.WriteSeparator();
			
			WriteExecImmediatWhenNotExists(
				"ALL_TABLES", 
				String.Format("OWNER='{0}' AND TABLE_NAME='{1}'", entity.Persistence.Schema, entity.Persistence.Name), 
				sql);
			updater.WriteLine();

			genie.Config.NotifyAssistants("Create", entity, sql.ToString(true));

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

			ProcessUpdateAttributes(entity, attributes);
			
			ProcessPrimaryKey(entity);
			
			ProcessUniqueIds(entity);
			
			creator.WriteLine();
			updater.WriteLine();
			WriteComment(CommentTarget.Table, entity.Persistence.FullName, entity.Doc);

			foreach (IAttribute a in attributes)
			{
				if (a.Persistence.Persisted)
				{
					WriteComment(CommentTarget.Column, 
					             String.Format("{0}.{1}", entity.Persistence.FullName, a.Persistence.Name),
					             a.Doc);
				}
			}
		}
		
		private void ProcessPrimaryKey(IEntity entity)
		{
			ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			sql.WriteCreatePrimaryKey(entity.PrimaryId, environment);
			ISpellHint hint = genie.FindHint(entity.PrimaryId);
			if (hint != null)
				sql.WriteText(hint.GetText(entity.PrimaryId));
			creator.WriteFrom(sql);
			creator.WriteSeparator();
			
			WriteExecImmediatWhenNotExists(
				"ALL_CONSTRAINTS", 
				String.Format("OWNER='{0}' AND TABLE_NAME='{1}' AND CONSTRAINT_TYPE='P'", 
			              entity.Persistence.Schema, 
			              entity.Persistence.Name), 
				sql);
			genie.Config.NotifyAssistants("Create", entity.PrimaryId, sql.ToString(true));
			updater.WriteLine();
		}
		
		private void ProcessUniqueIds(IEntity entity)
		{
			ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
				sql.ClearAll();
				if (!useUniqueIndexes)
				{
					sql.WriteCreateUniqueConstraint(uid, environment);
					creator.WriteFrom(sql);
					creator.WriteSeparator();
					WriteExecImmediatWhenNotExists(
						"ALL_CONSTRAINTS", 
						String.Format("OWNER='{0}' AND TABLE_NAME='{1}' AND CONSTRAINT_TYPE='U' AND CONSTRAINT_NAME='{2}'",
					              entity.Persistence.Schema, 
					              entity.Persistence.Name,
					              uid.Persistence.Name), 
						sql);
					uid.Index.Processed = true;
					genie.Config.NotifyAssistants("Create", uid, sql.ToString(true));
					updater.WriteLine();
				}
			}
		}
		
		private void ProcessIndexes()
		{
			ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
			foreach(IIndex index in Model.PhysicalModel.Indexes)
			{
				if (!index.Processed && index.Generate)
				{
					sql.ClearAll();
					sql.WriteCreateIndex(index, environment);
					ISpellHint hint = genie.FindHint(index);
					if (hint != null)
						sql.WriteText(hint.GetText(index));
					creator.WriteFrom(sql);
					creator.WriteSeparator();
					StringBuilder sqlFrom = new StringBuilder();
					sqlFrom.Append("SELECT I.OWNER, I.INDEX_NAME, COUNT(*) ");
					sqlFrom.Append("FROM ALL_INDEXES I ");
					sqlFrom.Append(" INNER JOIN ALL_IND_COLUMNS IC ON I.OWNER = IC.INDEX_OWNER AND I.INDEX_NAME = IC.INDEX_NAME");
					sqlFrom.Append(" LEFT OUTER JOIN ALL_CONSTRAINTS AC ON I.INDEX_NAME = AC.CONSTRAINT_NAME AND I.TABLE_OWNER = AC.OWNER AND I.TABLE_NAME = AC.TABLE_NAME AND AC.CONSTRAINT_TYPE = 'P'");
					sqlFrom.Append(" LEFT OUTER JOIN(");
					for(int i = 0; i < index.Columns.Count; i++)
					{
						sqlFrom.AppendFormat("{0}SELECT '{1}' AS COLUMN_NAME FROM DUAL", 
						                     i == 0 ? "" : " UNION ",
						                     index.Columns[i].Attribute.Persistence.Name);
					}
					sqlFrom.Append(") IC2 ON IC.COLUMN_NAME = IC2.COLUMN_NAME");
					sqlFrom.AppendFormat(" WHERE I.OWNER='{0}' AND I.TABLE_OWNER = '{1}' AND I.TABLE_NAME='{2}' AND AC.CONSTRAINT_NAME IS NULL", 
					                     index.Schema, 
					                     index.Entity.Persistence.Schema,
					                     index.Entity.Persistence.Name);
					sqlFrom.Append(" GROUP BY I.OWNER, I.INDEX_NAME");
					sqlFrom.AppendFormat(" HAVING COUNT(*) = {0}", index.Columns.Count);
					sqlFrom.Append(" UNION ALL ");
					sqlFrom.AppendFormat("SELECT I.OWNER, I.INDEX_NAME, 1 FROM ALL_INDEXES I WHERE I.OWNER='{0}' AND I.INDEX_NAME='{1}'", 
					                     index.Schema, index.Name);

					WriteExecImmediatWhenNotExists(
						String.Format("({0}) T1", sqlFrom.ToString()), 
						"1=1",
						sql);
					genie.Config.NotifyAssistants("Create", index, sql.ToString(true));
					updater.WriteLine();
				}
			}
		}
		
		private void ProcessUpdateAttributes(IEntity entity, IAttributes attributes)
		{
			foreach (IAttribute a in attributes)
			{
				if (a.Persistence.Persisted)
				{
					ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
					updater.WriteLine("SELECT count(*) INTO {0} FROM ALL_TAB_COLUMNS WHERE OWNER='{1}' AND TABLE_NAME='{2}' AND COLUMN_NAME='{3}' AND ROWNUM=1;",
					                  VarNameFoundCount,
					                  entity.Persistence.Schema,
					                  entity.Persistence.Name, 
					                  a.Persistence.Name);
					updater.If("{0} <> 1", VarNameFoundCount);
					
					sql.WriteLine("ALTER TABLE {0} ADD ", entity.Persistence.FullName);
					sql.Indent++;
					sql.WriteLine("{0} {1} NULL", 
					              a.Persistence.Name,
					              environment.ToTypeName(a, false));
					WriteExecImmediat(sql);
					string updateColSql = sql.ToString(true);

					if (a.TypeDefinition.Required && a.TypeDefinition.HasDefault)
					{
						sql.ClearAll();
						sql.WriteLine("UPDATE {0} SET {1}={2}",
						              entity.Persistence.FullName,
						              a.Persistence.Name,
						              environment.ToDefaultValue(a));
						WriteExecImmediat(sql);
						updateColSql = updateColSql + Environment.NewLine + sql.ToString(true);

						sql.ClearAll();
						sql.WriteLine("ALTER TABLE {0} MODIFY ({1} NOT NULL)",
						              entity.Persistence.FullName,
						              a.Persistence.Name);
						WriteExecImmediat(sql);
						updateColSql = updateColSql + Environment.NewLine + sql.ToString(true);
					}
					updater.EndIf();

					genie.Config.NotifyAssistants("Update", a, updateColSql);
				}
			}
			updater.WriteLine();
		}
		#endregion
		
		
		private void ProcessRelations()
		{
			foreach (IRelation r in Model.Relations)
			{
				if (r is ISubtypeRelation)
					creator.WriteLine(updater.WriteCommentLine(
						"Subtyping reference: {0} is a {1}", 
						r.ForeignKey.ChildTable.FullName,
						r.ForeignKey.ParentTable.FullName));
				
				ICodeWriterPlSql sql = Model.Lamp.CodeWritersFactory.CreateCodeWriterPlSql();
				sql.WriteCreateForeignKey(r, environment);
				creator.WriteFrom(sql);
				creator.WriteSeparator();
				
				WriteExecImmediatWhenNotExists(
					"ALL_CONSTRAINTS", 
					String.Format("OWNER='{0}' AND CONSTRAINT_NAME='{1}' AND CONSTRAINT_TYPE='R'", 
				              r.ForeignKey.ChildTable.Persistence.Schema, 
				              r.ForeignKey.Name), 
					sql);
				updater.WriteLine();
				genie.Config.NotifyAssistants("Create", r, sql.ToString());
			}
		}

	}
}

