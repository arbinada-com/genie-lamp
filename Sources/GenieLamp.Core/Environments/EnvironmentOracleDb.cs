using System;
using System.Text;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Layers;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Environments
{
	class EnvironmentOracleDb : EnvironmentHelperBase
	{
		public EnvironmentOracleDb(TargetEnvironment target, string version)
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
			// Oracle 10g data types 
			// http://docs.oracle.com/cd/B19306_01/server.102/b14220/datatype.htm
			if (type is IScalarType)
			{
				BaseType bt = (type as IScalarType).BaseType;
				ITypeDefinition typeDef = typeDefinition != null ? typeDefinition : (type as IScalarType).TypeDefinition;
				switch (bt)
				{
				case BaseType.TypeBool:
					CheckNativeBoolean(type.Model.Lamp.Config.Layers.PersistenceConfig.BooleanValuePersistence);
					switch(type.Model.Lamp.Config.Layers.PersistenceConfig.BooleanValuePersistence)
					{
					case BooleanValuePersistence.YesNo:
					case BooleanValuePersistence.TrueFalse:
						s.Append("VARCHAR(1)");
						break;
					}
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
						s.Append("VARCHAR2");
					s.AppendFormat("({0})", typeDef.Length);
					break;
				case BaseType.TypeDate:
					s.Append("DATE");
					break;
				case BaseType.TypeDateTime:
					s.Append("DATE");
					break;
				case BaseType.TypeCurrency:
				case BaseType.TypeDecimal:
					if (typeDef.Precision > 0)
						s.AppendFormat("NUMBER({0}, {1})", typeDef.Length, typeDef.Precision);
					else
						s.AppendFormat("NUMBER({0})", typeDef.Length);
					break;
				case BaseType.TypeFloat:
					s.Append("BINARY_DOUBLE");
					break;
				case BaseType.TypeInt:
					if (typeDef.Length == 8)
						s.Append("NUMBER(19)");
					else if (typeDef.Length == 2)
						s.Append("NUMBER(5)");
					else if (typeDef.Length == 1)
						s.Append("NUMBER(3)");
					else
						s.Append("NUMBER(10)");
					break;
				case BaseType.TypeByte:
					s.Append("CHAR(1)");
					break;
				case BaseType.TypeUuid:
					s.Append("RAW(16)");
					break;
				}
			}
			else if (type is IEnumerationType)
			{
				s.AppendFormat("NUMBER({0})", Math.Ceiling(Math.Log10((type as IEnumerationType).Length)));
			}
			else if (type is IEntityType)
			{
				IEntityType et = (type as IEntityType);
				s.Append(et.Entity.Constraints.PrimaryId.Attributes[0].Persistence.TypeName);
			}
			
			return s.ToString();
			
		}

		private void CheckNativeBoolean(BooleanValuePersistence value)
		{
			if (value == BooleanValuePersistence.Native)
			{
				throw new GlException("Native boolean type is not supported by Oracle. Use 'BooleanValues' parameter of persistence layer configuration\n" +
					"Example: <Param name=\"BooleanValues\" value=\"YesNo\"/>");
			}
		}
		
		public override string ToDefaultValue(IAttribute attribute)
		{
			StringBuilder s = new StringBuilder();
			
			if (attribute.TypeDefinition.HasDefault && attribute.Type is IScalarType)
			{
				switch ((attribute.Type as IScalarType).BaseType)
				{
				case BaseType.TypeBool:
					CheckNativeBoolean(attribute.Model.Lamp.Config.Layers.PersistenceConfig.BooleanValuePersistence);
					switch(attribute.Model.Lamp.Config.Layers.PersistenceConfig.BooleanValuePersistence)
					{
					case BooleanValuePersistence.YesNo:
						s.Append(attribute.TypeDefinition.Default.ToLower().Equals("true") ? "'Y'" : "'N'");
						break;
					case BooleanValuePersistence.TrueFalse:
						s.Append(attribute.TypeDefinition.Default.ToLower().Equals("true") ? "'T'" : "'F'");
						break;
					}
					break;
				case BaseType.TypeDate:
				case BaseType.TypeTime:
				case BaseType.TypeDateTime:
					s.Append(attribute.TypeDefinition.Default.Equals("current", StringComparison.InvariantCultureIgnoreCase) ? 
					         "SYSDATE" : 
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

		public override IScalarType ImportType(string typeName, IAttributeTypeDefinitionImported attrTypeDef)
		{
			if (String.IsNullOrEmpty(typeName))
				return base.ImportType(typeName, attrTypeDef);

			Metamodel.Type targetType = GenieLamp.Instance.Model.Types.GetByName("int", true);
			string srcTypeName = typeName.ToUpper();

			if (srcTypeName == "NUMBER" || srcTypeName == "DECIMAL")
			{
				targetType = GenieLamp.Instance.Model.Types.GetByName("decimal", true);
				if (attrTypeDef.Precision == 0)
				{
					if (attrTypeDef.Length <= 19)
					{
						if (attrTypeDef.Length <= 3)
							targetType = GenieLamp.Instance.Model.Types.GetByName("byte", true);
						else if (attrTypeDef.Length <= 5)
							targetType = GenieLamp.Instance.Model.Types.GetByName("shortint", true);
						else if (attrTypeDef.Length > 10)
							targetType = GenieLamp.Instance.Model.Types.GetByName("bigint", true);
						else
							targetType = GenieLamp.Instance.Model.Types.GetByName("int", true);
						attrTypeDef.Length = 0;
					}
				}
			}
			else if (srcTypeName == "CHAR" || srcTypeName == "VARCHAR" || srcTypeName == "VARCHAR2")
			{
				if (attrTypeDef.Length == 1)
				{
					targetType = GenieLamp.Instance.Model.Types.GetByName("ansichar", true);
					attrTypeDef.Length = 0;
				}
				else
					targetType = GenieLamp.Instance.Model.Types.GetByName("ansistring", true);
			}
			else if (srcTypeName == "NCHAR" || srcTypeName == "NVARCHAR" || srcTypeName == "NVARCHAR2")
			{
				if (attrTypeDef.Length == 1)
				{
					targetType = GenieLamp.Instance.Model.Types.GetByName("char", true);
					attrTypeDef.Length = 0;
				}
				else
					targetType = GenieLamp.Instance.Model.Types.GetByName("string", true);
			}
			else if (srcTypeName == "DATE")
			{
				targetType = GenieLamp.Instance.Model.Types.GetByName("datetime", true);
				attrTypeDef.Length = 0;
			}
			else if (srcTypeName == "BINARY_DOUBLE")
			{
				targetType = GenieLamp.Instance.Model.Types.GetByName("float", true);
				attrTypeDef.Length = 0;
			}
			else if (srcTypeName == "BLOB")
			{
				targetType = GenieLamp.Instance.Model.Types.GetByName("blob", true);
			}


			return targetType as ScalarType;
		}
	}
}

