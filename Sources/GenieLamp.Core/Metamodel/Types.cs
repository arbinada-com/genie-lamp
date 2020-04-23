using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class Types : MetaObjectNamedCollection<IType, Metamodel.Type>, ITypes
	{
		#region Constructors
		public Types(Model model) :  base(model)
		{
			AddStandardTypes();
		}
		#endregion

		public static string ToTypeName(BaseType baseType)
		{
			string name = Enum.GetName(typeof(BaseType), baseType);
			return name.Substring(4).ToLower();
		}
		
		private void AddStandardTypes()
		{
			AddType(new StdType(Model, Const.EmptyName, "void", BaseType.TypeVoid,
			                    new TypeDefinition(
								!ScalarType.HasLengthProperty, 1,
			                   	!ScalarType.HasPrecisionProperty, 0,
			                   	!ScalarType.HasFixedProperty, true,
			                   	!ScalarType.HasRequiredProperty, true,
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "byte", BaseType.TypeByte,
			                    new TypeDefinition(
								ScalarType.HasLengthProperty, 1,
			                   	!ScalarType.HasPrecisionProperty, 0,
			                   	!ScalarType.HasFixedProperty, true,
			                   	!ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "tinyint", BaseType.TypeByte,
			                    new TypeDefinition(
								ScalarType.HasLengthProperty, 1,
			                   	!ScalarType.HasPrecisionProperty, 0,
			                   	!ScalarType.HasFixedProperty, true,
			                   	!ScalarType.HasRequiredProperty, false,
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "currency", BaseType.TypeCurrency,
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 18, 
			                    ScalarType.HasPrecisionProperty, 4,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "decimal", BaseType.TypeDecimal, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 28, 
			                    ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "float", BaseType.TypeFloat, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 28, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "int", BaseType.TypeInt, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 4, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "integer", BaseType.TypeInt, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 4, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "shortint", BaseType.TypeInt, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 2, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "smallint", BaseType.TypeInt,
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 2, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "bigint", BaseType.TypeInt,
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 8, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			
			AddType(new StdType(Model, Const.EmptyName, "date", BaseType.TypeDate, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 4, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "datetime", BaseType.TypeDateTime, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 8, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "time", BaseType.TypeTime, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 4, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			
			AddType(new StdType(Model, Const.EmptyName, "string", BaseType.TypeString, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 255, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "ansistring", BaseType.TypeAnsiString, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 255, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "char", BaseType.TypeChar, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 1, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "ansichar", BaseType.TypeAnsiChar, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 1, 
			                    !ScalarType.HasPrecisionProperty, 0,
			                    ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "varchar", BaseType.TypeAnsiString, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 255,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "nvarchar", BaseType.TypeString, 
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 255,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "text", BaseType.TypeAnsiString, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, int.MaxValue,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "ntext", BaseType.TypeString, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, int.MaxValue,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			
			AddType(new StdType(Model, Const.EmptyName, "boolean", BaseType.TypeBool, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 1,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "bool", BaseType.TypeBool,
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 1,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false,
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "binary", BaseType.TypeBinary,
			                    new TypeDefinition(
			                    ScalarType.HasLengthProperty, 255,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "blob", BaseType.TypeBinary, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, int.MaxValue,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, false,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "uuid", BaseType.TypeUuid, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 16,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
			AddType(new StdType(Model, Const.EmptyName, "guid", BaseType.TypeUuid, 
			                    new TypeDefinition(
			                    !ScalarType.HasLengthProperty, 16,
			                    !ScalarType.HasPrecisionProperty, 0,
			                    !ScalarType.HasFixedProperty, true,
			                    !ScalarType.HasRequiredProperty, false, 
			                    !ScalarType.HasDefaultProperty, Const.EmptyValue)));
		}
		
		private void AddType(Type t)
		{
			if (GetByName(t) != null) {
				throw new GlException("Type {0} is already declared", t);
			}
			else {
				Add(t);
			}
		}

		public void AddList(XmlNodeList list)
		{
			foreach (XmlNode node in list) {
				AddType(new UserType(Model, node));
			}
		}
		
		public EntityType AddEntityType(Entity entity)
		{
			EntityType t = new EntityType(Model, entity);
			AddType(t);
			return t;
		}
		
		public EnumerationType AddEnumerationType(Enumeration enumeration)
		{
			EnumerationType t = new EnumerationType(Model, enumeration);
			AddType(t);
			return t;
		}
		
		public Type FindType(QualName typeName)
		{
			Type t = Model.Types.GetByName(typeName.FullName);
			if (t == null) {
				t = Model.Types.GetByName(QualName.MakeFullName(Const.EmptyName, typeName.Name));
			}
			if (t == null) {
				// Try to find by name
				int typesCount = 0;
				foreach(Type t2 in this) {
					if (t2.Name.Equals(typeName.Name)) {
						t = t2;
						typesCount++;
					}
				}
				if (typesCount > 1)
					throw new GlException("Ambigous type name \"{0}\". You should specify schema", typeName.Name);
			}
			return t;
		}
		
	}
}

