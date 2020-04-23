using System;
using System.Xml;
using System.Text;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Layers;

namespace GenieLamp.Core.Utils
{
	class NamingConvention : INamingConvention
	{
		private NamingStyle style = NamingStyle.None;
		private LayerConfig owner;
		private int maxLength = int.MaxValue;
		private NamingConventionParams namingConventionParams;
		private MacroExpander macro = new MacroExpander();
		
		#region Constructors
		public NamingConvention(LayerConfig owner)
			: this(NamingStyle.None)
		{
			this.owner = owner;
			this.macro = new MacroExpander(owner.Macro);
			namingConventionParams = new NamingConventionParams(this);
		}
		
		public NamingConvention(LayerConfig owner, XmlNode node)
			: this(owner)
		{
			this.style = ParseStyle(Utils.Xml.GetAttrValue(node, "style"));
			this.maxLength = Utils.Xml.GetAttrValue(node, "maxLength", this.maxLength);
			namingConventionParams = new NamingConventionParams(this, owner.GenieLampConfig.Lamp.QueryNode(node, "./{0}:Param"));
		}

		internal NamingConvention(NamingStyle style)
		{
			this.style = style;
		}
		
		internal NamingConvention(string styleStr)
		{
			style = ParseStyle(styleStr);
		}
		#endregion

		public static NamingStyle ParseStyle(string styleStr)
		{
			if (styleStr.ToLower().Equals("uppercase"))
				return NamingStyle.UpperCase;
			else if (styleStr.ToLower().Equals("lowercase"))
				return NamingStyle.LowerCase;
			else if (styleStr.ToLower().Equals("camelcase"))
				return NamingStyle.CamelCase;
			else if (styleStr.ToLower().Equals("none"))
				return NamingStyle.None;
			else
				throw new GlException("Unsupported naming style: {0}", styleStr);
		}
		
		private string ToLowerCase(string name)
		{
			StringBuilder sb = new StringBuilder();
			bool lastLower = false;
			bool lastPrefix = false;
			for (int i = 0; i < name.Length; i++)
			{
				string s = name.Substring(i, 1);
				if (Char.IsLetterOrDigit(s, 0) || (s.Equals("_") && !lastPrefix))
				{
					if (s.ToUpper().Equals(s) && lastLower && !lastPrefix && !s.Equals("_") && !Char.IsDigit(s, 0))
						sb.Append("_");
					sb.Append(s.ToLower());
				}
				lastLower = s.ToLower().Equals(s);
				lastPrefix = s.Equals("_");
			}
			return sb.ToString();
		}
		
		private string ToUpperCase(string name)
		{
			return ToLowerCase(name).ToUpper();
		}
		
		private string ToCamelCase(string name)
		{
			StringBuilder sb = new StringBuilder();
			bool lastPrefix = false;
			bool lastLower = true;
			for (int i = 0; i < name.Length; i++)
			{
				string s = name.Substring(i, 1);
				if (Char.IsLetterOrDigit(s, 0))
				{
					if (lastPrefix || (lastLower && s.ToUpper().Equals(s)))
					{
						sb.Append(s.ToUpper());
					}
					else
					{
						sb.Append(s.ToLower());
					}
				}
				lastLower = s.ToLower().Equals(s);
				lastPrefix = s.Equals("_");
			}
			return sb.ToString();
		}
		
		public string TruncateName(string name, bool throwException = false, object source = null)
		{
			
			if (name.Length > 0 && name.Length > maxLength)
			{
				if (throwException)
					throw new GlException("Persistent name \"{0}\" length ({1}) exceeded the limit of {2}. {3}", 
					                      name, name.Length, maxLength, source.ToString());
				else
				{
					// trincating from last non-digit character to keep counter values
					int pos = name.Length;
					while(Char.IsDigit(name[pos - 1]))
						pos--;
					string newName =
						name.Substring(0, maxLength - (name.Length - pos)) +
						name.Substring(pos, name.Length - pos);
					if (source != null)
					{
						owner.GenieLampConfig.Lamp.Logger.Warning(
							WarningLevel.Low,
							"Persistent name \"{0}\" length ({1}) exceeded the limit of {2}. Persistent name was truncated to {3}. Source: {4}",
							name, 
							name.Length,
							maxLength, 
							newName,
							source);
					}
					return newName;
				}
			}
			return name;
		}

		public LayerConfig Owner
		{
			get { return this.owner; }
		}
		
		public NamingConventionParams Params
		{
			get { return namingConventionParams; }
		}
		
		public MacroExpander Macro
		{
			get { return macro; }
		}

		#region INamingConvention implementation
		public string Convert(string name)
		{
			if (String.IsNullOrEmpty(name))
				return String.Empty;
			
			switch (style)
			{
			case NamingStyle.None:
				return TruncateName(name);
			case NamingStyle.UpperCase:
				return TruncateName(ToUpperCase(name));
			case NamingStyle.LowerCase:
				return TruncateName(ToLowerCase(name));
			case NamingStyle.CamelCase:
				return TruncateName(ToCamelCase(name));
			default:
				throw new GlException("Unsupported conversion naming style: {0}", style);
			}
		}

		public NamingStyle Style
		{
			get { return style; }
		}
		
		public int MaxLength
		{
			get { return maxLength; }
			set { maxLength = value; }
		}
		
		IParamsSimple INamingConvention.Params
		{
			get { return namingConventionParams; }
		}
		#endregion
	}
}

