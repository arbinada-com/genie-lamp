using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.Sqlite
{
	public class SqliteGenie : IGenie
	{
		private IGenieConfig config = null;
		private IModel model = null;
		
		public SqliteGenie()
		{
		}

		public IModel Model {
			get { return model; }
		}

		#region IGenie implementation
		public void Init(IGenieConfig config)
		{
			this.config = config;
		}

		public void Spell(IModel model)
		{
			this.model = model;
			CodeGenDb gen = new CodeGenDb(this);
			gen.Run();

		}

		public string Name {
			get { return "Genie of Sqlite database"; }
		}

		public Version Version {
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public IGenieConfig Config {
			get { return config; }
		}
		#endregion
	}
}

