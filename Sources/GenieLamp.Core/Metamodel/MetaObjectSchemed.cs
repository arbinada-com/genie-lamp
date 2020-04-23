using System;
using System.Xml;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class MetaObjectSchemed : MetaObject, IMetaObjectSchemed
	{
		protected string schema = Const.EmptyName;
		
		#region Constructors
		public MetaObjectSchemed(Model model, XmlNode node) : base(model, node)
		{
			Init(Model, node);
		}
		
		public MetaObjectSchemed(MetaObject owner, XmlNode node) : base(owner, node)
		{
			Init(Model, node);
			if (owner is MetaObjectSchemed)
				this.schema = (owner as MetaObjectSchemed).Schema;
		}
		
		public MetaObjectSchemed(Model model, string schema, string name) : base(model, name)
		{
			this.schema = schema;
		}
		
		public MetaObjectSchemed(MetaObject owner, string schema, string name) : base(owner, name)
		{
			this.schema = schema;
		}
		
		private void Init(Model model, XmlNode node)
		{
			QualName qn = new QualName(node, Model.DefaultSchema);
			schema = qn.Schema;
			if (schema == Const.EmptyName)
				throw new GlException("Both schema attribute and model default schema are not defined. Element: {0}", this);
		}
		#endregion
		
		protected override string GetFullName()
		{
			return QualName.MakeFullName(schema, name);
		}
		
		#region IMetaObjectSchemed implementation
		public string Schema {
			get { return schema; }
		}
		#endregion


	}
}

