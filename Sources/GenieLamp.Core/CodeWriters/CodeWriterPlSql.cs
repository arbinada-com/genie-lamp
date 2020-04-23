using System;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Metamodel.Physical;

namespace GenieLamp.Core.CodeWriters.Sql
{
	class CodeWriterPlSql : CodeWriterSql, ICodeWriterPlSql
	{
		public CodeWriterPlSql(GenieLamp lamp) : base(lamp)
		{
			this.ScopeSettings.ScopeEnd = "END;";
		}

		#region ICodeWriterPlSql implementation
		public void DeclareVariable(string varName, string typeName)
		{
			WriteLine("{0} {1};", varName, typeName);
		}

		public void If(string condition)
		{
			WriteLine("IF {0} THEN", condition);
			Indent++;
		}

		public void If(string conditionFormat, params object[] args)
		{
			this.If(String.Format(conditionFormat, args));
		}
		
		public void EndIf()
		{
			Indent--;
			WriteLine("END IF;");
		}
		
		public void WriteCreateSequence(IGenerator generator, IEnvironmentHelper environment)
		{
			WriteLine("CREATE SEQUENCE {0}", generator.Persistence.FullName);
			Indent++;
			WriteLine("START WITH {0}", generator.StartWith);
			WriteLine("INCREMENT BY {0}", generator.Increment);
			WriteLine("MINVALUE {0}", generator.MinValue);
			WriteLine("MAXVALUE {0}", generator.MaxValue);
		}


		public string WriteExecImmediat(string format, params object[] args)
		{
			return WriteExecImmediat(String.Format(format, args));
		}
		
		public string WriteExecImmediat(string sql)
		{
    		return WriteLine("EXECUTE IMMEDIATE {0}{1}", ToConst(sql), Separator);
		}

		public void WriteExecImmediat(ICodeWriterText sql)
		{
    		WriteLine("EXECUTE IMMEDIATE");
			Indent++;
			WriteText(ToConst(sql.ToString(true)) + Separator);
			Indent--;
		}

		public string WriteExecImmediatVariable(string variableName)
		{
    		return WriteLine("EXECUTE IMMEDIATE {0}{1}", variableName, Separator);
		}

		public void WriteSetVar(string variableName, string value)
		{
    		WriteLine("{0} := {1}{2}", variableName, ToConst(value), Separator);
		}

		public void WriteSetVar(string variableName, ICodeWriterText sql)
		{
    		WriteLine("{0} := ", variableName);
			Indent++;
			WriteText(ToConst(sql.ToString(true)) + Separator);
			Indent--;
		}

		public void WriteDropSequence(IGenerator generator, IEnvironmentHelper environment)
		{
			WriteLine("DROP SEQUENCE {0}", generator.Persistence.FullName);
		}

		public void WriteCreateIndex(IIndex index, IEnvironmentHelper environment)
		{
			WriteLine("CREATE {0} INDEX {1}", index.Unique ? "UNIQUE" : "", index.FullName);
			Indent++;
			WriteLine("ON {0} ({1})", index.Entity.Persistence.FullName, index.Columns.Attributes.ToPersistentNamesString(","));
			Indent--;
		}

		public void WriteDropIndex(IIndex index, IEnvironmentHelper environment)
		{
			WriteLine("DROP INDEX {0}", index.FullName);
		}
		#endregion




	}
}

