using System;
using System.CodeDom;
using System.Xml;
using System.Text;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.CodeWriters.CSharp
{
	class CodeWriterCSharp : CodeWriterText, ICodeWriterCSharp
	{
		#region NamespaceWriter
		class NamespaceWriter : ICSharpNamespaceWriter
		{
			private string name = Const.EmptyValue;
			private bool wasUsed = false;
			private ICodeWriterCSharp writer;
			
			public NamespaceWriter(ICodeWriterCSharp writer)
			{ 
				this.writer = writer;
			}

			public string Name
			{
				get { return this.name; }
			}
			
			public void BeginScope(string name)
			{
				writer.WriteLine("namespace {0}", name);
				writer.BeginScope();
				this.name = name;
				this.wasUsed = true;
			}
			
			public void BeginOrContinueScope(string name)
			{
				if (this.name != name)
				{
					if (this.name != Const.EmptyValue)
					{
						writer.EndScope();
						writer.WriteLine();
					}
					BeginScope(name);
				}
				this.wasUsed = true;
			}
			
			public void EndScope()
			{
				if (wasUsed)
					writer.EndScope();
				name = Const.EmptyValue;
			}
		}
		#endregion

		#region Constructors
		public CodeWriterCSharp(GenieLamp lamp) : base(lamp)
		{
			ScopeSettings.ScopeBegin = "{";
			ScopeSettings.ScopeEnd = "}";
		}
		#endregion
		
		#region ICodeWriterCSharp implementation

		public override void WriteDoc(IMetaObject metaObject)
		{
			if (!metaObject.HasDoc)
				return;
			WriteLine("/// <summary>");
			foreach(string line in metaObject.Doc.LabelLines)
			{
				WriteLine("/// {0}", line);
			}
			foreach(string line in metaObject.Doc.TextLines)
			{
				WriteLine("/// {0}", line);
			}
			WriteLine("/// </summary>");
		}

		public string AccessLevelToStr(AccessLevel accessLevel)
		{
			switch (accessLevel)
			{
			case AccessLevel.Private:
				return "private";
			case AccessLevel.Protected:
				return "protected";
			case AccessLevel.Internal:
				return "internal";
			case AccessLevel.Public:
				return "public";
			}
			return String.Empty;
		}

		public string VirtualisationLevelToStr(VirtualisationLevel virtualisationLevel)
		{
			switch (virtualisationLevel)
			{
			case VirtualisationLevel.None:
				return "";
			case VirtualisationLevel.Virtual:
				return "virtual";
			case VirtualisationLevel.Override:
				return "override";
			case VirtualisationLevel.Abstract:
				return "abstract";
			case VirtualisationLevel.New:
				return "new";
			}
			return String.Empty;
		}

		public void BeginStaticClass(AccessLevel accessLevel,
		                             bool partialClass,
		                             string className,
		                             string superclassName)
		{
			BeginClass(accessLevel,
			           "static",
			           partialClass,
			           className,
			           superclassName,
			           null);
		}

		public void BeginAbstractClass(AccessLevel accessLevel,
		                               bool partialClass,
		                               string className,
		                               string interfaces)
		{
			BeginClass(accessLevel,
			           "abstract",
			           partialClass,
			           className,
			           null,
			           interfaces);
		}

		public void BeginAbstractClass(AccessLevel accessLevel,
		                               bool partialClass,
		                               string className,
		                               string superclassName,
		                               string interfaces)
		{
			BeginClass(accessLevel,
			           "abstract",
			           partialClass,
			           className,
			           superclassName,
			           interfaces);
		}

		public void BeginClass(AccessLevel accessLevel, bool partialClass, string className, string interfaces)
		{
			BeginClass(accessLevel,
			           Const.EmptyName,
			           partialClass,
			           className,
			           null,
			           interfaces);
		}

		public void BeginClass(AccessLevel accessLevel, bool partialClass, string className, string superclassName, string interfaces)
		{
			BeginClass(accessLevel,
			           Const.EmptyName,
			           partialClass,
			           className,
			           superclassName,
			           interfaces);
		}

		public void BeginClass(AccessLevel accessLevel, string modifiers, bool partialClass, string className, string superclassName, string interfaces)
		{
			Write("{0} {1}{2}class {3}{4}",
			      AccessLevelToStr(accessLevel),
			      String.IsNullOrEmpty(modifiers) ? "" : modifiers + " ",
			      partialClass ? "partial " : "",
			      className,
			      String.IsNullOrEmpty(superclassName) ? "" : " : " + superclassName);
			if (!String.IsNullOrEmpty(interfaces))
			{
				Write("{0}{1}",
				      String.IsNullOrEmpty(superclassName) ? " : " : ", ",
				      interfaces);
			}
			WriteLine();
			BeginScope();
		}

		public void EndClass()
		{
			EndScope();
		}

		#region Properties
		public void WriteProperty(AccessLevel accessLevel,
		                          VirtualisationLevel virtualisationLevel,
		                          IAttribute attribute,
		                          IEnvironmentHelper environment,
		                          string propertyAttributes)
		{
			string defaultValue = null;
			if (attribute.Type is IEnumerationType)
			{
				defaultValue = String.Format("({0}){1}",
				                             environment.ToTypeName(attribute, true),
				                             (attribute.Type as IEnumerationType).Enumeration.DefaultItem.Value.ToString());
			}
			else if (attribute.TypeDefinition.HasDefault)
			{
				defaultValue = environment.ToDefaultValue(attribute);
			}

			if (defaultValue != null || IsAttributeIsStringLimitedLength(attribute))
			{
				StandardProperty(accessLevel,
				                 virtualisationLevel,
				                 attribute,
				                 environment,
				                 defaultValue,
				                 propertyAttributes);
			}
			else
			{
				if (!String.IsNullOrEmpty(propertyAttributes))
					WriteLine(propertyAttributes);
				SimpleProperty(accessLevel,
				               virtualisationLevel,
				               attribute,
				               environment);
			}
		}


		public void SimpleProperty(AccessLevel accessLevel,
		                           VirtualisationLevel virtualisationLevel,
		                           IAttribute attribute,
		                           IEnvironmentHelper environment)
		{
			SimpleProperty(accessLevel,
			               virtualisationLevel,
			               environment.ToTypeName(attribute, true),
			               attribute.Name,
			               true,
			               !attribute.ReadOnly);
		}


		public void SimpleProperty(AccessLevel accessLevel,
		                           VirtualisationLevel virtualisationLevel,
		                           string typeName,
		                           string name,
		                           bool getter,
		                           bool setter)
		{
			Write("{0} ", AccessLevelToStr(accessLevel));
			if (virtualisationLevel != VirtualisationLevel.None)
				Write("{0} ", VirtualisationLevelToStr(virtualisationLevel));
			Write("{0} {1} {2}", typeName, name, ScopeSettings.ScopeBegin);
			if (getter)
				Write(" get;");
			if (setter)
				Write(" set;");
			WriteLine(" {0}", ScopeSettings.ScopeEnd);
		}


		protected bool IsAttributeIsStringLimitedLength(IAttribute attribute)
		{
			return
				attribute.Type is IScalarType
				&& ((attribute.Type as IScalarType).BaseType == BaseType.TypeAnsiString
					    || (attribute.Type as IScalarType).BaseType == BaseType.TypeString)
					&& (attribute.TypeDefinition.HasLength || (attribute.Type as IScalarType).TypeDefinition.HasLength);
		}


		public void StandardProperty(AccessLevel accessLevel,
		                             VirtualisationLevel virtualisationLevel,
		                             IAttribute attribute,
		                             IEnvironmentHelper environment,
		                             string defaultValue,
		                             string propertyAttributes)
		{
			if (defaultValue == null || (defaultValue != null && defaultValue.Length == 0))
				DeclareMember(attribute, environment);
			else
				DeclareMember(attribute, defaultValue, environment);

			if (!String.IsNullOrEmpty(propertyAttributes))
				WriteLine(propertyAttributes);
			BeginProperty(accessLevel, virtualisationLevel, environment.ToTypeName(attribute, true), attribute.Name);
			WritePropertyGet(String.Format("return {0};", environment.ToMemberName(attribute)));
			if (!attribute.ReadOnly)
			{
				if (IsAttributeIsStringLimitedLength(attribute))
				{
					WritePropertySet(String.Format("{0} = value != null && value.Length > {1} ? value.Substring(0, {1}) : value;",
					                                      environment.ToMemberName(attribute),
					                                      attribute.TypeDefinition.Length));
				}
				else
					WritePropertySet(String.Format("{0} = value;", environment.ToMemberName(attribute)));
			}
			EndProperty();
		}

		public void WritePropertyGet(string expression)
		{
			WriteLine("get {0} {1} {2}", ScopeSettings.ScopeBegin, expression, ScopeSettings.ScopeEnd);
		}
		
		public void WritePropertyGet(string format, params object[] args)
		{
			WritePropertyGet(String.Format(format, args));
		}

		public void WritePropertySet(string expression)
		{
			WriteLine("set {0} {1} {2}", ScopeSettings.ScopeBegin, expression, ScopeSettings.ScopeEnd);
		}
		
		public void WritePropertySet(string format, params object[] args)
		{
			WritePropertySet(String.Format(format, args));
		}
		
		public void BeginProperty(string signature)
		{
			WriteLine(signature);
			BeginScope();
		}

		public void BeginProperty(string format, params object[] args)
		{
			BeginProperty(String.Format(format, args));
		}
		public void BeginProperty(AccessLevel accessLevel, VirtualisationLevel virtualisationLevel, string typeName, string name)
		{
			WriteLine("{0} {1}{2} {3}", 
			          AccessLevelToStr(accessLevel), 
			          String.IsNullOrEmpty(VirtualisationLevelToStr(virtualisationLevel)) ? "" : VirtualisationLevelToStr(virtualisationLevel) + " ",
			          typeName,
			          name);
			BeginScope();
		}
	
		public void BeginPropertyGet()
		{
			Write("get ");
			BeginScope();
		}
	
		public void BeginPropertySet()
		{
			Write("set ");
			BeginScope();
		}
		
		public void EndPropertyAccessor()
		{
			EndScope();
		}
	
		public void EndProperty()
		{
			EndScope();
		}
		#endregion

		public void BeginRegion(string name)
		{
			WriteLine("#region {0}", name);
		}

		public void EndRegion()
		{
			WriteLine("#endregion");
		}
		
		public void WriteUsing(string namespaceName)
		{
			WriteLine("using {0};", namespaceName);
		}

		public ICSharpNamespaceWriter CreateNamespaceWriter()
		{
			return new NamespaceWriter(this);
		}

		public void DeclareMember(IAttribute attribute, IEnvironmentHelper environment)
		{
			WriteLine("private {0} {1};", environment.ToTypeName(attribute, true), environment.ToMemberName(attribute));
		}

		public void DeclareMember(IAttribute attribute, string defaultValue, IEnvironmentHelper environment)
		{
			WriteLine("private {0} {1} = {2};",
			          environment.ToTypeName(attribute, true),
			          environment.ToMemberName(attribute),
			          defaultValue);
		}

		#region try-catch-finally
		public void BeginTry()
		{
			WriteLine("try");
			BeginScope();
		}

		public void EndTry()
		{
			EndScope();
		}

		public void BeginFinally()
		{
			WriteLine("finally");
			BeginScope();
		}

		public void EndFinally()
		{
			EndScope();
		}

		public void BeginCatch(string catchParams)
		{
			Write("catch(");
			if (!String.IsNullOrWhiteSpace(catchParams))
				Write(catchParams);
			WriteLine(")");
			BeginScope();
		}

		public void EndCatch()
		{
			EndScope();
		}
		#endregion

		public void BeginUsing(string expression)
		{
			WriteLine("using({0})", expression);
			BeginScope();
		}

		public void BeginUsing(string format, params object[] args)
		{
			BeginUsing(String.Format(format, args));
		}

		public void EndUsing()
		{
			EndScope();
		}

		public void If(string conditionFormat, params object[] args)
		{
			If(String.Format(conditionFormat, args));
		}

		public void If(string condition)
		{
			WriteLine("if ({0})", condition);
			BeginScope();
		}

		public void EndIf()
		{
			EndScope();
		}

		public void Else()
		{
			EndScope();
			WriteLine("else");
			BeginScope();
		}

		public void ElseIf(string condition)
		{
			EndScope();
			WriteLine("else if ({0})", condition);
			BeginScope();
		}

		public void ElseIf(string conditionFormat, params object[] args)
		{
			ElseIf(String.Format(conditionFormat, args));
		}

		public string AsAttributeValue(IAttribute attribute, IEnvironmentHelper environment)
		{
			return !attribute.TypeDefinition.Required && environment.IsNullable(attribute) ? ".Value" : "";
		}
		#endregion


	}

}

