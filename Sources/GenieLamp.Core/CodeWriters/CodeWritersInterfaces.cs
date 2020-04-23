using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Metamodel.Physical;
using GenieLamp.Core.CodeWriters.Sql;
using GenieLamp.Core.CodeWriters.CSharp;

namespace GenieLamp.Core.CodeWriters
{
	
	public interface ICodeWritersFactory
	{
		ICodeWriterText CreateCodeWriterText();
		ICodeWriterSql CreateCodeWriterSql();
		ICodeWriterTransactSql CreateCodeWriterTransactSql();
		ICodeWriterPlSql CreateCodeWriterPlSql();
		ICodeWriterCSharp CreateCodeWriterCSharp();
	}
	
	public delegate string MakeTextDelegate(object item, int itemIndex);

	public interface ICodeWriterText
	{
		ICodeWriterCommentSettings CommentSettings { get; }
		ICodeWriterScopeSettings ScopeSettings { get; }
		ICodeWriterStringLiteralSettings StringLiteralSettings { get; }
		string TabString { get; set; }
		Encoding DefaultOutFileEncoding { get; set; }
		string Separator { get; set; }
		int Indent { get; set; }
		string CurrentLine { get; }
		string[] GetLines();
		void ClearAll();
		void BeginScope();
		void EndScope();
		void WriteStdHeader(IGenie genie);
		void Write(string s);
		void Write(string format, params object[] args);
		void WriteLine();
		string WriteLine(string s);
		string WriteLine(string format, params object[] args);
		void WriteSeparator();
		void WriteText(string text, IMacroExpander macro = null);
		void WriteText(Stream stream, IMacroExpander macro = null);
		void WriteText(Assembly assembly, string ressourceId, IMacroExpander macro = null);
		void WriteFrom(ICodeWriterText source);
		void Save(string fileName);
		void Save(string fileName, Encoding encoding);
		//
		void BeginFunction(string fullSignature);
		void BeginFunction(string format, params object[] args);
		void EndFunction();
		//
		string ToSeparatedString(IList list, string separator);
		string ToSeparatedString(IList list, string separator, MakeTextDelegate textMaker);
		void WriteSeparatedString(IList list, string separator, MakeTextDelegate textMaker);
		//
		string ToConst(string constValue);
		string ToConst(string constValue, bool addBrackets);
		void WriteConst(string constValue);
		void WriteCommentLine();
		string WriteCommentLine(string s);
		string WriteCommentLine(string format, params object[] args);
		//
		void WriteDoc(IMetaObject metaObject);
		void BeginComment();
		void EndComment();
		//
		string ToString();
		string ToString(bool trimLastNewLine);
	}
	
	public interface ICodeWriterCommentSettings
	{
		string CommentLine { get; set; }
		string CommentBegin { get; set; }
		string CommentEnd { get; set; }
	}

	public interface ICodeWriterScopeSettings
	{
		string ScopeBegin { get; set; }
		string ScopeEnd { get; set; }
		string BatchBegin { get; set; }
		string BatchEnd { get; set; }
	}

	public interface ICodeWriterStringLiteralSettings
	{
		string LeftBracket { get; set; }
		string RightBracket { get; set; }
		string Delimiter { get; set; }
	}

	namespace Sql
	{
		public interface ICodeWriterSql : ICodeWriterText
		{
			void BeginBatch();
			void EndBatch();
			void WriteColumnDefinition(IAttribute attribute, IEnvironmentHelper environment);
			void WriteCreateTable(IEntity entity, IEnvironmentHelper environment);
			void WriteDropTable(IEntity entity, IEnvironmentHelper environment);
			void WriteCreateForeignKey(IRelation relation, IEnvironmentHelper environment);
			void WriteDropForeignKey(IRelation relation, IEnvironmentHelper environment);
			void WriteCreatePrimaryKey(IPrimaryId constraint, IEnvironmentHelper environment);
			void WriteDropPrimaryKey(IPrimaryId constraint, IEnvironmentHelper environment);
			void WriteCreateUniqueConstraint(IUniqueId constraint, IEnvironmentHelper environment);
			void WriteDropUniqueConstraint(IUniqueId constraint, IEnvironmentHelper environment);
		}

		public interface ICodeWriterTransactSql : ICodeWriterSql
		{
			void WriteCreateIndex(IIndex index, IEnvironmentHelper environment);
			string WriteSpExecuteSql(string expression);
			string WriteSpExecuteSql(string expressionFormat, params object[] args);
		}
		
