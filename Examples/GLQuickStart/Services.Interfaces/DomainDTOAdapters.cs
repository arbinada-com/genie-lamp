// Genie Lamp Core (1.1.4798.27721)
// ServiceStack services interfaces genie (1.0.4798.27724)
// Starter application (1.1.4798.27722)
// This file was automatically generated at 2013-03-14 16:56:47
// Do not modify it manually.

using System;
using System.Collections.Generic;
// Assembly required: ServiceStack.Interfaces.dll
using ServiceStack.Service;
// Assembly required: ServiceStack.Common.dll
using ServiceStack.ServiceClient.Web;
// Assembly required: ServiceStack.ServiceInterface
using ServiceStack.ServiceInterface.Auth;

namespace GenieLamp.Examples.QuickStart.Services.Adapters
{
	public static class WebClientFactory
	{
		public const string ServiceUrlKey = "GenieLamp.Examples.QuickStart.Services.Interfaces.ServiceUrl";
	    private static string serviceUrl = String.Empty;
	    public static string ServiceUrl
	    {
	        get
	        {
	            if (!String.IsNullOrEmpty(serviceUrl))
	                return serviceUrl;
	            serviceUrl = System.Configuration.ConfigurationManager.AppSettings[ServiceUrlKey].ToString();
	            if (String.IsNullOrEmpty(serviceUrl))
					throw new ApplicationException(
						String.Format("Service URL is empty. Specify key '{0}' in application configuration or set in runtime.", ServiceUrlKey));
	            return serviceUrl;
	        }
	        set { serviceUrl = value; }
	    }
	
		public const int DefaultTimeoutMsec = 30000;
		public static bool AuthRequired = false;
		public static string UserName;
		public static string Password;
		public static string AuthProviderName = CredentialsAuthProvider.Name;
		public static bool RemeberMe = false;
	
		public static int TimeoutMsec = DefaultTimeoutMsec;
		public static void SetDefaultTimeout()
		{
			TimeoutMsec = DefaultTimeoutMsec;
		}
	
		public static JsonServiceClient GetJsonClient()
		{
		    JsonServiceClient client = new JsonServiceClient(ServiceUrl);
		    if (AuthRequired)
		    {
				try
				{
					AuthResponse ar = client.Send<AuthResponse>(new Auth
					{
						provider = AuthProviderName,
						UserName = UserName,
						Password = Password,
						RememberMe = RemeberMe
					});
				}
				catch (WebServiceException e)
				{
					throw new ApplicationException(e.ErrorMessage, e);
				}
			}
			client.Timeout = new TimeSpan(0, 0, 0, 0, TimeoutMsec);
		    return client;
		}
	
		public static void CheckResponseStatus(ServiceStack.ServiceInterface.ServiceModel.ResponseStatus status)
		{
			if (status.ErrorCode != "200" && status.ErrorCode != "204")
			{
				throw new Exception(String.Format("{0}: {1}\n{2}",
				                                  status.ErrorCode,
				                                  status.Message,
				                                  status.StackTrace));
			}
		}
	}
	
	
	public class UnitOfWork
	{
	    public enum Action
	    {
	        Save,
	        Delete
	    }
	
	    public class WorkItem
	    {
			public WorkItem()
	        { }
	
	        public WorkItem(PersistentObjectDTOAdapter item, Action action)
	        {
				this.Item = item;
				this.Action = action;
			}
	
	        public PersistentObjectDTOAdapter Item { get; set; }
	        public Action Action { get; set; }
			public object DomainObject { get; set; }
	    }
	
	    private GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO uowDto = new GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO();
	
	    public UnitOfWork()
	    {
	        WorkItems = new List<WorkItem>();
	    }
	
	    public List<WorkItem> WorkItems { get; set; }
	
		public void Save(PersistentObjectDTOAdapter workItem)
		{
			WorkItems.Add(new WorkItem(workItem, Action.Save));
			uowDto.Save(workItem.DTO);
		}
	
