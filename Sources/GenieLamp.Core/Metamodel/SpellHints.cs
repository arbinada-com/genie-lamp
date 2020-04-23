using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class SpellHints : MetaObjectCollection<ISpellHint, SpellHint>, ISpellHints
	{
		private Dictionary<string, SpellHint> index = new Dictionary<string, SpellHint>();

		#region Constructors
		public SpellHints(Model model)
			: base(model)
		{
		}
		#endregion

		private void Reindex()
		{
			index.Clear();
			foreach(SpellHint hint in this)
			{
				if (index.ContainsKey(hint.FullName))
					throw new GlException("Duplicate detected {0}", hint.ToString());
				else
					index.Add(hint.FullTargetName, hint);
			}
		}

		public override void Add(SpellHint o)
		{
			base.Add(o);
			Reindex();
		}

		public void AddList(XmlNodeList list)
		{
			foreach(XmlNode spellHintNode in list)
			{
				base.Add(new SpellHint(Model, spellHintNode));
			}
			Reindex();
		}

		public SpellHint Find(string genieName, string targetVersion, IMetaObject source)
		{
			return this.Find(genieName, targetVersion, source.GetType().Name, source.FullName);
		}

		public SpellHint Find(string genieName, string targetVersion, string targetType, string targetName)
		{
			System.Diagnostics.Contracts.Contract.Requires(!String.IsNullOrEmpty(genieName));
			System.Diagnostics.Contracts.Contract.Requires(!String.IsNullOrEmpty(targetVersion));
			System.Diagnostics.Contracts.Contract.Requires(!String.IsNullOrEmpty(targetType));
			System.Diagnostics.Contracts.Contract.Requires(!String.IsNullOrEmpty(targetName));

			SpellHint hint = null;
			// Find suitable hit by degrading target name (both name and schema) to wildcards
			// and degrading target version to wildcard in second tour
			for(int i = 0; i < 2; i++)
			{
				Utils.QualName qualName = new Utils.QualName(targetName, "");
				if (index.TryGetValue(SpellHint.MakeFullTargetName(genieName, targetVersion, targetType, qualName.FullName), out hint))
					return hint;
				Utils.QualName qualName2 = new Utils.QualName(SpellHint.TargetNameWildcard, qualName.Schema);
				if (index.TryGetValue(SpellHint.MakeFullTargetName(genieName, targetVersion, targetType, qualName2.FullName), out hint))
					return hint;
				if (index.TryGetValue(SpellHint.MakeFullTargetName(genieName, targetVersion, targetType, SpellHint.TargetNameWildcard), out hint))
					return hint;
				targetVersion = GenieConfig.TargetVersionWildcard;
			}
			return hint;
		}

		#region ISpellHints implementation
		ISpellHint ISpellHints.Find(string genieName, string targetVersion, IMetaObject source)
		{
			return this.Find(genieName, targetVersion, source);
		}

		ISpellHint ISpellHints.Find(string genieName, string targetVersion, string targetType, string targetName)
		{
			return this.Find(genieName, targetVersion, targetType, targetName);
		}
		#endregion

	}
}

