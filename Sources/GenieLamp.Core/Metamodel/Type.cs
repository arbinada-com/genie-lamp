using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	/// <summary>
	/// Type.
	/// </summary>
	class Type : MetaObjectSchemed, IType
	{
		public Type(Model model, XmlNode node) : base(model, node)
		{
		}
		
		public Type(Model model, string schema, string name) : base(model, schema, name)
		{
		}
		
		public Types Types
		{
			get { return Model.Types as Types; }
		}
	}
	
	/// <summary>
	/// Scalar type.
	/// </summary>
	abstract class ScalarType : Type, IScalarType
	{
		public const bool HasLengthProperty = true;
		public const bool HasPrecisionProperty = true;
		public const bool HasFixedProperty = true;
		public const bool HasRequiredProperty = true;
		public const bool HasDefaultProperty = true;
		protected BaseType baseType;
		protected TypeDefinition typeDefinition;
		
		public ScalarType(Model model, XmlNode node) : base(model, node)
		{ 
			typeDefinition = new TypeDefinition(node);
		}
		
		public ScalarType(Model model, 
		                  string schema, 
		                  string name, 
		                  BaseType baseType, 
		                  TypeDefinition typeDefinition)
			: base(model, schema, name)
		{
			this.baseType = baseType;
			this.typeDefinition = typeDefinition;
		}

		public TypeDefinition TypeDefinition
		{
			get { return typeDefinition; }
		}

		public decimal? MaxValue
		{
			get
			{
				if (TypeDefinition.Precision == 0)
				{
					switch (BaseType)
					{
					case BaseType.TypeInt:
						if (TypeDefinition.Length == 1)
							return byte.MaxValue;
						if (TypeDefinition.Length == 2)
							return Int16.MaxValue;
						if (TypeDefinition.Length == 8)
							return Int64.MaxValue;
						return Int32.MaxValue;
					case BaseType.TypeDecimal:
						return Convert.ToDecimal(Math.Pow(10, TypeDefinition.Length));
					}
				}
				return null;
			}
		}
		
		#region IScalarType implementation
		public BaseType BaseType
		{
			get { return baseType; }
		}

		public bool IsIntegerValueType
		{
			get
			{
				return TypeDefinition.Precision == 0  &&
					(BaseType == BaseType.TypeInt ||
					 BaseType == BaseType.TypeDecimal);
			}
		}

		ITypeDefinition IScalarType.TypeDefinition
		{
			get { return typeDefinition; }
		}
		#endregion
	}
	
	
	/// <summary>
	/// Std type.
	/// </summary>
	class StdType : ScalarType, IScalarType
	{
		public StdType(Model model, 
		               string schema, 
		               string name, 
		               BaseType baseType,
		               TypeDefinition typeDefinition)
			: base(model,
			       schema,
			       name,
			       baseType,
			       typeDefinition)
		{
			this.baseType = baseType;
		}
	}
	
	/// <summary>
	/// User type.
	/// </summary>
	class UserType : ScalarType, IScalarType
	{
		private StdType baseStdType;

		#region Constructors
		public UserType(Model model, XmlNode node) : base(model, node)
		{
			string baseTypeName = Utils.Xml.GetAttrValue(node, "baseType");
			Type t = Model.Types.GetByName(baseTypeName, false);
			if (t == null)
				throw new GlException("Base \"{0}\" of user type \"{1}\" is not declared", baseTypeName, FullName);
			if (!(t is StdType))
				throw new GlException("Base \"{0}\" of user type \"{1}\" should be a one of standard types. User types redefinition is not allowed",
				              baseTypeName, FullName);
			baseStdType = (t as StdType);
			baseType = baseStdType.BaseType;
			typeDefinition = new TypeDefinition(node, baseStdType.TypeDefinition);
		}
		#endregion

		public StdType BaseStdType
		{
			get { return baseStdType; }
		}
	}
	
	/// <summary>
	/// Entity type.
	/// </summary>
	class EntityType : Type, IEntityType
	{
		private Entity entity;
		
		public EntityType(Model model, Entity entity)
			: base(model, entity.Schema, entity.Name)
		{
			this.entity = entity;
		}
		
		public Entity Entity
		{
			get { return entity; }
		}
		
		#region IEntityType implementation
		IEntity IEntityType.Entity
		{
			get { return entity; }
		}
		#endregion
	}
	
	/// <summary>
	/// Enumeration type.
	/// </summary>
	class EnumerationType : Type, IEnumerationType
	{
		private Enumeration enumeration;

		public EnumerationType(Model model, Enumeration enumeration)
			: base(model, enumeration.Schema, enumeration.Name)
		{
			this.enumeration = enumeration;
		}
		
		#region IEnumerationType implementation
		public IEnumeration Enumeration
		{
			get { return enumeration; }
		}
		
		public int Length
		{
			get { return this.enumeration.Items.Count; }
		}
		#endregion






	}
	
	
}

