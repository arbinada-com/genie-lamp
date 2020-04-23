using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class NamingHelperStack : NamingHelper
	{
		public const string ParamName_Query = "Gl_Query";
		public const string ParamName_QueryParams = "Gl_QueryParams";
		public const string ParamName_PageNumber = "Gl_PageNum";
		public const string ParamName_PageSize = "Gl_PageSize";
		public const string ParamName_SortOrderProperty = "Gl_OrderBy";
		public const string ParamName_SortOrderAsc = "Gl_OrderAsc";

		public static string ToServiceName(string name)
		{
			return String.Format("{0}Service", name);
		}

		public static string ToServiceName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment == null)
				return ToServiceName(entity.Name);
			return ToServiceName(environment.ToTypeName(entity, true));
		}


		public static string ToServiceRequestName(string name)
		{
			return String.Format("{0}Request", name);
		}

		public static string ToServiceRequestName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment !=  null)
				return ToServiceRequestName(environment.ToTypeName(entity, true));
			return ToServiceRequestName(entity.Name);
		}


		public static string ToServiceResponseName(string name)
		{
			return String.Format("{0}Response", name);
		}

		public static string ToServiceResponseName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment !=  null)
				return ToServiceResponseName(environment.ToTypeName(entity, true));
			return ToServiceResponseName(entity.Name);
		}


		public static string ToServiceResponseCollectionName(string name)
		{
			return String.Format("{0}ListResponse", name);
		}

		public static string ToServiceResponseCollectionName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment !=  null)
				return ToServiceResponseCollectionName(environment.ToTypeName(entity, true));
			return ToServiceResponseCollectionName(entity.Name);
		}


		public static string ToDTOCollectionTypeName(string name)
		{
			return String.Format("List<{0}>", name);
		}

		public static string ToDTOCollectionTypeName(IEntity entity, IEnvironmentHelper environment)
		{
			return ToDTOCollectionTypeName(ToDTOTypeName(entity, environment));
		}

		public static string ToDTOCollectionPropertyName(string name)
		{
			return String.Format("{0}List", name);
		}

		public static string ToDTOCollectionPropertyName(IEntity entity)
		{
			return ToDTOCollectionPropertyName(ToDTOPropertyName(entity));
		}


		public static string ToDTOAdapterTypeName(string name)
		{
			return name;
		}

		public static string ToDTOAdapterTypeName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment != null)
				return ToDTOAdapterTypeName(environment.ToTypeName(entity, true));
			return ToDTOAdapterTypeName(entity.Name);
		}


		public static string ToDTOAdapterCollectionTypeName(string name)
		{
			return String.Format("{0}Collection", name);
		}

		public static string ToDTOAdapterCollectionTypeName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment !=  null)
				return ToDTOAdapterCollectionTypeName(environment.ToTypeName(entity, true));
			return ToDTOAdapterCollectionTypeName(entity.Name);
		}
	}
}