		public void Delete(PersistentObjectDTOAdapter workItem)
		{
			WorkItems.Add(new WorkItem(workItem, Action.Delete));
			uowDto.Delete(workItem.DTO);
		}
	
		public void Commit()
		{
	        GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceRequest request =
				new GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceRequest();
	        request.UnitOfWork = uowDto;
	        GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceResponse response =
				WebClientFactory.GetJsonClient().Post<GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceResponse>("/Persistence", request);
			if (response.CommitResult.HasError)
				throw new Exception(String.Format("{0}\n{1}", response.CommitResult.Message, response.CommitResult.ExceptionString));
	        foreach (WorkItem wi in WorkItems)
	        {
	            if (wi.Action != Action.Delete)
	            {
	                wi.Item.DTO = response.UpdatedObjects[wi.Item.DTO.Internal_ObjectId];
	            }
	        }
		}
	}
	
	
	
	public abstract class DomainObjectDTOAdapter : System.ComponentModel.INotifyPropertyChanged
	{
		protected GenieLamp.Examples.QuickStart.Services.Interfaces.DomainObjectDTO dto;
	
		#region Constructors
		public DomainObjectDTOAdapter()
		{ }
	
		public DomainObjectDTOAdapter(GenieLamp.Examples.QuickStart.Services.Interfaces.DomainObjectDTO dto)
		{
			this.dto = dto;
		}
		#endregion
	
		public bool Changed
		{
			get { return this.dto.Changed; }
		}
	
