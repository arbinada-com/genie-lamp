using System;

using GenieLamp.Core;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class OperationHelper : HelperBase
	{
		public OperationHelper(ICodeWriterCSharp cw, IEnvironmentHelper environment)
			: base(cw, environment)
		{
		}

		public static string GetRestServicePath(IEntityOperation operation)
		{
			return RestServiceHelper.GetRestServicePath(operation.Entity, operation.Name);
		}

		public static string GetServiceMethodName(IEntityOperation operation)
		{
			return operation.Name;
		}

		public void WriteOperationsSpec(IEntity entity)
		{
			foreach(IEntityOperation operation in entity.Operations)
			{
				cw.WriteLine(RestServiceHelper.GetRestServiceSpec(entity, GetServiceMethodName(operation)));
			}
		}

		public static string GetParamClassName(IEntityOperation operation)
		{
			return String.Format("{0}{1}Params", operation.Entity.Name, operation.Name);
		}

		public string GetParamClassName(IEntityOperation operation, bool fullName)
		{
			return String.Format("{0}{1}Params", environment.ToTypeName(operation.Entity, fullName), operation.Name);
		}

		public static string GetParamClassPropertyName(IEntityOperation operation)
		{
			return String.Format("{0}Params", operation.Name);
		}

		public void WriteParamClassesProperties(IEntity entity)
		{
			ServicesInterfacesHelper intfh = new ServicesInterfacesHelper(cw, environment);
			foreach(IEntityOperation operation in entity.Operations)
			{
				intfh.WriteProperty(GetParamClassName(operation), GetParamClassPropertyName(operation));
			}
		}

		public void WriteParamClassesPropertiesInitialization(IEntity entity)
		{
			foreach(IEntityOperation operation in entity.Operations)
			{
				cw.WriteLine("{0} = new {1}();", GetParamClassPropertyName(operation), GetParamClassName(operation));
			}
		}

		public void WriteParamClasses(IEntity entity)
		{
			foreach(IEntityOperation operation in entity.Operations)
			{
				cw.BeginClass(AccessLevel.Public, false,
				              GetParamClassName(operation),
				              null);
				if (operation.Access == EntityOperationAccess.Public)
				{
					foreach(IEntityOperationParam param in operation.Params)
					{
						WriteOperationParam(param);
					}
					if (operation.Params.HasReturns)
						WriteOperationParam(operation.Params.Returns);
				}
				cw.EndClass();
				cw.WriteLine();
			}
		}

		public string GetParamPropertyName(IEntityOperationParam param)
		{
			return param.Name;
		}

		private void WriteOperationParam(IEntityOperationParam param)
		{
			if (!(param.Type is IScalarType))
				throw new GlException("Only scalar types are supported for services layer operations. " +
				                      "Change types of operation parameter or set operation access to 'internal'\n" +
				                      "Operation: {0}. Parameter: {1}",
				                      (param.Owner as IEntityOperation).Name, param.Name);
			(new ServicesInterfacesHelper(cw, environment))
						.WriteProperty(environment.ToTypeName(param.Type, param.TypeDefinition, true), GetParamPropertyName(param));
		}

	}
}

