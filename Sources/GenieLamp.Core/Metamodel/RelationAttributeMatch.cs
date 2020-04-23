using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class RelationAttributeMatch : MetaObject, IRelationAttributeMatch
	{
		string attributeName = Const.EmptyName;
		string attributeName2 = Const.EmptyName;
		private Attribute attribute = null;
		private Attribute attribute2 = null;
		
		#region Constructors
		public RelationAttributeMatch(Relation owner, XmlNode node) : base(owner, node)
		{
			attributeName = Utils.Xml.GetAttrValue(node, "attribute", Const.EmptyName);
			attributeName2 = Utils.Xml.GetAttrValue(node, "attribute2", Const.EmptyName);
			SetAttributes();
		}
		
		internal RelationAttributeMatch(Relation owner, Attribute attribute, Attribute attribute2) : base(owner, attribute.Name)
		{
			attributeName = attribute.Name;
			this.attribute = attribute;
			attributeName2 = attribute2.Name;
			this.attribute2 = attribute2;
			SetAttributes();
		}

		private void SetAttributes()
		{
			// Establish attributes references
			attribute = UpdateAttribute(Relation.Entity, attributeName);
			attribute2 = UpdateAttribute(Relation.Entity2, attributeName2);
		}
		#endregion

		public void Update()
		{
			SetAttributes();
		}
		
		private Attribute UpdateAttribute(Entity attrEntity, string attrName)
		{
			if (attrEntity == null)
				throw new GlException("Cannot update attribute \"{0}\" entity is null. {1}", attrName, Relation);
			
			return attrEntity.Attributes.GetByName(attrName);
		}
		
		#region IConsistency implementation
		public void Check()
		{
			CheckAttribute(attribute, Relation.Entity, "", attributeName);
			CheckAttribute(attribute2, Relation.Entity2, "2", attributeName2);
		}
		
		private void CheckAttribute(Attribute attribute, Entity entity, string suffix, string attrName)
		{
			if (attribute == null)
				throw new GlException("Attribute{0} \"{1}\" not found. {2}. {3}", 
				                      suffix, attrName, entity, Relation);
			
			if (attribute.Type is EntityType)
				throw new GlException("Attribute{0} has an entity type. Entity typed attributes are not allowed in relation attributes matching. {1}. {2}. {3}",
				                      suffix, Relation, attribute, entity);
		}
		#endregion
		
		public override string ToString()
		{
			return String.Format("{0} = {1} ({2})", 
			                     Attribute == null ? attributeName : Attribute.Name, 
			                     Attribute2 == null ? attributeName2 : Attribute2.Name, 
			                     GetType().Name);
		}

		public Relation Relation
		{
			get { return base.Owner as Relation; }
		}

		public Attribute Attribute
		{
			get { return attribute; }
		}

		public Attribute Attribute2
		{
			get { return attribute2; }
		}
		
		#region IRelationAttributeMatch implementation
		IAttribute IRelationAttributeMatch.Attribute
		{
			get { return attribute; }
		}

		IAttribute IRelationAttributeMatch.Attribute2
		{
			get { return attribute2; }
		}
		#endregion


	}
}

