public class UnitOfWork
{
    enum Action
    {
        Save,
        Delete
    }

    class WorkItem
    {
        public WorkItem(%ClassName_PersistentProxyAdapter% item, Action action)
        {
            this.Item = item;
            this.Action = action;
        }

        public %ClassName_PersistentProxyAdapter% Item { get; set; }
        public Action Action { get; set; }
    }

    private List<WorkItem> workItems = new List<WorkItem>();

    public UnitOfWork()
    { }

    public UnitOfWork(%ClassName_PersistentProxyAdapter% objectToSave)
        : this()
    {
        Save(objectToSave);
    }

    public void Save(%ClassName_PersistentProxyAdapter% o)
    {
        workItems.Add(new WorkItem(o, Action.Save));
    }

    public void Delete(%ClassName_PersistentProxyAdapter% o)
    {
        workItems.Add(new WorkItem(o, Action.Delete));
    }

    public %NAMESPACE_SERVICES%.CommitResult Commit()
    {
        %NAMESPACE_SERVICES%.CommitResult result = new %NAMESPACE_SERVICES%.CommitResult();

        PersistenceServiceClient ps = new PersistenceServiceClient();
        try
        {
            try
            {
                UnitOfWork uow = ps.CreateUnitOfWork();
                List<UnitOfWork.WorkItem> workItemsProxy = uow.WorkItems.ToList<UnitOfWork.WorkItem>();
                foreach (WorkItem workItem in workItems)
                {
                    UnitOfWork.WorkItem workItemProxy = new UnitOfWork.WorkItem();
                    workItemProxy.Item = workItem.Item.Proxy;
                    switch (workItem.Action)
                    {
                        case Action.Save:
                            workItemProxy.Action = UnitOfWork.Action.Save;
                            break;
                        case Action.Delete:
                            workItemProxy.Action = UnitOfWork.Action.Delete;
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

