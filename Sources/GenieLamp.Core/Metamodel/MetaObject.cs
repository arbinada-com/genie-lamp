using System;
using System.Xml;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class MetaObject : IMetaObject
	{
		protected string name = Const.EmptyName;
		private Doc doc = null;
		private MacroExpander macro = null;
		private MetaObject owner;
		private Model model;
		private bool processed = false;
		private bool nameLocked = false;
		private bool definedInModel = false;

		
		#region Constructors
		public MetaObject(Model model)
		{
			if (model == null)
				throw new GlException("Provided model reference is null");
			SetModel(model);
			macro = new MacroExpander(model.Lamp.Macro);
			Init();
		}
		
		public MetaObject(MetaObject owner)
		{
			if (owner == null)
				throw new GlException("Provided owner reference is null");
			if (owner.Model == null)
				throw new GlException("Provided owner model reference is null");
			this.owner = owner;
			SetModel(owner.Model);
			this.name = Guid.NewGuid().ToString();
			macro = new MacroExpander(owner.Macro);
			Init();
		}
			
		public MetaObject(Model model, string name)
			: this(model)
		{
			this.name = QualName.ExtractName(name);
			Init();
		}
		
		public MetaObject(MetaObject owner, string name)
			: this(owner)
		{
			this.name = QualName.ExtractName(name);
			Init();
		}
		
		public MetaObject(MetaObject owner, XmlNode node)
			: this(owner, Const.EmptyName)
		{ 
			Init(node);
		}
		
		public MetaObject(Model model, XmlNode node)
			: this(model)
		{ 
			Init(node);
		}

		private void SetModel(Model model)
		{
			this.model = model;
			model.MetaObjects.Add(this);
		}

		private void Init()
		{
			this.SpellHintParams = new SpellHintParams(this);
		}
		
		private void Init(XmlNode node)
		{
			definedInModel = true;
			Init();
			this.name = QualName.ExtractName(Utils.Xml.GetAttrValue(node, "name", Const.EmptyName));
			XmlNodeList docNodes = Model.QueryNode(node, "./{0}:Doc");
			if (docNodes.Count != 0)
				doc = new Doc(this, docNodes[0]);
			this.SpellHintParams = new SpellHintParams(this, Model.QueryNode(node, "./{0}:SpellHintParam"));
		}
		#endregion

		public string Name
		{
			get { return name; }
			internal set 
			{ 
				if (!nameLocked)
					this.name = macro.Subst(value); 
			}
		}

		public void LockName()
		{
			nameLocked = true;
		}
		
		public void UnlockName()
		{
			nameLocked = false;
		}

		public string FullName
		{
			get { return GetFullName(); }
		}

		protected virtual string GetFullName()
		{
			return Name;
		}
		
		public override string ToString()
		{
			return String.Format("{0} ({1}, owner: {2})", GetFullName(), GetType().Name, owner == null ? "model" : owner.FullName);
		}

		public MetaObject Owner
		{
			get { return this.owner; }
		}
		
		public Model Model
		{
			get { return this.model; }
		}
		
		public MacroExpander Macro
		{
			get { return this.macro; }
		}
		
		public GenieLamp Lamp
		{
			get { return model.Lamp; }
		}
		
		public ILogger Logger
		{
			get { return model.Lamp.Logger; }
		}

		public SpellHintParams SpellHintParams { get; protected set; }

		public bool DefinedInModel
		{
			get { return definedInModel; }
		}

		
		#region IMetaObject implementation
		string IMetaObject.Name
		{
			get { return this.name; }
		}

		string IMetaObject.FullName
		{
			get { return GetFullName(); }
		}

		IMetaObject IMetaObject.Owner
		{
			get { return this.owner; }
		}
		
		IDoc IMetaObject.Doc
		{
			get { return this.doc; }
		}

		public bool Processed
		{
			get	{ return this.processed; }
			set	{ processed = value; }
		}

		public virtual bool Equals(MetaObject metaObject)
		{
			return this.FullName.Equals(metaObject.FullName, StringComparison.InvariantCultureIgnoreCase);
		}

		bool IMetaObject.Equals(IMetaObject metaObject)
		{
			return this.Equals(metaObject);
		}

		public bool HasDoc
		{
			get { return this.doc != null; }
		}

		IModel IMetaObject.Model
		{
			get { return this.Model; }
		}

		IParamsSimple IMetaObject.SpellHintParams
		{
			get { return this.SpellHintParams; }
		}
		#endregion
	}
}

