using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel.Physical
{
	class Index : MetaObjectSchemed, IIndex
	{
		public const string MacroCounter = "%COUNTER_INDEX%";
		public const string DefaultNameTemplate = "IX" + MacroExpander.MacroCounter + "_" + MacroExpander.MacroTable;
		
		private IndexColumns columns;
		private bool unique = false;
		private bool generate = true;

		#region Constructors
		internal Index(Entity owner, XmlNode indexNode)
			: base(owner, indexNode)
		{
			this.columns = new IndexColumns(this);
			if (Utils.Xml.IsAttrExists(indexNode, "name"))
			{
				this.name = Utils.Xml.GetAttrValue(indexNode, "name");
				LockName();
			}
			this.schema = owner.Persistence.Schema;
			this.unique = Utils.Xml.GetAttrValue(indexNode, "unique", this.unique);
			foreach(XmlNode node in indexNode.ChildNodes)
			{
				if (node.Name == "OnAttribute")
				{
					string attrName = Utils.Xml.GetAttrValue(node, "name");
					Attribute a = owner.Attributes.GetByName(attrName);
					if (a == null)
					{
						throw new GlException("Index attribute '{0}' not found. Entity: {1}", attrName, owner.FullName);
					}

					IndexColumn col = new IndexColumn(this)
					{
						Attribute = a,
						Order = IndexColumn.ParseOrder(Utils.Xml.GetAttrValue(node, "order"))
					};
					this.columns.Add(col);
				}
			}
		}


		public Index(Entity owner)
			: base(owner, owner.Persistence.Schema, Const.EmptyName)
		{
			this.columns = new IndexColumns(this);
		}
		#endregion
		
		private void CheckOwner(Entity sourceOwner)
		{
			if (!this.Owner.Equals(sourceOwner))
				throw new GlException("Error index initialisatoin. Source entity \"{0}\" is not the same. Owner: {1}",
				                      sourceOwner.FullName, this.Entity);
		}
		
		internal void InitBy(ForeignKey foreignKey)
		{
			CheckOwner(foreignKey.ChildTable);
			this.columns.CopyAttributes<ForeignKey>(foreignKey.ChildTableColumns);
			if (foreignKey.Owner.HasIndexName)
			{
				this.name = foreignKey.Owner.IndexName;
				this.LockName();
			}
			Update();
		}
		
		internal void InitBy(EntityConstraint constraint)
		{
			CheckOwner(constraint.Entity);
			this.unique = constraint is PrimaryId || constraint is UniqueId;
			this.columns.CopyAttributes<EntityConstraint>(constraint.Attributes);
			if (constraint.Persistence.NameDefined)
			{
				this.name = constraint.Persistence.Name;
				this.LockName();
			}
			Update();
		}
		
		public void Update()
		{
			Macro.SetMacroCounter(Entity.Macro.GetMacroCounter(Index.MacroCounter));
			Macro.SetMacroColumns(columns.Attributes.ToPersistentNamesString("_"));
			Macro.SetMacro("%COLUMNS_HASH%", columns.Attributes.GetNamesHash(this.Unique ? "U" : String.Empty).ToString());
			string nameTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ValueByName("Index.Template", DefaultNameTemplate);
		 	this.Name = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.TruncateName(
				Macro.Subst(nameTemplate));
			Entity.Macro.SetMacroCounter(Index.MacroCounter, Macro.GetMacroCounter() + 1);
		}

		public void Check()
		{
			if (columns.Count == 0)
				throw new GlException("Index \"{0}\" has no columns. Owner: {1}",
				                      this.FullName, this.Entity);
		}
		
		public Entity Entity
		{
			get	{ return this.Owner as Entity; }
		}

		public override string ToString()
		{
			return string.Format("{0}(Unique: {1}, Generate: {2}, Column(s): {3})",
			                     base.ToString(), Unique, Generate, Columns.Attributes.ToPersistentNamesString(","));
		}


		public IndexColumns Columns
		{
			get	{ return this.columns; }
		}

		#region IIndex implementation
		IEntity IIndex.Entity
		{
			get	{ return this.Entity; }
		}

		IIndexColumns IIndex.Columns
		{
			get	{ return this.columns; }
//			set
//			{
//				this.columns.Clear();
//				foreach(IAttribute ia in value)
//				{
//					Attribute a = Entity.Attributes.GetByName(ia.Name, true);
//					columns.Add(a);
//				}
//				Update();
//			}
		}

		public bool Unique
		{
			get	{ return this.unique; }
			set { this.unique = value; }
		}

		public bool Generate {
			get { return this.generate; }
			internal set { generate = value; }
		}
		#endregion


	}
}

