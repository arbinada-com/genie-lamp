using System;
using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;


namespace GenieLamp.Core.Patterns
{
	class PatternConfig : IPatternConfig
	{
		protected GenieLampConfig owner;
		private string name;
		private PatternConfigParams configParams;
		private MacroExpander macro;
		private PatternApplyMode applyToAllMode = PatternApplyMode.Include;
		private string schema = String.Empty;
		private string persistentSchema = String.Empty;

		public PatternApply PatternApply { get; private set; }
		public string PersistentSchema
		{
			get { return this.persistentSchema; }
			protected set { persistentSchema = value; }
		}
		public bool PersistentSchemaDefined { get; private set; }
		public string Schema
		{
			get { return this.schema; }
			protected set { schema = value;	}
		}

		#region Constructors
		public PatternConfig(GenieLampConfig owner, XmlNode node)
		{
			this.owner = owner;
			this.name = Utils.Xml.GetAttrValue(node, "name");
			this.OnPersistentLayer = bool.Parse(Utils.Xml.GetAttrValue(node, "onPersistentLayer"));

			macro = new MacroExpander(owner.Macro);
			this.PatternApply = new PatternApply(owner);
			configParams = new PatternConfigParams(this, owner.Lamp.QueryNode(node, "./{0}:Param"));

			this.schema = Params.ValueByName("Schema", this.schema);
			this.PersistentSchemaDefined = Params.ParamByName("PersistentSchema", false) != null;
			this.persistentSchema = Params.ValueByName("PersistentSchema", this.persistentSchema);

			AddPatternApplyItems(owner.Lamp.QueryNode(node, "./{0}:Exclude"), PatternApplyMode.Exclude);
			AddPatternApplyItems(owner.Lamp.QueryNode(node, "./{0}:Include"), PatternApplyMode.Include);
		}

		private void AddPatternApplyItems(XmlNodeList nodes, PatternApplyMode applyMode)
		{
			foreach (XmlNode applyItemNode in nodes)
			{
				PatternApplyItemType itemType = PatternApplyItemType.Entity;
				string name = Utils.Xml.GetAttrValue(applyItemNode, "entity", Const.EmptyName);
				if (name != Const.EmptyName)
					PatternApply.AddItem(itemType, name, applyMode);
			}
		}
		#endregion

		/// <summary>
		/// This method called by GL core when model file is loading
		/// after initialisation of types and before any other initialisations
		/// </summary>
		public virtual void Prepare()
		{ }

		/// <summary>
		/// This method called bu GL core after the model is completely loaded
		/// and all implicit metaobjects are created (i.e. relations)
		/// </summary>
		public virtual void Implement()
		{ }

		/// <summary>
		/// This method called by GL core when the model is updated but before call of Implement()
		/// </summary>
		public virtual void Update()
		{
			PatternApply.Update();
		}

		public GenieLampConfig GenieLampConfig
		{
			get { return owner; }
		}

		public Model Model
		{
			get { return owner.Lamp.Model; }
		}

		public MacroExpander Macro
		{
			get { return macro; }
		}

		#region IPatternConfig implementation
		public bool OnPersistentLayer { get ; private set; }

		public string Name
		{
			get { return this.name; }
		}

		public IParamsSimple Params
		{
			get { return configParams; }
		}

		public virtual bool AppliedTo(IEntity entity)
		{
			return
				(applyToAllMode == PatternApplyMode.Include &&
				 !this.PatternApply.IsExcluded(PatternApplyItemType.Entity, entity.FullName))
				|| (applyToAllMode == PatternApplyMode.Include &&
				 this.PatternApply.IsIncluded(PatternApplyItemType.Entity, entity.FullName));
		}

		public bool AppliedToEntityOrSupertypes(IEntity entity)
		{
			bool applied = AppliedTo(entity);
			if (entity.HasSupertype)
				return applied || AppliedTo(entity.Supertype);
			return applied;
		}
		#endregion
	}

}

