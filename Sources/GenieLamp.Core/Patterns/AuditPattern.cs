using System;
using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Patterns
{
	class AuditPattern : PatternConfig, IAuditPattern
	{
		public const string NodeName = "Audit";
		private string attributesPrefix = "";

		#region Constructors
		public AuditPattern(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
			attributesPrefix = Params.ValueByName("AttributesPrefix", attributesPrefix);
			this.CreatedByAttributeName = attributesPrefix + "CreatedBy";
			this.CreatedDateAttributeName = attributesPrefix + "CreatedDate";
			this.LastModifiedByAttributeName = attributesPrefix + "LastModifiedBy";
			this.LastModifiedDateAttributeName = attributesPrefix + "LastModifiedDate";
		}
		#endregion

		public override void Implement()
		{
			foreach (Entity entity in Model.Entities)
			{
				if (this.AppliedTo(entity))
				{
					ScalarType actorType = (ScalarType)Model.Types.GetByName("string");
					actorType.TypeDefinition.Length = 64;
					actorType.TypeDefinition.Required = false;
					ScalarType actionDateType = (ScalarType)Model.Types.GetByName("datetime");
					actionDateType.TypeDefinition.Required = false;

					entity.AddAttribute(
						this.CreatedByAttributeName,
						actorType);
					entity.AddAttribute(
						this.CreatedDateAttributeName,
						actionDateType);
					entity.AddAttribute(
						this.LastModifiedByAttributeName,
						actorType);
					entity.AddAttribute(
						this.LastModifiedDateAttributeName,
						actionDateType);
				}
			}
		}

		#region IAuditPattern implementation
		public override bool AppliedTo(IEntity entity)
		{
			return !entity.HasSupertype && entity.Persistence.Persisted && base.AppliedTo(entity);
		}

		public string CreatedByAttributeName { get; private set; }

		public string CreatedDateAttributeName { get; private set; }

		public string LastModifiedByAttributeName { get; private set; }

		public string LastModifiedDateAttributeName { get; private set; }
		#endregion
	}
}