		public interface ICodeWriterPlSql : ICodeWriterSql
		{
			void DeclareVariable(string varName, string typeName);
			void If(string condition);
			void If(string conditionFormat, params object[] args);
			void EndIf();
			string WriteExecImmediat(string sql);
			string WriteExecImmediat(string format, params object[] args);
			void WriteExecImmediat(ICodeWriterText sql);
			string WriteExecImmediatVariable(string variableName);
			void WriteCreateSequence(IGenerator generator, IEnvironmentHelper environment);
			void WriteDropSequence(IGenerator generator, IEnvironmentHelper environment);
			void WriteCreateIndex(IIndex index, IEnvironmentHelper environment);
			void WriteDropIndex(IIndex index, IEnvironmentHelper environment);
			void WriteSetVar(string variableName, string value);
			void WriteSetVar(string variableName, ICodeWriterText sql);
		}
	}
	
	namespace CSharp
	{
		public interface ICodeWriterCSharp : ICodeWriterText
		{
			string AccessLevelToStr(AccessLevel accessLevel);
			string VirtualisationLevelToStr(VirtualisationLevel virtualisationLevel);
			void BeginClass(AccessLevel accessLevel,
			                bool partialClass,
			                string className,
			                string interfaces);
			void BeginClass(AccessLevel accessLevel,
			                bool partialClass,
			                string className,
			                string superclassName,
			                string interfaces);
			void BeginAbstractClass(AccessLevel accessLevel,
			                        bool partialClass,
			                        string className,
			                        string interfaces);
			void BeginStaticClass(AccessLevel accessLevel,
			                      bool partialClass,
			                      string className,
			                      string superclassName);
			void EndClass();
			void WriteProperty(AccessLevel accessLevel,
			                   VirtualisationLevel virtualisationLevel,
			                   IAttribute attribute,
			                   IEnvironmentHelper environment,
			                   string propertyAttributes);
			void SimpleProperty(AccessLevel accessLevel,
			                    VirtualisationLevel virtualisationLevel,
			                    string typeName,
			                    string name,
			                    bool getter,
			                    bool setter);
			void SimpleProperty(AccessLevel accessLevel,
			                    VirtualisationLevel virtualisationLevel,
			                    IAttribute attribute,
			                    IEnvironmentHelper environment);
			void StandardProperty(AccessLevel accessLevel,
			                      VirtualisationLevel virtualisationLevel,
			                      IAttribute attribute,
			                      IEnvironmentHelper environment,
			                      string defaultValue,
			                      string propertyAttributes);
			void BeginProperty(string signature);
			void BeginProperty(string format, params object[] args);
			void BeginProperty(AccessLevel accessLevel, VirtualisationLevel virtualisationLevel, string typeName, string name);
			void BeginPropertyGet();
			void BeginPropertySet();
			void WritePropertyGet(string expression);
			void WritePropertyGet(string format, params object[] args);
			void WritePropertySet(string expression);
			void WritePropertySet(string format, params object[] args);
			void EndPropertyAccessor();
			void EndProperty();
			void BeginRegion(string name);
			void EndRegion();
			void WriteUsing(string namespaceName);
			void DeclareMember(IAttribute attribute, IEnvironmentHelper environment);
			void DeclareMember(IAttribute attribute, string defaultValue, IEnvironmentHelper environment);

			void BeginTry();
			void EndTry();
			void BeginFinally();
			void EndFinally();
			void BeginCatch(string catchParams);
			void EndCatch();

			void BeginUsing(string expression);
			void BeginUsing(string format, params object[] args);
			void EndUsing();

			void If(string condition);
			void If(string conditionFormat, params object[] args);
			void EndIf();
			void Else();
			void ElseIf(string condition);
			void ElseIf(string conditionFormat, params object[] args);

			string AsAttributeValue(IAttribute attribute, IEnvironmentHelper environment);

			ICSharpNamespaceWriter CreateNamespaceWriter();
		}
		
		public enum AccessLevel
		{
			Private,
			Protected,
			Internal,
			Public
		}
		
		public enum VirtualisationLevel
		{
			None,
			Abstract,
			Virtual,
			Override,
			New
		}
		
		public interface ICSharpNamespaceWriter
		{
			void BeginScope(string name);
			void BeginOrContinueScope(string name);
			void EndScope();
			string Name { get; }
		}
	}
	
}

