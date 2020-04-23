using System;

namespace GenieLamp.Core.Metamodel
{
	class SubtypeRelation : Relation, ISubtypeRelation
	{
		public SubtypeRelation(Model model,
		                       Entity subtypeEntity)
			: base(model,
			       subtypeEntity.Supertype.Schema,
			       subtypeEntity.Supertype.Name,
			       Const.EmptyName,
			       false,
			       subtypeEntity.Schema,
			       subtypeEntity.Name,
			       Const.EmptyName,
			       false,
			       true,
			       Cardinality.R1_1,
			       new Persistence(subtypeEntity,
			                true,
			                subtypeEntity.Persistence.SchemaDefined,
			                subtypeEntity.Persistence.Schema,
			                false,
			                Const.EmptyName,
			                false,
			                Const.EmptyName
			                )
			       )
		{
			foreach (Attribute a in subtypeEntity.Constraints.PrimaryId.Attributes)
			{
				Attribute a2 = subtypeEntity.Supertype.Constraints.PrimaryId.Attributes.GetByName(a.Name);
				RelationAttributeMatch am = new RelationAttributeMatch(this, a, a2);
				this.AttributesMatch.Add(am);
			}
		}


		public override void Update()
		{
			base.Update();
			if (foreignKey.Index != null)
				foreignKey.Index.Generate = false;
		}
	}
}

