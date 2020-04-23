using System;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class EntityEventHandlers : MetaObjectCollection<IEntityEventHandler, EntityEventHandler>, IEntityEventHandlers
	{
		public Entity Entity { get; private set; }

		#region Constructors
		public EntityEventHandlers(Entity owner)
			: base(owner.Model)
		{
			this.Entity = owner;
		}

		public EntityEventHandlers(Entity owner, XmlNode entityNode)
			: this(owner)
		{
			foreach(XmlNode handlerNode in Entity.Model.QueryNode(entityNode, "./{0}:EventHandler"))
			{
				EntityEventHandler handler = new EntityEventHandler(this.Entity, handlerNode);
				this.Add(handler);
			}
		}
		#endregion
	}
}

