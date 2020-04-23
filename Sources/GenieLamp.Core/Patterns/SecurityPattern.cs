using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Patterns
{
	class SecurityPattern : PatternConfig, ISecurityPattern
	{
		public const string NodeName = "Security";

		#region Constructors
		public SecurityPattern(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
		}
		#endregion
	}
}

