// Genie Lamp Core (1.1.4594.29523)
// ServiceStack services genie (1.0.4594.29525)
// Starter application (1.1.4594.29524)
// This file was automatically generated at 2012-07-30 16:36:49
// Do not modify it manually.

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
// Assembly required: ServiceStack.dll
// Assembly required: ServiceStack.Common.dll
// Assembly required: ServiceStack.Interfaces.dll
// Assembly required: ServiceStack.ServiceInterfaces.dll
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

using Arbinada.GenieLamp.Warehouse.Services.Interfaces;

namespace Arbinada.GenieLamp.Warehouse.Services
{
	public class DomainQueryFactory
	{
		public static Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams ToDomainQueryParams(ServicesQueryParams queryParams)
		{
			Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams domainQueryParams = null;
	        if (queryParams != null)
	        {
	            domainQueryParams = new Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams();
	            foreach (ServicesQueryParam param in queryParams)
				{
	                domainQueryParams.AddParam(param.Name, param.Value);
				}
	        }
			return domainQueryParams;
		}
	}
	
	public partial class PersistenceService : RestServiceBase<PersistenceRequest>, IRequiresRequestContext
	{
		public override object OnGet(PersistenceRequest request)
		{
			return new PersistenceResponse();
		}
		
		public override object OnPost(PersistenceRequest request)
		{
			PersistenceResponse response = new PersistenceResponse();
			response.CommitResult = PersistenceService.Commit(request.UnitOfWork, response.UpdatedObjects);
			return response;
		}
		
		public static CommitResult Commit(UnitOfWorkDTO unitOfWork, UpdatedObjects updatedObjects)
		{
			CommitResult commitResult = new CommitResult();
			Arbinada.GenieLamp.Warehouse.Persistence.UnitOfWork uow = new Arbinada.GenieLamp.Warehouse.Persistence.UnitOfWork();
			foreach(UnitOfWorkDTO.WorkItem wi in unitOfWork.WorkItems)
			{
				InitDomain(wi);
				if (wi.Action == UnitOfWorkDTO.Action.Save)
				{
					uow.Save(wi.DomainObject);
				}
				else
				{
					uow.Delete(wi.DomainObject);
				}
			}
			try
			{
				uow.Commit();
				if (updatedObjects != null) updatedObjects.Clear();
				foreach(UnitOfWorkDTO.WorkItem wi in unitOfWork.WorkItems)
				{
					if (wi.Action != UnitOfWorkDTO.Action.Delete)
					{
						InitDTO(wi);
						if (updatedObjects != null) updatedObjects.Add(wi.Item.Internal_ObjectId, wi.Item);
					}
				}
			}
			catch(Exception e)
			{
				commitResult.HasError = true;
				commitResult.Message = e.Message;
				commitResult.ExceptionString = e.ToString();
			}
			return commitResult;
		}
		
