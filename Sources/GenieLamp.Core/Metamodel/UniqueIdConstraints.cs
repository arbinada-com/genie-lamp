using System;
using System.Xml;
using System.Collections.Generic;


namespace GenieLamp.Core.Metamodel
{
	class UniqueIdConstraints : 
		MetaObjectCollection<IUniqueId, UniqueId>,
		IEnumerable<IUniqueId>,
		IUniqueIdConstraints
	{
		private Entity entity;

		#region Constructors
		public UniqueIdConstraints(Entity entity)
			: base(entity.Model)
		{
			this.entity = entity;
		}

		public UniqueIdConstraints(Entity entity, XmlNode node)
			: base(entity.Model)
		{
			this.entity = entity;
			XmlNodeList uniqueIdNodes = entity.Model.QueryNode(node, "./{0}:UniqueId");
			foreach(XmlNode uniqueIdNode in uniqueIdNodes) {
				Add(new UniqueId(entity, uniqueIdNode));
			}
		}
		#endregion

		public void Update()
		{
			int i = 1;
			foreach(UniqueId uid in this) {
				uid.Macro.SetMacroCounter(i++);
				uid.Update();
			}
		}
		
		public Entity Entity {
			get { return this.entity; }
		}
	}
}

