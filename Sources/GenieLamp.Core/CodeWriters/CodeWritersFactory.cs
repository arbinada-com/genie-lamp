using System;

using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.CodeWriters.Sql;

namespace GenieLamp.Core.CodeWriters
{
	class CodeWritersFactory : ICodeWritersFactory
	{
		private GenieLamp lamp;
		
		public CodeWritersFactory(GenieLamp lamp)
		{
			this.lamp = lamp;
		}

		#region ICodeWritersFactory implementation
		public ICodeWriterText CreateCodeWriterText()
		{
			return new CodeWriterText(lamp);
		}

		public ICodeWriterSql CreateCodeWriterSql()
		{
			return new CodeWriterSql(lamp);
		}

		public ICodeWriterTransactSql CreateCodeWriterTransactSql()
		{
			return new CodeWriterTransactSql(lamp);
		}

		public ICodeWriterPlSql CreateCodeWriterPlSql()
		{
			return new CodeWriterPlSql(lamp);
		}

		public ICodeWriterCSharp CreateCodeWriterCSharp()
		{
			return new CodeWriterCSharp(lamp);
		}
		#endregion
	}
}

