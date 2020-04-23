using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.CodeDom.Compiler;
using System.Collections;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.CodeWriters
{
	class CodeWriterText : ICodeWriterText
	{
		private IndentedTextWriter writer = null;
		private string tabString = "\t";
		private StringWriter stringWriter = new StringWriter();
		private Encoding defaultEncoding = new UTF8Encoding(true); // UTF-8 with BOM
		private CodeWriterCommentSettings commentSettings = new CodeWriterCommentSettings();
		private CodeWriterScopeSettings scopeSettings = new CodeWriterScopeSettings();
		private CodeWriterStringLiteralSettings stringLiteralSettings = new CodeWriterStringLiteralSettings();
		private string separator = ";";

		protected Layers.LayerConfig LayerConfig { get; set; }

		internal GenieLamp Lamp { get; private set; }

		#region Constructors
		public CodeWriterText(GenieLamp lamp)
		{
			this.Lamp = lamp;
			CreateWriter();
		}
		
		protected void CreateWriter()
		{
			stringWriter = new StringWriter();
			writer = new IndentedTextWriter(stringWriter, tabString);
		}
		#endregion

		#region ICodeWriterText implementation
		public ICodeWriterCommentSettings CommentSettings
		{
			get { return commentSettings; }
		}

		public ICodeWriterScopeSettings ScopeSettings
		{
			get { return scopeSettings; }
		}
		
		public ICodeWriterStringLiteralSettings StringLiteralSettings
		{
			get { return this.stringLiteralSettings; }
		}
		
		public Encoding DefaultOutFileEncoding
		{
			get { return defaultEncoding; }
			set { defaultEncoding = value; }
		}
		
		public string TabString
		{
			get { return tabString; }
			set
			{
				if (!tabString.Equals(value))
				{
					tabString = value;
					CreateWriter();
				}
			}
		}

		public int Indent
		{
			get { return writer.Indent; }
			set { writer.Indent = value; }
		}

		public string Separator
		{
			get { return this.separator; }
			set { separator = value; }
		}

		public void ClearAll()
		{
			CreateWriter();
		}

		public void WriteStdHeader(IGenie genie)
		{
			WriteCommentLine("{0} ({1})", this.Lamp.GetTitle(), this.Lamp.Version);
			WriteCommentLine("{0} ({1})", genie.Name, genie.Version);
			WriteCommentLine("Starter application ({0})", System.Reflection.Assembly.GetEntryAssembly().GetName().Version);
			WriteCommentLine("This file was automatically generated at {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
			WriteCommentLine("Do not modify it manually.");
			WriteLine();
		}

		public void Write(string s)
		{
			writer.Write(s);
		}
	
		public void WriteLine()
		{
			writer.WriteLine();
		}
		
		public string WriteLine(string s)
		{
			writer.WriteLine(s);
			return s;
		}

		public void Write(string format, params object[] args)
		{
			writer.Write(format, args);
		}

		public string WriteLine(string format, params object[] args)
		{
			return WriteLine(String.Format(format, args));
		}

		#region WriteText
		public void WriteText(string text, IMacroExpander macro = null)
		{
			string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			foreach(string line in lines)
			{
				if (macro != null)
					WriteLine(macro.Subst(line));
				else
					WriteLine(line);
			}
		}

		public void WriteText(Stream stream, IMacroExpander macro = null)
		{
	        using (StreamReader reader = new StreamReader(stream))
			{
            	WriteText(reader.ReadToEnd(), macro);
			}
		}

		public void WriteText(Assembly assembly, string ressourceId, IMacroExpander macro = null)
		{
			string assemblyName = assembly.GetName().Name;
			if (!ressourceId.StartsWith(assemblyName))
				ressourceId = String.Format("{0}.{1}", assemblyName, ressourceId);
			using (Stream stream = assembly.GetManifestResourceStream(ressourceId))
			{
				WriteText(stream, macro);
			}
		}
		#endregion

		#region Comments and docc
		public void WriteCommentLine()
		{
			WriteLine("{0}", CommentSettings.CommentLine);
		}
		
		public string WriteCommentLine(string s)
		{
			return WriteLine("{0} {1}", CommentSettings.CommentLine, s);
		}
		
		public string WriteCommentLine(string format, params object[] args)
		{
			return WriteCommentLine(String.Format(format, args));
		}

		public void BeginComment()
		{
			WriteLine(CommentSettings.CommentBegin);
		}

		public void EndComment()
		{
			WriteLine(CommentSettings.CommentEnd);
		}

		public virtual void WriteDoc(IMetaObject metaObject)
		{
			if (!metaObject.HasDoc)
				return;
			foreach(string line in metaObject.Doc.LabelLines)
			{
				WriteCommentLine(line);
			}
			foreach(string line in metaObject.Doc.TextLines)
			{
				WriteCommentLine(line);
			}
		}

		#endregion

		public void Save(string fileName)
		{
			Save(fileName, defaultEncoding);
		}
		
		public void Save(string fileName, Encoding encoding)
		{
			using (StreamWriter sw = new StreamWriter(fileName, false, encoding))
			{
				sw.WriteLine(stringWriter.ToString());
			}
		}

		public virtual void BeginScope()
		{
			if (!ScopeSettings.ScopeBegin.Equals(Const.EmptyValue))
				WriteLine(ScopeSettings.ScopeBegin);
			Indent++;
		}
	
		public virtual void EndScope()
		{
			Indent--;
			if (!ScopeSettings.ScopeEnd.Equals(Const.EmptyValue))
				WriteLine(ScopeSettings.ScopeEnd);
		}
		
		public virtual void WriteSeparator()
		{
			WriteLine(this.Separator);
		}
		
		public string CurrentLine
		{
			get
			{
				string s = this.ToString();
				int pos = s.LastIndexOf(Environment.NewLine);
				if (pos < 0)
					pos = 0;
				return s.Substring(pos);
			}
		}


		public void WriteFrom(ICodeWriterText source)
		{
			string[] lines = source.GetLines();
			int count = 1;
			foreach(string line in lines)
			{
				if (count++ == lines.Length && line.Length == 0)
					continue;
				this.WriteLine(line);
			}
		}
		
		public override string ToString()
		{
			return ToString(false);
		}

		public virtual string ToString(bool trimLastNewLine)
		{
			if (trimLastNewLine)
				return (writer.InnerWriter as StringWriter).GetStringBuilder().ToString().TrimEnd(new char[]{'\r', '\n'});
			else
				return (writer.InnerWriter as StringWriter).GetStringBuilder().ToString();
		}

		public string[] GetLines()
		{
			return this.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}

		public string ToSeparatedString(IList list, string separator)
		{
			return ToSeparatedString(list, separator, null);
		}

		public string ToSeparatedString(IList list, string separator, MakeTextDelegate textMaker)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			foreach(object item in list)
			{
				sb.AppendFormat("{0}{1}",
				                textMaker == null ? item : textMaker(item, count++),
				                separator);
			}
			if (sb.Length >= separator.Length)
				sb.Remove(sb.Length - separator.Length, separator.Length);
			return sb.ToString();
		}

		public void WriteSeparatedString(IList list, string separator, MakeTextDelegate textMaker)
		{
			WriteLine(ToSeparatedString(list, separator, textMaker));
		}

		#region Constants & names
		public virtual string ToConst(string constValue)
		{
			return ToConst(constValue, true);
		}

		public virtual string ToConst(string constValue, bool addBrackets)
		{
			return String.Format("{0}{1}{2}",
			                     addBrackets ? stringLiteralSettings.LeftBracket :  "",
			                     stringLiteralSettings.SetDelimiters(constValue),
			                     addBrackets ? stringLiteralSettings.RightBracket : "");

		}

		public void WriteConst(string constValue)
		{
			Write(ToConst(constValue));
		}
		#endregion


		#region Fucntions
		public virtual void BeginFunction(string fullSignature)
		{
			WriteLine(fullSignature);
			BeginScope();
		}

		public virtual void BeginFunction(string format, params object[] args)
		{
			BeginFunction(String.Format(format, args));
		}

		public virtual void EndFunction()
		{
			EndScope();
		}
		#endregion

		#endregion
	}
}

