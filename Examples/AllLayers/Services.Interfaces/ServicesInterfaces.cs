// Genie Lamp Core (1.1.4594.29523)
// ServiceStack services interfaces genie (1.0.4594.29525)
// Starter application (1.1.4594.29524)
// This file was automatically generated at 2012-07-30 16:36:49
// Do not modify it manually.

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
// Assembly required: ServiceStack.Interfaces.dll
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace Arbinada.GenieLamp.Warehouse.Services.Interfaces
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
	[RestService("/Persistence")]
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
	    [DataMember]
	    public string Name { get; set; }
	    [DataMember]
	    public object Value { get; set; }
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
	namespace Core
	{
		[DataContract]
		[RestService("//EntityTypeService")]
		[RestService("//EntityTypeService/Id/{Id}")]
		[RestService("//EntityTypeService/FullName/{FullName}")]
		[RestService("//EntityTypeService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class EntityTypeRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string FullName { get; set; }
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
			public EntityTypeDTO EntityTypeDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class EntityTypeResponse
		{
			[DataMember]
			public EntityTypeDTO EntityTypeDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public EntityTypeResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class EntityTypeListResponse
		{
			[DataMember]
			public List<EntityTypeDTO> EntityTypeDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public EntityTypeListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.EntityTypeDTOList = new List<EntityTypeDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//EntityRegistryService")]
		[RestService("//EntityRegistryService/Id/{Id}")]
		[RestService("//EntityRegistryService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class EntityRegistryRequest
		{
			[DataMember]
			public int? Id { get; set; }
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
			public EntityRegistryDTO EntityRegistryDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class EntityRegistryResponse
		{
			[DataMember]
			public EntityRegistryDTO EntityRegistryDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public EntityRegistryResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class EntityRegistryListResponse
		{
			[DataMember]
			public List<EntityRegistryDTO> EntityRegistryDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public EntityRegistryListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.EntityRegistryDTOList = new List<EntityRegistryDTO>();
			}
			#endregion
		}
		
		
	}
	
	namespace Warehouse
	{
		[DataContract]
		[RestService("//ExampleOneToOneService")]
		[RestService("//ExampleOneToOneService/Id/{Id}")]
		[RestService("//ExampleOneToOneService/Name/{Name}")]
		[RestService("//ExampleOneToOneService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//ExampleOneToOneService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class ExampleOneToOneRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Name { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public ExampleOneToOneDTO ExampleOneToOneDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class ExampleOneToOneResponse
		{
			[DataMember]
			public ExampleOneToOneDTO ExampleOneToOneDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ExampleOneToOneResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class ExampleOneToOneListResponse
		{
			[DataMember]
			public List<ExampleOneToOneDTO> ExampleOneToOneDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ExampleOneToOneListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.ExampleOneToOneDTOList = new List<ExampleOneToOneDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//ExampleOneToOneExService")]
		[RestService("//ExampleOneToOneExService/Id/{Id}")]
		[RestService("//ExampleOneToOneExService/ExempleOneToOneId/{ExempleOneToOneId}")]
		[RestService("//ExampleOneToOneExService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//ExampleOneToOneExService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class ExampleOneToOneExRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public int? ExempleOneToOneId { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public ExampleOneToOneExDTO ExampleOneToOneExDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class ExampleOneToOneExResponse
		{
			[DataMember]
			public ExampleOneToOneExDTO ExampleOneToOneExDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ExampleOneToOneExResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class ExampleOneToOneExListResponse
		{
			[DataMember]
			public List<ExampleOneToOneExDTO> ExampleOneToOneExDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ExampleOneToOneExListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.ExampleOneToOneExDTOList = new List<ExampleOneToOneExDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//ProductTypeService")]
		[RestService("//ProductTypeService/Id/{Id}")]
		[RestService("//ProductTypeService/Code/{Code}")]
		[RestService("//ProductTypeService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//ProductTypeService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class ProductTypeRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Code { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public ProductTypeDTO ProductTypeDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class ProductTypeResponse
		{
			[DataMember]
			public ProductTypeDTO ProductTypeDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ProductTypeResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class ProductTypeListResponse
		{
			[DataMember]
			public List<ProductTypeDTO> ProductTypeDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ProductTypeListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.ProductTypeDTOList = new List<ProductTypeDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//ProductService")]
		[RestService("//ProductService/Id/{Id}")]
		[RestService("//ProductService/RefCode/{RefCode}")]
		[RestService("//ProductService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//ProductService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class ProductRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string RefCode { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
		[RestService("//StoreTypeService")]
		[RestService("//StoreTypeService/Id/{Id}")]
		[RestService("//StoreTypeService/Name/{Name}")]
		[RestService("//StoreTypeService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//StoreTypeService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class StoreTypeRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Name { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public StoreTypeDTO StoreTypeDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class StoreTypeResponse
		{
			[DataMember]
			public StoreTypeDTO StoreTypeDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreTypeResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class StoreTypeListResponse
		{
			[DataMember]
			public List<StoreTypeDTO> StoreTypeDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreTypeListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.StoreTypeDTOList = new List<StoreTypeDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//StoreService")]
		[RestService("//StoreService/Id/{Id}")]
		[RestService("//StoreService/Code/{Code}")]
		[RestService("//StoreService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//StoreService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		[RestService("//StoreService/Check")]
		[RestService("//StoreService/RecordReceived")]
		[RestService("//StoreService/GetQuantity")]
		public partial class StoreRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Code { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public StoreDTO StoreDTO { get; set; }
			[DataMember]
			public StoreCheckParams CheckParams { get; set; }
			[DataMember]
			public StoreRecordReceivedParams RecordReceivedParams { get; set; }
			[DataMember]
			public StoreGetQuantityParams GetQuantityParams { get; set; }
			
		}
		
		public class StoreCheckParams
		{
		}
		
		public class StoreRecordReceivedParams
		{
			[DataMember]
			public int productId { get; set; }
			[DataMember]
			public int qty { get; set; }
			[DataMember]
			public decimal price { get; set; }
			[DataMember]
			public DateTime date { get; set; }
			[DataMember]
			public bool Result { get; set; }
		}
		
		public class StoreGetQuantityParams
		{
			[DataMember]
			public System.Collections.Generic.IList<int> product { get; set; }
			[DataMember]
			public DateTime date { get; set; }
			[DataMember]
			public int[] Result { get; set; }
		}
		
		
		[DataContract]
		public partial class StoreResponse
		{
			[DataMember]
			public StoreDTO StoreDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			[DataMember]
			public StoreCheckParams CheckParams { get; set; }
			[DataMember]
			public StoreRecordReceivedParams RecordReceivedParams { get; set; }
			[DataMember]
			public StoreGetQuantityParams GetQuantityParams { get; set; }
			
			#region Constructors
			public StoreResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class StoreListResponse
		{
			[DataMember]
			public List<StoreDTO> StoreDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.StoreDTOList = new List<StoreDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//ContractorService")]
		[RestService("//ContractorService/Id/{Id}")]
		[RestService("//ContractorService/Name/{Name}")]
		[RestService("//ContractorService/Phone/{Phone}")]
		[RestService("//ContractorService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//ContractorService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class ContractorRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string Name { get; set; }
			[DataMember]
			public string Phone { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public ContractorDTO ContractorDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class ContractorResponse
		{
			[DataMember]
			public ContractorDTO ContractorDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ContractorResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class ContractorListResponse
		{
			[DataMember]
			public List<ContractorDTO> ContractorDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public ContractorListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.ContractorDTOList = new List<ContractorDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//PartnerService")]
		[RestService("//PartnerService/Id/{Id}")]
		[RestService("//PartnerService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class PartnerRequest
		{
			[DataMember]
			public int? Id { get; set; }
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
			public PartnerDTO PartnerDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class PartnerResponse
		{
			[DataMember]
			public PartnerDTO PartnerDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public PartnerResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class PartnerListResponse
		{
			[DataMember]
			public List<PartnerDTO> PartnerDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public PartnerListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.PartnerDTOList = new List<PartnerDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//StoreDocService")]
		[RestService("//StoreDocService/Id/{Id}")]
		[RestService("//StoreDocService/RefNum/{RefNum}")]
		[RestService("//StoreDocService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//StoreDocService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class StoreDocRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public string RefNum { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public StoreDocDTO StoreDocDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class StoreDocResponse
		{
			[DataMember]
			public StoreDocDTO StoreDocDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreDocResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class StoreDocListResponse
		{
			[DataMember]
			public List<StoreDocDTO> StoreDocDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreDocListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.StoreDocDTOList = new List<StoreDocDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//StoreDocLineService")]
		[RestService("//StoreDocLineService/StoreDocId/{StoreDocId}/Position/{Position}")]
		[RestService("//StoreDocLineService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//StoreDocLineService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class StoreDocLineRequest
		{
			[DataMember]
			public int? StoreDocId { get; set; }
			[DataMember]
			public int? Position { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public StoreDocLineDTO StoreDocLineDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class StoreDocLineResponse
		{
			[DataMember]
			public StoreDocLineDTO StoreDocLineDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreDocLineResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class StoreDocLineListResponse
		{
			[DataMember]
			public List<StoreDocLineDTO> StoreDocLineDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreDocLineListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.StoreDocLineDTOList = new List<StoreDocLineDTO>();
			}
			#endregion
		}
		
		
		[DataContract]
		[RestService("//StoreTransactionService")]
		[RestService("//StoreTransactionService/Id/{Id}")]
		[RestService("//StoreTransactionService/EntityRegistryId/{EntityRegistryId}")]
		[RestService("//StoreTransactionService/Gl_PageNum/{Gl_PageNum}/Gl_PageSize/{Gl_PageSize}/Gl_OrderBy/{Gl_OrderBy}/Gl_OrderAsc/{Gl_OrderAsc}")]
		public partial class StoreTransactionRequest
		{
			[DataMember]
			public int? Id { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
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
			public StoreTransactionDTO StoreTransactionDTO { get; set; }
			
		}
		
		
		[DataContract]
		public partial class StoreTransactionResponse
		{
			[DataMember]
			public StoreTransactionDTO StoreTransactionDTO { get; set; }
			[DataMember]
			public CommitResult CommitResult { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreTransactionResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.CommitResult = new CommitResult();
			}
			#endregion
		}
		
		[DataContract]
		public partial class StoreTransactionListResponse
		{
			[DataMember]
			public List<StoreTransactionDTO> StoreTransactionDTOList { get; set; }
			[DataMember]
			public ResponseStatus ResponseStatus { get; set; }
			
			#region Constructors
			public StoreTransactionListResponse()
			{
				this.ResponseStatus = new ResponseStatus();
				this.StoreTransactionDTOList = new List<StoreTransactionDTO>();
			}
			#endregion
		}
		
		
	}
	#endregion
	
}

