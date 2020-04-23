using System;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Metamodel.Physical
{
	class ForeignKeys : MetaObjectCollection<IForeignKey, ForeignKey>, IForeignKeys
	{
		#region Constructors
		public ForeignKeys(Model model)
			: base(model)
		{
		}
		#endregion
		
		#region Factory
		public ForeignKey CreateForeignKey(Relation owner)
		{
			ForeignKey fk = new ForeignKey(owner);
			Add(fk);
			return fk;
		}
		#endregion
		
		public void Update()
		{
			foreach(Entity en in Model.Entities)
			{
				en.Macro.SetMacroCounter(ForeignKey.MacroCounter, 1);
			}
			foreach(ForeignKey fk in this)
				fk.Update();
			CreateIndexes();
		}
		
		public void CreateIndexes()
		{
			foreach(ForeignKey fk in this)
			{
				if (!fk.HasIndex)
				{
					Index index = this.Model.PhysicalModel.Indexes.CreateIndex(fk);
					fk.Index = index;
					fk.Index.Generate = Model.Lamp.Config.Layers.PersistenceConfig.Params.ValueByName("ForeignKey.CreateIndex", true);
				}
			}
		}

		public void Check()
		{
			foreach(ForeignKey fk in this)
				fk.Check();
		}
	}
}

