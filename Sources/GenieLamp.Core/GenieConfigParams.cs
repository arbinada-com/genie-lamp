using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core
{
	class GenieConfigParams : ParamsSimple
	{
		private GenieConfig owner;

		#region Constructors
		public GenieConfigParams(GenieConfig owner, XmlNodeList nodeList)
			: base(nodeList, owner.Macro)
		{
			this.owner = owner;
		}
		#endregion
		
		
		public new IParamSimple GetByName(string name, bool throwException = false, object source = null)
		{
			IParamSimple param = base.GetByName(name, false, source);
			if (param == null && throwException)
				throw new GlException("Parameter \"{0}\" not found (Genie: {1})", name, owner.AssemblyName);
			return param;
		}

	}
}

