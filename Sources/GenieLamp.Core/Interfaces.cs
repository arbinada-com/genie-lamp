using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters;

namespace GenieLamp.Core
{
	public abstract class Const
	{
		public const string EmptyName = "";
		public const string EmptyValue = "";
	}
	
	#region Project
    public interface IGenieLamp
    {
        IGenies Genies { get; }
        IModel Model { get; }
		ILogger Logger { get; }
		IGenieLampConfig Config { get; }
		ICodeWritersFactory CodeWritersFactory { get; }
		Version Version { get; }
		string ProjectFileName { get; }
		string ProjectDirectory { get; }
		string ProjectName { get; }
		string ProjectVersion { get; }
		IGenieLampUtils GenieLampUtils { get; }
		IMacroExpander Macro { get; }
    }
	
    public interface IGenieLampSpellConfig // To inject spell parameters from caller
    {
        string FileName { get; }
		WarningLevel MinWarningLevel { get; }
    }
	
	public interface IGenieLampUtils
	{
		IUtilsXml Xml { get; }
		IEnvironmentHelper GetEnvironmentHelper(TargetEnvironment target, string version = Const.EmptyValue);
		INamingConvention GetNamingConvention(NamingStyle style);
		ISqlStringBuilder GetSqlStringBuilder();
	}
	
    public interface IGenies : IEnumerable<IGenie>
    {
        IGenie this[string name] { get; }
    }
	#endregion
	
	#region Target environments
	public interface IEnvironmentHelper
	{
		string BaseNamespace { get; set; }
		string ToTypeName(IAttribute attribute, bool fullName);
		string ToTypeName(IAttribute attribute, bool fullName, bool forceNullable);
		string ToTypeName(IEntity entity, bool fullName);
		string ToTypeName(IEntityType type, bool fullName);
		string ToTypeName(IType type, ITypeDefinition typeDefinition, bool fullName);
		string ToTypeName(IType type, ITypeDefinition typeDefinition, IPersistence persistence, bool fullName, bool forceNullable);
		string ToIntfName(IEntity entity, bool fullName);
		string ToCollectionIntfName(IEntity entity, bool fullName);
		string ToCollectionTypeName(IEntity entity, bool fullName);
		string TypeNameToIntfName(string typeName);
		bool IsNullable(IAttribute attribute);
		bool IsNullable(IType type, ITypeDefinition typeDefinition);
		string ToDefaultValue(IAttribute attribute);
		TargetEnvironment Target { get; }
		string Version { get; set; }
		string ToParamName(IAttribute attribute);
		string ToParams(IAttributes attributes);
		string ToArguments(IAttributes attributes);
		string ToMemberName(IAttribute attribute);
		IScalarType ImportType(string typeName, IAttributeTypeDefinitionImported attrTypeDef);
		string ToOperationSignature(IEntityOperation operation, bool fullTypeName);
	}

	public enum TargetEnvironment
	{
		Undefined = 0,
		CSharp,
		OracleDb,
		OraclePlSql,
		SqlServer,
		Sqlite,
		Delphi
	}
	#endregion
	
	#region Utilities
	public interface IUtilsXml
	{
		IDocHelper CreateDocHelper(string rootName);
		IDocHelper CreateDocHelper(XmlDocument doc);
	}
	
	public interface IDocHelper
	{
		XmlDocument Document { get; }
		XmlNode CreateElement(string name);
		/// <summary>
		/// Adds attribute to current node.
		/// </summary>
		/// <returns>
		/// The attribute node.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
		XmlAttribute AddAttribute(string name, string value);
		XmlNode CurrentNode { get; set; }
		/// <summary>
		/// Creates the comment node and add it as child of current or document element nodes.
		/// </summary>
		/// <returns>
		/// The comment node.
		/// </returns>
		/// <param name='format'>
		/// Format.
		/// </param>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		XmlNode CreateComment(string format, params object[] args);
		/// <summary>
		/// Creates the comment node and add it as child of current or document element nodes.
		/// </summary>
		/// <returns>
		/// The comment.
		/// </returns>
		/// <param name='comment'>
		/// Comment.
		/// </param>
		XmlNode CreateComment(string comment);
		/// <summary>
		/// Insert node newChildNode as the first child of current node.
		/// </summary>
		/// <returns>
		/// Inserted node
		/// </returns>
		/// <param name='newChildNode'>
		/// Node.
		/// </param>
		XmlNode AddFirstChild(XmlNode newChildNode);
		/// <summary>
		/// Sets the current node to document element (root) node.
		/// </summary>
		void SetCurrentToRoot();
	}
	
