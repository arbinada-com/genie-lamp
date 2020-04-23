using System;
using System.Collections.Generic;
using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel.Physical
{
	class Indexes : MetaObjectCollection<IIndex, Index>, IIndexes
	{
		#region Constructors
		public Indexes(Model model)
			: base(model)
		{
		}
		#endregion
		
		#region Factory
		public Index CreateIndex(Entity entity)
		{
			Index index = new Index(entity);
			Add(index);
			return index;
		}
		
		public Index CreateIndex(ForeignKey foreignKey)
		{
			Index index = CreateIndex(foreignKey.ChildTable);
			index.InitBy(foreignKey);
			return index;
		}
		#endregion

		public void AddList(XmlNodeList indexNodes)
		{
			AddList(null, indexNodes);
		}

		public void AddList(Entity owner, XmlNodeList indexNodes)
		{
			foreach(XmlNode indexNode in indexNodes)
			{
				if (owner == null)
				{
					if (!Utils.Xml.IsAttrExists(indexNode, "entityName"))
						throw new GlException("Index must specify 'entityName' attribute when defined out of entity");
					QualName entityName = new QualName(indexNode, "entitySchema", "entityName", Model.DefaultSchema);
					//Utils.Xml.GetAttrValue(indexNode, "entityName", Const.EmptyName);
					owner = Model.Entities.GetByName(entityName.FullName);
					if (owner == null)
						throw new GlException("Index was specified for non existing entity '{0}'", entityName);
				}
				Index index = new Index(owner, indexNode);
				Add(index);
			}
		}
		
		public void Update()
		{
			foreach(Entity entity in Model.Entities)
			{
				entity.Macro.SetMacroCounter(Index.MacroCounter, 1);
			}
			foreach(Index i in this)
			{
				i.Update();
			}
		}

		public void Check()
		{
			Dictionary<string, Index> checkedIndexes = new Dictionary<string, Index>();
			foreach(Index i in this)
			{
				// Unique indexes are more prioritary if duplicated columns will be detected
				if (i.Generate && i.Unique)
					CheckIndex(i, checkedIndexes);
			}
			foreach(Index i in this)
			{
				if (i.Generate && !i.Unique)
					CheckIndex(i, checkedIndexes);
			}
		}

		private void CheckIndex(Index index, Dictionary<string, Index> checkedIndexes)
		{
			index.Check();
			Index index2;
			if (checkedIndexes.TryGetValue(index.FullName, out index2))
			{
				throw new GlException("Duplicate index name used\nIndex1: {0}\nIndex2: {1}", index, index2);
			}

			// Check mutual columns inclision
			foreach(Index index3 in checkedIndexes.Values)
			{
				bool inclusion = true;
//				?? full inclusion
//				bool inclusion = index3.Columns.Count == index.Columns.Count;
				if (inclusion)
				{
					int count = index3.Columns.Count;
					foreach(Attribute a in index.Columns.Attributes)
					{
						inclusion = inclusion && index3.Columns.Attributes.Contains(a);
						if (!inclusion || --count == 0)
							break;
					}
					if (inclusion)
					{
						index.Generate = false;
						Model.Logger.Warning(
							WarningLevel.Medium,
							"Index \"{0}\" is based on colum(s) already included in other index \"{1}\"\nIndex \"{0}\" is ignored and will not be created.",
							index.FullName, index3.FullName, index.FullName);
					}
				}
			}

			if (index.Generate)
				checkedIndexes.Add(index.FullName, index);
		}

		public IIndexes GetByEntity(IEntity entity)
		{
			Indexes indexList = new Indexes(this.Model);
			foreach(Index ix in this)
			{
				if (ix.Entity.Equals(entity))
				{
					indexList.Add(ix);
				}
			}
			return indexList;
		}

		#region IIndexes implementation
		public IIndex CreateIndex(IEntity owner)
		{
			return CreateIndex(Model.Entities.GetByName(owner.FullName, true, this));
		}

		IIndexes IIndexes.GetByEntity(IEntity entity)
		{
			return this.GetByEntity(entity);
		}
		#endregion


	}
}

