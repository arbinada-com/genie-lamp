// Genie Lamp Core (1.1.4594.29523)
// ServiceStack services interfaces genie (1.0.4594.29525)
// Starter application (1.1.4594.29524)
// This file was automatically generated at 2012-07-30 16:36:49
// Do not modify it manually.

using System;
using System.Collections.Generic;
// Assembly required: ServiceStack.Interfaces.dll
using ServiceStack.Service;
// Assembly required: ServiceStack.Common.dll
using ServiceStack.ServiceClient.Web;

namespace Arbinada.GenieLamp.Warehouse.Services.Adapters
{
	public static class WebClientFactory
	{
	    private static string serviceUrl = String.Empty;
	    public static string ServiceUrl
	    {
	        get
	        {
	            if (!String.IsNullOrEmpty(serviceUrl))
	                return serviceUrl;
	            serviceUrl = System.Configuration.ConfigurationManager.AppSettings["ServiceUrl"].ToString();
	            if (String.IsNullOrEmpty(serviceUrl))
	                throw new ApplicationException("Service URL is empty. Specify it in App.config or set in runtime.");
	            return serviceUrl;
	        }
	        set { serviceUrl = value; }
	    }
	
	    public static JsonServiceClient GetJsonClient()
	    {
	        return new JsonServiceClient(ServiceUrl);
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
	
	    private Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO uowDto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO();
	
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
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceRequest request =
				new Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceRequest();
	        request.UnitOfWork = uowDto;
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceResponse response =
				WebClientFactory.GetJsonClient().Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceResponse>("/Persistence", request);
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
	
	
	
	public abstract class DomainObjectDTOAdapter
	{
		protected Arbinada.GenieLamp.Warehouse.Services.Interfaces.DomainObjectDTO dto;
	
		#region Constructors
		public DomainObjectDTOAdapter()
		{ }
	
		public DomainObjectDTOAdapter(Arbinada.GenieLamp.Warehouse.Services.Interfaces.DomainObjectDTO dto)
		{
			this.dto = dto;
		}
		#endregion
	
		public bool Changed
		{
			get { return this.dto.Changed; }
			protected set { this.dto.Changed = value; }
		}
	
		public Arbinada.GenieLamp.Warehouse.Services.Interfaces.DomainObjectDTO DTO
		{
			get { return this.dto; }
			internal set { this.dto = value; }
		}
	}
	
	public abstract class PersistentObjectDTOAdapter : DomainObjectDTOAdapter
	{
		#region Constructors
		public PersistentObjectDTOAdapter()
		{ }
	
		public PersistentObjectDTOAdapter(Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistentObjectDTO dto) : base(dto)
		{ }
		#endregion
	
		public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistentObjectDTO DTO
		{
			get { return this.dto as Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistentObjectDTO; }
			internal set { this.dto = value; }
		}
	
		public abstract Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false);
		public abstract Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false);
	
