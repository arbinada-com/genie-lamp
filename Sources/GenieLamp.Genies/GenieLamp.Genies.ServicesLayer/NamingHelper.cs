using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.ServicesLayer
{
	public class NamingHelper
	{
		public const string ServiceName_Persistence = "Persistence";

		public const string ClassName_PersistentObjectDTO = "PersistentObjectDTO";
		public const string ClassName_PersistentObjectDTOAdapter = "PersistentObjectDTOAdapter";
		public const string ClassName_UnitOfWorkDTO = "UnitOfWorkDTO";
		public const string ClassName_UnitOfWorkDomain = "UnitOfWork";
		public const string ClassName_UnitOfWorkDTOAdapter = "UnitOfWork";
		public const string ClassName_CommitResult = "CommitResult";
		public const string ClassName_DomainObjectDTO = "DomainObjectDTO";
		public const string ClassName_ServicesQueryParams = "ServicesQueryParams";
		public const string ClassName_ServicesQueryParam = "ServicesQueryParam";

		public const string EnumName_DomainTypes = "DomainTypes";

		public const string VarName_DomainObject = "domainObject";
		public const string VarName_UnitOfWork = "unitOfWork";

		public const string PropertyName_DomainObject = "DomainObject";
		public const string PropertyName_DomainTypeId = "Internal_DomainTypeId";
		public const string PropertyName_InternalObjectId = "Internal_ObjectId";
		public const string PropertyName_CommitResult = "CommitResult";
		public const string PropertyName_DTOChanged = "Changed";
		public const string PropertyName_UnitOfWorkDTO = "UnitOfWork";
		public const string PropertyName_AdapterDTO = "DTO";


		public static string ToDTOTypeName(string className)
		{
			return String.Format("{0}DTO", className);
		}

		public static string ToDTOTypeName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment == null)
				return ToDTOTypeName(entity.Name);
			return ToDTOTypeName(environment.ToTypeName(entity.Type, true));
		}

		public static string ToDTOPropertyName(IEntity entity)
		{
			return ToDTOTypeName(entity, null);
		}

		public static string ToDTOPropertyName(IAttribute attribute)
		{
			return attribute.IsMigrated && !attribute.IsDeclared && !attribute.IsPrimaryId ?
				attribute.Migration.Name : attribute.Name;
		}


		public static string ToDomainTypeName(IEntity entity, IEnvironmentHelper environment)
		{
			if (environment == null)
				return entity.Name;
			return environment.ToTypeName(entity.Type, true);
		}


		public static string ToDomainTypesEnumItemName(IEntity entity)
		{
			return String.Format("Type{0}", entity.Name);
		}

	}
}

