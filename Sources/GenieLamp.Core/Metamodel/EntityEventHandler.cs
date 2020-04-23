using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class EntityEventHandler : MetaObject, IEntityEventHandler
	{

		public EntityEventHandlerType Type { get; internal set; }

		#region Constructors
		public EntityEventHandler(Entity owner, XmlNode node)
			: base(owner)
		{
			this.Name = Utils.Xml.GetAttrValue(node, "type");
			this.Type = ToHandlerType(Utils.Xml.GetAttrValue(node, "type"));
		}
		#endregion

		public static EntityEventHandlerType ToHandlerType(string handlerTypeName)
		{
			switch(handlerTypeName.ToLower())
			{
			case "save":
				return EntityEventHandlerType.Save;
			case "delete":
				return EntityEventHandlerType.Delete;
			case "flush":
				return EntityEventHandlerType.Flush;
			case "validate":
				return EntityEventHandlerType.Validate;
			default:
				throw new GlException("Invalid handler type '{0}'", handlerTypeName);
			}
		}
	}
}

