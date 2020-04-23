using System;
using System.Collections.Generic;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class Schemas : MetaObjectNamedCollection<ISchema, Schema>, ISchemas
	{
		public Schemas(Model model)
			: base(model)
		{
		}

		public void Update()
		{
			foreach(Entity ent in Model.Entities)
			{
				Schema schema = this.GetByName(ent.Schema, false);
				if (schema == null)
				{
					schema = new Schema(Model, ent.Schema);
					this.Add(schema.Name, schema);
				}
				schema.Update();
			}
		}
	}
}

