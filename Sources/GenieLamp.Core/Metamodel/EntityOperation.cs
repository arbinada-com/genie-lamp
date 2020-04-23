using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class EntityOperation : MetaObject, IEntityOperation
	{
		#region Constructors
		public EntityOperation(Entity owner, XmlNode node)
			: base(owner, node)
		{
			this.Access = ToEntityOperationAccess(Utils.Xml.GetAttrValue(node, "access"));
			this.Params = new EntityOperationParams(this, node);
		}
		#endregion

		public EntityOperationParams Params { get; private set; }
		public EntityOperationAccess Access { get; private set; }

		public static EntityOperationAccess ToEntityOperationAccess(string name)
		{
			switch(name.ToLower())
			{
			case "public":
				return EntityOperationAccess.Public;
			case "internal":
				return EntityOperationAccess.Internal;
			default:
				throw new GlException("Invalid EntityOperationAccess value '{0}'", name);
			}
		}

		public override string ToString()
		{
			return string.Format("Operation '{0}' (entity: {0})", Name, Owner.FullName);
		}

		public Entity Entity
		{
			get { return this.Owner as Entity; }
		}

		#region IEntityOperation implementation
		IEntity IEntityOperation.Entity
		{
			get { return this.Entity; }
		}

		IEntityOperationParams IEntityOperation.Params
		{
			get { return this.Params; }
		}
		#endregion
	}
}

