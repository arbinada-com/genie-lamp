using System;
using System.Text;
using System.Collections.Generic;

namespace GenieLamp.Core.Utils
{
	public class MacroExpander : IMacroExpander
	{
		public const string MacroCounter = "%COUNTER%";
		public const string MacroTable = "%TABLE%";
		public const string MacroColumns = "%COLUMNS%";
		private Dictionary<string, string> macros = new Dictionary<string, string>();
		private MacroExpander parent = null;
		
		#region Constructors
		public MacroExpander()
		{
		}
		
		public MacroExpander(MacroExpander parent)
		{ 
			this.parent = parent;
		}
		#endregion

		public MacroExpander Parent
		{
			get { return this.parent; }
		}
		
		public void SetMacroCounter(int substValue)
		{
			SetMacro(MacroCounter, substValue.ToString());
		}

		public void SetMacroCounter(string macro, int substValue)
		{
			SetMacro(macro, substValue.ToString());
		}

		public int GetMacroCounter()
		{
			return GetMacroCounter(MacroCounter);
		}

		public int GetMacroCounter(string macro)
		{
			int count;
			if (int.TryParse(GetMacroValue(macro), out count))
				return count;
			return 1;
		}

		public void SetMacroTable(string substValue)
		{
			SetMacro(MacroTable, substValue);
		}
		
		public void SetMacroColumns(string substValue)
		{
			SetMacro(MacroColumns, substValue);
		}
		
		public string GetMacroValue(string macro)
		{
			if (macros.ContainsKey(macro))
				return macros[macro];
			else
				return String.Empty;
		}
		
		public void Clear()
		{
			macros.Clear();
		}
		
		public int Count
		{
			get { return macros.Count; }
		}
		
		#region IMacroExpander implementation
		public void SetMacro(string macro, string subst)
		{
			if (macros.ContainsKey(macro))
				macros[macro] = subst;
			else
				macros.Add(macro, subst);
		}

		public string Subst(string source)
		{
			string target = source;
			foreach (KeyValuePair<string, string> macro in macros)
			{
				target = target.Replace(macro.Key, macro.Value);
			}
			if (parent == null)
				return Utils.Sys.ExpandEnvironmentVariables(target); // Evironment variables has lower priority if same name is used
			else
				return parent.Subst(target);
		}

		public IMacroExpander CreateChild()
		{
			return new MacroExpander(this);
		}
		#endregion
	}
}

