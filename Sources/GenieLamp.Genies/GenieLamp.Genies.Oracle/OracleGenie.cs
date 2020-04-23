using System;
using System.Xml;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.Oracle
{
	public class OracleGenie : IGenieOfPersistence
	{
		private IGenieConfig config = null;
		private IModel model = null;
		private DbUpdater dbUpdate;

		#region Constructors
		public OracleGenie()
		{
			dbUpdate = new DbUpdater(this);
		}
		#endregion


		public ISpellHint FindHint(IMetaObject source)
		{
			return Model.SpellHints.Find(this.Config.Name, this.Config.TargetVersion, source);
		}

		#region IGenie implementation
		public void Init(IGenieConfig config)
		{
			this.config = config;
		}

		public void Spell(IModel model)
		{
			this.model = model;
			CodeGenDb genDb = new CodeGenDb(this);
			genDb.Run();
			dbUpdate.ScriptFileName = genDb.UpdateScriptFileName;
		}

		public IModel Model
		{
			get { return model; }
		}


		public string Name
		{
			get { return "Genie of Oracle database"; }
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

