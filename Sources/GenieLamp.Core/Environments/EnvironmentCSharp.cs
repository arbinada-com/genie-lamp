using System;
using System.Text;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Environments
{
	class EnvironmentCSharp : EnvironmentHelperBase
	{
		public EnvironmentCSharp(TargetEnvironment target, string version)
			: base(target, version)
		{
		}

		public override string ToTypeName(IType type,
		                                  ITypeDefinition typeDefinition,
		                                  IPersistence persistence,
		                                  bool fullName,
		                                  bool forceNullable)
		{
			if (type is IEntityType)
			{
				return fullName ? AddBaseNamespace((type as IEntityType).Entity.FullName) : (type as IEntityType).Entity.Name;
			}
			else if (type is IEnumerationType)
			{
				string typeName = fullName ? AddBaseNamespace((type as IEnumerationType).FullName) : (type as IEnumerationType).Name;
				if (forceNullable)
					typeName += "?";
				return typeName;
			}
			else if (type is IScalarType)
			{
				IScalarType st = (type as IScalarType);
				ITypeDefinition typeDef = typeDefinition != null ? typeDefinition : st.TypeDefinition;
				string typeName;
				switch (st.BaseType)
				{
				case BaseType.TypeVoid:
					typeName = "void";
					break;
				case BaseType.TypeAnsiChar:
				case BaseType.TypeChar:
					if (typeDef.Length == 1)
						typeName = "char";
					else
						return String.Format("char[{0}]", typeDef.Length);
					break;
				case BaseType.TypeAnsiString:
				case BaseType.TypeString:
					return "string";
				case BaseType.TypeDate:
				case BaseType.TypeDateTime:
				case BaseType.TypeTime:
					typeName = "DateTime";
					break;
				case BaseType.TypeBinary:
					return typeDef.HasLength ? String.Format("byte[{0}]", typeDef.Length) : "byte[]";
				case BaseType.TypeBool:
					typeName = "bool";
					break;
				case BaseType.TypeByte:
					typeName = "byte";
					break;
				case BaseType.TypeCurrency:
				case BaseType.TypeDecimal:
					typeName = "decimal";
					break;
				case BaseType.TypeFloat:
					typeName = "double";
					break;
				case BaseType.TypeInt:
					typeName = typeDef.Length == 2 ? "short" : typeDef.Length == 8 ? "long" : "int";
					break;
				case BaseType.TypeUuid:
					typeName = "Guid";
					break;
				default:
					throw new GlException("Unsupported base type: {0}. {1}", st.BaseType, type);
				}

				if (!typeDef.Required || forceNullable)
					return typeName + "?";
				return ToCollectionTypeName(typeName, typeDef);
			}
			else
				throw new GlException("Unsupported type: {0}", type);

		}

		public override bool IsNullable(IType type, ITypeDefinition typeDefinition)
		{
			if (type is IScalarType)
			{
				IScalarType st = (type as IScalarType);
				switch (st.BaseType)
				{
				case BaseType.TypeChar:
					return (typeDefinition.Length == 1);
				case BaseType.TypeAnsiChar:
				case BaseType.TypeAnsiString:
				case BaseType.TypeDate:
				case BaseType.TypeDateTime:
				case BaseType.TypeTime:
				case BaseType.TypeBool:
				case BaseType.TypeByte:
				case BaseType.TypeCurrency:
				case BaseType.TypeDecimal:
				case BaseType.TypeFloat:
				case BaseType.TypeInt:
				case BaseType.TypeUuid:
					return true;
				default:
					return false;
				}
			}
			else if (type is IEnumerationType)
				return true;
			else
				return false;
		}

		public override string ToDefaultValue(IAttribute attribute)
		{
			StringBuilder s = new StringBuilder();
			
			if (attribute.TypeDefinition.HasDefault && attribute.Type is IScalarType)
			{
				switch ((attribute.Type as IScalarType).BaseType)
				{
				case BaseType.TypeBool:
					s.Append(attribute.TypeDefinition.Default.ToLower().Equals("true") ? "true" : "false");
					break;
				case BaseType.TypeDate:
				case BaseType.TypeTime:
				case BaseType.TypeDateTime:
					s.Append(attribute.TypeDefinition.Default.Equals("current", StringComparison.InvariantCultureIgnoreCase) ?
					         "DateTime.Now" :
					         String.Format("DateTime.Parse(\"{0}\")", attribute.TypeDefinition.Default));
					break;
				default:
					s.Append(attribute.TypeDefinition.Default);
					break;
				}
			}
			return s.ToString();
		}

		public override string ToMemberName(IAttribute attribute)
		{
			return String.Format("m_{0}", attribute.Name);
		}

		private string ToCollectionTypeName(string typeName, ITypeDefinition typeDef)
		{
			switch(typeDef.CollectionType)
			{
			case CollectionType.None:
				return typeName;
			case CollectionType.Array:
				return String.Format("{0}[]", typeName);
			case CollectionType.List:
				return String.Format("System.Collections.Generic.IList<{0}>", typeName);
			default:
				throw new GlException("Unsupported param collection type '{0}'", Enum.GetName(typeof(CollectionType), typeDef.CollectionType));
			}
		}


		/// <summary>
		/// Compose operation signature.
		/// </summary>
		/// <returns>
		/// Operation signature string.
		/// </returns>
		/// <param name='operation'>
		/// Entity operation object
		/// </param>
		/// <param name='fullTypeName'>
		/// Use full names of types
		/// </param>
		public override string ToOperationSignature(IEntityOperation operation, bool fullTypeName)
		{
			StringBuilder sb = new StringBuilder();
			if (operation.Params.HasReturns)
				sb.AppendFormat("{0} ", ToTypeName(operation.Params.Returns.Type, operation.Params.Returns.TypeDefinition, fullTypeName));
			else
				sb.AppendFormat("void ");
			sb.AppendFormat("{0}(", operation.Name);
			int i = 0;
			foreach(IEntityOperationParam param in operation.Params)
			{
				if (i++ > 0)
					sb.AppendFormat(", ");
				sb.AppendFormat("{0}{1} {2}",
				                param.IsOut ? "out " : param.IsRef ? "ref " : "",
				                ToTypeName(param.Type, param.TypeDefinition, fullTypeName),
				                param.Name);
			}
			sb.AppendFormat(")");
			return sb.ToString();
		}

	}
}

