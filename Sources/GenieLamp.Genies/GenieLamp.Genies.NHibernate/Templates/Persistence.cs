using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using NHibernate;
using NHibernate.Cfg;

namespace %PERSISTENCE_NAMESPACE%
{
	#region Interfaces
    public interface %InterfaceName_PersistentObject%
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
								new NHibernate.Event.ISaveOrUpdateEventListener[] { new %PATTERNS_NAMESPACE%.%ClassName_DomainEventHandler%() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.SaveUpdate,
								new NHibernate.Event.ISaveOrUpdateEventListener[] { new %PATTERNS_NAMESPACE%.%ClassName_DomainEventHandler%() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.Update,
								new NHibernate.Event.ISaveOrUpdateEventListener[] { new %PATTERNS_NAMESPACE%.%ClassName_DomainEventHandler%() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.Delete,
								new NHibernate.Event.IDeleteEventListener[] { new %PATTERNS_NAMESPACE%.%ClassName_DomainEventHandler%() });
						configuration.AppendListeners(
								NHibernate.Event.ListenerType.FlushEntity,
								new NHibernate.Event.IFlushEntityEventListener[] { new %PATTERNS_NAMESPACE%.%ClassName_DomainEventHandler%() });
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
				session = sessionFactory.OpenSession(new %PATTERNS_NAMESPACE%.%ClassName_CommonEntityInterceptor%());
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
            if (!(o is %InterfaceName_PersistentObject%))
            {
                throw new ApplicationException(
                    String.Format("Unit of work should implement %InterfaceName_PersistentObject% interface. {0}", o.ToString()));
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
                            (workItem.Item as %InterfaceName_PersistentObject%).Save(tx);
                            break;
                        case Action.Delete:
                            (workItem.Item as %InterfaceName_PersistentObject%).Delete(tx);
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
