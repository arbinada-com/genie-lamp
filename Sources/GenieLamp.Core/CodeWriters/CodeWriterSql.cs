using System;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Environments;

namespace GenieLamp.Core.CodeWriters.Sql
{
	class CodeWriterSql : CodeWriterText, ICodeWriterSql
	{
		#region Constructors
		public CodeWriterSql(GenieLamp lamp)
			: base(lamp)
		{
			CommentSettings.CommentLine = "--";
			CommentSettings.CommentBegin = "/*";
			CommentSettings.CommentEnd = "*/";
			
			ScopeSettings.ScopeBegin = "BEGIN";
			ScopeSettings.ScopeEnd = "END";

			StringLiteralSettings.LeftBracket = "'";
			StringLiteralSettings.RightBracket = "'";
			StringLiteralSettings.Delimiter = "'";
		}
		#endregion
		
		
		#region ICodeWriterSql implementation
		public void BeginBatch()
		{
			if (!String.IsNullOrEmpty(ScopeSettings.BatchBegin))
				WriteLine(ScopeSettings.BatchBegin);
		}

		public void EndBatch()
		{
			if (!String.IsNullOrEmpty(ScopeSettings.BatchEnd))
				WriteLine(ScopeSettings.BatchEnd);
		}


		public virtual void WriteCreateTable(IEntity entity, IEnvironmentHelper environment)
		{
			WriteLine("CREATE TABLE {0} (", entity.Persistence.FullName);
			Indent++;
			int count = 1;
			foreach (IAttribute a in entity.Attributes)
			{
				if (a.Persistence.Persisted)
				{
					WriteColumnDefinition(a, environment);
					WriteLine(count == entity.Attributes.PersistentCount ? "" : ",");
					count++;
				}
			}
			Indent--;
			WriteLine(")");
		}

		public virtual void WriteColumnDefinition(IAttribute attribute, IEnvironmentHelper environment)
		{
			Write("{0} {1}{2} {3}NULL",
			      attribute.Persistence.Name,
			      environment.ToTypeName(attribute, false),
			      environment.ToDefaultValue(attribute).Length > 0 ? " DEFAULT " + environment.ToDefaultValue(attribute) : "",
			      attribute.TypeDefinition.Required ? "NOT " : "" );
		}
		
		public virtual void WriteDropTable(IEntity entity, IEnvironmentHelper environment)
		{
			WriteLine("DROP TABLE {0}", entity.Persistence.FullName);
		}
		
		public virtual void WriteCreateForeignKey(IRelation relation, IEnvironmentHelper environment)
		{
			if (relation.Persistence.Persisted)
			{
				WriteLine("ALTER TABLE {0}", relation.ForeignKey.ChildTable.Persistence.FullName);
				Indent++;
				WriteLine("ADD CONSTRAINT {0} FOREIGN KEY", relation.ForeignKey.Name);
				Indent++;
				WriteLine("({0})", relation.ForeignKey.ChildTableColumns.ToPersistentNamesString(", "));
				Indent--;
				WriteLine("REFERENCES {0}", relation.ForeignKey.ParentTable.Persistence.FullName);
				Indent++;
				WriteLine("({0})", relation.ForeignKey.ParentTableColumns.ToPersistentNamesString(", "));
			}
		}
		
		public virtual void WriteDropForeignKey(IRelation relation, IEnvironmentHelper environment)
		{
			if (relation.Persistence.Persisted)
			{
				WriteLine("ALTER TABLE {0} DROP CONSTRAINT {1}",
				          relation.ForeignKey.ChildTable.Persistence.FullName,
				          relation.ForeignKey.Name);
			}
		}
		

		public virtual void WriteCreatePrimaryKey(IPrimaryId constraint, IEnvironmentHelper environment)
		{
			WriteLine("ALTER TABLE {0}", constraint.Entity.Persistence.FullName);
			Indent++;
			WriteLine("ADD CONSTRAINT {0} PRIMARY KEY ({1})", 
			              constraint.Persistence.Name,
			              constraint.Attributes.ToPersistentNamesString(","));
		}

		public void WriteDropPrimaryKey(IPrimaryId constraint, IEnvironmentHelper environment)
		{
			WriteLine("ALTER TABLE {0} DROP PRIMARY KEY {1}",
			          constraint.Entity.Persistence.FullName,
			          constraint.Persistence.Name);
		}

		public void WriteCreateUniqueConstraint(IUniqueId constraint, IEnvironmentHelper environment)
		{
			WriteLine("ALTER TABLE {0}", constraint.Entity.Persistence.FullName);
			Indent++;
			WriteLine("ADD CONSTRAINT {0} UNIQUE ({1})", 
			              constraint.Persistence.Name,
			              constraint.Attributes.ToPersistentNamesString(","));
		}

		public void WriteDropUniqueConstraint(IUniqueId constraint, IEnvironmentHelper environment)
		{
			WriteLine("ALTER TABLE {0} DROP UNIQUE {1}",
			          constraint.Entity.Persistence.FullName,
			          constraint.Persistence.Name);
		}
		#endregion


	}
}

