using System;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class EntityOperations : MetaObjectCollection<IEntityOperation, EntityOperation>, IEntityOperations
	{
		public Entity Entity { get; private set; }

		#region Constructors
		public EntityOperations(Entity owner)
			: base(owner.Model)
		{
			this.Entity = owner;
		}

		public EntityOperations(Entity owner, XmlNode entityNode)
			: this(owner)
		{
			foreach(XmlNode operationNode in owner.Model.QueryNode(entityNode, "./{0}:Operation"))
			{
				EntityOperation operation = new EntityOperation(owner, operationNode);
				this.Add(operation);
			}
		}
		#endregion
	}
}

