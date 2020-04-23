// Genie Lamp Core (1.1.4798.27721)
// ServiceStack services interfaces genie (1.0.4798.27724)
// Starter application (1.1.4798.27722)
// This file was automatically generated at 2013-03-14 16:56:47
// Do not modify it manually.

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
// Assembly required: ServiceStack.Interfaces.dll
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace GenieLamp.Examples.QuickStart.Services.Interfaces
{
	[DataContract]
	public class CommitResult
	{
	    public CommitResult()
	    {
	        HasError = false;
	        Message = String.Empty;
	        ExceptionString = String.Empty;
	    }
	
	    [DataMember]
	    public bool HasError { get; set; }
	    [DataMember]
	    public string Message { get; set; }
	    [DataMember]
	    public string ExceptionString { get; set; }
	}
	
	
	public class CommitException : Exception
	{
		public CommitException(CommitResult result) : base(result.Message)
		{
			this.CommitResult = result;
		}
	
		public CommitResult CommitResult { get; private set; }
	}
	
	
	[DataContract]
	public class UnitOfWorkDTO
	{
	    [DataContract]
	    public enum Action
	    {
	        [EnumMember] Save,
	        [EnumMember] Delete
	    }
	
	    [DataContract]
	    public class WorkItem
	    {
	        public WorkItem()
	        { }
	
	        public WorkItem(PersistentObjectDTO item, Action action)
	        {
				this.Item = item;
				this.Action = action;
			}
	
	        [DataMember]
	        public PersistentObjectDTO Item { get; set; }
	        [DataMember]
	        public Action Action { get; set; }
			public object DomainObject { get; set; }
	    }
	
	    [DataMember]
	    public List<WorkItem> WorkItems { get; set; }
	
	    public UnitOfWorkDTO()
	    {
	        WorkItems = new List<WorkItem>();
	    }
	
		public void Save(PersistentObjectDTO workItem)
		{
			WorkItems.Add(new WorkItem(workItem, Action.Save));
		}
	
		public void Delete(PersistentObjectDTO workItem)
		{
			WorkItems.Add(new WorkItem(workItem, Action.Delete));
		}
	}
	
	[DataContract]
	[Route("/Persistence")]
	public class PersistenceRequest
	{
	    [DataMember]
	    public UnitOfWorkDTO UnitOfWork { get; set; }
	}
	
	[DataContract]
	public class UpdatedObjects :
		Dictionary<string, PersistentObjectDTO>,
		IEnumerable<PersistentObjectDTO>
	{
		public bool Update<T>(ref T dto) where T : DomainObjectDTO
		{
			if (dto != null)
			{
				PersistentObjectDTO found = null;
				if (this.TryGetValue(dto.Internal_ObjectId, out found))
				{
				    dto = found as T;
	                return true;
	            }
			}
	        return false;
		}
	
	    public new IEnumerator<PersistentObjectDTO> GetEnumerator()
	    {
	        return this.Values.GetEnumerator();
	    }
	}
	
	
	[DataContract]
	public class PersistenceResponse : IHasResponseStatus
	{
	    public PersistenceResponse()
	    {
	        this.ResponseStatus = new ResponseStatus();
	        this.CommitResult = new CommitResult();
	        this.UpdatedObjects = new UpdatedObjects();
	    }
	
	    [DataMember]
	    public UpdatedObjects UpdatedObjects { get; set; }
	    [DataMember]
	    public CommitResult CommitResult { get; set; }
	
	    [DataMember]
	    public ResponseStatus ResponseStatus { get; set; }
	}
	
	
	[DataContract]
	public class ServicesQueryParam
	{
	    public ServicesQueryParam(string name, object value)
	    {
	        this.Name = name;
	        this.Value = value;
	    }
	
		[DataContract]
		public enum QueryParamType
		{
		    [EnumMember]ParamTypeObject,
		    [EnumMember]ParamTypeString,
		    [EnumMember]ParamTypeInt,
		    [EnumMember]ParamTypeDecimal,
		    [EnumMember]ParamTypeFloat,
		    [EnumMember]ParamTypeBool,
		    [EnumMember]ParamTypeDateTime
		}
	
	    [DataMember]
	    public string Name { get; set; }
		[DataMember]
		public QueryParamType ParamType { get; set; }
		[DataMember]
		public object ObjectValue { get; set; }
		[DataMember]
		public string StringValue { get; set; }
		[DataMember]
		public int IntValue { get; set; }
		[DataMember]
		public decimal DecimalValue { get; set; }
		[DataMember]
		public float FloatValue { get; set; }
		[DataMember]
		public bool BoolValue { get; set; }
		[DataMember]
		public DateTime DateTimeValue { get; set; }
	
		public object Value
		{
		    get
		    {
		        switch (this.ParamType)
		        {
		            case QueryParamType.ParamTypeString:
		                return StringValue;
		            case QueryParamType.ParamTypeInt:
		                return IntValue;
		            case QueryParamType.ParamTypeDecimal:
		                return DecimalValue;
		            case QueryParamType.ParamTypeFloat:
		                return FloatValue;
		            case QueryParamType.ParamTypeBool:
		                return BoolValue;
		            case QueryParamType.ParamTypeDateTime:
		                return DateTimeValue;
		            default:
		                return ObjectValue;
		        }
		    }
		    private set
		    {
		        if (value is int)
		        {
		            IntValue = (int)value;
		            this.ParamType = QueryParamType.ParamTypeInt;
		        }
		        else if (value is string)
		        {
		            StringValue = value.ToString();
		            this.ParamType = QueryParamType.ParamTypeString;
		        }
		        else if (value is decimal)
		        {
		            DecimalValue = (decimal)value;
		            this.ParamType = QueryParamType.ParamTypeDecimal;
		        }
		        else if (value is float)
		        {
		            FloatValue = (float)value;
		            this.ParamType = QueryParamType.ParamTypeFloat;
		        }
		        else if (value is bool)
		        {
		            BoolValue = (bool)value;
		            this.ParamType = QueryParamType.ParamTypeBool;
		        }
		        else if (value is DateTime)
		        {
		            DateTimeValue = (DateTime)value;
		            this.ParamType = QueryParamType.ParamTypeDateTime;
		        }
		        else
		        {
		            ObjectValue = value;
		            this.ParamType = QueryParamType.ParamTypeObject;
		        }
		    }
		}
	}
	
	public class ServicesQueryParams : System.Collections.Generic.List<ServicesQueryParam>
	{
		public static ServicesQueryParams CreateParams()
		{
			return new ServicesQueryParams();
		}
	
		public ServicesQueryParams AddParam(string name, object value)
		{
			this.Add(new ServicesQueryParam(name, value));
			return this;
		}
	}
	
	
	
	#region Entities
	namespace QuickStart
	{
		[DataContract]
		[Route("/CustomerService")]
		[Route("/CustomerService/Id/{Id}")]
		[Route("/CustomerService/Code/{Code}")]
		[Route("/CustomerService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class CustomerRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Code { get; set; }
			[DataMember]
			public string Gl_Query { get; set; }
			[DataMember]
			public ServicesQueryParams Gl_QueryParams { get; set; }
			[DataMember]
			public int? Gl_PageNum { get; set; }
			[DataMember]
			public int? Gl_PageSize { get; set; }
			[DataMember]
			public string Gl_OrderBy { get; set; }
			[DataMember]
			public bool? Gl_OrderAsc { get; set; }
			[DataMember]
			public CustomerDTO CustomerDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class CustomerResponse
		{
			[DataMember]
			public CustomerDTO CustomerDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public CustomerResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class CustomerListResponse
		{
			[DataMember]
			public List<CustomerDTO> CustomerDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public CustomerListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CustomerDTOList = new List<CustomerDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[Route("/ProductService")]
		[Route("/ProductService/Id/{Id}")]
		[Route("/ProductService/Reference/{Reference}")]
		[Route("/ProductService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class ProductRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Reference { get; set; }
			[DataMember]
			public string Gl_Query { get; set; }
			[DataMember]
			public ServicesQueryParams Gl_QueryParams { get; set; }
			[DataMember]
			public int? Gl_PageNum { get; set; }
			[DataMember]
			public int? Gl_PageSize { get; set; }
			[DataMember]
			public string Gl_OrderBy { get; set; }
			[DataMember]
			public bool? Gl_OrderAsc { get; set; }
			[DataMember]
			public ProductDTO ProductDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class ProductResponse
		{
			[DataMember]
			public ProductDTO ProductDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ProductResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class ProductListResponse
		{
			[DataMember]
			public List<ProductDTO> ProductDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ProductListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.ProductDTOList = new List<ProductDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[Route("/PurchaseOrderService")]
		[Route("/PurchaseOrderService/Id/{Id}")]
		[Route("/PurchaseOrderService/Number/{Number}")]
		[Route("/PurchaseOrderService/CustomerId/{CustomerId}")]
		[Route("/PurchaseOrderService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		[Route("/PurchaseOrderService/Validate")]
		public partial class PurchaseOrderRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Number { get; set; }
			[DataMember]
			public int? CustomerId { get; set; }
			[DataMember]
			public string Gl_Query { get; set; }
			[DataMember]
			public ServicesQueryParams Gl_QueryParams { get; set; }
			[DataMember]
			public int? Gl_PageNum { get; set; }
			[DataMember]
			public int? Gl_PageSize { get; set; }
			[DataMember]
			public string Gl_OrderBy { get; set; }
			[DataMember]
			public bool? Gl_OrderAsc { get; set; }
			[DataMember]
			public PurchaseOrderDTO PurchaseOrderDTO { get; set; }
			[DataMember]
			public PurchaseOrderValidateParams ValidateParams { get; set; }
			
		}
		
		public class PurchaseOrderValidateParams
		{
		}
		
		
		[DataContract]
		public partial class PurchaseOrderResponse
		{
			[DataMember]
			public PurchaseOrderDTO PurchaseOrderDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			[DataMember]
			public PurchaseOrderValidateParams ValidateParams { get; set; }
			
			#region Constructors
			public PurchaseOrderResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class PurchaseOrderListResponse
		{
			[DataMember]
			public List<PurchaseOrderDTO> PurchaseOrderDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public PurchaseOrderListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.PurchaseOrderDTOList = new List<PurchaseOrderDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[Route("/PurchaseOrderLineService")]
		[Route("/PurchaseOrderLineService/Id/{Id}")]
		[Route("/PurchaseOrderLineService/PurchaseOrderId/{PurchaseOrderId}/Position/{Position}")]
		[Route("/PurchaseOrderLineService/PurchaseOrderId/{PurchaseOrderId}")]
		[Route("/PurchaseOrderLineService/ProductId/{ProductId}")]
		[Route("/PurchaseOrderLineService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class PurchaseOrderLineRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public int? PurchaseOrderId { get; set; }
			[DataMember]
			public short? Position { get; set; }
			[DataMember]
			public int? ProductId { get; set; }
			[DataMember]
			public string Gl_Query { get; set; }
			[DataMember]
			public ServicesQueryParams Gl_QueryParams { get; set; }
			[DataMember]
			public int? Gl_PageNum { get; set; }
			[DataMember]
			public int? Gl_PageSize { get; set; }
			[DataMember]
			public string Gl_OrderBy { get; set; }
			[DataMember]
			public bool? Gl_OrderAsc { get; set; }
			[DataMember]
			public PurchaseOrderLineDTO PurchaseOrderLineDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class PurchaseOrderLineResponse
		{
			[DataMember]
			public PurchaseOrderLineDTO PurchaseOrderLineDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public PurchaseOrderLineResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class PurchaseOrderLineListResponse
		{
			[DataMember]
			public List<PurchaseOrderLineDTO> PurchaseOrderLineDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public PurchaseOrderLineListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.PurchaseOrderLineDTOList = new List<PurchaseOrderLineDTO>();
			}
			#endregion
		}
		
		
	}
	#endregion
	
}

