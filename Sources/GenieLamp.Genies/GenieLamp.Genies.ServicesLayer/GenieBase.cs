using System;
using System.Xml;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.ServicesLayer
{
	public abstract class GenieBase : IGenie
	{
		protected IGenieConfig config = null;
		protected IModel model = null;

		public GenieBase()
		{
		}

		public IModel Model
		{
			get { return model; }
		}

		public IGenieLamp Lamp
		{
			get { return Model.Lamp; }
		}

		#region IGenie implementation
		public virtual void Init(IGenieConfig config)
		{
			this.config = config;
		}

		public virtual void Spell(IModel model)
		{
			this.model = model;
		}

		public abstract string Name { get; }

		public Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public IGenieConfig Config
		{
			get { return config; }
		}
		#endregion

	}
}

