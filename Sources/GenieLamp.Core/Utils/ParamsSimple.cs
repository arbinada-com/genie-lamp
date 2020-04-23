using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Utils
{
	class ParamsSimple : GlNamedCollection<IParamSimple, ParamSimple>, IParamsSimple
	{
		private MacroExpander macro = new MacroExpander();

		#region Constructors
		public ParamsSimple(MacroExpander macro)
			: base()
		{
			this.macro = new MacroExpander(macro);
		}
		
		public ParamsSimple(XmlNodeList nodeList, MacroExpander macro)
			: base()
		{
			this.macro = new MacroExpander(macro);
			foreach (XmlNode paramNode in nodeList)
			{
				ParamSimple p = new ParamSimple(this, paramNode);
				Add(p.Name, p);
			}
		}
		#endregion

		public MacroExpander Macro
		{
			get { return macro; }
		}

		#region IParamsSimple implementation
		public IParamSimple ParamByName(string name, bool throwException)
		{
			ParamSimple param = this.GetByName(name, false);
			if (param == null && throwException)
				throw new GlException("Parameter \"{0}\" not found", name);
			return param;
		}
		
		public IParamSimple ParamByName(string name, string defaultValue)
		{
			ParamSimple param = this.GetByName(name, false);
			if (param == null)
				param = new ParamSimple(this, name, defaultValue);
			return param;
		}

		public string ValueByName(string name, string defaultValue)
		{
			ParamSimple param = this.GetByName(name, false);
			if (param == null)
				return defaultValue;
			return param.Value;
		}

		public bool ValueByName(string name, bool defaultValue)
		{
			ParamSimple param = this.GetByName(name, false);
			if (param == null)
				return defaultValue;
			bool result;
			if (!bool.TryParse(param.Value, out result))
				throw new GlException("Invalid boolean value \"{0}\". Parameter: {1}", param.Value, name);
			return result;
		}

		public int ValueByName(string name, int defaultValue)
		{
			ParamSimple param = this.GetByName(name, false);
			if (param == null)
				return defaultValue;
			int result;
			if (!int.TryParse(param.Value, out result))
				throw new GlException("Invalid integer value \"{0}\". Parameter: {1}", param.Value, name);
			return result;
		}
		#endregion


	}
}

