﻿// Genie Lamp Core (1.1.4798.27721)
// Genie of NHibernate framework (1.0.4798.27723)
// Starter application (1.1.4798.27722)
// This file was automatically generated at 2013-03-14 16:56:47
// Do not modify it manually.

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using NHibernate;
using NHibernate.Cfg;

namespace GenieLamp.Examples.QuickStart.Persistence
{
	#region Interfaces
    public interface IPersistentObject
    {
        void Save(ITransaction outerTransaction = null);
        void Delete(ITransaction outerTransaction = null);
    }
	#endregion

    public delegate void SessionManagerConfigurationHandler(NHibernate.Cfg.Configuration configuration);

    public static class SessionManager
    {
        private static ISessionFactory sessionFactory = null;
        private static Configuration configuration = null;
        private static object locking = 0;

        public static SessionManagerConfigurationHandler OnConfigure;

        static SessionManager()
        { }

        public static ISessionFactory SessionFactory
        {
            get 
            {
                if (sessionFactory == null)
                {
                    lock (locking)
                    {
                        sessionFactory = Configuration.BuildSessionFactory();
                    }
                }
                return sessionFactory; 
            }
        }

        public static Configuration Configuration
        {
            get 
            {
                if (configuration == null)
                {
                    lock (locking)
                    {
                        configuration = new Configuration();
                        configuration.Configure();
                        if (OnConfigure != null)
                            OnConfigure(configuration);
						// Event listeners initializaton for domain entities
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.Save,
								new NHibernate.Event.ISaveOrUpdateEventListener[] { new GenieLamp.Examples.QuickStart.Patterns.DomainEventHandler() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.SaveUpdate,
								new NHibernate.Event.ISaveOrUpdateEventListener[] { new GenieLamp.Examples.QuickStart.Patterns.DomainEventHandler() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.Update,
								new NHibernate.Event.ISaveOrUpdateEventListener[] { new GenieLamp.Examples.QuickStart.Patterns.DomainEventHandler() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.Delete,
								new NHibernate.Event.IDeleteEventListener[] { new GenieLamp.Examples.QuickStart.Patterns.DomainEventHandler() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.FlushEntity,
								new NHibernate.Event.IFlushEntityEventListener[] { new GenieLamp.Examples.QuickStart.Patterns.DomainEventHandler() });
                    }
                }
                return configuration; 
            }
        }

		public static void OpenSession()
        {
        	GetSession();
        }

        public static ISession GetSession()
        {
            ISession session;
            if (NHibernate.Context.CurrentSessionContext.HasBind(SessionFactory))
            {
                session = sessionFactory.GetCurrentSession();
            }
            else
            {
				session = sessionFactory.OpenSession(new GenieLamp.Examples.QuickStart.Patterns.CommonEntityInterceptor());
                //session = sessionFactory.OpenSession();
                NHibernate.Context.CurrentSessionContext.Bind(session);
            }
            return session;
        }

        public static void CloseSession()
        {
            if (NHibernate.Context.CurrentSessionContext.HasBind(SessionFactory))
            {
                ISession session = sessionFactory.GetCurrentSession();
				session.Flush();
                session.Clear();
                session.Close();
                session.Dispose();
                NHibernate.Context.CurrentSessionContext.Unbind(SessionFactory);
            }
        }

        public static IsolationLevel IsolationLevel
        {
            get
            {
                string levelStr;
                if (!Configuration.Properties.TryGetValue("connection.isolation", out levelStr))
                    return IsolationLevel.ReadCommitted;

                if (levelStr.Equals("Chaos", StringComparison.InvariantCultureIgnoreCase))
                    return IsolationLevel.Chaos;
                else if (levelStr.Equals("ReadUncommitted", StringComparison.InvariantCultureIgnoreCase))
                    return IsolationLevel.ReadUncommitted;
                else if (levelStr.Equals("RepeatableRead", StringComparison.InvariantCultureIgnoreCase))
                    return IsolationLevel.RepeatableRead;
                else if (levelStr.Equals("Serializable", StringComparison.InvariantCultureIgnoreCase))
                    return IsolationLevel.Serializable;
                else
                    return IsolationLevel.ReadCommitted;
            }
        }
    }


    public class UnitOfWork : IDisposable
    {
        enum Action
        {
            Save,
            Delete
        }

        class WorkItem
        {
            public WorkItem(object item, Action action)
            {
                this.Item = item;
                this.Action = action;
            }

            public object Item { get; set; }
            public Action Action { get; set; }
        }

        private List<WorkItem> workItems = new List<WorkItem>();

        public UnitOfWork()
        {
        }

        public UnitOfWork(object objectToSave)
            : this()
        {
            Save(objectToSave);
        }

        private void CheckPersistence(object o)
        {
            if (!(o is IPersistentObject))
            {
                throw new ApplicationException(
                    String.Format("Unit of work should implement IPersistentObject interface. {0}", o.ToString()));
            }
        }

        public void Save(object o)
        {
            CheckPersistence(o);
            workItems.Add(new WorkItem(o, Action.Save));
        }

        public void Delete(object o)
        {
            CheckPersistence(o);
            workItems.Add(new WorkItem(o, Action.Delete));
        }

        public void Commit()
        {
            Commit(SessionManager.IsolationLevel);
        }

        public void Commit(IsolationLevel isolationLevel)
        {
            ISession session = SessionManager.GetSession();
            ITransaction tx = session.BeginTransaction(isolationLevel);
            try
            {
                foreach (WorkItem workItem in workItems)
                {
                    switch(workItem.Action)
                    {
                        case Action.Save:
                            (workItem.Item as IPersistentObject).Save(tx);
                            break;
                        case Action.Delete:
                            (workItem.Item as IPersistentObject).Delete(tx);
                            break;
                    }
                }
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
        }

        void IDisposable.Dispose()
        {
			SessionManager.CloseSession();
        }
    }
}

namespace NHibernate.Driver
{
	public class MonoSQLiteDriver : NHibernate.Driver.ReflectionBasedDriver
	{
		public MonoSQLiteDriver()
            : base(
            "Mono.Data.Sqlite",
            "Mono.Data.Sqlite",
            "Mono.Data.Sqlite.SqliteConnection",
            "Mono.Data.Sqlite.SqliteCommand")
		{
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInSql
		{
			get	{ return true; }
		}

		public override string NamedPrefix
		{
			get { return "@"; }
		}

		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}
	}
}

namespace GenieLamp.Examples.QuickStart.Queries
{
	using NHibernate.Criterion;

	public class SortOrder
	{
		public SortOrder(string propertyName, bool ascending)
		{
			this.PropertyName = propertyName;
			this.Ascending = ascending;
		}
	
		public string PropertyName { get; set; }
		public bool Ascending { get; set; }
	}
	
	public class HqlParam
	{
	    public HqlParam(string name, object value)
	    {
	        this.Name = name;
	        this.Value = value;
	    }
	    public string Name { get; set; }
	    public object Value { get; set; }
	}
	
	public class DomainQueryParams : System.Collections.Generic.List<HqlParam>
	{
		public static DomainQueryParams CreateParams()
		{
			return new DomainQueryParams();
		}
	
		public DomainQueryParams AddParam(string name, object value)
		{
			this.Add(new HqlParam(name, value));
			return this;
		}
	}
	
	public class QueryFactory
	{
	    public static IQuery CreateQuery(string hql, DomainQueryParams hqlParams)
	    {
	        IQuery query = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateQuery(hql);
	        if (hqlParams != null)
	        {
	            foreach (HqlParam param in hqlParams)
	            {
	                query = query.SetParameter(param.Name, param.Value);
	            }
	        }
	        return query;
	    }
	}
}

namespace GenieLamp.Examples.QuickStart.Patterns
{
	#region Interfaces
	public interface IOnSave
	{
		void OnSave(EventHandlerBase sender, NHibernate.Event.SaveOrUpdateEvent e);
	}

	public interface IOnDelete
	{
		void OnDelete(EventHandlerBase sender, NHibernate.Event.DeleteEvent e, Iesi.Collections.ISet transientEntities);
	}

	public interface IOnFlush
	{
		void OnFlush(EventHandlerBase sender, NHibernate.Event.FlushEntityEvent e);
	}

	#endregion

	public class EventHandlerBase
	{
		public bool HasDirtyProperties(NHibernate.Event.FlushEntityEvent e)
		{
			if (!e.EntityEntry.RequiresDirtyCheck(e.Entity) || !e.EntityEntry.ExistsInDatabase || e.EntityEntry.LoadedState == null)
				return false;
			object[] currentState = e.EntityEntry.Persister.GetPropertyValues(e.Entity, e.Session.EntityMode);
			object[] loadedState = e.EntityEntry.LoadedState;
			for(int i = 0; i < e.EntityEntry.Persister.EntityMetamodel.Properties.Length; i++)
			{
				if(!NHibernate.Intercept.LazyPropertyInitializer.UnfetchedProperty.Equals(currentState[i])
				   && e.EntityEntry.Persister.EntityMetamodel.Properties[i].Type.IsDirty(loadedState[i], currentState[i], e.Session))
					return true;
			}
			return false;
		}
	}


	public class DomainEventHandler :
		EventHandlerBase,
		NHibernate.Event.ISaveOrUpdateEventListener,
		NHibernate.Event.IFlushEntityEventListener,
		NHibernate.Event.IDeleteEventListener
	{
		public void OnSaveOrUpdate(NHibernate.Event.SaveOrUpdateEvent e)
		{
			if (e.Entity is IOnSave)
				(e.Entity as IOnSave).OnSave(this, e);
		}

		public void OnFlushEntity(NHibernate.Event.FlushEntityEvent e)
		{
			if (e.EntityEntry.Status == NHibernate.Engine.Status.Deleted)
				return;
			if (e.Entity is IOnFlush)
				(e.Entity as IOnFlush).OnFlush(this, e);
		}

		#region IDeleteEventListener implementation
		public void OnDelete(NHibernate.Event.DeleteEvent e)
		{
			if (e.Entity is IOnDelete)
				(e.Entity as IOnDelete).OnDelete(this, e, null);
		}

		public void OnDelete(NHibernate.Event.DeleteEvent e, Iesi.Collections.ISet transientEntities)
		{
			if (e.Entity is IOnDelete)
				(e.Entity as IOnDelete).OnDelete(this, e, transientEntities);
		}
		#endregion
	}


	public class CommonEntityInterceptor : EmptyInterceptor
	{
		private object locker = new object();
		private ISession session = null;
		
		public override void SetSession(ISession session)
		{
			base.SetSession(session);
			this.session = session;
		}
		
		public override bool OnSave(object entity, object id, object[] state,
		                            string[] propertyNames, NHibernate.Type.IType[] types)
		{
			//base.OnSave(entity, id, state, propertyNames, types);
			bool modified = false;
			return modified;
		}

		public string GetActorName()
		{
			return String.IsNullOrEmpty(System.Threading.Thread.CurrentPrincipal.Identity.Name)
				? "?" : System.Threading.Thread.CurrentPrincipal.Identity.Name;
		}

	}


}


