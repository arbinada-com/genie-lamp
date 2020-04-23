using System;
using System.Collections.Generic;

namespace GenieLamp.Core.Layers
{
	class LayerKeywords
	{
		private Dictionary<string, string> keywords = new Dictionary<string, string>();

		#region Constructors
		public LayerKeywords()
		{ }

		public LayerKeywords(string[] keywords)
		{
			foreach(string keyword in keywords)
			{
				this.keywords.Add(ToCanonicalName(keyword), keyword);
			}

		}
		#endregion

		private string ToCanonicalName(string name)
		{
			return name.ToUpper();
		}

		/// <summary>
		/// Checks if name is not the SQL/persistence layer keyword
		/// </summary>
		/// <returns>
		/// The name.
		/// </returns>
		/// <param name='name'>
		/// If set to <c>true</c> name.
		/// </param>
		public bool Contains(string name)
		{
			return keywords.ContainsKey(ToCanonicalName(name));
		}

		public string CheckAndModify(string name)
		{
			if (Contains(name))
			{
				return "gl_" + name;
			}
			return name;
		}
	}
}

