public static class %ClassName_WebClientFactory%
{
	public const string ServiceUrlKey = "%NAMESPACE_SERVICES_INTERFACES%.ServiceUrl";
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
	public static bool AuthRequired = %AuthRequired%;
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


public class %ClassName_UnitOfWorkDTOAdapter%
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

        public WorkItem(%ClassName_PersistentObjectDTOAdapter% item, Action action)
        {
			this.Item = item;
			this.Action = action;
		}

        public %ClassName_PersistentObjectDTOAdapter% Item { get; set; }
        public Action Action { get; set; }
		public object DomainObject { get; set; }
    }

    private %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO% uowDto = new %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%();

    public %ClassName_UnitOfWorkDTOAdapter%()
    {
        WorkItems = new List<WorkItem>();
    }

    public List<WorkItem> WorkItems { get; set; }

	public void Save(%ClassName_PersistentObjectDTOAdapter% workItem)
	{
		WorkItems.Add(new WorkItem(workItem, Action.Save));
		uowDto.Save(workItem.%PropertyName_AdapterDTO%);
	}

	public void Delete(%ClassName_PersistentObjectDTOAdapter% workItem)
	{
		WorkItems.Add(new WorkItem(workItem, Action.Delete));
		uowDto.Delete(workItem.%PropertyName_AdapterDTO%);
	}

	public void Commit()
	{
        %NAMESPACE_SERVICES_INTERFACES%.PersistenceRequest request =
			new %NAMESPACE_SERVICES_INTERFACES%.PersistenceRequest();
        request.%PropertyName_UnitOfWorkDTO% = uowDto;
        %NAMESPACE_SERVICES_INTERFACES%.PersistenceResponse response =
			%ClassName_WebClientFactory%.GetJsonClient().Post<%NAMESPACE_SERVICES_INTERFACES%.PersistenceResponse>("/Persistence", request);
		if (response.CommitResult.HasError)
			throw new Exception(String.Format("{0}\n{1}", response.CommitResult.Message, response.CommitResult.ExceptionString));
        foreach (WorkItem wi in WorkItems)
        {
            if (wi.Action != Action.Delete)
            {
                wi.Item.%PropertyName_AdapterDTO% = response.UpdatedObjects[wi.Item.%PropertyName_AdapterDTO%.Internal_ObjectId];
            }
        }
	}
}



public abstract class %ClassName_DomainObjectDTOAdapter% : System.ComponentModel.INotifyPropertyChanged
{
	protected %NAMESPACE_SERVICES_DTO%.%ClassName_DomainObjectDTO% dto;

	#region Constructors
	public %ClassName_DomainObjectDTOAdapter%()
	{ }

	public %ClassName_DomainObjectDTOAdapter%(%NAMESPACE_SERVICES_DTO%.%ClassName_DomainObjectDTO% dto)
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

	public %NAMESPACE_SERVICES_DTO%.%ClassName_DomainObjectDTO% DTO
	{
		get { return this.dto; }
		internal set { this.dto = value; }
	}


	#region INotifyPropertyChanged
	public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	#endregion
}

public abstract class %ClassName_PersistentObjectDTOAdapter% : %ClassName_DomainObjectDTOAdapter%
{
	#region Constructors
	public %ClassName_PersistentObjectDTOAdapter%()
	{ }

	public %ClassName_PersistentObjectDTOAdapter%(%NAMESPACE_SERVICES_DTO%.%ClassName_PersistentObjectDTO% dto) : base(dto)
	{ }
	#endregion

	public new %NAMESPACE_SERVICES_DTO%.%ClassName_PersistentObjectDTO% DTO
	{
		get { return this.dto as %NAMESPACE_SERVICES_DTO%.%ClassName_PersistentObjectDTO%; }
		internal set { this.dto = value; }
	}

	public abstract %NAMESPACE_SERVICES_INTERFACES%.CommitResult Save(bool throwException = false);
	public abstract %NAMESPACE_SERVICES_INTERFACES%.CommitResult Delete(bool throwException = false);

    protected %NAMESPACE_SERVICES_INTERFACES%.CommitResult PersistenceAction<T>(%NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action action, bool throwException) where T : %NAMESPACE_SERVICES_DTO%.%ClassName_DomainObjectDTO%
    {
        %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO% uow = new %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%();
		switch(action)
		{
		case %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action.Save:
        	uow.Save(this.dto as %NAMESPACE_SERVICES_DTO%.%ClassName_PersistentObjectDTO%);
			break;
		case %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action.Delete:
        	uow.Delete(this.dto as %NAMESPACE_SERVICES_DTO%.%ClassName_PersistentObjectDTO%);
			break;
		}
        %NAMESPACE_SERVICES_INTERFACES%.PersistenceRequest request = new %NAMESPACE_SERVICES_INTERFACES%.PersistenceRequest();
        request.%PropertyName_UnitOfWorkDTO% = uow;
        %NAMESPACE_SERVICES_INTERFACES%.PersistenceResponse responce = %ClassName_WebClientFactory%.GetJsonClient().Post<%NAMESPACE_SERVICES_INTERFACES%.PersistenceResponse>("/Persistence", request);
        if (throwException && responce.CommitResult.HasError)
            throw new %NAMESPACE_SERVICES_INTERFACES%.CommitException(responce.CommitResult);
		if (!responce.CommitResult.HasError)
		{
			T dto = this.dto as T;
			responce.UpdatedObjects.Update<T>(ref dto);
			this.dto = dto;
		}
        return responce.CommitResult;
    }
}



