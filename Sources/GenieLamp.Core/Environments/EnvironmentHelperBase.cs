using System;
using System.Text;

using GenieLamp.Core;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Environments
{
	abstract class EnvironmentHelperBase : IEnvironmentHelper
	{
		private TargetEnvironment target = TargetEnvironment.Undefined;
		private string version = Const.EmptyValue;
		private string baseNamespace = Const.EmptyValue;

		#region Constructors
		protected EnvironmentHelperBase(TargetEnvironment target, string version)
		{
			this.target = target;
			this.version = version;
		}
		#endregion
		
		public static IEnvironmentHelper CreateHelper(TargetEnvironment target, string version)
		{
			switch (target)
			{
			case TargetEnvironment.CSharp:
				return new EnvironmentCSharp(target, version);
			case TargetEnvironment.OracleDb:
				return new EnvironmentOracleDb(target, version);
			case TargetEnvironment.SqlServer:
				return new EnvironmentSqlServer(target, version);
			case TargetEnvironment.Sqlite:
				return new EnvironmentSqliteDb(target, version);
			default:
				throw new GlException("Target environment not implemented. {0}", target);
			}
		}

		protected string AddBaseNamespace(string name)
		{
			if (BaseNamespace.Length == 0)
				return name;
			else
				return String.Format("{0}.{1}", BaseNamespace, name);
		}


		#region IEnvironmentHelper implementation
		public string BaseNamespace
		{
			get { return this.baseNamespace; }
			set { baseNamespace = value.Trim().TrimEnd('.'); }
		}

		public virtual string ToTypeName(IAttribute attribute, bool fullName)
		{
			return ToTypeName(attribute.Type, attribute.TypeDefinition, attribute.Persistence, fullName, false);
		}

		public virtual string ToTypeName(IAttribute attribute, bool fullName, bool forceNullable)
		{
			return ToTypeName(attribute.Type, attribute.TypeDefinition, attribute.Persistence, fullName, forceNullable);
		}

		public virtual string ToTypeName(IEntity entity, bool fullName)
		{
			return ToTypeName(entity.Type, null, null, fullName, false);
		}

		public virtual string ToTypeName(IEntityType type, bool fullName)
		{
			return ToTypeName(type, null, null, fullName, false);
		}

		public virtual string ToTypeName(IType type, ITypeDefinition typeDefinition, bool fullName)
		{
			return ToTypeName(type, typeDefinition, null, fullName, false);
		}

		public abstract string ToTypeName(IType type, ITypeDefinition typeDefinition, IPersistence persistence, bool fullName, bool forceNullable);

		public virtual string ToIntfName(IEntity entity, bool fullName)
		{
			return TypeNameToIntfName(ToTypeName(entity, fullName));
		}

		public virtual string ToCollectionIntfName(IEntity entity, bool fullName)
		{
			return TypeNameToIntfName(ToCollectionTypeName(entity, fullName));
		}

		public virtual string TypeNameToIntfName(string typeName)
		{
			int pos = typeName.LastIndexOf('.');
			if (pos < 0)
				return "I" + typeName;
			return typeName.Substring(0, pos + 1) + "I" + typeName.Substring(pos + 1);
		}

		public virtual string ToCollectionTypeName(IEntity entity, bool fullName)
		{
			return String.Format("{0}Collection", ToTypeName(entity, fullName));
		}

		public virtual bool IsNullable(IAttribute attribute)
		{
			return IsNullable(attribute.Type, attribute.TypeDefinition);
		}

		public virtual bool IsNullable(IType type, ITypeDefinition typeDefinition)
		{
			return false;
		}

		public TargetEnvironment Target
		{
			get { return target; }
		}

		public string Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public abstract string ToDefaultValue(IAttribute attribute);

		public virtual string ToParamName(IAttribute attribute)
		{
			return attribute.Name.Substring(0, 1).ToLower() +
				(attribute.Name.Length > 1 ?
				 (attribute.IsMigrated ? attribute.Migration.Name : attribute.Name).Substring(1)
				 : "");
		}

		public virtual string ToParams(IAttributes attributes, bool withTypes)
		{
			StringBuilder sb = new StringBuilder();
			int count = attributes.Count - 1;
			foreach (IAttribute a in attributes)
			{
				if (withTypes)
					sb.AppendFormat("{0} ", ToTypeName(a, true));
				sb.AppendFormat("{0}{1}",
				                ToParamName(a),
				                count == 0 ? "" : ", ");
				count--;
			}
			return sb.ToString();
		}

		public virtual string ToParams(IAttributes attributes)
		{
			return ToParams(attributes, true);
		}

		public virtual string ToArguments(IAttributes attributes)
		{
			return ToParams(attributes, false);
		}

		public virtual string ToMemberName(IAttribute attribute)
		{
			return ToParamName(attribute);
		}

		public virtual IScalarType ImportType(string typeName, IAttributeTypeDefinitionImported attrTypeDef)
		{
			Metamodel.Type type = GenieLamp.Instance.Model.Types.GetByName(String.IsNullOrEmpty(typeName) ? "int" : typeName);
			if (type == null || !(type is ScalarType))
			{
				type = GenieLamp.Instance.Model.Types.GetByName("int");
			}
			return type as ScalarType;
		}

		// Method generation
		public virtual string ToOperationSignature(IEntityOperation operation, bool fullTypeName)
		{
			throw new NotSupportedException("Operation signature should be implemented in concrete environment");
		}
		#endregion
	}
}

