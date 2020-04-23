using System;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Environments;
using GenieLamp.Core.Metamodel.Physical;

namespace GenieLamp.Core.CodeWriters.Sql
{
	class CodeWriterTransactSql : CodeWriterSql, ICodeWriterTransactSql
	{
		public CodeWriterTransactSql(GenieLamp lamp)
			: base(lamp)
		{
			this.ScopeSettings.BatchBegin = "";
			this.ScopeSettings.BatchEnd = "GO";
		}

		#region ICodeWriterTransactSql implementation
		public override void WriteColumnDefinition(IAttribute attribute, IEnvironmentHelper environment)
		{
			string identitySpec = String.Empty;
			if (attribute.IsPrimaryId && attribute.Entity.PrimaryId.HasGenerator)
			{
				identitySpec = String.Format(" IDENTITY({0}, {1})",
				                             attribute.Entity.PrimaryId.Generator.StartWith,
				                             attribute.Entity.PrimaryId.Generator.Increment);
			}

			Write("{0} {1}{2} {3}NULL{4}",
			      attribute.Persistence.Name,
			      environment.ToTypeName(attribute, false),
			      environment.ToDefaultValue(attribute).Length > 0 ? " DEFAULT " + environment.ToDefaultValue(attribute) : "",
			      attribute.TypeDefinition.Required ? "NOT " : "",
			      identitySpec );
		}
		
		public void WriteCreateIndex(IIndex index, IEnvironmentHelper environment)
		{
			WriteLine("CREATE {0}INDEX {1}", index.Unique ? "UNIQUE " : "", index.Name);
			Indent++;
			WriteLine("ON {0} ({1})", index.Entity.Persistence.FullName, index.Columns.Attributes.ToPersistentNamesString(","));
			Indent--;
		}

		public string WriteSpExecuteSql(string expression)
		{
			return WriteLine("EXEC sp_executesql N{0}{1}",
			                 ToConst(expression, true),
			                 Separator);
		}

		public string WriteSpExecuteSql(string expressionFormat, params object[] args)
		{
			return WriteSpExecuteSql(String.Format(expressionFormat, args));
		}
		#endregion
	}
}

