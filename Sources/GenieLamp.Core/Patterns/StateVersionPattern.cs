using System;
using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Patterns
{
	class StateVersionPattern : PatternConfig, IStateVersionPattern
	{
		public const string NodeName = "StateVersion";

		private string attributeName = "Version";
		private string typeName = "int";
		private string unsavedValue = Const.EmptyValue;
		private ScalarType type;

		#region Constructors
		public StateVersionPattern(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
			attributeName = Params.ValueByName("Attribute.Name", attributeName);
			typeName = Params.ValueByName("Attribute.Type", typeName);
			unsavedValue = Params.ValueByName("UnsavedValue", unsavedValue);
		}
		#endregion


		public string TypeName
		{
			get { return typeName; }
		}

		public ScalarType AttributeType
		{
			get { return this.type; }
		}

		public override void Update()
		{
			base.Update();
			type = owner.Lamp.Model.Types.GetByName(typeName, true) as ScalarType;
		}

		public override void Implement()
		{
			Model.Logger.TraceLine("Implementig state version pattern...");
			foreach(Entity entity in Model.Entities)
			{
				if (!AppliedTo(entity))
				     continue;

				Metamodel.Attribute a = entity.Attributes.GetByName(AtributeName, false);
				if (a != null && !a.Type.Equals(AttributeType))
				{
					throw new GlException("Entity \"{0}\" has attribute \"{1}\" but its type \"{2}\" is not the same that defined in pattern configuration",
					                      entity.FullName,
					                      a.Name,
					                      a.Type.FullName);
				}
				if (a == null)
				{
					a = new Metamodel.Attribute(entity, AtributeName, AttributeType);
					a.TypeDefinition.Required = true;
					entity.Attributes.Add(a);
				}
			}
		}

		#region IStateVersionPattern implementation
		public bool IsUsed(IAttribute attribute)
		{
			return attribute.Name == this.attributeName;
		}

		public string AtributeName
		{
			get { return attributeName; }
		}

		IScalarType IStateVersionPattern.AttributeType
		{
			get { return this.type; }
		}

		public override bool AppliedTo(IEntity entity)
		{
			return !entity.HasSupertype && entity.Persistence.Persisted && base.AppliedTo(entity);
		}

		public string UnsavedValue
		{
			get { return this.unsavedValue; }
		}
		#endregion


	}
}

