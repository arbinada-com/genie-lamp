using System;
using System.Collections.Generic;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class Entities : MetaObjectNamedCollection<IEntity, Entity>, IEntities
	{
		
		public Entities(Model model) : base(model)
		{
		}
		
		
		public void AddList(XmlNodeList list)
		{
			foreach(XmlNode node in list)
			{
				Entity entity = new Entity(Model, node);
				if (GetByName(entity.FullName) != null) {
					throw new GlException("Entity {0} was declared more that once", entity.FullName);
				}
				else
				{
					Add(entity);
				}
			}
		}
		
		public void Update()
		{
			int count = 1;
			foreach(Entity ent in this)
			{
				ent.Id = count++;
				ent.Update();
			}
		}

		public void Check()
		{
			Dictionary<string, Entity> tables = new Dictionary<string, Entity>();
			Dictionary<string, Entity> names = new Dictionary<string, Entity>();
			foreach(Entity ent in this)
			{
				ent.Check();
				Entity table;
				if (tables.TryGetValue(ent.Persistence.FullName, out table))
					throw new GlException("Entity \"{0}\" and \"{1}\" use the same persistent name \"{2}\"", 
					                      table.FullName,
					                      ent.FullName,
					                      ent.Persistence.FullName);
				tables.Add(ent.Persistence.FullName, ent);

				if (names.TryGetValue(ent.Name.ToLower(), out table))
					throw new GlException("Entity name \"{0}\" is used more than twice. Name reuse is not allowed for entities regardless the schema.\nEntity 1: {1}\nEntity 2: {2}",
					                      table.FullName,
					                      table.FullName,
					                      ent.FullName);
				names.Add(ent.Name.ToLower(), ent);
			}
		}
	}
}

