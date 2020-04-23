<%@ language C# %>
<%@ using System.Collections.Generic %>
<% if (data.ContainsKey("patterns") && data["patterns"].Contains("Localization")) { %>
public class L
{
	public static GNU.Gettext.GettextResourceManager Catalog
	{
		get { return new GNU.Gettext.GettextResourceManager(<%=data["pattern.Localization"][0] %>); }
	}
}

<% } %>

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
public class %ClassName_UnitOfWorkDTO%
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

        public WorkItem(%ClassName_PersistentObjectDTO% item, Action action)
        {
			this.Item = item;
			this.Action = action;
		}

        [DataMember]
        public %ClassName_PersistentObjectDTO% Item { get; set; }
        [DataMember]
        public Action Action { get; set; }
		public object DomainObject { get; set; }
    }

    [DataMember]
    public List<WorkItem> WorkItems { get; set; }

    public %ClassName_UnitOfWorkDTO%()
    {
        WorkItems = new List<WorkItem>();
    }

	public void Save(%ClassName_PersistentObjectDTO% workItem)
	{
		WorkItems.Add(new WorkItem(workItem, Action.Save));
	}

	public void Delete(%ClassName_PersistentObjectDTO% workItem)
	{
		WorkItems.Add(new WorkItem(workItem, Action.Delete));
	}
}


