using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class TypeDefinition : ITypeDefinition
	{
		private bool hasLength = false;
		private int length = 0;
		private bool hasPrecision = false;
		private int precision = 0;
		private bool hasFixed = false;
		private bool isFixed = false;
		private bool hasRequired = false;
		private bool isRequired = true;
		private bool hasDefault = false;
		private string defaultValue = Const.EmptyValue;
		private CollectionType collectionType = CollectionType.None;
		
		#region Constructors
		public TypeDefinition(XmlNode node, TypeDefinition basedOn)
		{
			hasLength = Utils.Xml.IsAttrExists(node, "length");
			length = Utils.Xml.GetAttrValue(node, "length", basedOn != null && basedOn.HasLength ? basedOn.Length : length);
			hasPrecision = Utils.Xml.IsAttrExists(node, "precision");
			precision = Utils.Xml.GetAttrValue(node, "precision", basedOn != null && basedOn.HasPrecision ? basedOn.Precision : precision);
			hasFixed = Utils.Xml.IsAttrExists(node, "fixed");
			isFixed = Utils.Xml.GetAttrValue(node, "fixed", basedOn != null && basedOn.HasFixed ? basedOn.Fixed : isFixed);
			hasRequired = Utils.Xml.IsAttrExists(node, "required");
			isRequired = Utils.Xml.GetAttrValue(node, "required", basedOn != null && basedOn.HasRequired ? basedOn.Required : isRequired);
			hasDefault = Utils.Xml.IsAttrExists(node, "default");
			defaultValue = Utils.Xml.GetAttrValue(node, "default", basedOn != null && basedOn.HasDefault ? basedOn.Default : defaultValue);
			collectionType = ToParamCollection(Utils.Xml.GetAttrValue(node, "collectionType"));
		}
		
		public TypeDefinition(XmlNode node) : this(node, null)
		{
		}
		
		public TypeDefinition(TypeDefinition source)
			:this(source.hasLength,
			      source.length,
			      source.hasPrecision,
			      source.precision,
			      source.hasFixed,
			      source.isFixed, 
			      source.hasRequired,
			      source.isRequired,
			      source.hasDefault,
			      source.defaultValue)
		{
		}

		public TypeDefinition(bool hasLength,
		                      int length,
		                      bool hasPrecision,
		                      int precision,
		                      bool hasFixed,
		                      bool isFixed, 
		                      bool hasRequired,
		                      bool isRequired,
		                      bool hasDefault,
		                      string defaultValue)
		{
			this.hasLength = hasLength;
			this.length = length;
			this.hasPrecision = hasPrecision;
			this.precision = precision;
			this.hasFixed = hasFixed;
			this.isFixed = isFixed;
			this.hasRequired = hasRequired;
			this.isRequired = isRequired;
			this.hasDefault = hasDefault;
			this.defaultValue = defaultValue;
		}

		internal TypeDefinition()
		{
		}
		#endregion
			
		public static CollectionType ToParamCollection(string name)
		{
			switch (name.ToLower())
			{
			case "none":
				return CollectionType.None;
			case "array":
				return CollectionType.Array;
			case "list":
				return CollectionType.List;
			default:
				throw new GlException("Invalid collection type '{0}'", name);
			}
		}


		#region ITypeDefinition implementation
		public bool HasLength
		{
			get { return this.hasLength; }
		}
		
		public int Length
		{
			get { return this.length; }
			internal set { this.length = value; }
		}
	
		public bool HasPrecision
		{
			get { return this.hasPrecision; }
		}
	
		public int Precision
		{
			get { return this.precision; }
		}
	
		public bool HasFixed
		{
			get { return hasFixed; }
		}
	
		public bool Fixed
		{
			get { return isFixed; }
		}
	
		public bool HasRequired
		{
			get { return hasRequired; }
		}

		public bool Required
		{
			get { return isRequired; }
			internal set { isRequired = value; }
		}

		public bool HasDefault
		{
			get { return hasDefault; }
		}

		public string Default
		{
			get { return defaultValue; }
		}

		public CollectionType CollectionType
		{
			get { return collectionType; }
			internal set {collectionType = value; }
		}
		#endregion
	}
}

