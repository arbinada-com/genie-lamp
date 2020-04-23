using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class EntityAttributes : Attributes<Entity>, IEntityAttributes
	{
		#region Constructors
		public EntityAttributes(Entity owner)
			: base(owner)
		{ }

		public EntityAttributes(Entity owner, XmlNode node)
			: base(owner)
		{ 
			MigrateSupertypeAttributes();

			XmlNodeList attrNodeList = Model.QueryNode(node, ".//{0}:Attribute");
			foreach (XmlNode attrNode in attrNodeList)
			{
				Attribute a = new Attribute(Owner, attrNode);
				Attribute declared = GetByName(a.Name);
				if (declared != null)
				{
					if (declared.IsMigrated)
						throw new GlException("Attribute \"{0}\" already declared in supertype. {1}", a.Name, Owner);
					else
						throw new GlException("Duplicate attribute \"{0}\". {1}", a.Name, Owner);
				}
				Add(a);
			}
			if (Count == 0 && !Owner.HasSupertype)
				throw new GlException("Entity \"{0}\" has no attributes", Owner);
		}
		#endregion
		
		private void MigrateSupertypeAttributes()
		{
			if (Owner.HasSupertype)
			{
				// Migrate supertype id attributes
				if (Owner.Supertype.Constraints.PrimaryId == null)
					throw new GlException("Cannot implement supertyping. Supertype entity \"{0}\" has no primary id. {1}", 
					              Owner.Supertype.FullName, Owner);
				foreach (Attribute a in Owner.Supertype.Constraints.PrimaryId.Attributes)
				{
					Attribute migrated = new Attribute(Owner, a);
					migrated.Name = a.Name;
					migrated.PrimaryId = true;
					migrated.Migrate(a);
					this.Add(migrated);
				}
			}
		}

		#region IEntityAttributes implementation
		public IEntity Entity
		{
			get { return this.Owner; }
		}
		#endregion






	}
}

