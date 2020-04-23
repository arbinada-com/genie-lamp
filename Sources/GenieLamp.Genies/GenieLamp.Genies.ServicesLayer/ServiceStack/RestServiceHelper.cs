using System;
using System.Text;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class RestServiceHelper
	{
		public const int MaxPageSize = 300;
		public const string ErrorCode = "400";

		public RestServiceHelper()
		{
		}

		public static string GetRestServicePath(IEntity entity, string path)
		{
			return String.Format("/{0}{1}",
			                     NamingHelperStack.ToServiceName(entity, null),
			                     String.IsNullOrEmpty(path) ? "" :
			                     	path.StartsWith("/") ? path : "/" + path);
		}

		public static string GetRestServiceSpec(IEntity entity, string path)
		{
			return String.Format("[Route(\"{0}\")]",
			                     GetRestServicePath(entity, path));
		}

		public static string GetRestServiceSpec(IEntity entity, IAttributes attributes)
		{
			StringBuilder sb = new StringBuilder();
			foreach(IAttribute a in attributes)
			{
				sb.AppendFormat("/{0}/{{{0}}}", a.Name);
			}
			return GetRestServiceSpec(entity, sb.ToString());
		}

	}
}