	    protected Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult PersistenceAction<T>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action action, bool throwException) where T : Arbinada.GenieLamp.Warehouse.Services.Interfaces.DomainObjectDTO
	    {
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO uow = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO();
			switch(action)
			{
			case Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save:
	        	uow.Save(this.dto as Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistentObjectDTO);
				break;
			case Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete:
	        	uow.Delete(this.dto as Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistentObjectDTO);
				break;
			}
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceRequest();
	        request.UnitOfWork = uow;
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceResponse responce = WebClientFactory.GetJsonClient().Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceResponse>("/Persistence", request);
	        if (throwException && responce.CommitResult.HasError)
	            throw new Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitException(responce.CommitResult);
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
		where T : class, new()
	{
		private bool isChanged = false;
		private bool isSorted = false;
		private int newlyAddedIndex = -1;
	
	    protected List<T> collection = new List<T>();
	
	    protected int InternalAdd(T item)
	    {
	        collection.Add(item);
	        return collection.Count - 1;
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
	    }
	
	    public void RemoveAt(int index)
	    {
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
	    }
	
	    public void Clear()
	    {
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
	        collection.RemoveAt(index); // will notify
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
	
		public bool IsChanged
		{
			get { return isChanged; }
		}
	
		private void NotifyListChanged(System.ComponentModel.ListChangedEventArgs args)
		{
			isChanged = true;
	        if (ListChanged != null)
	            ListChanged(this, args);
		}
	}
	
	
	public abstract class PersistentDTOAdapterCollection<T, TDTO> : DTOAdapterCollection<T>
		where T : PersistentObjectDTOAdapter, new()
		where TDTO : Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistentObjectDTO
	{
		public virtual Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
		{
			return PersistenceAction(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
		}
	
		public virtual Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(T item, bool throwException = false)
		{
			Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult cr = item.Delete(false);
			if (cr.HasError)
			{
				if (throwException)
				{
					throw new Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitException(cr);
				}
			}
			else
			{
				this.Remove(item);
			}
			return cr;
		}
	
		public virtual Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult DeleteAll(bool throwException = false)
		{
			return PersistenceAction(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
		}
	
	    protected Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult PersistenceAction(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action action, bool throwException)
	    {
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO uow = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO();
			foreach(T item in this)
			{
				switch(action)
				{
				case Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save:
		        	uow.Save(item.DTO);
					break;
				case Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete:
		        	uow.Delete(item.DTO);
					break;
				}
			}
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceRequest();
	        request.UnitOfWork = uow;
	        Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceResponse responce = WebClientFactory.GetJsonClient().Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.PersistenceResponse>("/Persistence", request);
	        if (throwException && responce.CommitResult.HasError)
	            throw new Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitException(responce.CommitResult);
			if (!responce.CommitResult.HasError)
			{
				if (action == Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete)
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
				}
			}
	        return responce.CommitResult;
	    }
	
	}
	
	#region Entities
	namespace Core
	{
		public partial class EntityType : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public EntityType()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO();
			}
			
			public EntityType(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static EntityType GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse>(String.Format("/EntityTypeService/Id/{0}", id))
					.EntityTypeDTO;
				return dto == null ? null : new EntityType(dto);
			}
			
			public static EntityType GetByFullName(string fullName)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeResponse>(String.Format("/EntityTypeService/FullName/{0}", fullName))
					.EntityTypeDTO;
				return dto == null ? null : new EntityType(dto);
			}
			
			
			public virtual void Refresh()
			{
				EntityType o = EntityType.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string FullName
			{
				get { return this.DTO.FullName; }
				set { this.DTO.FullName = value; this.Changed = true; }
			}
			public string ShortName
			{
				get { return this.DTO.ShortName; }
				set { this.DTO.ShortName = value; this.Changed = true; }
			}
			public string Description
			{
				get { return this.DTO.Description; }
				set { this.DTO.Description = value; this.Changed = true; }
			}
			
		}
		
		public partial class EntityTypeCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Core.EntityType, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO>
		{
			#region Constructors
			public EntityTypeCollection ()
			{
			}
			public EntityTypeCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Core.EntityType(dto));
				}
			}
			#endregion
			
