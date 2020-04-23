using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class EntityConstraints : IEntityConstraints
	{
		private PrimaryId primaryId = null;
		private UniqueIdConstraints uniqueIds = null;
		private Entity entity;
		
		#region Constructors
		public EntityConstraints(Entity entity)
		{
			uniqueIds = new UniqueIdConstraints(entity);
		}

		public EntityConstraints(Entity entity, XmlNode entityNode)
		{
			this.entity = entity;
			
			if (entity.HasSupertype) {
				primaryId = new PrimaryId(entity, entity.Supertype.Constraints.PrimaryId);
			}
			else {
				XmlNodeList pidNodes = entity.Model.QueryNode(entityNode, "./{0}:PrimaryId");
				if (pidNodes.Count > 1)
					throw new GlException("Primary identifier is declared more than once. {0}", entity);
				else if (pidNodes.Count == 1) {
					primaryId = new PrimaryId(entity, pidNodes[0]);
				}
				else { // pidNodes.Count == 0
					primaryId = null;
					foreach(Attribute a in entity.Attributes) {
						if (a.PrimaryId) {
							if (primaryId == null)
								primaryId = new PrimaryId(entity, a);
							else
								primaryId.Attributes.Add(a);
						}
					}
				}
				
			}
			
			uniqueIds = new UniqueIdConstraints(entity, entityNode);
			foreach(Attribute a in entity.Attributes) {
				if (a.UniqueId) {
					UniqueId uc = new UniqueId(entity, entity.Name);
					uc.Attributes.Add(a);
					uniqueIds.Add(uc);
				}
			}
		}
		#endregion

		public void Update()
		{
			if (PrimaryId != null)
				PrimaryId.Update();
			UniqueIds.Update();
		}
		
		#region IConsistency implementation
		public void Check()
		{
			if (primaryId ==  null)
				throw new GlException("Primary identifier is not declared. {0}", entity);
			primaryId.Check();
			foreach(UniqueId uid in uniqueIds)
				uid.Check();
		}
		#endregion

		public Entity Entity {
			get { return entity as Entity; }
		}
		
		public PrimaryId PrimaryId {
			get { return primaryId; }
			internal set { primaryId = value; }
		}
		
		public UniqueIdConstraints UniqueIds {
			get { return uniqueIds; }
		}
		
		#region IEntityConstraints implementation
		IPrimaryId IEntityConstraints.PrimaryId {
			get { return primaryId; }
		}

		IUniqueIdConstraints IEntityConstraints.UniqueIds {
			get { return uniqueIds; }
		}
		#endregion
	}
}

