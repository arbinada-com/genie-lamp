using System;

using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel.Physical
{
	class ForeignKey : MetaObject, IForeignKey
	{
		public const string MacroCounter = "%COUNTER_FK%";

		private Relation relation;
		private Entity childTable;
		private Attributes<ForeignKey> childTableColumns;
		private Entity parentTable;
		private Attributes<ForeignKey> parentTableColumns;
		private Index index;
	
		#region Constructors
		public ForeignKey(Relation owner)
			: base(owner)
		{
			this.relation = owner;
			this.name = relation.Persistence.Name;
			childTableColumns = new Attributes<ForeignKey>(this);
			parentTableColumns = new Attributes<ForeignKey>(this);

			SetReference();
		}
		#endregion

		public new Relation Owner
		{
			get { return base.Owner as Relation; }
		}
		
		private void SetReference()
		{
			childTable = relation.ChildEntity;
			childTableColumns.Copy<Relation>(relation.ChildAttributes);
			parentTable = relation.ParentEntity;
			parentTableColumns.Copy<Relation>(relation.ParentAttributes);
		}
		
		public void Update()
		{
			SetReference();
			Macro.SetMacroCounter(ChildTable.Macro.GetMacroCounter(ForeignKey.MacroCounter));
			Macro.SetMacro("%REF_TABLE%", ParentTable.Persistence.Name);
			Macro.SetMacro("%PARENT_TABLE%", ParentTable.Persistence.Name);
			Macro.SetMacro("%TABLE%", ChildTable.Persistence.Name);
			Macro.SetMacro("%CHILD_TABLE%", ChildTable.Persistence.Name);
			Macro.SetMacro("%COLUMNS_HASH%", this.ChildTableColumns.GetNamesHash(String.Empty).ToString());
			Macro.SetMacroColumns(ChildTableColumns.ToPersistentNamesString("_"));

			foreach (Attribute a in ChildTableColumns)
			{
				if (a.Persistence.NameDefined || a.Persistence.NameLocked)
					continue;
				string colNameTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName("ForeignKey.ColumnTemplate", a.Persistence.Name).Value;
				string attrPersistentNameBase = ChildTable.Macro.Subst(Macro.Subst(colNameTemplate));
				int i = 2;
				string attrPersistentName = attrPersistentNameBase;
				bool generatedColName = false;
				Attribute found = ChildTable.Attributes.GetByPersistentName(attrPersistentName);
				while (found != null && found != a)
				{
					attrPersistentName = String.Format("{0}{1}", attrPersistentNameBase, i++);
					generatedColName = true;
					found = ChildTable.Attributes.GetByPersistentName(attrPersistentName);
				}
				a.Persistence.UpdateNames(Const.EmptyName, attrPersistentName);
				if (generatedColName && relation.ChildNavigate)
					Logger.Warning(WarningLevel.Medium, 
					               "Persistent name \"{0}\" was generated. Specify persistent name to avoid it. Entity: {1}. Attribute: {2}", 
					               attrPersistentName, a.Entity.FullName, a.FullName);
			}

			string constraintTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ValueByName("ForeignKey.ConstraintTemplate", Name);
			if (!this.relation.Persistence.NameDefined)
			{
				this.Name =  this.Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.TruncateName(
					Macro.Subst(Macro.Subst(constraintTemplate)));
			}

			ChildTable.Macro.SetMacroCounter(ForeignKey.MacroCounter, Macro.GetMacroCounter() + 1);
		}

		public void Check()
		{
			if (childTableColumns.Count == 0)
				throw new GlException("Foreign key \"{0}\" has no columns. Owner: {1}",
				                      this.FullName, this.Owner);
		}

		public override string ToString()
		{
			return string.Format("{0}(Parent: {1}, child: {2})", base.ToString(), this.ParentTable.FullName, this.ChildTable.FullName);
		}

		public Entity ChildTable
		{
			get { return childTable; }
		}

		public Attributes<ForeignKey>ChildTableColumns
		{
			get { return childTableColumns; }
		}

		public Index Index
		{
			get { return this.index; }
			internal set { this.index = value; }
		}

		#region IForeignKey implementation
		IEntity IForeignKey.ChildTable
		{
			get { return childTable; }
		}

		IAttributes IForeignKey.ChildTableColumns
		{
			get { return childTableColumns; }
		}

		public IEntity ParentTable
		{
			get { return parentTable; }
		}

		public IAttributes ParentTableColumns
		{
			get { return parentTableColumns; }
		}

		public bool HasIndex
		{
			get { return this.index != null; }
		}

		IIndex IForeignKey.Index
		{
			get { return this.index; }
		}
		#endregion


	}
}

