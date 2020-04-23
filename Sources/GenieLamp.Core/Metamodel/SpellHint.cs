using System;
using System.Xml;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Metamodel
{
	class SpellHint : MetaObject, ISpellHint
	{
		public const string TargetNameWildcard = "*";
		private string genieName;
		private string targetVersion;
		private string targetType;
		private string targetName;
		private string text = String.Empty;

		#region Constructors
		public SpellHint(Model model, XmlNode node)
			: base(model, node)
		{
			this.genieName = Utils.Xml.GetAttrValue(node, "genie");
			this.targetVersion = Utils.Xml.GetAttrValue(node, "targetVersion", GenieConfig.TargetVersionWildcard);
			this.targetType = Utils.Xml.GetAttrValue(node, "targetType");
			this.targetName = Utils.Xml.GetAttrValue(node, "targetName", TargetNameWildcard);
			this.SpellHintParams = new SpellHintParams(this, Model.QueryNode(node, "./{0}:Param"));
			this.text = Utils.Xml.GetNodeText(node);
			foreach (XmlNode defineNode in Model.QueryNode(node, "./{0}:Define"))
			{
				this.Macro.SetMacro(Utils.Xml.GetAttrValue(defineNode, "name"), Utils.Xml.GetAttrValue(defineNode, "value"));
			}

			this.SpellHintProperties = new ParamsSimple(Model.QueryNode(node, "./{0}:Property"), Macro);
		}
		#endregion

		public ParamsSimple SpellHintProperties { get; private set; }

		public static string MakeFullTargetName(string genieName, string targetVersion, string targetType, string targetName)
		{
			return String.Format("{0}.{1}.{2}.{3}", genieName, targetVersion, targetType, targetName);
		}

		public string FullTargetName
		{
			get { return SpellHint.MakeFullTargetName(genieName, targetVersion, targetType, targetName); }
		}

		public override string ToString()
		{
			return string.Format("(SpellHint: Genie={0}, TargetVersion={1}, TargetType={2}, TargetName={3})", GenieName, TargetVersion, TargetType, TargetName);
		}

		#region ISpellHint implementation
		public string TargetName
		{
			get { return this.targetName; }
		}

		public string TargetType
		{
			get { return this.targetType; }
		}

		public string GenieName
		{
			get	{ return this.genieName; }
		}

		public string TargetVersion
		{
			get	{ return this.targetVersion; }
		}

		public string Text
		{
			get { return this.Macro.Subst(this.text); }
		}

		public string GetText(IMetaObject source)
		{
			IMetaObject current = source;
			while (current != null)
			{
				foreach(ParamSimple sourceParam in current.SpellHintParams)
				{
					ParamSimple targetParam = this.SpellHintParams.FindParam(sourceParam.Name, sourceParam.Value);
					if (targetParam == null)
						targetParam = this.SpellHintParams.FindParam(sourceParam.Name, SpellHint.TargetNameWildcard);
					if (targetParam != null)
						return this.Macro.Subst(targetParam.Text);
				}
				current = current.Owner;
			}
			return this.Text;
		}

		IParamsSimple ISpellHint.Properties
		{
			get { return this.SpellHintProperties; }
		}
		#endregion
	}
}

