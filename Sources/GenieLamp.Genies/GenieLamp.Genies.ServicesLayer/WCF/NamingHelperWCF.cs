using System;
using System.IO;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer.WCF
{
	public abstract class NamingHelperWCF : NamingHelper
	{
		public const string IntfName_PersistenceService = "IPersistenceService";

		public static string GetEntityServiceName(IEntity entity)
		{
			return String.Format("I{0}Service", entity.Name);
		}

		public static string GetEntityServiceClient(IEntity entity)
		{
			return String.Format("{0}ServiceClient", entity.Name);
		}


		public static string MethodSignature_Create(IEntity entity)
		{
        	return MethodSignature_Create(entity.Name);
		}

		public static string MethodSignature_Create(string className)
		{
        	return String.Format("{0}()", MethodName_Create(className));
		}

		public static string MethodName_Create(string className)
		{
        	return String.Format("Create{0}", className);
		}

	}
}

