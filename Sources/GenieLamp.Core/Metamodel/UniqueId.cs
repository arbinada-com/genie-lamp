using System;
using System.Xml;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Metamodel.Physical;

namespace GenieLamp.Core.Metamodel
{
	class UniqueId : EntityConstraint, IUniqueId, IConsistency
	{
		private Index index;
		
		#region Constructors
		public UniqueId(Entity owner, XmlNode node) : base(owner, node)
		{ 
			Init();
		}
		
		public UniqueId(Entity owner, string name) : base(owner, name)
		{
			Init();
		}
		
		private void Init()
		{
			this.index = Model.PhysicalModel.Indexes.CreateIndex(this.Entity);
		}
		#endregion

		public void Update()
		{
			Macro.SetMacroColumns(Attributes.ToPersistentNamesString("_"));
			string constraintTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName("Unique.ConstraintTemplate", Persistence.Name).Value;
			Persistence.UpdateNames(Const.EmptyName, Macro.Subst(constraintTemplate));
			index.InitBy(this);
			index.Generate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName(
						"Unique.CreateIndex", "true").AsBool;
		}
		

		public Physical.Index Index
		{
			get { return this.index; }
		}

		#region IConsistency implementation
		public override void Check()
		{
			base.Check();
			if (Attributes.Count == 0)
				throw new GlException("Unique identifier \"{0}\" has no attributes. {1}", FullName, Entity);
		}
		#endregion

		#region IUniqueIdConstraint implementation
		Physical.IIndex IUniqueId.Index
		{
			get { return this.index; }
		}
		#endregion








	}
}