public abstract class %ClassName_DTOAdapterCollection%<T> :
    System.Collections.Generic.IList<T>,
    System.ComponentModel.IBindingList,
    System.ComponentModel.ICancelAddNew
	where T : %ClassName_DomainObjectDTOAdapter%, new()
{
	private bool listChanged = false;
	private bool itemsChanged = false;
	private bool isSorted = false;
	private int newlyAddedIndex = -1;
    private List<T> %VarName_DTOAdapterCollection% = new List<T>();

    protected int InternalAdd(T item)
    {
        %VarName_DTOAdapterCollection%.Add(item);
		item.PropertyChanged += ItemsPropertyChanged;
        return %VarName_DTOAdapterCollection%.Count - 1;
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
        return %VarName_DTOAdapterCollection%.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        %VarName_DTOAdapterCollection%.Insert(index, item);
        NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
            System.ComponentModel.ListChangedType.ItemAdded,
            index, -1));
		item.PropertyChanged += ItemsPropertyChanged;
    }

    public void RemoveAt(int index)
    {
		%VarName_DTOAdapterCollection%[index].PropertyChanged -= ItemsPropertyChanged;
        %VarName_DTOAdapterCollection%.RemoveAt(index);
        NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
            System.ComponentModel.ListChangedType.ItemDeleted,
            index, -1));
    }

    public T this[int index]
    {
        get { return %VarName_DTOAdapterCollection%[index]; }
        set { %VarName_DTOAdapterCollection%[index] = value; }
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
		foreach(T item in %VarName_DTOAdapterCollection%)
		{
			item.PropertyChanged -= ItemsPropertyChanged;
		}
        %VarName_DTOAdapterCollection%.Clear();
        NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
            System.ComponentModel.ListChangedType.Reset,
            -1));
    }

    public bool Contains(T item)
    {
        return %VarName_DTOAdapterCollection%.Contains(item);
    }

    public void CopyTo(T[] array, int index)
    {
        %VarName_DTOAdapterCollection%.CopyTo(array, index);
    }

    public int Count
    {
        get { return %VarName_DTOAdapterCollection%.Count; }
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
        return %VarName_DTOAdapterCollection%.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return %VarName_DTOAdapterCollection%.GetEnumerator();
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
        return %VarName_DTOAdapterCollection%.Count - 1;
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
        get { return this.%VarName_DTOAdapterCollection%[index]; }
        set
		{
			this.%VarName_DTOAdapterCollection%[index] = (T)value;
            NotifyListChanged(new System.ComponentModel.ListChangedEventArgs(
                System.ComponentModel.ListChangedType.ItemChanged,
                index, index));
		}
    }

    public void CopyTo(Array array, int index)
    {
        %VarName_DTOAdapterCollection%.ToArray().CopyTo(array, index);
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


public abstract class %ClassName_PersistentDTOAdapterCollection%<T, TDTO> : %ClassName_DTOAdapterCollection%<T>
	where T : %ClassName_PersistentObjectDTOAdapter%, new()
	where TDTO : %NAMESPACE_SERVICES_DTO%.%ClassName_PersistentObjectDTO%
{
	public virtual %NAMESPACE_SERVICES_INTERFACES%.CommitResult Save(bool throwException = true)
	{
		return PersistenceAction(%NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action.Save, throwException);
	}

	public virtual %NAMESPACE_SERVICES_INTERFACES%.CommitResult Delete(T item, bool throwException = false)
	{
		%NAMESPACE_SERVICES_INTERFACES%.CommitResult cr = item.Delete(false);
		if (cr.HasError)
		{
			if (throwException)
			{
				throw new %NAMESPACE_SERVICES_INTERFACES%.CommitException(cr);
			}
		}
		else
		{
			this.Remove(item);
		}
		return cr;
	}

	public virtual %NAMESPACE_SERVICES_INTERFACES%.CommitResult DeleteAll(bool throwException = false)
	{
		return PersistenceAction(%NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action.Delete, throwException);
	}

    protected %NAMESPACE_SERVICES_INTERFACES%.CommitResult PersistenceAction(%NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action action, bool throwException)
    {
        %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO% uow = new %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%();
		foreach(T item in this)
		{
			switch(action)
			{
			case %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action.Save:
	        	uow.Save(item.DTO);
				break;
			case %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action.Delete:
	        	uow.Delete(item.DTO);
				break;
			}
		}
        %NAMESPACE_SERVICES_INTERFACES%.PersistenceRequest request = new %NAMESPACE_SERVICES_INTERFACES%.PersistenceRequest();
        request.%PropertyName_UnitOfWorkDTO% = uow;
        %NAMESPACE_SERVICES_INTERFACES%.PersistenceResponse responce = %ClassName_WebClientFactory%.GetJsonClient().Post<%NAMESPACE_SERVICES_INTERFACES%.PersistenceResponse>("/Persistence", request);
        if (throwException && responce.CommitResult.HasError)
            throw new %NAMESPACE_SERVICES_INTERFACES%.CommitException(responce.CommitResult);
		if (!responce.CommitResult.HasError)
		{
			if (action == %NAMESPACE_SERVICES_INTERFACES%.%ClassName_UnitOfWorkDTO%.Action.Delete)
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