	public interface IMacroExpander
	{
		void SetMacro(string macro, string subst);
		string Subst(string source);
		IMacroExpander CreateChild();
	}

	public interface ISqlStringBuilder
	{
		void AppendLine(string format, params object[] args);
		void AppendLine();
		void Append(string format, params object[] args);
		void Append(string value);
		void Select(string format, params object[] args);
		void Select(string columnDescription);
		void From(string format, params object[] args);
		void From(string expression);
		void WhereAnd(string format, params object[] args);
		void WhereAnd(string expression);
		void OrderBy(string expression);
		void Union();
		void UnionAll();
		int Indent { get; set; }
		string ToString();
	}
	#endregion
	
	public interface IParamsSimple : IEnumerable<IParamSimple>
	{
		IParamSimple this[string name] { get; }
		IParamSimple ParamByName(string name, bool throwException = false);
		IParamSimple ParamByName(string name, string defaultValue);
		string ValueByName(string name, string defaultValue = Const.EmptyValue);
		bool ValueByName(string name, bool defaultValue);
		int ValueByName(string name, int defaultValue);
	}

	public interface IParamSimple
	{
		string Name { get; }
		string Value { get; }
		bool AsBool { get; }
		int AsInt { get; }
		string Text { get; }
	}
	
	#region Naming
	public enum NamingStyle
	{
		None,
		UpperCase,
		LowerCase,
		CamelCase
	}
	
	public interface INamingConvention
	{
		NamingStyle Style { get; }
		string Convert(string name);
		int MaxLength { get; }
		IParamsSimple Params { get; }
	}
	#endregion

	
	#region Logger
	public enum WarningLevel
	{
		Low = 0,
		Medium = 1,
		High = 2
	}
	
	public interface ILogger
	{
		void Trace(string message);
		void Trace(string message, params object[] args);
		void TraceLine(string message);
		void TraceLine(string message, params object[] args);
		void Debug(string message);
		void Debug(string message, params object[] args);
		void DebugLine(string message, params object[] args);
		void DebugLine(string message);
		void Warning(WarningLevel level, string message);
		void Warning(WarningLevel level, string message, params object[] args);
		WarningLevel MinWarningLevel { get; set; }
		int WarningCount { get; }
		void Error(string format, params object[] args);
		void Error(string message);
		int ErrorCount { get; }
		void ProgressStep();
		bool Echo {get; set; }
	}
	#endregion
	
	
	#region Configurations
	public interface IGenieLampConfig
	{
		Layers.ILayersConfig Layers { get; }
		Patterns.IPatterns Patterns { get; }
	}
	#endregion
	
	#region Genies
	public interface IGenieConfig
	{
		string Name { get; }
		string AssemblyName { get; }
		string TypeName { get; }
		bool Active { get; }
		string OutDir { get; }
		bool SingleOutFile { get; }
		string OutFileName { get; }
		Encoding OutFileEncoding { get; }
		bool UpdateDatabase	{ get;	}
		string TargetVersion { get; }
		IParamsSimple Params { get; }
		IMacroExpander Macro { get; }
		void NotifyAssistants(string eventName, IMetaObject sender, string text, params object[] args);
	}
	
    public interface IGenie
    {
		IGenieConfig Config { get; }
        void Init(IGenieConfig config);
        void Spell(IModel model);
        string Name { get; }
		Version Version { get; }
        IModel Model { get; }
    }

	public interface IGenieAssistantConfig
	{
		IGenieConfig ParentConfig { get; }
		IParamsSimple Params { get; }
	}

	public interface IGenieAssistant
	{
        void Init(IGenieAssistantConfig config, IGenie master);
		void HandleEvent(string eventName, IMetaObject sender, string text, params object[] args);
	}

    public interface IGenieOfPersistence : IGenie
	{
		void UpdateDatabase();
	}
	#endregion
	
}

