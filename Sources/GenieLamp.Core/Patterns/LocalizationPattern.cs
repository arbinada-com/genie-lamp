using System;

using System.Xml;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Patterns
{
	class LocalizationPattern : PatternConfig, ILocalizationPattern
	{
		public const string NodeName = "Localization";

		#region Constructors
		public LocalizationPattern(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
		}
		#endregion

		#region IAuditPattern implementation
		#endregion
	}
}

