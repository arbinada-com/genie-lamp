using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class EntityConstraint : MetaObject, IEntityConstraint, IConsistency
	{
		private EntityConstraintAttributes attributes;
		private Persistence persistence;

		#region Constructor
		public EntityConstraint(Entity owner, XmlNode constraintNode) 
			: base(owner, constraintNode)
		{
			persistence = new Persistence(this, constraintNode);
			attributes = new EntityConstraintAttributes(this);
			AddAttributes(Model.QueryNode(constraintNode, "./{0}:OnAttribute"));
			AddAttributes(Model.QueryNode(constraintNode, "./{0}:Attribute"));
			if (this.Attributes.Count == 0)
				throw new GlException("Constraint definition is incomplete: no attributes found. {0}. Constraint: {1}. {2}", 
				                      Name, GetType().Name, Entity);
		}
		
		public EntityConstraint(Entity entity, string name)
			: base(entity, name)
		{ 
			persistence = new Persistence(this,
			                              entity.Persistence.Persisted,
			                              entity.Persistence.SchemaDefined,
			                              entity.Persistence.Schema,
			                              false,
			                              name,
			                              false,
			                              Const.EmptyName
			                              );
			attributes = new EntityConstraintAttributes(this);
		}
		#endregion
		
		private void AddAttributes(XmlNodeList attrNodes)
		{
			foreach (XmlNode node in attrNodes) {
				string attrName = Utils.Xml.GetAttrValue(node, "name");
				Attribute a = Entity.Attributes.GetByName(attrName);
				if (a == null)
					throw new GlException("Constraint attribute \"{0}\" not found. {1}. {2}", attrName, this, Entity);
				attributes.Add(a);
			}
		}
		
		public Entity Entity {
			get { return Owner as Entity; }
		}
		
		public EntityConstraintAttributes Attributes {
			get { return attributes; }
		}
		
		public Persistence Persistence {
			get { return this.persistence; }
		}

		#region IConsistency implementation
		public virtual void Check()
		{
			foreach(Attribute a in this.Attributes)
			{
				if (!(a.Type is ScalarType || a.Type is IEnumerationType))
				    throw new GlException("Entity constraint should be based on scalar attributes only. Attribute: {0}. Relation: {1}",
					                      a, this.ToString());
			}
		}
		#endregion

		#region IEntityConstraint implementation
		IAttributes IEntityConstraint.Attributes {
			get { return this.attributes; }
		}

		
		IPersistence IPersistent.Persistence {
			get { return this.persistence; }
		}

		IEntity IEntityConstraint.Entity {
			get { return this.Owner as IEntity; }
		}
		
		public bool Composite {
			get { return attributes.Count > 1; }
		}

		public bool ContainsAttribute(IAttribute attribute)
		{
			return attributes.GetByName(attribute.Name) != null;
		}

		public bool MatchAttributes(IAttributes attributes)
		{
			int matchCount = 0;
			foreach(IAttribute attribute in attributes) {
				if (ContainsAttribute(attribute))
					matchCount++;
			}
			return matchCount == attributes.Count;
		}
		#endregion


	}
}

