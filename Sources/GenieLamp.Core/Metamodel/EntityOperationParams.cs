using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class EntityOperationParams : MetaObjectCollection<IEntityOperationParam, EntityOperationParam>, IEntityOperationParams
	{
		public EntityOperation Operation { get; private set; }
		public EntityOperationReturn Returns { get; private set; }

		#region Constructors
		public EntityOperationParams(EntityOperation owner)
			: base(owner.Model)
		{
			this.Operation = owner;
			this.Returns = new EntityOperationReturn(owner);
		}

		public EntityOperationParams(EntityOperation owner, XmlNode operationNode)
			: this(owner)
		{
			foreach(XmlNode paramNode in owner.Model.QueryNode(operationNode, "./{0}:Param"))
			{
				EntityOperationParam param = new EntityOperationParam(owner, paramNode);
				this.Add(param);
			}

			this.Returns = new EntityOperationReturn(owner);
			XmlNodeList returnsNodes = owner.Model.QueryNode(operationNode, "./{0}:Returns");
			if (returnsNodes.Count > 1)
				throw new GlException("{0} has more than one returns", owner.ToString());
			if (returnsNodes.Count == 1)
				this.Returns = new EntityOperationReturn(owner, returnsNodes[0]);
		}
		#endregion

		#region IEntityOperationParams implementation
		public bool HasReturns
		{
			get
			{ return this.Returns != null &&
				this.Returns.Type != null &&
				(this.Returns.Type as ScalarType).BaseType != BaseType.TypeVoid;
			}
		}

		IEntityOperationReturn IEntityOperationParams.Returns
		{
			get { return this.Returns; }
		}
		#endregion
	}
}

