using System;

namespace GenieLamp.Core.CodeWriters
{
	class CodeWriterStringLiteralSettings : ICodeWriterStringLiteralSettings
	{

		public CodeWriterStringLiteralSettings()
		{
			this.LeftBracket = "\"";
			this.RightBracket = "\"";
			this.Delimiter = "\\";
		}

		public string SetDelimiters(string literal)
		{
			string delimited = literal.Replace(LeftBracket, Delimiter + LeftBracket);
			if (LeftBracket != RightBracket)
				delimited = delimited.Replace(RightBracket, Delimiter + RightBracket);
			return delimited;
		}

		#region ICodeWriterStringLiteralSettings implementation
		public string LeftBracket { get; set; }

		public string RightBracket { get; set; }

		public string Delimiter { get; set; }
		#endregion
	}
}

