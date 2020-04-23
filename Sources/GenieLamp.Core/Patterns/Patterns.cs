using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Patterns
{
	class Patterns : GlNamedCollection<IPatternConfig, PatternConfig>, IPatterns
	{
		public RegistryPattern Registry { get; private set; }
		public StateVersionPattern StateVersion { get; private set; }
		public AuditPattern Audit { get; private set; }
		public LocalizationPattern Localization { get; private set; }
		public SecurityPattern Security { get; private set; }

		#region Constructors
		public Patterns(GenieLampConfig owner, XmlNode node)
			: base()
		{
			XmlNodeList nodeList = owner.Lamp.QueryNode(node, "./{0}:Pattern");
			foreach (XmlNode patternNode in nodeList)
			{
				PatternConfig pattern;
				string patternName = Utils.Xml.GetAttrValue(patternNode, "name");
				if (patternName.Equals(RegistryPattern.NodeName, StringComparison.InvariantCultureIgnoreCase))
				{
					pattern = this.Registry = new RegistryPattern(owner, patternNode);
				}
				else if(patternName.Equals(StateVersionPattern.NodeName, StringComparison.InvariantCultureIgnoreCase))
				{
					pattern = this.StateVersion = new StateVersionPattern(owner, patternNode);
				}
				else if(patternName.Equals(AuditPattern.NodeName, StringComparison.InvariantCultureIgnoreCase))
				{
					pattern = this.Audit = new AuditPattern(owner, patternNode);
				}
				else if (patternName.Equals(LocalizationPattern.NodeName, StringComparison.InvariantCultureIgnoreCase))
				{
					pattern = this.Localization = new LocalizationPattern(owner, patternNode);
				}
				else if (patternName.Equals(SecurityPattern.NodeName, StringComparison.InvariantCultureIgnoreCase))
				{
					pattern = this.Security = new SecurityPattern(owner, patternNode);
				}
				else
				{
					pattern = new PatternConfig(owner, patternNode);
				}
				this.Add(pattern.Name, pattern);
			}
		}
		#endregion

		public void Prepare()
		{
			foreach(PatternConfig pattern in this)
			{
				pattern.Prepare();
			}
		}

		public void Update()
		{
			foreach(PatternConfig pattern in this)
			{
				pattern.Update();
			}
		}

		public void Implement()
		{
			foreach(PatternConfig pattern in this)
			{
				pattern.Implement();
			}
		}

		#region IPatternsConfig implementation
		IStateVersionPattern IPatterns.StateVersion
		{
			get { return this.StateVersion; }
		}

		IRegistryPattern IPatterns.Registry
		{
			get { return this.Registry; }
		}

		IAuditPattern IPatterns.Audit
		{
			get { return this.Audit; }
		}

		ILocalizationPattern IPatterns.Localization
		{
			get { return this.Localization; }
		}

		ISecurityPattern IPatterns.Security
		{
			get { return this.Security; }
		}
		#endregion


	}
}

