using System;
using System.Text;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class RelationAttributes : Attributes<Relation>
	{
		public RelationAttributes(Relation owner) : base(owner)
		{
		}
	}
	
	class RelationAttributesMatch : 
		MetaObjectCollection<IRelationAttributeMatch, RelationAttributeMatch>, 
		IRelationAttributesMatch
	{
		private Relation owner;
		
		#region Constructor
		internal RelationAttributesMatch(Relation owner) : base(owner.Model)
		{ 
			this.owner = owner;
		}
		
		public RelationAttributesMatch(Relation owner, XmlNode node) : base(owner.Model)
		{
			this.owner = owner;
			XmlNodeList attrMatchList = Model.QueryNode(node, "./{0}:AttributeMatch");
			foreach (XmlNode attrMatchNode in attrMatchList)
			{
				Add(new RelationAttributeMatch(owner, attrMatchNode));
			}
		}
		#endregion
		
		public void Update()
		{
			foreach (RelationAttributeMatch attrMatch in this)
			{
				attrMatch.Update();
			}
		}
		
		private RelationAttributes MakeUniqueAttributesCollection(int originCollectionNumber)
		{
			RelationAttributes attributes = new RelationAttributes(this.owner);
			foreach (RelationAttributeMatch attrMatch in this)
			{
				Attribute a = originCollectionNumber == 1 ? attrMatch.Attribute : attrMatch.Attribute2;
				if (a != null && attributes.GetByName(a.Name) == null)
					attributes.Add(a);
			}
			return attributes;
		}

		public RelationAttributes Attributes
		{
			get { return MakeUniqueAttributesCollection(1); }
		}

		public RelationAttributes Attributes2
		{
			get { return MakeUniqueAttributesCollection(2); }
		}
		
		#region IRelationAttributesMatch implementation
		public bool ContainsAttribute(IAttribute attribute)
		{
			bool found = false;
			foreach (RelationAttributeMatch attrMatch in this)
			{
				found = (attrMatch.Attribute != null && attrMatch.Attribute.Equals(attribute)) ||
					(attrMatch.Attribute2 != null && attrMatch.Attribute2.Equals(attribute));
				if (found)
					break;
			}
			return found;
		}

		IAttributes IRelationAttributesMatch.Attributes
		{
			get { return MakeUniqueAttributesCollection(1); }
		}

		IAttributes IRelationAttributesMatch.Attributes2
		{
			get { return MakeUniqueAttributesCollection(2); }
		}
		#endregion






	}
}