			public static EntityTypeCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeListResponse>("/EntityTypeService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new EntityTypeCollection(response.EntityTypeDTOList);
			}
		}
		
		public partial class EntityRegistry : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public EntityRegistry()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO();
			}
			
			public EntityRegistry(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static EntityRegistry GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryResponse>(String.Format("/EntityRegistryService/Id/{0}", id))
					.EntityRegistryDTO;
				return dto == null ? null : new EntityRegistry(dto);
			}
			
			
			public virtual void Refresh()
			{
				EntityRegistry o = EntityRegistry.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			
			public int? EntityTypeIdId
			{
				get { return this.DTO.EntityTypeIdId; }
				set { this.DTO.EntityTypeIdId = value; this.Changed = true; }
			}
		}
		
		public partial class EntityRegistryCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Core.EntityRegistry, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO>
		{
			#region Constructors
			public EntityRegistryCollection ()
			{
			}
			public EntityRegistryCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Core.EntityRegistry(dto));
				}
			}
			#endregion
			
			public static EntityRegistryCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryListResponse>("/EntityRegistryService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new EntityRegistryCollection(response.EntityRegistryDTOList);
			}
		}
		
	}
	
	namespace Warehouse
	{
		public partial class ExampleOneToOne : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public ExampleOneToOne()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO();
			}
			
			public ExampleOneToOne(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static ExampleOneToOne GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse>(String.Format("/ExampleOneToOneService/Id/{0}", id))
					.ExampleOneToOneDTO;
				return dto == null ? null : new ExampleOneToOne(dto);
			}
			
			public static ExampleOneToOne GetByName(string name)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse>(String.Format("/ExampleOneToOneService/Name/{0}", name))
					.ExampleOneToOneDTO;
				return dto == null ? null : new ExampleOneToOne(dto);
			}
			
			public static ExampleOneToOne GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneResponse>(String.Format("/ExampleOneToOneService/EntityRegistryId/{0}", entityRegistryId))
					.ExampleOneToOneDTO;
				return dto == null ? null : new ExampleOneToOne(dto);
			}
			
			
			public virtual void Refresh()
			{
				ExampleOneToOne o = ExampleOneToOne.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string Name
			{
				get { return this.DTO.Name; }
				set { this.DTO.Name = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class ExampleOneToOneCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.ExampleOneToOne, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO>
		{
			#region Constructors
			public ExampleOneToOneCollection ()
			{
			}
			public ExampleOneToOneCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.ExampleOneToOne(dto));
				}
			}
			#endregion
			
			public static ExampleOneToOneCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneListResponse>("/ExampleOneToOneService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new ExampleOneToOneCollection(response.ExampleOneToOneDTOList);
			}
		}
		
		public partial class ExampleOneToOneEx : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public ExampleOneToOneEx()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO();
			}
			
			public ExampleOneToOneEx(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static ExampleOneToOneEx GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse>(String.Format("/ExampleOneToOneExService/Id/{0}", id))
					.ExampleOneToOneExDTO;
				return dto == null ? null : new ExampleOneToOneEx(dto);
			}
			
			public static ExampleOneToOneEx GetByExempleOneToOneId(int exempleOneToOneId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse>(String.Format("/ExampleOneToOneExService/ExempleOneToOneId/{0}", exempleOneToOneId))
					.ExampleOneToOneExDTO;
				return dto == null ? null : new ExampleOneToOneEx(dto);
			}
			
			public static ExampleOneToOneEx GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExResponse>(String.Format("/ExampleOneToOneExService/EntityRegistryId/{0}", entityRegistryId))
					.ExampleOneToOneExDTO;
				return dto == null ? null : new ExampleOneToOneEx(dto);
			}
			
			
			public virtual void Refresh()
			{
				ExampleOneToOneEx o = ExampleOneToOneEx.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string Caption
			{
				get { return this.DTO.Caption; }
				set { this.DTO.Caption = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int ExempleOneToOneId
			{
				get { return this.DTO.ExempleOneToOneId; }
				set { this.DTO.ExempleOneToOneId = value; this.Changed = true; }
			}
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class ExampleOneToOneExCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.ExampleOneToOneEx, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO>
		{
			#region Constructors
			public ExampleOneToOneExCollection ()
			{
			}
			public ExampleOneToOneExCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.ExampleOneToOneEx(dto));
				}
			}
			#endregion
			
			public static ExampleOneToOneExCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExListResponse>("/ExampleOneToOneExService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new ExampleOneToOneExCollection(response.ExampleOneToOneExDTOList);
			}
		}
		
		public partial class ProductType : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public ProductType()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO();
			}
			
			public ProductType(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static ProductType GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse>(String.Format("/ProductTypeService/Id/{0}", id))
					.ProductTypeDTO;
				return dto == null ? null : new ProductType(dto);
			}
			
			public static ProductType GetByCode(string code)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse>(String.Format("/ProductTypeService/Code/{0}", code))
					.ProductTypeDTO;
				return dto == null ? null : new ProductType(dto);
			}
			
			public static ProductType GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeResponse>(String.Format("/ProductTypeService/EntityRegistryId/{0}", entityRegistryId))
					.ProductTypeDTO;
				return dto == null ? null : new ProductType(dto);
			}
			
			
			public virtual void Refresh()
			{
				ProductType o = ProductType.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string Code
			{
				get { return this.DTO.Code; }
				set { this.DTO.Code = value; this.Changed = true; }
			}
			public string Name
			{
				get { return this.DTO.Name; }
				set { this.DTO.Name = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class ProductTypeCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.ProductType, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO>
		{
			#region Constructors
			public ProductTypeCollection ()
			{
			}
			public ProductTypeCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.ProductType(dto));
				}
			}
			#endregion
			
			public static ProductTypeCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeListResponse>("/ProductTypeService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new ProductTypeCollection(response.ProductTypeDTOList);
			}
		}
		
		public partial class Product : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public Product()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO();
			}
			
			public Product(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static Product GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse>(String.Format("/ProductService/Id/{0}", id))
					.ProductDTO;
				return dto == null ? null : new Product(dto);
			}
			
			public static Product GetByRefCode(string refCode)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse>(String.Format("/ProductService/RefCode/{0}", refCode))
					.ProductDTO;
				return dto == null ? null : new Product(dto);
			}
			
			public static Product GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductResponse>(String.Format("/ProductService/EntityRegistryId/{0}", entityRegistryId))
					.ProductDTO;
				return dto == null ? null : new Product(dto);
			}
			
			
			public virtual void Refresh()
			{
				Product o = Product.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string RefCode
			{
				get { return this.DTO.RefCode; }
				set { this.DTO.RefCode = value; this.Changed = true; }
			}
			public string Caption
			{
				get { return this.DTO.Caption; }
				set { this.DTO.Caption = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int? TypeId
			{
				get { return this.DTO.TypeId; }
				set { this.DTO.TypeId = value; this.Changed = true; }
			}
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class ProductCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Product, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO>
		{
			#region Constructors
			public ProductCollection ()
			{
			}
			public ProductCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Product(dto));
				}
			}
			#endregion
			
			public static ProductCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductListResponse>("/ProductService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new ProductCollection(response.ProductDTOList);
			}
		}
		
		public partial class StoreType : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public StoreType()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO();
			}
			
			public StoreType(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static StoreType GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse>(String.Format("/StoreTypeService/Id/{0}", id))
					.StoreTypeDTO;
				return dto == null ? null : new StoreType(dto);
			}
			
			public static StoreType GetByName(string name)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse>(String.Format("/StoreTypeService/Name/{0}", name))
					.StoreTypeDTO;
				return dto == null ? null : new StoreType(dto);
			}
			
			public static StoreType GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeResponse>(String.Format("/StoreTypeService/EntityRegistryId/{0}", entityRegistryId))
					.StoreTypeDTO;
				return dto == null ? null : new StoreType(dto);
			}
			
			
			public virtual void Refresh()
			{
				StoreType o = StoreType.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string Name
			{
				get { return this.DTO.Name; }
				set { this.DTO.Name = value; this.Changed = true; }
			}
			public string Caption
			{
				get { return this.DTO.Caption; }
				set { this.DTO.Caption = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class StoreTypeCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreType, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO>
		{
			#region Constructors
			public StoreTypeCollection ()
			{
			}
			public StoreTypeCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreType(dto));
				}
			}
			#endregion
			
			public static StoreTypeCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeListResponse>("/StoreTypeService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new StoreTypeCollection(response.StoreTypeDTOList);
			}
		}
		
		public partial class Store : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public Store()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO();
			}
			
			public Store(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static Store GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse>(String.Format("/StoreService/Id/{0}", id))
					.StoreDTO;
				return dto == null ? null : new Store(dto);
			}
			
			public static Store GetByCode(string code)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse>(String.Format("/StoreService/Code/{0}", code))
					.StoreDTO;
				return dto == null ? null : new Store(dto);
			}
			
			public static Store GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse>(String.Format("/StoreService/EntityRegistryId/{0}", entityRegistryId))
					.StoreDTO;
				return dto == null ? null : new Store(dto);
			}
			
			
			public virtual void Refresh()
			{
				Store o = Store.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			void Check()
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest();
				request.CheckParams = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreCheckParams();
				request.StoreDTO = this.DTO;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse response = WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse>("/StoreService/Check", request);
				this.DTO = response.StoreDTO;
			}
			bool RecordReceived(int productId, int qty, decimal price, DateTime date)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest();
				request.RecordReceivedParams = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRecordReceivedParams();
				request.RecordReceivedParams.productId = productId;
				request.RecordReceivedParams.qty = qty;
				request.RecordReceivedParams.price = price;
				request.RecordReceivedParams.date = date;
				request.StoreDTO = this.DTO;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse response = WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse>("/StoreService/RecordReceived", request);
				this.DTO = response.StoreDTO;
				return request.RecordReceivedParams.Result;
			}
			int[] GetQuantity(System.Collections.Generic.IList<int> product, DateTime date)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest();
				request.GetQuantityParams = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreGetQuantityParams();
				request.GetQuantityParams.product = product;
				request.GetQuantityParams.date = date;
				request.StoreDTO = this.DTO;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse response = WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreResponse>("/StoreService/GetQuantity", request);
				this.DTO = response.StoreDTO;
				return request.GetQuantityParams.Result;
			}
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string Code
			{
				get { return this.DTO.Code; }
				set { this.DTO.Code = value; this.Changed = true; }
			}
			public string Caption
			{
				get { return this.DTO.Caption; }
				set { this.DTO.Caption = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int? StoreTypeId
			{
				get { return this.DTO.StoreTypeId; }
				set { this.DTO.StoreTypeId = value; this.Changed = true; }
			}
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class StoreCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Store, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO>
		{
			#region Constructors
			public StoreCollection ()
			{
			}
			public StoreCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Store(dto));
				}
			}
			#endregion
			
			public static StoreCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreListResponse>("/StoreService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new StoreCollection(response.StoreDTOList);
			}
		}
		
		public partial class Contractor : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public Contractor()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO();
			}
			
			public Contractor(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static Contractor GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse>(String.Format("/ContractorService/Id/{0}", id))
					.ContractorDTO;
				return dto == null ? null : new Contractor(dto);
			}
			
			public static Contractor GetByName(string name)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse>(String.Format("/ContractorService/Name/{0}", name))
					.ContractorDTO;
				return dto == null ? null : new Contractor(dto);
			}
			
			public static Contractor GetByPhone(string phone)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse>(String.Format("/ContractorService/Phone/{0}", phone))
					.ContractorDTO;
				return dto == null ? null : new Contractor(dto);
			}
			
			public static Contractor GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorResponse>(String.Format("/ContractorService/EntityRegistryId/{0}", entityRegistryId))
					.ContractorDTO;
				return dto == null ? null : new Contractor(dto);
			}
			
			
			public virtual void Refresh()
			{
				Contractor o = Contractor.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string Name
			{
				get { return this.DTO.Name; }
				set { this.DTO.Name = value; this.Changed = true; }
			}
			public string Address
			{
				get { return this.DTO.Address; }
				set { this.DTO.Address = value; this.Changed = true; }
			}
			public string Phone
			{
				get { return this.DTO.Phone; }
				set { this.DTO.Phone = value; this.Changed = true; }
			}
			public string Email
			{
				get { return this.DTO.Email; }
				set { this.DTO.Email = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class ContractorCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Contractor, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO>
		{
			#region Constructors
			public ContractorCollection ()
			{
			}
			public ContractorCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Contractor(dto));
				}
			}
			#endregion
			
			public static ContractorCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorListResponse>("/ContractorService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new ContractorCollection(response.ContractorDTOList);
			}
		}
		
		public partial class Partner : Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Contractor
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public Partner()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO();
			}
			
			public Partner(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static new Partner GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerResponse>(String.Format("/PartnerService/Id/{0}", id))
					.PartnerDTO;
				return dto == null ? null : new Partner(dto);
			}
			
			
			public override void Refresh()
			{
				Partner o = Partner.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public DateTime Since
			{
				get { return this.DTO.Since; }
				set { this.DTO.Since = value; this.Changed = true; }
			}
			
		}
		
		public partial class PartnerCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Partner, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO>
		{
			#region Constructors
			public PartnerCollection ()
			{
			}
			public PartnerCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.Partner(dto));
				}
			}
			#endregion
			
			public static PartnerCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerListResponse>("/PartnerService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new PartnerCollection(response.PartnerDTOList);
			}
		}
		
		public partial class StoreDoc : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public StoreDoc()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO();
			}
			
			public StoreDoc(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static StoreDoc GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse>(String.Format("/StoreDocService/Id/{0}", id))
					.StoreDocDTO;
				return dto == null ? null : new StoreDoc(dto);
			}
			
			public static StoreDoc GetByRefNum(string refNum)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse>(String.Format("/StoreDocService/RefNum/{0}", refNum))
					.StoreDocDTO;
				return dto == null ? null : new StoreDoc(dto);
			}
			
			public static StoreDoc GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocResponse>(String.Format("/StoreDocService/EntityRegistryId/{0}", entityRegistryId))
					.StoreDocDTO;
				return dto == null ? null : new StoreDoc(dto);
			}
			
			
			public virtual void Refresh()
			{
				StoreDoc o = StoreDoc.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public string RefNum
			{
				get { return this.DTO.RefNum; }
				set { this.DTO.RefNum = value; this.Changed = true; }
			}
			public DateTime Created
			{
				get { return this.DTO.Created; }
				set { this.DTO.Created = value; this.Changed = true; }
			}
			public string Name
			{
				get { return this.DTO.Name; }
				set { this.DTO.Name = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class StoreDocCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreDoc, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO>
		{
			#region Constructors
			public StoreDocCollection ()
			{
			}
			public StoreDocCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreDoc(dto));
				}
			}
			#endregion
			
			public static StoreDocCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocListResponse>("/StoreDocService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new StoreDocCollection(response.StoreDocDTOList);
			}
		}
		
		public partial class StoreDocLine : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public StoreDocLine()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO();
			}
			
			public StoreDocLine(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static StoreDocLine GetById(int storeDocId, int position)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse>(String.Format("/StoreDocLineService/StoreDocId/{0}/Position/{1}", storeDocId, position))
					.StoreDocLineDTO;
				return dto == null ? null : new StoreDocLine(dto);
			}
			
			public static StoreDocLine GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineResponse>(String.Format("/StoreDocLineService/EntityRegistryId/{0}", entityRegistryId))
					.StoreDocLineDTO;
				return dto == null ? null : new StoreDocLine(dto);
			}
			
			
			public virtual void Refresh()
			{
				StoreDocLine o = StoreDocLine.GetById(this.StoreDocId, this.Position);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Position
			{
				get { return this.DTO.Position; }
				set { this.DTO.Position = value; this.Changed = true; }
			}
			public int Quantity
			{
				get { return this.DTO.Quantity; }
				set { this.DTO.Quantity = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int StoreDocId
			{
				get { return this.DTO.StoreDocId; }
				set { this.DTO.StoreDocId = value; this.Changed = true; }
			}
			public int ProductId
			{
				get { return this.DTO.ProductId; }
				set { this.DTO.ProductId = value; this.Changed = true; }
			}
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class StoreDocLineCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreDocLine, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO>
		{
			#region Constructors
			public StoreDocLineCollection ()
			{
			}
			public StoreDocLineCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreDocLine(dto));
				}
			}
			#endregion
			
			public static StoreDocLineCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineListResponse>("/StoreDocLineService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new StoreDocLineCollection(response.StoreDocLineDTOList);
			}
		}
		
		public partial class StoreTransaction : PersistentObjectDTOAdapter
		{
			public new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO DTO
			{
				get { return base.DTO as Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO; }
				internal set { base.DTO = value; }
			}
			#region Constructors
			public StoreTransaction()
			{
				this.dto = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO();
			}
			
			public StoreTransaction(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto) : base(dto)
			{
			}
			#endregion
			
			public static StoreTransaction GetById(int id)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse>(String.Format("/StoreTransactionService/Id/{0}", id))
					.StoreTransactionDTO;
				return dto == null ? null : new StoreTransaction(dto);
			}
			
			public static StoreTransaction GetByEntityRegistryId(int? entityRegistryId)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto = WebClientFactory.GetJsonClient()
					.Get<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionResponse>(String.Format("/StoreTransactionService/EntityRegistryId/{0}", entityRegistryId))
					.StoreTransactionDTO;
				return dto == null ? null : new StoreTransaction(dto);
			}
			
			
			public virtual void Refresh()
			{
				StoreTransaction o = StoreTransaction.GetById(this.Id);
				this.DTO = o != null && o.DTO != null ? o.DTO : new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO();
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Save(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Save, throwException);
			}
			
			public override Arbinada.GenieLamp.Warehouse.Services.Interfaces.CommitResult Delete(bool throwException = false)
			{
				return PersistenceAction<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO>(Arbinada.GenieLamp.Warehouse.Services.Interfaces.UnitOfWorkDTO.Action.Delete, throwException);
			}
			
			
			#region Operations
			#endregion
			public int Id
			{
				get { return this.DTO.Id; }
				set { this.DTO.Id = value; this.Changed = true; }
			}
			public DateTime TxDate
			{
				get { return this.DTO.TxDate; }
				set { this.DTO.TxDate = value; this.Changed = true; }
			}
			public Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.Direction Direction
			{
				get { return this.DTO.Direction; }
				set { this.DTO.Direction = value; this.Changed = true; }
			}
			public Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.State State
			{
				get { return this.DTO.State; }
				set { this.DTO.State = value; this.Changed = true; }
			}
			public int Quantity
			{
				get { return this.DTO.Quantity; }
				set { this.DTO.Quantity = value; this.Changed = true; }
			}
			public int Version
			{
				get { return this.DTO.Version; }
				set { this.DTO.Version = value; this.Changed = true; }
			}
			public string CreatedBy
			{
				get { return this.DTO.CreatedBy; }
				set { this.DTO.CreatedBy = value; this.Changed = true; }
			}
			public DateTime? CreatedDate
			{
				get { return this.DTO.CreatedDate; }
				set { this.DTO.CreatedDate = value; this.Changed = true; }
			}
			public string LastModifiedBy
			{
				get { return this.DTO.LastModifiedBy; }
				set { this.DTO.LastModifiedBy = value; this.Changed = true; }
			}
			public DateTime? LastModifiedDate
			{
				get { return this.DTO.LastModifiedDate; }
				set { this.DTO.LastModifiedDate = value; this.Changed = true; }
			}
			
			public int SupplierId
			{
				get { return this.DTO.SupplierId; }
				set { this.DTO.SupplierId = value; this.Changed = true; }
			}
			public int StoreId
			{
				get { return this.DTO.StoreId; }
				set { this.DTO.StoreId = value; this.Changed = true; }
			}
			public int ProductId
			{
				get { return this.DTO.ProductId; }
				set { this.DTO.ProductId = value; this.Changed = true; }
			}
			public int CustomerId
			{
				get { return this.DTO.CustomerId; }
				set { this.DTO.CustomerId = value; this.Changed = true; }
			}
			public int? EntityRegistryId
			{
				get { return this.DTO.EntityRegistryId; }
				set { this.DTO.EntityRegistryId = value; this.Changed = true; }
			}
		}
		
		public partial class StoreTransactionCollection : PersistentDTOAdapterCollection<Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreTransaction, Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO>
		{
			#region Constructors
			public StoreTransactionCollection ()
			{
			}
			public StoreTransactionCollection (List<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO> dtoCollection)
			{
				foreach (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO dto in dtoCollection)
				{
					this.collection.Add(new Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse.StoreTransaction(dto));
				}
			}
			#endregion
			
			public static StoreTransactionCollection GetByQuery(string query, Arbinada.GenieLamp.Warehouse.Services.Interfaces.ServicesQueryParams queryParams, int pageNum = 0, int pageSize = 20)
			{
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionRequest request = new Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionRequest();
				request.Gl_Query = query;
				request.Gl_QueryParams = queryParams;
				request.Gl_PageNum = pageNum;
				request.Gl_PageSize = pageSize;
				Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionListResponse response =
					WebClientFactory.GetJsonClient()
					.Post<Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionListResponse>("/StoreTransactionService", request);
				WebClientFactory.CheckResponseStatus(response.ResponseStatus);
				return new StoreTransactionCollection(response.StoreTransactionDTOList);
			}
		}
		
	}
	#endregion
	
}

