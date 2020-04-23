using System;
using System.Collections.Generic;

namespace GenieLamp.Core.Metamodel.Physical
{
	/// <summary>
	/// Physical model contains objects that cannot be mapped directly from logical model
	/// and/or need to be specified with parameters of target platforms
	/// </summary>
	public interface IPhysicalModel
	{
		IIndexes Indexes { get; }
		IForeignKeys ForeignKeys { get; }
	}

	public interface IIndexes :  IMetaObjectCollection<IIndex>
	{
		IIndex CreateIndex(IEntity owner);
		IIndexes GetByEntity(IEntity entity);
	}

	public interface IForeignKeys :  IMetaObjectCollection<IForeignKey>
	{
	}

	public interface IForeignKey : IMetaObject
	{
		IEntity ChildTable { get; }
		IAttributes ChildTableColumns { get; }
		IEntity ParentTable { get; }
		IAttributes ParentTableColumns { get; }
		bool HasIndex { get; }
		IIndex Index { get; }
	}

	public enum ColumnOrder
	{
		Asc,
		Desc
	}

	public interface IIndexColumn
	{
		IAttribute Attribute { get; }
		ColumnOrder Order { get; }
	}

	public interface IIndexColumns : IList<IIndexColumn>
	{
		IAttributes Attributes { get; }
	}

	public interface IIndex : IMetaObjectSchemed
	{
		IEntity Entity { get; }
		IIndexColumns Columns { get; }
		bool Unique { get; set; }
		bool Generate { get; }
	}
}

