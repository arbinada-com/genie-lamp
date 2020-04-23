using System;
using System.Text;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Environments
{
	class EnvironmentSqliteDb : EnvironmentHelperBase
	{
		public EnvironmentSqliteDb(TargetEnvironment target, string version)
			: base(target, version)
		{
		}

		#region implemented abstract members of GenieLamp.Core.Environments.EnvironmentHelperBase
		public override string ToTypeName(IType type, 
		                                  ITypeDefinition typeDefinition,
		                                  IPersistence persistence, 
		                                  bool fullName,
		                                  bool forceNullable)
		{
			if (persistence != null && persistence.TypeDefined)
				return persistence.TypeName;
			
			StringBuilder s = new StringBuilder();
			if (type is IScalarType)
			{
				BaseType bt = (type as IScalarType).BaseType;
				ITypeDefinition typeDef = typeDefinition != null ? typeDefinition : (type as IScalarType).TypeDefinition;
				switch (bt)
				{
				case BaseType.TypeBool:
					s.Append("BOOLEAN");
					break;
				case BaseType.TypeAnsiChar:
				case BaseType.TypeChar:
				case BaseType.TypeAnsiString:
				case BaseType.TypeString:
					if (bt == BaseType.TypeString)
						s.Append("N");
					if (typeDef.Fixed || typeDef.Length == 1)
						s.Append("CHAR");
					else
						s.Append("VARCHAR");
					s.AppendFormat("({0})", typeDef.Length);
					break;
				case BaseType.TypeDate:
					s.Append("DATE");
					break;
				case BaseType.TypeDateTime:
					s.Append("DATETIME");
					break;
				case BaseType.TypeCurrency:
				case BaseType.TypeDecimal:
					if (typeDef.Precision > 0)
						s.AppendFormat("DECIMAL({0}, {1})", typeDef.Length, typeDef.Precision);
					else
						s.AppendFormat("DECIMAL({0})", typeDef.Length);
					break;
				case BaseType.TypeFloat:
					s.Append("FLOAT");
					break;
				case BaseType.TypeInt:
					if (typeDef.Length == 8)
						s.Append("BIGINT");
					else if (typeDef.Length == 2)
						s.Append("SMALLINT");
					else if (typeDef.Length == 1)
						s.Append("TINYINT");
					else
						s.Append("INTEGER");
					break;
				case BaseType.TypeByte:
					s.Append("TINYINT");
					break;
				}
			}
			else if (type is IEnumerationType)
			{
				if ((type as IEnumerationType).Length <= byte.MaxValue)
					s.AppendFormat("TINYINT");
				else if ((type as IEnumerationType).Length <= Int16.MaxValue)
					s.AppendFormat("SMALLINT");
				else
					s.AppendFormat("INTEGER");
			}
			else if (type is IEntityType)
			{
				IEntityType et = (type as IEntityType);
				s.Append(et.Entity.Constraints.PrimaryId.Attributes[0].Persistence.TypeName);
			}
			
			return s.ToString();
			
		}
		
		public override string ToDefaultValue(IAttribute attribute)
		{
			StringBuilder s = new StringBuilder();
			
			if (attribute.TypeDefinition.HasDefault && attribute.Type is IScalarType)
			{
				switch ((attribute.Type as IScalarType).BaseType)
				{
				case BaseType.TypeBool:
					s.Append(attribute.TypeDefinition.Default.ToLower().Equals("true") ? "1" : "0");
					break;
				case BaseType.TypeDate:
				case BaseType.TypeTime:
				case BaseType.TypeDateTime:
					s.Append(attribute.TypeDefinition.Default.Equals("current", StringComparison.InvariantCultureIgnoreCase) ? 
					         "current_timestamp" :
					         String.Format("'{0}'", attribute.TypeDefinition.Default));
					break;
				default:
					s.Append(attribute.TypeDefinition.Default);
					break;
				}
			}
			return s.ToString();
		}
		#endregion
	}
}

