using System;
using System.Xml;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.Template
{
	public class TemplateGenie
	{
		private IGenieConfig config = null;
		private IModel model = null;
		
		#region Constructors
		public TemplateGenie()
		{
		}
		#endregion

		public IModel Model
		{
			get { return model; }
		}

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
		}

		public string Name
		{
			get { return "Genie of Text templating"; }
		}

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

