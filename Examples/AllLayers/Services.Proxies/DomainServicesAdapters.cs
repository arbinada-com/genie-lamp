// Genie Lamp Core (1.1.4527.19944)
// Genie of WCF Services Proxies (1.0.4527.19946)
// Starter application (1.1.4527.19945)
// This file was automatically generated at 2012-05-24 11:04:55
// Do not modify it manually.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Arbinada.GenieLamp.Warehouse.Services.Proxies
{
	public class UnitOfWork
	{
	    enum Action
	    {
	        Save,
	        Delete
	    }
	
	    class WorkItem
	    {
	        public WorkItem(PersistentProxyAdapter item, Action action)
	        {
	            this.Item = item;
	            this.Action = action;
	        }
	
	        public PersistentProxyAdapter Item { get; set; }
	        public Action Action { get; set; }
	    }
	
	    private List<WorkItem> workItems = new List<WorkItem>();
	
	    public UnitOfWork()
	    { }
	
	    public UnitOfWork(PersistentProxyAdapter objectToSave)
	        : this()
	    {
	        Save(objectToSave);
	    }
	
	    public void Save(PersistentProxyAdapter o)
	    {
	        workItems.Add(new WorkItem(o, Action.Save));
	    }
	
	    public void Delete(PersistentProxyAdapter o)
	    {
	        workItems.Add(new WorkItem(o, Action.Delete));
	    }
	
	    public Arbinada.GenieLamp.Warehouse.Services.CommitResult Commit()
	    {
	        Arbinada.GenieLamp.Warehouse.Services.CommitResult result = new Arbinada.GenieLamp.Warehouse.Services.CommitResult();
	
	        PersistenceServiceClient ps = new PersistenceServiceClient();
	        try
	        {
	            try
	            {
	                UnitOfWorkProxy uow = ps.CreateUnitOfWork();
	                List<UnitOfWorkProxy.WorkItem> workItemsProxy = uow.WorkItems.ToList<UnitOfWorkProxy.WorkItem>();
	                foreach (WorkItem workItem in workItems)
	                {
	                    UnitOfWorkProxy.WorkItem workItemProxy = new UnitOfWorkProxy.WorkItem();
	                    workItemProxy.Item = workItem.Item.Proxy;
	                    switch (workItem.Action)
	                    {
	                        case Action.Save:
	                            workItemProxy.Action = UnitOfWorkProxy.Action.Save;
	                            break;
	                        case Action.Delete:
	                            workItemProxy.Action = UnitOfWorkProxy.Action.Delete;
	                            break;
	                    }
	                    workItemsProxy.Add(workItemProxy);
	                }
	                uow.WorkItems = workItemsProxy.ToArray();
	                result = ps.Commit(uow);
	            }
	            finally
	            {
	                ps.Close();
	            }
	        }
	        catch (Exception e)
	        {
	            result.HasError = true;
	            result.Message = e.Message;
	            result.ExceptionString = e.ToString();
	        }
	        return result;
	    }
	}
	
	
	public abstract class ServiceProxyAdapter
	{
		private DomainObjectProxy domainObjectProxy;
		internal  DomainObjectProxy Proxy
		{
			get {
				return domainObjectProxy;
			}
			set {
				domainObjectProxy = value;
			}
		}
	}
	
	public abstract class PersistentProxyAdapter : ServiceProxyAdapter
	{
		internal new PersistentObjectProxy Proxy
		{
			get {
				return (PersistentObjectProxy)base.Proxy;
			}
			set {
				base.Proxy = value;
			}
		}
	}
	
	#region Entities
	namespace Warehouse
	{
		public partial class ProductType : PersistentProxyAdapter
		{
			internal new Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductTypeProxy Proxy
			{
				get {
					return (Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductTypeProxy)base.Proxy;
				}
				set {
					base.Proxy = value;
				}
			}
			
			#region Constructors
			public ProductType()
			{
				using(ProductTypeServiceClient client = new ProductTypeServiceClient())
				{
					Proxy = client.CreateProductType();
					client.Close();
				}
			}
			
			public ProductType(Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductTypeProxy source)
			{
				if (source == null) throw new NullReferenceException("Source proxy object cannot be null");
				this.Proxy = source;
			}
			#endregion
			
			public static ProductType GetById(int id)
			{
				using(ProductTypeServiceClient client = new ProductTypeServiceClient())
				{
					return new ProductType(client.GetProductTypeById(id));
				}
			}
			
			public static ProductType GetByCode(string code)
			{
				using(ProductTypeServiceClient client = new ProductTypeServiceClient())
				{
					return new ProductType(client.GetProductTypeByCode(code));
				}
			}
			
			public virtual int Id
			{
				get {
					return this.Proxy.Id;
				}
				set {
					this.Proxy.Id = value;
				}
			}
			public virtual string Code
			{
				get {
					return this.Proxy.Code;
				}
				set {
					this.Proxy.Code = value;
				}
			}
			public virtual string Name
			{
				get {
					return this.Proxy.Name;
				}
				set {
					this.Proxy.Name = value;
				}
			}
			
		}
		
		
		public partial class Product : PersistentProxyAdapter
		{
			internal new Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductProxy Proxy
			{
				get {
					return (Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductProxy)base.Proxy;
				}
				set {
					base.Proxy = value;
				}
			}
			
			#region Constructors
			public Product()
			{
				using(ProductServiceClient client = new ProductServiceClient())
				{
					Proxy = client.CreateProduct();
					client.Close();
				}
			}
			
			public Product(Arbinada.GenieLamp.Warehouse.Services.Warehouse.ProductProxy source)
			{
				if (source == null) throw new NullReferenceException("Source proxy object cannot be null");
				this.Proxy = source;
			}
			#endregion
			
			public static Product GetById(int id)
			{
				using(ProductServiceClient client = new ProductServiceClient())
				{
					return new Product(client.GetProductById(id));
				}
			}
			
			public static Product GetByRefCode(string refCode)
			{
				using(ProductServiceClient client = new ProductServiceClient())
				{
					return new Product(client.GetProductByRefCode(refCode));
				}
			}
			
			public virtual int Id
			{
				get {
					return this.Proxy.Id;
				}
				set {
					this.Proxy.Id = value;
				}
			}
			public virtual string RefCode
			{
				get {
					return this.Proxy.RefCode;
				}
				set {
					this.Proxy.RefCode = value;
				}
			}
			public virtual string Caption
			{
				get {
					return this.Proxy.Caption;
				}
				set {
					this.Proxy.Caption = value;
				}
			}
			private Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.ProductType m_Type;
			public virtual Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.ProductType Type
			{
				get {
					return this.Proxy.Type;
				}
				set {
					this.Proxy.Type = value;
				}
			}
			
		}
		
		
		public partial class Store : PersistentProxyAdapter
		{
			internal new Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreProxy Proxy
			{
				get {
					return (Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreProxy)base.Proxy;
				}
				set {
					base.Proxy = value;
				}
			}
			
			#region Constructors
			public Store()
			{
				using(StoreServiceClient client = new StoreServiceClient())
				{
					Proxy = client.CreateStore();
					client.Close();
				}
			}
			
			public Store(Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreProxy source)
			{
				if (source == null) throw new NullReferenceException("Source proxy object cannot be null");
				this.Proxy = source;
			}
			#endregion
			
			public static Store GetById(int id)
			{
				using(StoreServiceClient client = new StoreServiceClient())
				{
					return new Store(client.GetStoreById(id));
				}
			}
			
			public static Store GetByCode(string code)
			{
				using(StoreServiceClient client = new StoreServiceClient())
				{
					return new Store(client.GetStoreByCode(code));
				}
			}
			
			public virtual int Id
			{
				get {
					return this.Proxy.Id;
				}
				set {
					this.Proxy.Id = value;
				}
			}
			public virtual string Code
			{
				get {
					return this.Proxy.Code;
				}
				set {
					this.Proxy.Code = value;
				}
			}
			public virtual string Caption
			{
				get {
					return this.Proxy.Caption;
				}
				set {
					this.Proxy.Caption = value;
				}
			}
			
		}
		
		
		public partial class Contractor : PersistentProxyAdapter
		{
			internal new Arbinada.GenieLamp.Warehouse.Services.Warehouse.ContractorProxy Proxy
			{
				get {
					return (Arbinada.GenieLamp.Warehouse.Services.Warehouse.ContractorProxy)base.Proxy;
				}
				set {
					base.Proxy = value;
				}
			}
			
			#region Constructors
			public Contractor()
			{
				using(ContractorServiceClient client = new ContractorServiceClient())
				{
					Proxy = client.CreateContractor();
					client.Close();
				}
			}
			
			public Contractor(Arbinada.GenieLamp.Warehouse.Services.Warehouse.ContractorProxy source)
			{
				if (source == null) throw new NullReferenceException("Source proxy object cannot be null");
				this.Proxy = source;
			}
			#endregion
			
			public static Contractor GetById(int id)
			{
				using(ContractorServiceClient client = new ContractorServiceClient())
				{
					return new Contractor(client.GetContractorById(id));
				}
			}
			
			public static Contractor GetByName(string name)
			{
				using(ContractorServiceClient client = new ContractorServiceClient())
				{
					return new Contractor(client.GetContractorByName(name));
				}
			}
			
			public static Contractor GetByPhone(string phone)
			{
				using(ContractorServiceClient client = new ContractorServiceClient())
				{
					return new Contractor(client.GetContractorByPhone(phone));
				}
			}
			
			public virtual int Id
			{
				get {
					return this.Proxy.Id;
				}
				set {
					this.Proxy.Id = value;
				}
			}
			public virtual string Name
			{
				get {
					return this.Proxy.Name;
				}
				set {
					this.Proxy.Name = value;
				}
			}
			public virtual string Address
			{
				get {
					return this.Proxy.Address;
				}
				set {
					this.Proxy.Address = value;
				}
			}
			public virtual string Phone
			{
				get {
					return this.Proxy.Phone;
				}
				set {
					this.Proxy.Phone = value;
				}
			}
			public virtual string Email
			{
				get {
					return this.Proxy.Email;
				}
				set {
					this.Proxy.Email = value;
				}
			}
			
		}
		
		
		public partial class Partner : Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Contractor
		{
			internal new Arbinada.GenieLamp.Warehouse.Services.Warehouse.PartnerProxy Proxy
			{
				get {
					return (Arbinada.GenieLamp.Warehouse.Services.Warehouse.PartnerProxy)base.Proxy;
				}
				set {
					base.Proxy = value;
				}
			}
			
			#region Constructors
			public Partner()
			{
				using(PartnerServiceClient client = new PartnerServiceClient())
				{
					Proxy = client.CreatePartner();
					client.Close();
				}
			}
			
			public Partner(Arbinada.GenieLamp.Warehouse.Services.Warehouse.PartnerProxy source)
			{
				if (source == null) throw new NullReferenceException("Source proxy object cannot be null");
				this.Proxy = source;
			}
			#endregion
			
			public static new Partner GetById(int id)
			{
				using(PartnerServiceClient client = new PartnerServiceClient())
				{
					return new Partner(client.GetPartnerById(id));
				}
			}
			
			public virtual DateTime Since
			{
				get {
					return this.Proxy.Since;
				}
				set {
					this.Proxy.Since = value;
				}
			}
			
		}
		
		
		public partial class StoreTransaction : PersistentProxyAdapter
		{
			internal new Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreTransactionProxy Proxy
			{
				get {
					return (Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreTransactionProxy)base.Proxy;
				}
				set {
					base.Proxy = value;
				}
			}
			
			#region Constructors
			public StoreTransaction()
			{
				using(StoreTransactionServiceClient client = new StoreTransactionServiceClient())
				{
					Proxy = client.CreateStoreTransaction();
					client.Close();
				}
			}
			
			public StoreTransaction(Arbinada.GenieLamp.Warehouse.Services.Warehouse.StoreTransactionProxy source)
			{
				if (source == null) throw new NullReferenceException("Source proxy object cannot be null");
				this.Proxy = source;
			}
			#endregion
			
			public static StoreTransaction GetById(int id)
			{
				using(StoreTransactionServiceClient client = new StoreTransactionServiceClient())
				{
					return new StoreTransaction(client.GetStoreTransactionById(id));
				}
			}
			
			public virtual int Id
			{
				get {
					return this.Proxy.Id;
				}
				set {
					this.Proxy.Id = value;
				}
			}
			public virtual DateTime TxDate
			{
				get {
					return this.Proxy.TxDate;
				}
				set {
					this.Proxy.TxDate = value;
				}
			}
			public virtual Arbinada.GenieLamp.Warehouse.Services.Warehouse.Direction Direction
			{
				get {
					return this.Proxy.Direction;
				}
				set {
					this.Proxy.Direction = value;
				}
			}
			public virtual Arbinada.GenieLamp.Warehouse.Services.Warehouse.State State
			{
				get {
					return this.Proxy.State;
				}
				set {
					this.Proxy.State = value;
				}
			}
			private Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Store m_Store;
			public virtual Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Store Store
			{
				get {
					return this.Proxy.Store;
				}
				set {
					this.Proxy.Store = value;
				}
			}
			private Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Product m_Product;
			public virtual Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Product Product
			{
				get {
					return this.Proxy.Product;
				}
				set {
					this.Proxy.Product = value;
				}
			}
			private Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Contractor m_Supplier;
			public virtual Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Contractor Supplier
			{
				get {
					return this.Proxy.Supplier;
				}
				set {
					this.Proxy.Supplier = value;
				}
			}
			private Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Contractor m_Customer;
			public virtual Arbinada.GenieLamp.Warehouse.Services.Proxies.Warehouse.Contractor Customer
			{
				get {
					return this.Proxy.Customer;
				}
				set {
					this.Proxy.Customer = value;
				}
			}
			public virtual int Quantity
			{
				get {
					return this.Proxy.Quantity;
				}
				set {
					this.Proxy.Quantity = value;
				}
			}
			
		}
		
		
	}
	#endregion
	
}

