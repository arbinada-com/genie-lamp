using System;
using System.Xml;
using System.Collections.Generic;

namespace GenieLamp.Core.Metamodel
{
	class Relations : MetaObjectCollection<IRelation, Relation>, IRelations
	{
		public Relations(Model model) : base(model)
		{ }
		
		public void AddList(XmlNodeList list)
		{
			foreach(XmlNode relNode in list)
			{
				Add(new Relation(Model, relNode));
			}
		}
		
		public void Update()
		{
			foreach(Entity en in Model.Entities) {
				int i = 1;
				foreach(Relation r in this) {
					switch (r.Cardinality) {
					case Cardinality.R1_1:
						if (r.Entity == en)
							r.Macro.SetMacroCounter(i++);
						break;
					case Cardinality.RM_M:
						break;
					case Cardinality.R1_M:
						if (r.Entity2 == en)
							r.Macro.SetMacroCounter(i++);
						break;
					case Cardinality.RM_1:
						if (r.Entity == en)
							r.Macro.SetMacroCounter(i++);
						break;
					}
				}
			}
			
			foreach(Relation r in this)
				r.Update();
		}
		
		public bool RelationExists(Entity entity, Entity entity2, string name, string name2)
		{
			foreach(Relation r in this) {
				if (r.Entity == entity && r.Entity2 == entity2 &&
				    (name == "*" || r.Name == name) && 
				    (name2 == "*" || r.Name2 == name2))
					return true;
			}
			return false;
		}
		
		
		public void CreateSubtypingRelations()
		{
			foreach (Entity entity in Model.Entities)
			{
				if (entity.HasSupertype)
				{
					SubtypeRelation r = new SubtypeRelation(Model, entity);
					Add(r);
				}
			}
		}


		public void CreateImplicitesRelations()
		{
			foreach (Entity entity in Model.Entities)
			{
				IEnumerator<Attribute> attrEnumerator = entity.Attributes.GetEnumerator();
				attrEnumerator.Reset();
				while (attrEnumerator.MoveNext())
				{
					Attribute attribute = attrEnumerator.Current;
					if (attribute.Type is EntityType)
					{
						Entity entity2 = (attribute.Type as EntityType).Entity;
						Relation r = new Relation(Model,
						                          entity.Schema,
						                          entity.Name,
						                          attribute.Name,
						                          true,
						                          entity2.Schema,
						                          entity2.Name,
						                          Const.EmptyName,
						                          false,
						                          attribute.TypeDefinition.Required,
						                          Cardinality.RM_1,
						                          new Persistence(attribute,
						                                          attribute.Persistence.Persisted,
						                                          attribute.Entity.Persistence.SchemaDefined,
						                                          attribute.Entity.Persistence.Schema,
						                                          attribute.Persistence.NameDefined, //false,
						                                          attribute.Persistence.Name, //Const.EmptyName,
						                                          false,
						                                          Const.EmptyName
						                                          )
						                          );

						entity.Attributes.Remove(attribute);
						foreach (Attribute pidAttr in entity2.Constraints.PrimaryId.Attributes)
						{
							Attribute ma = new Attribute(entity, pidAttr);
							ma.Name = attribute.Name;
							ma.TypeDefinition.Required = attribute.TypeDefinition.Required;
							ma.Migrate(pidAttr);
							if (attribute.Persistence.NameDefined)
							{
								ma.Persistence.Name = attribute.Persistence.Name;
								ma.Persistence.LockName();
							}
							entity.Attributes.Insert(attribute, ma);
							r.AttributesMatch.Add(
								new RelationAttributeMatch(r, ma, pidAttr));
						}
						this.Add(r);
						r.Update();
						
						attrEnumerator = entity.Attributes.GetEnumerator();
						attrEnumerator.Reset();
					}
				}
			}
		}

		
		#region IConsistency implementation
		public void Check()
		{
			foreach(Relation r in this)
				r.Check();
		}
		#endregion
		

	}
}

