using System;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Metamodel.Physical
{
	class PhysicalModel : IPhysicalModel
	{
//		private Model model;
		private Indexes indexes;
		private ForeignKeys foreignKeys;
		
		#region Constructors
		public PhysicalModel(Model model)
		{
//			this.model = model;
			indexes = new Indexes(model);
			foreignKeys = new ForeignKeys(model);
		}
		#endregion
		
		public void Update()
		{
			foreignKeys.Update();
			indexes.Update();
		}

		public void Check()
		{
			foreignKeys.Check();
			indexes.Check();
		}

		public Indexes Indexes
		{
			get {return this.indexes; }
		}

		public ForeignKeys ForeignKeys
		{
			get { return foreignKeys; }
		}
		
		#region IPersistenceLayer implementation
		IIndexes IPhysicalModel.Indexes
		{
			get {return this.indexes; }
		}

		IForeignKeys IPhysicalModel.ForeignKeys
		{
			get { return foreignKeys; }
		}
		#endregion
	}
}

