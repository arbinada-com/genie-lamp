using System;
using System.Xml;

using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class EntityOperationParam : MetaObject, IEntityOperationParam
	{
		private QualName typeName;

		#region Constructors
		public EntityOperationParam(EntityOperation owner)
			: base(owner)
		{
			this.Type = Model.Types.FindType(new QualName("void", ""));
			this.IsRaw = false;
			this.IsRef = false;
			this.IsOut = false;
			this.TypeDefinition = new TypeDefinition() { Required = true };
		}

		public EntityOperationParam(EntityOperation owner, XmlNode node)
			: base(owner, node)
		{
			typeName = new QualName(Utils.Xml.GetAttrValue(node, "type"), owner.Model.DefaultSchema);
			this.Type = Model.Types.FindType(typeName);
			if (this.Type == null)
				throw new GlException("Type \"{0}\" not found. Entity: {1}. Operation: {2}",
				                      typeName, owner.Owner.FullName, owner.Name);
			this.IsRaw = bool.Parse(Utils.Xml.GetAttrValue(node, "raw"));
			this.IsRef = bool.Parse(Utils.Xml.GetAttrValue(node, "ref"));
			this.IsOut = bool.Parse(Utils.Xml.GetAttrValue(node, "out"));

			this.TypeDefinition = new TypeDefinition(node);
			if (!this.TypeDefinition.HasRequired)
				this.TypeDefinition.Required = true;
		}
		#endregion

		public Metamodel.Type Type { get; internal set; }
		public TypeDefinition TypeDefinition { get; private set; }
		public EntityOperation Operation
		{
			get { return this.Owner as EntityOperation; }
		}


		#region IEntityOperationParam implementation
		public bool IsRaw { get; internal set; }
		public bool IsRef { get; internal set; }
		public bool IsOut { get; internal set; }

		IType IEntityOperationParam.Type
		{
			get { return this.Type; }
		}

		ITypeDefinition IEntityOperationParam.TypeDefinition
		{
			get { return this.TypeDefinition; }
		}

		public override string ToString()
		{
			return string.Format("EntityOperationParam (Name: {0}, operation: {1}, entity: {2})", Name, Operation.Name, Operation.Entity.FullName);
		}
		#endregion
	}
}

