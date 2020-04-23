using System;

namespace GenieLamp.Core.CodeWriters
{
	class CodeWriterCommentSettings : ICodeWriterCommentSettings
	{

		public CodeWriterCommentSettings()
		{
			this.CommentLine = "//";
			this.CommentBegin = "/*";
			this.CommentEnd = "*/";
		}

		#region ICodeWriterCommentSettings implementation
		public string CommentLine { get; set; }

		public string CommentBegin { get; set; }

		public string CommentEnd { get; set; }
		#endregion
	}
}