		private static void InitDomain(UnitOfWorkDTO.WorkItem wi)
		{
			switch((DomainTypes)wi.Item.GetInternal_DomainTypeId())
			{
				case DomainTypes.TypeEntityType:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
					}
					Arbinada.GenieLamp.Warehouse.Services.Core.EntityTypeService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType);
					break;
				}
				case DomainTypes.TypeEntityRegistry:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry();
					}
					Arbinada.GenieLamp.Warehouse.Services.Core.EntityRegistryService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry);
					break;
				}
				case DomainTypes.TypeExampleOneToOne:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ExampleOneToOneService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne);
					break;
				}
				case DomainTypes.TypeExampleOneToOneEx:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ExampleOneToOneExService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx);
					break;
				}
				case DomainTypes.TypeProductType:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductTypeService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType);
					break;
				}
				case DomainTypes.TypeProduct:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product);
					break;
				}
				case DomainTypes.TypeStoreType:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreTypeService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType);
					break;
				}
				case DomainTypes.TypeStore:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store);
					break;
				}
				case DomainTypes.TypeContractor:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ContractorService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor);
					break;
				}
				case DomainTypes.TypePartner:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.PartnerService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner);
					break;
				}
				case DomainTypes.TypeStoreDoc:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreDocService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc);
					break;
				}
				case DomainTypes.TypeStoreDocLine:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO).StoreDocId, (wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO).Position);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreDocLineService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine);
					break;
				}
				case DomainTypes.TypeStoreTransaction:
				{
					wi.DomainObject = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.GetById((wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction();
					}
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreTransactionService.DTOToDomainObject(wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO, wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction);
					break;
				}
				default: throw new ApplicationException("Cannot save non-persistent object");
			}
		}
		
		private static void InitDTO(UnitOfWorkDTO.WorkItem wi)
		{
			switch((DomainTypes)wi.Item.GetInternal_DomainTypeId())
			{
				case DomainTypes.TypeEntityType:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Core.EntityTypeService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO);
					break;
				}
				case DomainTypes.TypeEntityRegistry:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Core.EntityRegistryService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO);
					break;
				}
				case DomainTypes.TypeExampleOneToOne:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ExampleOneToOneService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO);
					break;
				}
				case DomainTypes.TypeExampleOneToOneEx:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ExampleOneToOneExService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO);
					break;
				}
				case DomainTypes.TypeProductType:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductTypeService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO);
					break;
				}
				case DomainTypes.TypeProduct:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO);
					break;
				}
				case DomainTypes.TypeStoreType:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreTypeService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO);
					break;
				}
				case DomainTypes.TypeStore:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO);
					break;
				}
				case DomainTypes.TypeContractor:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.ContractorService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO);
					break;
				}
				case DomainTypes.TypePartner:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.PartnerService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO);
					break;
				}
				case DomainTypes.TypeStoreDoc:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreDocService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO);
					break;
				}
				case DomainTypes.TypeStoreDocLine:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreDocLineService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO);
					break;
				}
				case DomainTypes.TypeStoreTransaction:
				{
					(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction).Refresh();
					Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreTransactionService.DomainObjectToDTO(wi.DomainObject as Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction, wi.Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO);
					break;
				}
				default: throw new ApplicationException("Cannot process non-persistent object");
			}
		}
	}
	
	#region Entities
	namespace Core
	{
		public partial class EntityTypeService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetById(request.Id.Value);
					}
					else if (request.FullName != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(request.FullName);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetPage(0, 20, null);
							}
							responseList.EntityTypeDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO();
						EntityTypeService.DomainObjectToDTO(domain, dto);
						response.EntityTypeDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.EntityTypeDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.EntityTypeDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.EntityTypeDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType> list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.EntityTypeDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto)
			{
				dto.Id = domain.Id;
				dto.FullName = domain.FullName;
				dto.ShortName = domain.ShortName;
				dto.Description = domain.Description;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType domain)
			{
				domain.FullName = dto.FullName;
				domain.ShortName = dto.ShortName;
				domain.Description = dto.Description;
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO();
					EntityTypeService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class EntityRegistryService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(request.Id.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetPage(0, 20, null);
							}
							responseList.EntityRegistryDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO();
						EntityRegistryService.DomainObjectToDTO(domain, dto);
						response.EntityRegistryDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.EntityRegistryDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.EntityRegistryDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.EntityRegistryDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry> list = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.EntityRegistryDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO dto)
			{
				dto.Id = domain.Id;
				dto.EntityTypeIdId = domain.EntityType == null ? (int?)null : domain.EntityType.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry domain)
			{
				if (dto.EntityTypeIdId != null)
				{
					domain.EntityType = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetById(dto.EntityTypeIdId.Value);
				}
				else
				{
					domain.EntityType = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO();
					EntityRegistryService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
	}
	
	namespace Warehouse
	{
		public partial class ExampleOneToOneService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetById(request.Id.Value);
					}
					else if (request.Name != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetByName(request.Name);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetPage(0, 20, null);
							}
							responseList.ExampleOneToOneDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO();
						ExampleOneToOneService.DomainObjectToDTO(domain, dto);
						response.ExampleOneToOneDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.ExampleOneToOneDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.ExampleOneToOneDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.ExampleOneToOneDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.ExampleOneToOneDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto)
			{
				dto.Id = domain.Id;
				dto.Name = domain.Name;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne domain)
			{
				domain.Name = dto.Name;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO();
					ExampleOneToOneService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class ExampleOneToOneExService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetById(request.Id.Value);
					}
					else if (request.ExempleOneToOneId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetByExempleOneToOneId(request.ExempleOneToOneId.Value);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetPage(0, 20, null);
							}
							responseList.ExampleOneToOneExDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO();
						ExampleOneToOneExService.DomainObjectToDTO(domain, dto);
						response.ExampleOneToOneExDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.ExampleOneToOneExDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.ExampleOneToOneExDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.ExampleOneToOneExDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.ExampleOneToOneExDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto)
			{
				dto.Id = domain.Id;
				dto.Caption = domain.Caption;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.ExempleOneToOneId = domain.ExampleOneToOne.Id;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx domain)
			{
				domain.Caption = dto.Caption;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				domain.ExampleOneToOne = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne.GetById(dto.ExempleOneToOneId);
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO();
					ExampleOneToOneExService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class ProductTypeService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetById(request.Id.Value);
					}
					else if (request.Code != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetByCode(request.Code);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetPage(0, 20, null);
							}
							responseList.ProductTypeDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO();
						ProductTypeService.DomainObjectToDTO(domain, dto);
						response.ProductTypeDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.ProductTypeDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.ProductTypeDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.ProductTypeDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.ProductTypeDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto)
			{
				dto.Id = domain.Id;
				dto.Code = domain.Code;
				dto.Name = domain.Name;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType domain)
			{
				domain.Code = dto.Code;
				domain.Name = dto.Name;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO();
					ProductTypeService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class ProductService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetById(request.Id.Value);
					}
					else if (request.RefCode != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetByRefCode(request.RefCode);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetPage(0, 20, null);
							}
							responseList.ProductDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO();
						ProductService.DomainObjectToDTO(domain, dto);
						response.ProductDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.ProductDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.ProductDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.ProductDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.ProductDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto)
			{
				dto.Id = domain.Id;
				dto.RefCode = domain.RefCode;
				dto.Caption = domain.Caption;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.TypeId = domain.Type == null ? (int?)null : domain.Type.Id;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product domain)
			{
				domain.RefCode = dto.RefCode;
				domain.Caption = dto.Caption;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				if (dto.TypeId != null)
				{
					domain.Type = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType.GetById(dto.TypeId.Value);
				}
				else
				{
					domain.Type = null;
				}
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO();
					ProductService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class StoreTypeService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetById(request.Id.Value);
					}
					else if (request.Name != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetByName(request.Name);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetPage(0, 20, null);
							}
							responseList.StoreTypeDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO();
						StoreTypeService.DomainObjectToDTO(domain, dto);
						response.StoreTypeDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.StoreTypeDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.StoreTypeDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.StoreTypeDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.StoreTypeDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto)
			{
				dto.Id = domain.Id;
				dto.Name = domain.Name;
				dto.Caption = domain.Caption;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType domain)
			{
				domain.Name = dto.Name;
				domain.Caption = dto.Caption;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO();
					StoreTypeService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class StoreService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetById(request.Id.Value);
					}
					else if (request.Code != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetByCode(request.Code);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetPage(0, 20, null);
							}
							responseList.StoreDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO();
						StoreService.DomainObjectToDTO(domain, dto);
						response.StoreDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.CheckParams != null)
					{
						Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store domain = null;
						if (request.StoreDTO != null) domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetById(request.StoreDTO.Id);
						if (domain == null) domain = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store();
						DTOToDomainObject(request.StoreDTO, domain);
						domain.Save();
						domain.Check();
						DomainObjectToDTO(domain, request.StoreDTO);
						response.StoreDTO = request.StoreDTO;
						response.CheckParams = request.CheckParams;
					}
					else if (request.RecordReceivedParams != null)
					{
						Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store domain = null;
						if (request.StoreDTO != null) domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetById(request.StoreDTO.Id);
						if (domain == null) domain = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store();
						DTOToDomainObject(request.StoreDTO, domain);
						domain.Save();
						request.RecordReceivedParams.Result = domain.RecordReceived(request.RecordReceivedParams.productId, request.RecordReceivedParams.qty, request.RecordReceivedParams.price, request.RecordReceivedParams.date);
						DomainObjectToDTO(domain, request.StoreDTO);
						response.StoreDTO = request.StoreDTO;
						response.RecordReceivedParams = request.RecordReceivedParams;
					}
					else if (request.GetQuantityParams != null)
					{
						Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store domain = null;
						if (request.StoreDTO != null) domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetById(request.StoreDTO.Id);
						if (domain == null) domain = new Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store();
						DTOToDomainObject(request.StoreDTO, domain);
						domain.Save();
						request.GetQuantityParams.Result = domain.GetQuantity(request.GetQuantityParams.product, request.GetQuantityParams.date);
						DomainObjectToDTO(domain, request.StoreDTO);
						response.StoreDTO = request.StoreDTO;
						response.GetQuantityParams = request.GetQuantityParams;
					}
					else if (request.StoreDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.StoreDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.StoreDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.StoreDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto)
			{
				dto.Id = domain.Id;
				dto.Code = domain.Code;
				dto.Caption = domain.Caption;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.StoreTypeId = domain.StoreType == null ? (int?)null : domain.StoreType.Id;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store domain)
			{
				domain.Code = dto.Code;
				domain.Caption = dto.Caption;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				if (dto.StoreTypeId != null)
				{
					domain.StoreType = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType.GetById(dto.StoreTypeId.Value);
				}
				else
				{
					domain.StoreType = null;
				}
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO();
					StoreService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class ContractorService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetById(request.Id.Value);
					}
					else if (request.Name != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetByName(request.Name);
					}
					else if (request.Phone != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetByPhone(request.Phone);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetPage(0, 20, null);
							}
							responseList.ContractorDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO();
						ContractorService.DomainObjectToDTO(domain, dto);
						response.ContractorDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.ContractorDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.ContractorDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.ContractorDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.ContractorDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto)
			{
				dto.Id = domain.Id;
				dto.Name = domain.Name;
				dto.Address = domain.Address;
				dto.Phone = domain.Phone;
				dto.Email = domain.Email;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor domain)
			{
				domain.Name = dto.Name;
				domain.Address = dto.Address;
				domain.Phone = dto.Phone;
				domain.Email = dto.Email;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO();
					ContractorService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class PartnerService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner.GetById(request.Id.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner.GetPage(0, 20, null);
							}
							responseList.PartnerDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO();
						PartnerService.DomainObjectToDTO(domain, dto);
						response.PartnerDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.PartnerDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.PartnerDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.PartnerDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.PartnerDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO dto)
			{
				Arbinada.GenieLamp.Warehouse.Services.Warehouse.ContractorService.DomainObjectToDTO(domain, dto);
				dto.Since = domain.Since;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner domain)
			{
				Arbinada.GenieLamp.Warehouse.Services.Warehouse.ContractorService.DTOToDomainObject(dto, domain);
				domain.Since = dto.Since;
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO();
					PartnerService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class StoreDocService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetById(request.Id.Value);
					}
					else if (request.RefNum != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetByRefNum(request.RefNum);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetPage(0, 20, null);
							}
							responseList.StoreDocDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO();
						StoreDocService.DomainObjectToDTO(domain, dto);
						response.StoreDocDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.StoreDocDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.StoreDocDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.StoreDocDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.StoreDocDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto)
			{
				dto.Id = domain.Id;
				dto.RefNum = domain.RefNum;
				dto.Created = domain.Created;
				dto.Name = domain.Name;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc domain)
			{
				domain.RefNum = dto.RefNum;
				domain.Created = dto.Created;
				domain.Name = dto.Name;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO();
					StoreDocService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class StoreDocLineService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine domain = null;
				try
				{
					if (request.StoreDocId != null && request.Position != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.GetById(request.StoreDocId.Value, request.Position.Value);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.GetPage(0, 20, null);
							}
							responseList.StoreDocLineDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO();
						StoreDocLineService.DomainObjectToDTO(domain, dto);
						response.StoreDocLineDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.StoreDocLineDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.StoreDocLineDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.StoreDocLineDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.StoreDocLineDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.StoreDocId != null && request.Position != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine.DeleteById(request.StoreDocId.Value, request.Position.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto)
			{
				dto.Position = domain.Position;
				dto.Quantity = domain.Quantity;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.StoreDocId = domain.Doc.Id;
				dto.ProductId = domain.Product.Id;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine domain)
			{
				domain.Position = dto.Position;
				domain.Quantity = dto.Quantity;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				domain.Doc = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc.GetById(dto.StoreDocId);
				domain.Product = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetById(dto.ProductId);
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO();
					StoreDocLineService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
		public partial class StoreTransactionService : RestServiceBase<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionRequest>, IRequiresRequestContext
		{
			public override object OnGet(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse();
				response.ResponseStatus.ErrorCode = "200";
				Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.GetById(request.Id.Value);
					}
					else if (request.EntityRegistryId != null)
					{
						domain = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.GetByEntityRegistryId(request.EntityRegistryId.Value);
					}
					else
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] { new Arbinada.GenieLamp.Warehouse.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.GetPage(0, 20, null);
							}
							responseList.StoreTransactionDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					
					if (domain != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO();
						StoreTransactionService.DomainObjectToDTO(domain, dto);
						response.StoreTransactionDTO = dto;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnPost(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.StoreTransactionDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.StoreTransactionDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.StoreTransactionDTO = (uow.WorkItems[0].Item as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionListResponse responseList = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction> list = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 20);
							responseList.StoreTransactionDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			public override object OnDelete(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionRequest request)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse response = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction.DeleteById(request.Id.Value);
						}
						catch(Exception e)
						{
							response.CommitResult.HasError = true;
							response.CommitResult.Message = e.Message;
							response.CommitResult.ExceptionString = e.ToString();
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error deleting object";
							response.ResponseStatus.StackTrace = e.ToString();
						}
					}
					else
					{
						response.CommitResult.HasError = true;
						response.CommitResult.Message = "Object primary id is empty";
						response.ResponseStatus.ErrorCode = "400";
						response.ResponseStatus.Message = "Object primary id is empty";
					}
				}
				catch(Exception e)
				{
					response.ResponseStatus.ErrorCode = "400";
					response.ResponseStatus.Message = e.Message + '\n' + e.ToString();
					response.ResponseStatus.StackTrace = e.ToString();
				}
				return response;
			}
			
			internal static void DomainObjectToDTO(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction domain, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto)
			{
				dto.Id = domain.Id;
				dto.TxDate = domain.TxDate;
				dto.Direction = (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.Direction)((int)domain.Direction);
				dto.State = (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.State)((int)domain.State);
				dto.Quantity = domain.Quantity;
				dto.Version = domain.Version;
				dto.CreatedBy = domain.CreatedBy;
				dto.CreatedDate = domain.CreatedDate;
				dto.LastModifiedBy = domain.LastModifiedBy;
				dto.LastModifiedDate = domain.LastModifiedDate;
				dto.SupplierId = domain.Supplier.Id;
				dto.StoreId = domain.Store.Id;
				dto.ProductId = domain.Product.Id;
				dto.CustomerId = domain.Customer.Id;
				dto.EntityRegistryId = domain.EntityRegistry == null ? (int?)null : domain.EntityRegistry.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto, Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction domain)
			{
				domain.TxDate = dto.TxDate;
				domain.Direction = (Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Direction)((int)dto.Direction);
				domain.State = (Arbinada.GenieLamp.Warehouse.Domain.Warehouse.State)((int)dto.State);
				domain.Quantity = dto.Quantity;
				domain.Version = dto.Version;
				domain.CreatedBy = dto.CreatedBy;
				domain.CreatedDate = dto.CreatedDate;
				domain.LastModifiedBy = dto.LastModifiedBy;
				domain.LastModifiedDate = dto.LastModifiedDate;
				domain.Supplier = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetById(dto.SupplierId);
				domain.Store = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store.GetById(dto.StoreId);
				domain.Product = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product.GetById(dto.ProductId);
				domain.Customer = Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor.GetById(dto.CustomerId);
				if (dto.EntityRegistryId != null)
				{
					domain.EntityRegistry = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry.GetById(dto.EntityRegistryId.Value);
				}
				else
				{
					domain.EntityRegistry = null;
				}
			}
			
			protected List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO> DomainCollectionToDTO(IList<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction> domainCollection)
			{
				List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO> dtoCollection = new List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO>();
				foreach(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction domain in domainCollection)
				{
					Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO();
					StoreTransactionService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
		}
		
		
	}
	#endregion
	
}

