using System;

namespace GenieLamp.Core.Metamodel
{
	class Schema : MetaObject, ISchema
	{
		#region Constructors
		public Schema(Model model, string name)
			: base(model, name)
		{
			this.Persistence = new Persistence(this);
			if (!String.IsNullOrEmpty(model.DefaultPersistentSchema))
			{
				this.Persistence.Name = model.DefaultPersistentSchema;
				this.Persistence.LockName();
			}
		}
		#endregion

		public Persistence Persistence { get; private set; }

		public void Update()
		{
			this.Persistence.UpdateNames("", Name);
		}

		#region IPersistent implementation
		IPersistence IPersistent.Persistence
		{
			get { return this.Persistence; }
		}
		#endregion


	}
}


