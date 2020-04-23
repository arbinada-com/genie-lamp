using System;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class EntityOperationReturn : EntityOperationParam, IEntityOperationReturn
	{
		public const string ParamName = "Result";

		#region Constructors
		public EntityOperationReturn(EntityOperation owner)
			: base(owner)
		{
			this.Name = ParamName;
			this.Type = owner.Model.Types.GetByName("void", true, this);
		}

		public EntityOperationReturn(EntityOperation owner, XmlNode node)
			: base(owner, node)
		{
			this.Name = ParamName;
		}
		#endregion
	}
}