		protected void NotifyPropertyChanged(string info)
		{
			this.dto.Changed = true; 
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(info));
			}
		}
	
		public GenieLamp.Examples.QuickStart.Services.Interfaces.DomainObjectDTO DTO
		{
			get { return this.dto; }
			internal set { this.dto = value; }
		}
	
	
		#region INotifyPropertyChanged
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
	
	public abstract class PersistentObjectDTOAdapter : DomainObjectDTOAdapter
	{
		#region Constructors
		public PersistentObjectDTOAdapter()
		{ }
	
		public PersistentObjectDTOAdapter(GenieLamp.Examples.QuickStart.Services.Interfaces.PersistentObjectDTO dto) : base(dto)
		{ }
		#endregion
	
		public new GenieLamp.Examples.QuickStart.Services.Interfaces.PersistentObjectDTO DTO
		{
			get { return this.dto as GenieLamp.Examples.QuickStart.Services.Interfaces.PersistentObjectDTO; }
			internal set { this.dto = value; }
		}
	
		public abstract GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Save(bool throwException = false);
		public abstract GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Delete(bool throwException = false);
	
	    protected GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult PersistenceAction<T>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action action, bool throwException) where T : GenieLamp.Examples.QuickStart.Services.Interfaces.DomainObjectDTO
	    {
	        GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO uow = new GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO();
			switch(action)
			{
			case GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Save:
	        	uow.Save(this.dto as GenieLamp.Examples.QuickStart.Services.Interfaces.PersistentObjectDTO);
				break;
			case GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete:
	        	uow.Delete(this.dto as GenieLamp.Examples.QuickStart.Services.Interfaces.PersistentObjectDTO);
				break;
			}
	        GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceRequest();
	        request.UnitOfWork = uow;
	        GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceResponse responce = WebClientFactory.GetJsonClient().Post<GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceResponse>("/Persistence", request);
	        if (throwException && responce.CommitResult.HasError)
	            throw new GenieLamp.Examples.QuickStart.Services.Interfaces.CommitException(responce.CommitResult);
			if (!responce.CommitResult.HasError)
			{
				T dto = this.dto as T;
				responce.UpdatedObjects.Update<T>(ref dto);
				this.dto = dto;
			}
	        return responce.CommitResult;
	    }
	}
	
	
	
	public abstract class DTOAdapterCollection<T> :
	    System.Collections.Generic.IList<T>,
	    System.ComponentModel.IBindingList,
	    System.ComponentModel.ICancelAddNew
		where T : DomainObjectDTOAdapter, new()
	{
		private bool listChanged = false;
		private bool itemsChanged = false;
		private bool isSorted = false;
		private int newlyAddedIndex = -1;
	    private List<T> collection = new List<T>();
	
	    protected int InternalAdd(T item)
	    {
	        collection.Add(item);
			item.PropertyChanged += ItemsPropertyChanged;
	        return collection.Count - 1;
	    }
	
		protected void ResetChanged()
		{
			listChanged = false;
			itemsChanged = false;
		}
	
		public bool ItemsChanged
		{
			get { return this.itemsChanged; }
		}
	
		public bool Changed
		{
			get { return listChanged || itemsChanged; }
		}
	
		private void ItemsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
		{
			this.itemsChanged = true;
		}
	
		private void NotifyListChanged(System.ComponentModel.ListChangedEventArgs args)
		{
			listChanged = true;
	        if (ListChanged != null)
	            ListChanged(this, args);
		}
	
		#region IList<T>
	    public int IndexOf(T item)
	    {
	        return collection.IndexOf(item);
	    }
	
	    public void Insert(int index, T item)
	    {
	        collection.Insert(index, item);
	        NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
	            System.ComponentModel.ListChangedType.ItemAdded,
	            index, -1));
			item.PropertyChanged += ItemsPropertyChanged;
	    }
	
	    public void RemoveAt(int index)
	    {
			collection[index].PropertyChanged -= ItemsPropertyChanged;
	        collection.RemoveAt(index);
	        NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
	            System.ComponentModel.ListChangedType.ItemDeleted,
	            index, -1));
	    }
	
	    public T this[int index]
	    {
	        get { return collection[index]; }
	        set { collection[index] = value; }
	    }
	
	    public void Add(T item)
	    {
	        int index = InternalAdd(item);
			NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
	            System.ComponentModel.ListChangedType.ItemAdded,
	            index));
			item.PropertyChanged += ItemsPropertyChanged;
	    }
	
	    public void Clear()
	    {
			foreach(T item in collection)
			{
				item.PropertyChanged -= ItemsPropertyChanged;
			}
	        collection.Clear();
	        NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
	            System.ComponentModel.ListChangedType.Reset,
	            -1));
	    }
	
	    public bool Contains(T item)
	    {
	        return collection.Contains(item);
	    }
	
	    public void CopyTo(T[] array, int index)
	    {
	        collection.CopyTo(array, index);
	    }
	
	    public int Count
	    {
	        get { return collection.Count; }
	    }
	
	    public bool IsReadOnly
	    {
	        get { return false; }
	    }
	
	    public bool Remove(T item)
	    {
			int index = this.IndexOf(item);
			if (index == -1)
				return false;
	        this.RemoveAt(index); // will notify
			return true;
	    }
	
	    public IEnumerator<T> GetEnumerator()
	    {
	        return collection.GetEnumerator();
	    }
	
	    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	    {
	        return collection.GetEnumerator();
	    }
		#endregion
	
		#region IBindingList
	    public void AddIndex(System.ComponentModel.PropertyDescriptor property)
	    {
	        throw new NotImplementedException();
	    }
	
	    object System.ComponentModel.IBindingList.AddNew()
	    {
			T item = new T();
			newlyAddedIndex = InternalAdd(item);
	        return item;
	    }
	
	    public bool AllowEdit
	    {
	        get { return true; }
	    }
	
	    public bool AllowNew
	    {
	        get { return true; }
	    }
	
	    public bool AllowRemove
	    {
	        get { return true; }
	    }
	
	    public void ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
	    {
	        throw new NotImplementedException();
	    }
	
	    public int Find(System.ComponentModel.PropertyDescriptor property, object key)
	    {
	        throw new NotImplementedException();
	    }
	
	    public bool IsSorted
	    {
	        get { return isSorted; }
	    }
	
	    public event System.ComponentModel.ListChangedEventHandler ListChanged;
	
	    public void RemoveIndex(System.ComponentModel.PropertyDescriptor property)
	    {
	        throw new NotImplementedException();
	    }
	
	    public void RemoveSort()
	    {
	        throw new NotImplementedException();
	    }
	
	    public System.ComponentModel.ListSortDirection SortDirection
	    {
	        get { throw new NotImplementedException(); }
	    }
	
	    public System.ComponentModel.PropertyDescriptor SortProperty
	    {
	        get { throw new NotImplementedException(); }
	    }
	
	    public bool SupportsChangeNotification
	    {
	        get { return true; }
	    }
	
	    public bool SupportsSearching
	    {
	        get { return false; }
	    }
	
	    public bool SupportsSorting
	    {
	        get { return false; }
	    }
	
	    int System.Collections.IList.Add(object value)
	    {
	        this.Add((T)value);
	        return collection.Count - 1;
	    }
	
	    bool System.Collections.IList.Contains(object value)
	    {
	        return this.Contains((T)value);
	    }
	
	    int System.Collections.IList.IndexOf(object value)
	    {
	        return this.IndexOf((T)value);
	    }
	
	    void System.Collections.IList.Insert(int index, object value)
	    {
	        this.Insert(index, (T)value);
	    }
	
	    public bool IsFixedSize
	    {
	        get { return false; }
	    }
	
	    void System.Collections.IList.Remove(object value)
	    {
			this.Remove((T)value);
	    }
	
	    object System.Collections.IList.this[int index]
	    {
	        get { return this.collection[index]; }
	        set
			{
				this.collection[index] = (T)value;
	            NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
	                System.ComponentModel.ListChangedType.ItemChanged,
	                index, index));
			}
	    }
	
	    public void CopyTo(Array array, int index)
	    {
	        collection.ToArray().CopyTo(array, index);
	    }
	
	    public bool IsSynchronized
	    {
	        get { throw new NotImplementedException(); }
	    }
	
	    public object SyncRoot
	    {
	        get { throw new NotImplementedException(); }
	    }
		#endregion
	
	    #region ICancelAddNew
	    /* MSDN
	     * In some scenarios, such as Windows Forms complex data binding, the collection
	     * may receive CancelNew or EndNew calls for items other than the newly added item.
	     * (Each item is typically a row in a data view.)
	     * Ignore these calls; cancel or commit the new item only when that item's index is specified.
	     */
	    public void CancelNew(int itemIndex)
	    {
			if (itemIndex == newlyAddedIndex)
			{
				newlyAddedIndex = -1;
	        	this.RemoveAt(itemIndex);
			}
	    }
	
	    public void EndNew(int itemIndex)
	    {
	        if (itemIndex == newlyAddedIndex)
	        {
				newlyAddedIndex = -1;
	            NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
	                System.ComponentModel.ListChangedType.ItemAdded,
	                itemIndex));
	        }
	    }
	    #endregion
	}
	
	
	public abstract class PersistentDTOAdapterCollection<T, TDTO> : DTOAdapterCollection<T>
		where T : PersistentObjectDTOAdapter, new()
		where TDTO : GenieLamp.Examples.QuickStart.Services.Interfaces.PersistentObjectDTO
	{
		public virtual GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Save(bool throwException = true)
		{
			return PersistenceAction(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
		}
	
		public virtual GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Delete(T item, bool throwException = false)
		{
			GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult cr = item.Delete(false);
			if (cr.HasError)
			{
				if (throwException)
				{
					throw new GenieLamp.Examples.QuickStart.Services.Interfaces.CommitException(cr);
				}
			}
			else
			{
				this.Remove(item);
			}
			return cr;
		}
	
		public virtual GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult DeleteAll(bool throwException = false)
		{
			return PersistenceAction(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
		}
	
	    protected GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult PersistenceAction(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action action, bool throwException)
	    {
	        GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO uow = new GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO();
			foreach(T item in this)
			{
				switch(action)
				{
				case GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Save:
		        	uow.Save(item.DTO);
					break;
				case GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete:
		        	uow.Delete(item.DTO);
					break;
				}
			}
	        GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceRequest();
	        request.UnitOfWork = uow;
	        GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceResponse responce = WebClientFactory.GetJsonClient().Post<GenieLamp.Examples.QuickStart.Services.Interfaces.PersistenceResponse>("/Persistence", request);
	        if (throwException && responce.CommitResult.HasError)
	            throw new GenieLamp.Examples.QuickStart.Services.Interfaces.CommitException(responce.CommitResult);
			if (!responce.CommitResult.HasError)
			{
				if (action == GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete)
				{
					this.Clear();
				}
				else
				{
					foreach(T item in this)
					{
						TDTO dto = item.DTO as TDTO;
						responce.UpdatedObjects.Update<TDTO>(ref dto);
						item.DTO = dto;
					}
					ResetChanged();
				}
			}
	        return responce.CommitResult;
	    }
	
	}
	
	#region Entities
	namespace QuickStart
	{
		public partial class Customer : PersistentObjectDTOAdapter
		{
			public new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO DTO
			{
				get { return base.DTO as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public Customer()
			{
				this.dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO();
			}
			
			public Customer(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static Customer GetById(int id)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse>(String.Format("/CustomerService/Id/{0}", id))
					.CustomerDTO;
				return dto == null ? null : new Customer(dto);
			}
			
			public static Customer GetByCode(string code)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerResponse>(String.Format("/CustomerService/Code/{0}", code))
					.CustomerDTO;
				return dto == null ? null : new Customer(dto);
			}
			
			
			public virtual void Refresh()
			{
				Customer o = Customer.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO();
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; NotifyPropertyChanged("Id"); }
			}
			public string Code
			{
				get { return this.DTO.Code; }
				set { this.DTO.Code = value; NotifyPropertyChanged("Code"); }
			}
			public string Name
			{
				get { return this.DTO.Name; }
				set { this.DTO.Name = value; NotifyPropertyChanged("Name"); }
			}
			public string Phone
			{
				get { return this.DTO.Phone; }
				set { this.DTO.Phone = value; NotifyPropertyChanged("Phone"); }
			}
			public string Email
			{
				get { return this.DTO.Email; }
				set { this.DTO.Email = value; NotifyPropertyChanged("Email"); }
			}
			
		}
		
		public partial class CustomerCollection : PersistentDTOAdapterCollection<GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.Customer, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO>
		{
			#region Constructors
			public CustomerCollection ()
			{
			}
			public CustomerCollection (List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO> dtoCollection)
			{
				foreach (GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO dto in dtoCollection)
				{
					this.InternalAdd(new GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.Customer(dto));
				}
			}
			#endregion
			
			public static CustomerCollection GetByQuery(string query, GenieLamp.Examples.QuickStart.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 300)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerListResponse>("/CustomerService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new CustomerCollection(response.CustomerDTOList);
			}
			
		}
		
		public partial class Product : PersistentObjectDTOAdapter
		{
			public new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO DTO
			{
				get { return base.DTO as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public Product()
			{
				this.dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO();
			}
			
			public Product(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static Product GetById(int id)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse>(String.Format("/ProductService/Id/{0}", id))
					.ProductDTO;
				return dto == null ? null : new Product(dto);
			}
			
			public static Product GetByReference(string reference)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductResponse>(String.Format("/ProductService/Reference/{0}", reference))
					.ProductDTO;
				return dto == null ? null : new Product(dto);
			}
			
			
			public virtual void Refresh()
			{
				Product o = Product.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO();
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; NotifyPropertyChanged("Id"); }
			}
			public string Reference
			{
				get { return this.DTO.Reference; }
				set { this.DTO.Reference = value; NotifyPropertyChanged("Reference"); }
			}
			public string Name
			{
				get { return this.DTO.Name; }
				set { this.DTO.Name = value; NotifyPropertyChanged("Name"); }
			}
			
		}
		
		public partial class ProductCollection : PersistentDTOAdapterCollection<GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.Product, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO>
		{
			#region Constructors
			public ProductCollection ()
			{
			}
			public ProductCollection (List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO> dtoCollection)
			{
				foreach (GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO dto in dtoCollection)
				{
					this.InternalAdd(new GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.Product(dto));
				}
			}
			#endregion
			
			public static ProductCollection GetByQuery(string query, GenieLamp.Examples.QuickStart.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 300)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductListResponse>("/ProductService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new ProductCollection(response.ProductDTOList);
			}
			
		}
		
		public partial class PurchaseOrder : PersistentObjectDTOAdapter
		{
			public new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO DTO
			{
				get { return base.DTO as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public PurchaseOrder()
			{
				this.dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO();
			}
			
			public PurchaseOrder(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static PurchaseOrder GetById(int id)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse>(String.Format("/PurchaseOrderService/Id/{0}", id))
					.PurchaseOrderDTO;
				return dto == null ? null : new PurchaseOrder(dto);
			}
			
			public static PurchaseOrder GetByNumber(string number)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse>(String.Format("/PurchaseOrderService/Number/{0}", number))
					.PurchaseOrderDTO;
				return dto == null ? null : new PurchaseOrder(dto);
			}
			
			
			public virtual void Refresh()
			{
				PurchaseOrder o = PurchaseOrder.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO();
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			#region Operations
			public void Validate()
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest();
				request.ValidateParams = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderValidateParams();
				request.PurchaseOrderDTO = this.DTO;
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse response = WebClientFactory.GetJsonClient()
					.Post<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderResponse>("/PurchaseOrderService/Validate", request);
				if (response.ResponseStatus.ErrorCode == "400")
				{
					throw new Exception(response.ResponseStatus.Message);
				}
				this.DTO = response.PurchaseOrderDTO;
			}
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; NotifyPropertyChanged("Id"); }
			}
			public string Number
			{
				get { return this.DTO.Number; }
				set { this.DTO.Number = value; NotifyPropertyChanged("Number"); }
			}
			public DateTime IssueDate
			{
				get { return this.DTO.IssueDate; }
				set { this.DTO.IssueDate = value; NotifyPropertyChanged("IssueDate"); }
			}
			public bool Validated
			{
				get { return this.DTO.Validated; }
				set { this.DTO.Validated = value; NotifyPropertyChanged("Validated"); }
			}
			public DateTime? ShipmentDate
			{
				get { return this.DTO.ShipmentDate; }
				set { this.DTO.ShipmentDate = value; NotifyPropertyChanged("ShipmentDate"); }
			}
			public decimal TotalAmount
			{
				get { return this.DTO.TotalAmount; }
				set { this.DTO.TotalAmount = value; NotifyPropertyChanged("TotalAmount"); }
			}
			
			public int CustomerId
			{
				get { return this.DTO.CustomerId; }
				set { this.DTO.CustomerId = value; NotifyPropertyChanged("CustomerId"); }
			}
		}
		
		public partial class PurchaseOrderCollection : PersistentDTOAdapterCollection<GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.PurchaseOrder, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO>
		{
			#region Constructors
			public PurchaseOrderCollection ()
			{
			}
			public PurchaseOrderCollection (List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO> dtoCollection)
			{
				foreach (GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO dto in dtoCollection)
				{
					this.InternalAdd(new GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.PurchaseOrder(dto));
				}
			}
			#endregion
			
			public static PurchaseOrderCollection GetByQuery(string query, GenieLamp.Examples.QuickStart.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 300)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse>("/PurchaseOrderService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new PurchaseOrderCollection(response.PurchaseOrderDTOList);
			}
			
			public static PurchaseOrderCollection GetCollectionByCustomerId(int customerId)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderRequest();
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse response = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderListResponse>(String.Format("/PurchaseOrderService/CustomerId/{0}", customerId));
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new PurchaseOrderCollection(response.PurchaseOrderDTOList);
			}
			
		}
		
		public partial class PurchaseOrderLine : PersistentObjectDTOAdapter
		{
			public new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO DTO
			{
				get { return base.DTO as GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public PurchaseOrderLine()
			{
				this.dto = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO();
			}
			
			public PurchaseOrderLine(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static PurchaseOrderLine GetById(int id)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse>(String.Format("/PurchaseOrderLineService/Id/{0}", id))
					.PurchaseOrderLineDTO;
				return dto == null ? null : new PurchaseOrderLine(dto);
			}
			
			public static PurchaseOrderLine GetByPurchaseOrderIdPosition(int purchaseOrderId, short position)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineResponse>(String.Format("/PurchaseOrderLineService/PurchaseOrderId/{0}/Position/{1}", purchaseOrderId, position))
					.PurchaseOrderLineDTO;
				return dto == null ? null : new PurchaseOrderLine(dto);
			}
			
			
			public virtual void Refresh()
			{
				PurchaseOrderLine o = PurchaseOrderLine.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO();
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override GenieLamp.Examples.QuickStart.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO>(GenieLamp.Examples.QuickStart.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; NotifyPropertyChanged("Id"); }
			}
			public short Position
			{
				get { return this.DTO.Position; }
				set { this.DTO.Position = value; NotifyPropertyChanged("Position"); }
			}
			public decimal Price
			{
				get { return this.DTO.Price; }
				set { this.DTO.Price = value; NotifyPropertyChanged("Price"); }
			}
			public int Quantity
			{
				get { return this.DTO.Quantity; }
				set { this.DTO.Quantity = value; NotifyPropertyChanged("Quantity"); }
			}
			
			public int PurchaseOrderId
			{
				get { return this.DTO.PurchaseOrderId; }
				set { this.DTO.PurchaseOrderId = value; NotifyPropertyChanged("PurchaseOrderId"); }
			}
			public int ProductId
			{
				get { return this.DTO.ProductId; }
				set { this.DTO.ProductId = value; NotifyPropertyChanged("ProductId"); }
			}
		}
		
		public partial class PurchaseOrderLineCollection : PersistentDTOAdapterCollection<GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.PurchaseOrderLine, GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO>
		{
			#region Constructors
			public PurchaseOrderLineCollection ()
			{
			}
			public PurchaseOrderLineCollection (List<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO> dtoCollection)
			{
				foreach (GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO dto in dtoCollection)
				{
					this.InternalAdd(new GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart.PurchaseOrderLine(dto));
				}
			}
			#endregion
			
			public static PurchaseOrderLineCollection GetByQuery(string query, GenieLamp.Examples.QuickStart.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 300)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse>("/PurchaseOrderLineService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new PurchaseOrderLineCollection(response.PurchaseOrderLineDTOList);
			}
			
			public static PurchaseOrderLineCollection GetCollectionByPurchaseOrderId(int purchaseOrderId)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest();
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse response = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse>(String.Format("/PurchaseOrderLineService/PurchaseOrderId/{0}", purchaseOrderId));
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new PurchaseOrderLineCollection(response.PurchaseOrderLineDTOList);
			}
			
			public static PurchaseOrderLineCollection GetCollectionByProductId(int productId)
			{
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest request = new GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineRequest();
				GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse response = WebClientFactory.GetJsonClient()
					.Get<GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineListResponse>(String.Format("/PurchaseOrderLineService/ProductId/{0}", productId));
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new PurchaseOrderLineCollection(response.PurchaseOrderLineDTOList);
			}
			
		}
		
	}
	#endregion
	
}

