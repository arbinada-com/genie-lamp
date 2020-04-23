using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class DbSchemaImportGenie : IGenie
	{
		private IGenieConfig config = null;
		private IModel model = null;
		
		public DbSchemaImportGenie()
		{
		}

		public IModel Model
		{
			get { return model; }
		}

		public IGenieLamp Lamp
		{
			get { return model.Lamp; }
		}

		#region IGenie implementation
		public void Init(IGenieConfig config)
		{
			this.config = config;
		}

		public void Spell(IModel model)
		{
			this.model = model;
			(new SchemaImporter(this)).Run();
		}

		public IGenieConfig Config
		{
			get { return config; }
		}

		public string Name
		{
			get { return "Genie of database schema import"; }
		}

		public Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}
		#endregion
	}
}

