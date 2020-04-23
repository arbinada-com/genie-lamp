using System;

namespace GenieLamp.Core.CodeWriters
{
	class CodeWriterScopeSettings : ICodeWriterScopeSettings
	{

		public CodeWriterScopeSettings()
		{
			this.ScopeBegin = Const.EmptyValue;
			this.ScopeEnd = Const.EmptyValue;
			this.BatchBegin = Const.EmptyValue;
			this.BatchEnd = Const.EmptyValue;
		}

		#region ICodeWriterScopeSettings implementation
		public string ScopeBegin { get; set; }
		public string ScopeEnd { get; set; }
		public string BatchBegin { get; set; }
		public string BatchEnd { get; set; }
		#endregion
	}
}

