using System;

using System.Text;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Layers;

namespace GenieLamp.Core.Environments
{
	class EnvironmentSqlServer : EnvironmentHelperBase
	{
		public EnvironmentSqlServer(TargetEnvironment target, string version)
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
					switch(type.Model.Lamp.Config.Layers.PersistenceConfig.BooleanValuePersistence)
					{
					case BooleanValuePersistence.YesNo:
					case BooleanValuePersistence.TrueFalse:
						s.Append("char(1)");
						break;
					case BooleanValuePersistence.Native:
						s.Append("bit");
						break;
					}
					break;
				case BaseType.TypeAnsiChar:
				case BaseType.TypeChar:
				case BaseType.TypeAnsiString:
				case BaseType.TypeString:
					if (bt == BaseType.TypeString)
						s.Append("n");
					if (typeDef.Fixed || typeDef.Length == 1)
						s.Append("char");
					else
						s.Append("varchar");
					s.AppendFormat("({0})", typeDef.Length);
					break;
				case BaseType.TypeDate:
					s.Append("date");
					break;
				case BaseType.TypeDateTime:
					s.Append("datetime");
					break;
				case BaseType.TypeCurrency:
				case BaseType.TypeDecimal:
					if (typeDef.Precision > 0)
						s.AppendFormat("numeric({0}, {1})", typeDef.Length, typeDef.Precision);
					else
						s.AppendFormat("numeric({0})", typeDef.Length);
					break;
				case BaseType.TypeFloat:
					s.Append("float");
					break;
				case BaseType.TypeInt:
					if (typeDef.Length == 8)
						s.Append("bigint");
					else if (typeDef.Length == 2)
						s.Append("smallint");
					else if (typeDef.Length == 1)
						s.Append("tinyint");
					else
						s.Append("int");
					break;
				case BaseType.TypeByte:
					s.Append("tinyint");
					break;
				}
			}
			else if (type is IEnumerationType)
			{
				if ((type as IEnumerationType).Length <= byte.MaxValue)
					s.AppendFormat("tinyint");
				else if ((type as IEnumerationType).Length <= Int16.MaxValue)
					s.AppendFormat("smallint");
				else
					s.AppendFormat("int");
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
					switch(attribute.Model.Lamp.Config.Layers.PersistenceConfig.BooleanValuePersistence)
					{
					case BooleanValuePersistence.YesNo:
						s.Append(attribute.TypeDefinition.Default.ToLower().Equals("true") ? "'Y'" : "'N'");
						break;
					case BooleanValuePersistence.TrueFalse:
						s.Append(attribute.TypeDefinition.Default.ToLower().Equals("true") ? "'T'" : "'F'");
						break;
					case BooleanValuePersistence.Native:
						s.Append(attribute.TypeDefinition.Default.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? "1" : "0");
						break;
					}
					break;
				case BaseType.TypeDate:
				case BaseType.TypeTime:
				case BaseType.TypeDateTime:
					s.Append(attribute.TypeDefinition.Default.Equals("current", StringComparison.InvariantCultureIgnoreCase) ? 
					         "getdate()" : 
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

