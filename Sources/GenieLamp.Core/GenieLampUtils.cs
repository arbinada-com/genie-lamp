using System;
using System.Xml;

using GenieLamp.Core.Utils;
using GenieLamp.Core.Environments;

namespace GenieLamp.Core
{
	class GenieLampUtils : IGenieLampUtils
	{
		private GenieLamp lamp;
		
		public GenieLampUtils(GenieLamp lamp)
		{
			this.lamp = lamp;
		}

		public GenieLamp Lamp {
			get { return this.lamp; }
		}
		
		class UtilsXml : IUtilsXml
		{
			#region IUtilsXml implementation
			public IDocHelper CreateDocHelper(string rootName)
			{
				return new Xml.XmlDocHelper(rootName);
			}

			public IDocHelper CreateDocHelper(XmlDocument doc)
			{
				return new Xml.XmlDocHelper(doc);
			}
			#endregion
			
		}
		
		#region IGenieLampUtils implementation
		public IUtilsXml Xml {
			get { return new UtilsXml(); }
		}

		public IEnvironmentHelper GetEnvironmentHelper(TargetEnvironment target, string version)
		{
			return EnvironmentHelperBase.CreateHelper(target, version);
		}

		public INamingConvention GetNamingConvention(NamingStyle style)
		{
			return new NamingConvention(style);
		}

		public ISqlStringBuilder GetSqlStringBuilder()
		{
			return new SqlStringBuilder();
		}
		#endregion
	}
}

