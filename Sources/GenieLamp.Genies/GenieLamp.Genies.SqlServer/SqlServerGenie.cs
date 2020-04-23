using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.SqlServer
{
	public class SqlServerGenie : IGenieOfPersistence
	{
		private IGenieConfig config = null;
		private IModel model = null;
		private DbUpdater dbUpdate;

		public SqlServerGenie()
		{
			dbUpdate = new DbUpdater(this);
		}

		public IModel Model
		{
			get { return model; }
		}

		public ISpellHint FindHint(IMetaObject source)
		{
			return Model.SpellHints.Find(this.Config.Name, this.Config.TargetVersion, source);
		}

		public ISpellHint FindHint(string targetType, string targetName)
		{
			return Model.SpellHints.Find(this.Config.Name, this.Config.TargetVersion, targetType, targetName);
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

			dbUpdate.ScriptFileName = gen.OutFileNameDDLUpdate;
		}

		public string Name
		{
			get { return "Genie of SqlServer database"; }
		}

		public Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public IGenieConfig Config
		{
			get { return config; }
		}

		public void UpdateDatabase()
		{
			dbUpdate.Run();
		}
		#endregion
	}
}

