// Genie Lamp Core (1.1.4798.27721)
// ServiceStack services genie (1.0.4798.27724)
// Starter application (1.1.4798.27722)
// This file was automatically generated at 2013-03-14 16:56:47
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

using GenieLamp.Examples.QuickStart.Services.Interfaces;

namespace GenieLamp.Examples.QuickStart.Services
{
	public class DomainQueryFactory
	{
		public static GenieLamp.Examples.QuickStart.Queries.DomainQueryParams ToDomainQueryParams(ServicesQueryParams queryParams)
		{
			GenieLamp.Examples.QuickStart.Queries.DomainQueryParams domainQueryParams = null;
	        if (queryParams != null)
	        {
	            domainQueryParams = new GenieLamp.Examples.QuickStart.Queries.DomainQueryParams();
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
			GenieLamp.Examples.QuickStart.Persistence.UnitOfWork uow = new GenieLamp.Examples.QuickStart.Persistence.UnitOfWork();
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
				case DomainTypes.TypeCustomer:
				{
					wi.DomainObject = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetById((wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer();
					}
					GenieLamp.Examples.QuickStart.Services.QuickStart.CustomerService.DTOToDomainObject(wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO, wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer);
					break;
				}
				case DomainTypes.TypeProduct:
				{
					wi.DomainObject = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetById((wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new GenieLamp.Examples.QuickStart.Domain.QuickStart.Product();
					}
					GenieLamp.Examples.QuickStart.Services.QuickStart.ProductService.DTOToDomainObject(wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO, wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.Product);
					break;
				}
				case DomainTypes.TypePurchaseOrder:
				{
					wi.DomainObject = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetById((wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder();
					}
					GenieLamp.Examples.QuickStart.Services.QuickStart.PurchaseOrderService.DTOToDomainObject(wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO, wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder);
					break;
				}
				case DomainTypes.TypePurchaseOrderLine:
				{
					wi.DomainObject = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetById((wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO).Id);
					if (wi.DomainObject == null)
					{
						wi.DomainObject = new GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine();
					}
					GenieLamp.Examples.QuickStart.Services.QuickStart.PurchaseOrderLineService.DTOToDomainObject(wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO, wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine);
					break;
				}
				default: throw new ApplicationException("Cannot save non-persistent object");
			}
		}
		
		private static void InitDTO(UnitOfWorkDTO.WorkItem wi)
		{
			switch((DomainTypes)wi.Item.GetInternal_DomainTypeId())
			{
				case DomainTypes.TypeCustomer:
				{
					(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer).Refresh();
					GenieLamp.Examples.QuickStart.Services.QuickStart.CustomerService.DomainObjectToDTO(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer, wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO);
					break;
				}
				case DomainTypes.TypeProduct:
				{
					(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.Product).Refresh();
					GenieLamp.Examples.QuickStart.Services.QuickStart.ProductService.DomainObjectToDTO(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.Product, wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO);
					break;
				}
				case DomainTypes.TypePurchaseOrder:
				{
					(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder).Refresh();
					GenieLamp.Examples.QuickStart.Services.QuickStart.PurchaseOrderService.DomainObjectToDTO(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder, wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO);
					break;
				}
				case DomainTypes.TypePurchaseOrderLine:
				{
					(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine).Refresh();
					GenieLamp.Examples.QuickStart.Services.QuickStart.PurchaseOrderLineService.DomainObjectToDTO(wi.DomainObject as GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine, wi.Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO);
					break;
				}
				default: throw new ApplicationException("Cannot process non-persistent object");
			}
		}
	}
	
	#region Entities
	namespace QuickStart
	{
		public partial class CustomerService : RestServiceBase<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerRequest>, IRequiresRequestContext
		{
			public override object OnGet(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse();
				response.ResponseStatus.ErrorCode = "200";
				GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetById(request.Id.Value);
					}
					else if (request.Code != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetByCode(request.Code);
					}
					else
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new GenieLamp.Examples.QuickStart.Queries.SortOrder[] { new GenieLamp.Examples.QuickStart.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetPage(0, 300, null);
							}
							responseList.CustomerDTOList = DomainCollectionToDTO(list);
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
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO();
						CustomerService.DomainObjectToDTO(domain, dto);
						response.CustomerDTO = dto;
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
			
			public override object OnDelete(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.DeleteById(request.Id.Value);
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
			
			protected List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO> DomainCollectionToDTO(IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer> domainCollection)
			{
				List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO> dtoCollection = new List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO>();
				foreach(GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer domain in domainCollection)
				{
					GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO();
					CustomerService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
			
			public override object OnPost(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.CustomerDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.CustomerDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.CustomerDTO = (uow.WorkItems[0].Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer> list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 300);
							responseList.CustomerDTOList = DomainCollectionToDTO(list);
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
			
			internal static void DomainObjectToDTO(GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer domain, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto)
			{
				dto.Id = domain.Id;
				dto.Code = domain.Code;
				dto.Name = domain.Name;
				dto.Phone = domain.Phone;
				dto.Email = domain.Email;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto, GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer domain)
			{
				domain.Code = dto.Code;
				domain.Name = dto.Name;
				domain.Phone = dto.Phone;
				domain.Email = dto.Email;
			}
			
		}
		
		
		public partial class ProductService : RestServiceBase<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductRequest>, IRequiresRequestContext
		{
			public override object OnGet(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse();
				response.ResponseStatus.ErrorCode = "200";
				GenieLamp.Examples.QuickStart.Domain.QuickStart.Product domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetById(request.Id.Value);
					}
					else if (request.Reference != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetByReference(request.Reference);
					}
					else
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.Product> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new GenieLamp.Examples.QuickStart.Queries.SortOrder[] { new GenieLamp.Examples.QuickStart.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetPage(0, 300, null);
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
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO();
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
			
			public override object OnDelete(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.DeleteById(request.Id.Value);
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
			
			protected List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO> DomainCollectionToDTO(IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.Product> domainCollection)
			{
				List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO> dtoCollection = new List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO>();
				foreach(GenieLamp.Examples.QuickStart.Domain.QuickStart.Product domain in domainCollection)
				{
					GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO();
					ProductService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
			
			public override object OnPost(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.ProductDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.ProductDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.ProductDTO = (uow.WorkItems[0].Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.Product> list = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 300);
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
			
			internal static void DomainObjectToDTO(GenieLamp.Examples.QuickStart.Domain.QuickStart.Product domain, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto)
			{
				dto.Id = domain.Id;
				dto.Reference = domain.Reference;
				dto.Name = domain.Name;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto, GenieLamp.Examples.QuickStart.Domain.QuickStart.Product domain)
			{
				domain.Reference = dto.Reference;
				domain.Name = dto.Name;
			}
			
		}
		
		
		public partial class PurchaseOrderService : RestServiceBase<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest>, IRequiresRequestContext
		{
			public override object OnGet(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse();
				response.ResponseStatus.ErrorCode = "200";
				GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetById(request.Id.Value);
					}
					else if (request.Number != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetByNumber(request.Number);
					}
					else if (request.CustomerId != null)
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder> list = 
								GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetCollectionByCustomerId(request.CustomerId.Value);
							responseList.PurchaseOrderDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					else
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new GenieLamp.Examples.QuickStart.Queries.SortOrder[] { new GenieLamp.Examples.QuickStart.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetPage(0, 300, null);
							}
							responseList.PurchaseOrderDTOList = DomainCollectionToDTO(list);
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
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO();
						PurchaseOrderService.DomainObjectToDTO(domain, dto);
						response.PurchaseOrderDTO = dto;
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
			
			public override object OnDelete(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.DeleteById(request.Id.Value);
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
			
			protected List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO> DomainCollectionToDTO(IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder> domainCollection)
			{
				List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO> dtoCollection = new List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO>();
				foreach(GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder domain in domainCollection)
				{
					GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO();
					PurchaseOrderService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
			
			public override object OnPost(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.ValidateParams != null)
					{
						GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder domain = null;
						if (request.PurchaseOrderDTO != null) domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetById(request.PurchaseOrderDTO.Id);
						if (domain == null) domain = new GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder();
						DTOToDomainObject(request.PurchaseOrderDTO, domain);
						domain.Save();
						domain.Validate();
						DomainObjectToDTO(domain, request.PurchaseOrderDTO);
						response.PurchaseOrderDTO = request.PurchaseOrderDTO;
						response.ValidateParams = request.ValidateParams;
					}
					else if (request.PurchaseOrderDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.PurchaseOrderDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.PurchaseOrderDTO = (uow.WorkItems[0].Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder> list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 300);
							responseList.PurchaseOrderDTOList = DomainCollectionToDTO(list);
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
			
			internal static void DomainObjectToDTO(GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder domain, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto)
			{
				dto.Id = domain.Id;
				dto.Number = domain.Number;
				dto.IssueDate = domain.IssueDate;
				dto.Validated = domain.Validated;
				dto.ShipmentDate = domain.ShipmentDate;
				dto.TotalAmount = domain.TotalAmount;
				dto.CustomerId = domain.Customer.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto, GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder domain)
			{
				domain.Number = dto.Number;
				domain.IssueDate = dto.IssueDate;
				domain.Validated = dto.Validated;
				domain.ShipmentDate = dto.ShipmentDate;
				domain.Customer = GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer.GetById(dto.CustomerId);
			}
			
		}
		
		
		public partial class PurchaseOrderLineService : RestServiceBase<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest>, IRequiresRequestContext
		{
			public override object OnGet(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse();
				response.ResponseStatus.ErrorCode = "200";
				GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine domain = null;
				try
				{
					if (request.Id != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetById(request.Id.Value);
					}
					else if (request.PurchaseOrderId != null && request.Position != null)
					{
						domain = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetByPurchaseOrderIdPosition(request.PurchaseOrderId.Value, request.Position.Value);
					}
					else if (request.PurchaseOrderId != null)
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine> list = 
								GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetCollectionByPurchaseOrderId(request.PurchaseOrderId.Value);
							responseList.PurchaseOrderLineDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					else if (request.ProductId != null)
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine> list = 
								GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetCollectionByProductId(request.ProductId.Value);
							responseList.PurchaseOrderLineDTOList = DomainCollectionToDTO(list);
						}
						catch(Exception e)
						{
							responseList.ResponseStatus.ErrorCode = "400";
							responseList.ResponseStatus.Message = e.Message + '\n' + e.ToString();
							responseList.ResponseStatus.StackTrace = e.ToString();
						}
						return responseList;
					}
					else
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine> list = null;
							if (request.Gl_PageNum != null && request.Gl_PageSize != null)
							{
								if (request.Gl_OrderBy != null && request.Gl_OrderAsc != null)
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, new GenieLamp.Examples.QuickStart.Queries.SortOrder[] { new GenieLamp.Examples.QuickStart.Queries.SortOrder(request.Gl_OrderBy, request.Gl_OrderAsc.Value) });
								}
								else
								{
									list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetPage(request.Gl_PageNum.Value, request.Gl_PageSize.Value, null);
								}
							}
							else
							{
								list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetPage(0, 300, null);
							}
							responseList.PurchaseOrderLineDTOList = DomainCollectionToDTO(list);
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
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO();
						PurchaseOrderLineService.DomainObjectToDTO(domain, dto);
						response.PurchaseOrderLineDTO = dto;
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
			
			public override object OnDelete(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.Id != null)
					{
						try
						{
							GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.DeleteById(request.Id.Value);
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
			
			protected List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO> DomainCollectionToDTO(IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine> domainCollection)
			{
				List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO> dtoCollection = new List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO>();
				foreach(GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine domain in domainCollection)
				{
					GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO();
					PurchaseOrderLineService.DomainObjectToDTO(domain, dto);
					dtoCollection.Add(dto);
				}
				return dtoCollection;
			}
			
			public override object OnPost(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest request)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse response = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse();
				response.ResponseStatus.ErrorCode = "200";
				try
				{
					if (request.PurchaseOrderLineDTO != null)
					{
						UnitOfWorkDTO uow = new UnitOfWorkDTO();
						uow.Save(request.PurchaseOrderLineDTO);
						response.CommitResult = PersistenceService.Commit(uow, null);
						response.PurchaseOrderLineDTO = (uow.WorkItems[0].Item as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO);
						if (response.CommitResult.HasError == true)
						{
							response.ResponseStatus.ErrorCode = "400";
							response.ResponseStatus.Message = "Error saving object";
						}
					}
					else if (request.Gl_Query != null)
					{
						GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse responseList = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse();
						responseList.ResponseStatus.ErrorCode = "200";
						try
						{
							IList<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine> list = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine.GetPageByHQL(request.Gl_Query, DomainQueryFactory.ToDomainQueryParams(request.Gl_QueryParams), request.Gl_PageNum != null ? request.Gl_PageNum.Value : 0, request.Gl_PageSize != null ? request.Gl_PageSize.Value : 300);
							responseList.PurchaseOrderLineDTOList = DomainCollectionToDTO(list);
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
			
			internal static void DomainObjectToDTO(GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine domain, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto)
			{
				dto.Id = domain.Id;
				dto.Position = domain.Position;
				dto.Price = domain.Price;
				dto.Quantity = domain.Quantity;
				dto.PurchaseOrderId = domain.PurchaseOrder.Id;
				dto.ProductId = domain.Product.Id;
				dto.Changed = false;
			}
			
			internal static void DTOToDomainObject(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto, GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine domain)
			{
				domain.Position = dto.Position;
				domain.Price = dto.Price;
				domain.Quantity = dto.Quantity;
				domain.PurchaseOrder = GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder.GetById(dto.PurchaseOrderId);
				domain.Product = GenieLamp.Examples.QuickStart.Domain.QuickStart.Product.GetById(dto.ProductId);
			}
			
		}
		
		
	}
	#endregion
	
}

