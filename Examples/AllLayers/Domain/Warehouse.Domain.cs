// Genie Lamp Core (1.1.4594.29523)
// Genie of NHibernate framework (1.0.4594.29782)
// Starter application (1.1.4594.29524)
// This file was automatically generated at 2012-07-30 16:36:49
// Do not modify it manually.

using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

using NHibernate;
using NHibernate.Criterion;

namespace Arbinada.GenieLamp.Warehouse.Domain
{
	#region Enumerations
	namespace Warehouse
	{
		public enum State
		{
			InProgress = 0,
			Validated = 1
		}
		public enum Direction
		{
			Income = 0,
			Outcome = 1
		}
	}
	#endregion
	#region Entities classes
	namespace Core
	{
		public interface IEntityTypeCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType> { }
		
		public interface IEntityType
		{
		}
		
		public partial class EntityType : IEntityType, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject
		{
			public EntityType()
			{
			}
			
			public virtual int Id { get; set; }
			private string m_FullName;
			public virtual string FullName
			{
				get { return m_FullName; }
				set { m_FullName = value != null && value.Length > 255 ? value.Substring(0, 255) : value; }
			}
			private string m_ShortName;
			public virtual string ShortName
			{
				get { return m_ShortName; }
				set { m_ShortName = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			private string m_Description;
			public virtual string Description
			{
				get { return m_Description; }
				set { m_Description = value != null && value.Length > 1000 ? value.Substring(0, 1000) : value; }
			}
			
			public static EntityType GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<EntityType>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<EntityType>();
			}
			
			public static EntityType GetByFullName(string fullName)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<EntityType>()
					.Add(Expression.Eq("FullName", fullName))
					.UniqueResult<EntityType>();
			}
			
			public static System.Collections.Generic.IList<EntityType> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<EntityType>();
			}
			
			public static System.Collections.Generic.IList<EntityType> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return EntityType.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<EntityType> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(EntityType))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<EntityType>();
			}
			
			public static System.Collections.Generic.IList<EntityType> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return EntityType.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				EntityType o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<EntityType>();
			}
			
			public static System.Collections.Generic.IList<EntityType> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<EntityType>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<EntityType> GetList(System.Linq.Expressions.Expression<Func<EntityType, bool>> predicate)
			{
				return EntityType.CreateCriteria().Add(Expression.Where<EntityType>(predicate)).List<EntityType>();
			}
			
			public static EntityType GetUnique(System.Linq.Expressions.Expression<Func<EntityType, bool>> predicate)
			{
				return EntityType.CreateCriteria().Add(Expression.Where<EntityType>(predicate)).UniqueResult<EntityType>();
			}
			
		}
		
		public interface IEntityRegistryCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry> { }
		
		public interface IEntityRegistry
		{
		}
		
		public partial class EntityRegistry : IEntityRegistry, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject
		{
			public EntityRegistry()
			{
			}
			
			public virtual int Id { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType EntityType { get; set; }
			
			public static EntityRegistry GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<EntityRegistry>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<EntityRegistry>();
			}
			
			public static System.Collections.Generic.IList<EntityRegistry> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<EntityRegistry>();
			}
			
			public static System.Collections.Generic.IList<EntityRegistry> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return EntityRegistry.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<EntityRegistry> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(EntityRegistry))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<EntityRegistry>();
			}
			
			public static System.Collections.Generic.IList<EntityRegistry> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return EntityRegistry.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				EntityRegistry o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<EntityRegistry>();
			}
			
			// Relation (EntityType-EntityRegistryList): <Core.EntityRegistry>--M:1--<Core.EntityType>
			public static System.Collections.Generic.IList<EntityRegistry> GetCollectionByEntityTypeId(int? entityTypeIdId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityType.Id", entityTypeIdId))
					.List<EntityRegistry>();
			}
			
			public static System.Collections.Generic.IList<EntityRegistry> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<EntityRegistry>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<EntityRegistry> GetList(System.Linq.Expressions.Expression<Func<EntityRegistry, bool>> predicate)
			{
				return EntityRegistry.CreateCriteria().Add(Expression.Where<EntityRegistry>(predicate)).List<EntityRegistry>();
			}
			
			public static EntityRegistry GetUnique(System.Linq.Expressions.Expression<Func<EntityRegistry, bool>> predicate)
			{
				return EntityRegistry.CreateCriteria().Add(Expression.Where<EntityRegistry>(predicate)).UniqueResult<EntityRegistry>();
			}
			
		}
		
	}
	
	namespace Warehouse
	{
		public interface IExampleOneToOneCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne> { }
		
		public interface IExampleOneToOne
		{
		}
		
		public partial class ExampleOneToOne : IExampleOneToOne, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public ExampleOneToOne()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx Extension { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_Name;
			public virtual string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static ExampleOneToOne GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOne>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<ExampleOneToOne>();
			}
			
			public static ExampleOneToOne GetByName(string name)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOne>()
					.Add(Expression.Eq("Name", name))
					.UniqueResult<ExampleOneToOne>();
			}
			
			public static ExampleOneToOne GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOne>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<ExampleOneToOne>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOne> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<ExampleOneToOne>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOne> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return ExampleOneToOne.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOne> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(ExampleOneToOne))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<ExampleOneToOne>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOne> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return ExampleOneToOne.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				ExampleOneToOne o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOne>();
			}
			
			// Relation (EntityRegistry-ExampleOneToOne): <Warehouse.ExampleOneToOne>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<ExampleOneToOne> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<ExampleOneToOne>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOne> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<ExampleOneToOne>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOne> GetList(System.Linq.Expressions.Expression<Func<ExampleOneToOne, bool>> predicate)
			{
				return ExampleOneToOne.CreateCriteria().Add(Expression.Where<ExampleOneToOne>(predicate)).List<ExampleOneToOne>();
			}
			
			public static ExampleOneToOne GetUnique(System.Linq.Expressions.Expression<Func<ExampleOneToOne, bool>> predicate)
			{
				return ExampleOneToOne.CreateCriteria().Add(Expression.Where<ExampleOneToOne>(predicate)).UniqueResult<ExampleOneToOne>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IExampleOneToOneExCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx> { }
		
		public interface IExampleOneToOneEx
		{
		}
		
		public partial class ExampleOneToOneEx : IExampleOneToOneEx, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public ExampleOneToOneEx()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne ExampleOneToOne { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_Caption;
			public virtual string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static ExampleOneToOneEx GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOneEx>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<ExampleOneToOneEx>();
			}
			
			public static ExampleOneToOneEx GetByExempleOneToOneId(int exempleOneToOneId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOneEx>()
					.Add(Expression.Eq("ExempleOneToOneId", exempleOneToOneId))
					.UniqueResult<ExampleOneToOneEx>();
			}
			
			public static ExampleOneToOneEx GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOneEx>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<ExampleOneToOneEx>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<ExampleOneToOneEx>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return ExampleOneToOneEx.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(ExampleOneToOneEx))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<ExampleOneToOneEx>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return ExampleOneToOneEx.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				ExampleOneToOneEx o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ExampleOneToOneEx>();
			}
			
			// Relation (Extension-ExampleOneToOne): <Warehouse.ExampleOneToOne>--1:1--<Warehouse.ExampleOneToOneEx>
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetCollectionByExempleOneToOneId(int exempleOneToOneId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("ExampleOneToOne.Id", exempleOneToOneId))
					.List<ExampleOneToOneEx>();
			}
			
			// Relation (EntityRegistry-ExampleOneToOneEx): <Warehouse.ExampleOneToOneEx>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<ExampleOneToOneEx>();
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<ExampleOneToOneEx>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<ExampleOneToOneEx> GetList(System.Linq.Expressions.Expression<Func<ExampleOneToOneEx, bool>> predicate)
			{
				return ExampleOneToOneEx.CreateCriteria().Add(Expression.Where<ExampleOneToOneEx>(predicate)).List<ExampleOneToOneEx>();
			}
			
			public static ExampleOneToOneEx GetUnique(System.Linq.Expressions.Expression<Func<ExampleOneToOneEx, bool>> predicate)
			{
				return ExampleOneToOneEx.CreateCriteria().Add(Expression.Where<ExampleOneToOneEx>(predicate)).UniqueResult<ExampleOneToOneEx>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IProductTypeCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType> { }
		
		public interface IProductType
		{
		}
		
		public partial class ProductType : IProductType, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public ProductType()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType Parent { get; set; }
			public virtual Iesi.Collections.Generic.ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType> ProductSubtypes { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_Code;
			public virtual string Code
			{
				get { return m_Code; }
				set { m_Code = value != null && value.Length > 3 ? value.Substring(0, 3) : value; }
			}
			private string m_Name;
			public virtual string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static ProductType GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ProductType>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<ProductType>();
			}
			
			public static ProductType GetByCode(string code)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ProductType>()
					.Add(Expression.Eq("Code", code))
					.UniqueResult<ProductType>();
			}
			
			public static ProductType GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ProductType>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<ProductType>();
			}
			
			public static System.Collections.Generic.IList<ProductType> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<ProductType>();
			}
			
			public static System.Collections.Generic.IList<ProductType> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return ProductType.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<ProductType> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(ProductType))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<ProductType>();
			}
			
			public static System.Collections.Generic.IList<ProductType> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return ProductType.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				ProductType o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<ProductType>();
			}
			
			// Relation (ProductSubtypes-Parent): <Warehouse.ProductType>--1:M--<Warehouse.ProductType>
			public static System.Collections.Generic.IList<ProductType> GetCollectionByParentId(int? parentId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Parent.Id", parentId))
					.List<ProductType>();
			}
			
			// Relation (EntityRegistry-ProductType): <Warehouse.ProductType>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<ProductType> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<ProductType>();
			}
			
			public static System.Collections.Generic.IList<ProductType> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<ProductType>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<ProductType> GetList(System.Linq.Expressions.Expression<Func<ProductType, bool>> predicate)
			{
				return ProductType.CreateCriteria().Add(Expression.Where<ProductType>(predicate)).List<ProductType>();
			}
			
			public static ProductType GetUnique(System.Linq.Expressions.Expression<Func<ProductType, bool>> predicate)
			{
				return ProductType.CreateCriteria().Add(Expression.Where<ProductType>(predicate)).UniqueResult<ProductType>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IProductCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product> { }
		
		public interface IProduct
		{
		}
		
		public partial class Product : IProduct, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public Product()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType Type { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_RefCode;
			public virtual string RefCode
			{
				get { return m_RefCode; }
				set { m_RefCode = value != null && value.Length > 10 ? value.Substring(0, 10) : value; }
			}
			private string m_Caption;
			public virtual string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static Product GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Product>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<Product>();
			}
			
			public static Product GetByRefCode(string refCode)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Product>()
					.Add(Expression.Eq("RefCode", refCode))
					.UniqueResult<Product>();
			}
			
			public static Product GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Product>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<Product>();
			}
			
			public static System.Collections.Generic.IList<Product> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Product>();
			}
			
			public static System.Collections.Generic.IList<Product> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return Product.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<Product> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(Product))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Product>();
			}
			
			public static System.Collections.Generic.IList<Product> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return Product.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				Product o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Product>();
			}
			
			// Relation (Type-ProductList): <Warehouse.Product>--M:1--<Warehouse.ProductType>
			public static System.Collections.Generic.IList<Product> GetCollectionByType(int? typeId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Type.Id", typeId))
					.List<Product>();
			}
			
			// Relation (EntityRegistry-Product): <Warehouse.Product>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<Product> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<Product>();
			}
			
			public static System.Collections.Generic.IList<Product> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<Product>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<Product> GetList(System.Linq.Expressions.Expression<Func<Product, bool>> predicate)
			{
				return Product.CreateCriteria().Add(Expression.Where<Product>(predicate)).List<Product>();
			}
			
			public static Product GetUnique(System.Linq.Expressions.Expression<Func<Product, bool>> predicate)
			{
				return Product.CreateCriteria().Add(Expression.Where<Product>(predicate)).UniqueResult<Product>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IStoreTypeCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType> { }
		
		public interface IStoreType
		{
		}
		
		public partial class StoreType : IStoreType, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public StoreType()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Iesi.Collections.Generic.ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store> Stores { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_Name;
			public virtual string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 3 ? value.Substring(0, 3) : value; }
			}
			private string m_Caption;
			public virtual string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static StoreType GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreType>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<StoreType>();
			}
			
			public static StoreType GetByName(string name)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreType>()
					.Add(Expression.Eq("Name", name))
					.UniqueResult<StoreType>();
			}
			
			public static StoreType GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreType>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<StoreType>();
			}
			
			public static System.Collections.Generic.IList<StoreType> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreType>();
			}
			
			public static System.Collections.Generic.IList<StoreType> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return StoreType.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<StoreType> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(StoreType))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreType>();
			}
			
			public static System.Collections.Generic.IList<StoreType> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return StoreType.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				StoreType o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreType>();
			}
			
			// Relation (EntityRegistry-StoreType): <Warehouse.StoreType>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<StoreType> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<StoreType>();
			}
			
			public static System.Collections.Generic.IList<StoreType> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<StoreType>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<StoreType> GetList(System.Linq.Expressions.Expression<Func<StoreType, bool>> predicate)
			{
				return StoreType.CreateCriteria().Add(Expression.Where<StoreType>(predicate)).List<StoreType>();
			}
			
			public static StoreType GetUnique(System.Linq.Expressions.Expression<Func<StoreType, bool>> predicate)
			{
				return StoreType.CreateCriteria().Add(Expression.Where<StoreType>(predicate)).UniqueResult<StoreType>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IStoreCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store> { }
		
		public interface IStore
		{
			void Check();
			bool RecordReceived(int productId, int qty, decimal price, DateTime date);
			int[] GetQuantity(System.Collections.Generic.IList<int> product, DateTime date);
		}
		
		public partial class Store : IStore, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, NHibernate.Classic.IValidatable, Arbinada.GenieLamp.Warehouse.Patterns.IOnSave, Arbinada.GenieLamp.Warehouse.Patterns.IOnDelete, Arbinada.GenieLamp.Warehouse.Patterns.IOnFlush, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public Store()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType StoreType { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_Code;
			public virtual string Code
			{
				get { return m_Code; }
				set { m_Code = value != null && value.Length > 15 ? value.Substring(0, 15) : value; }
			}
			private string m_Caption;
			public virtual string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static Store GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Store>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<Store>();
			}
			
			public static Store GetByCode(string code)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Store>()
					.Add(Expression.Eq("Code", code))
					.UniqueResult<Store>();
			}
			
			public static Store GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Store>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<Store>();
			}
			
			public static System.Collections.Generic.IList<Store> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Store>();
			}
			
			public static System.Collections.Generic.IList<Store> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return Store.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<Store> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(Store))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Store>();
			}
			
			public static System.Collections.Generic.IList<Store> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return Store.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				Store o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Store>();
			}
			
			// Relation (StoreType-Stores): <Warehouse.Store>--M:1--<Warehouse.StoreType>
			public static System.Collections.Generic.IList<Store> GetCollectionByStoreTypeId(int? storeTypeId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("StoreType.Id", storeTypeId))
					.List<Store>();
			}
			
			// Relation (EntityRegistry-Store): <Warehouse.Store>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<Store> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<Store>();
			}
			
			public static System.Collections.Generic.IList<Store> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<Store>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<Store> GetList(System.Linq.Expressions.Expression<Func<Store, bool>> predicate)
			{
				return Store.CreateCriteria().Add(Expression.Where<Store>(predicate)).List<Store>();
			}
			
			public static Store GetUnique(System.Linq.Expressions.Expression<Func<Store, bool>> predicate)
			{
				return Store.CreateCriteria().Add(Expression.Where<Store>(predicate)).UniqueResult<Store>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IContractorCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor> { }
		
		public interface IContractor
		{
		}
		
		public partial class Contractor : IContractor, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public Contractor()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Iesi.Collections.Generic.ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction> Transactions { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_Name;
			public virtual string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_Address;
			public virtual string Address
			{
				get { return m_Address; }
				set { m_Address = value != null && value.Length > 255 ? value.Substring(0, 255) : value; }
			}
			private string m_Phone;
			public virtual string Phone
			{
				get { return m_Phone; }
				set { m_Phone = value != null && value.Length > 20 ? value.Substring(0, 20) : value; }
			}
			private string m_Email;
			public virtual string Email
			{
				get { return m_Email; }
				set { m_Email = value != null && value.Length > 80 ? value.Substring(0, 80) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static Contractor GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Contractor>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<Contractor>();
			}
			
			public static Contractor GetByName(string name)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Contractor>()
					.Add(Expression.Eq("Name", name))
					.UniqueResult<Contractor>();
			}
			
			public static Contractor GetByPhone(string phone)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Contractor>()
					.Add(Expression.Eq("Phone", phone))
					.UniqueResult<Contractor>();
			}
			
			public static Contractor GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Contractor>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<Contractor>();
			}
			
			public static System.Collections.Generic.IList<Contractor> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Contractor>();
			}
			
			public static System.Collections.Generic.IList<Contractor> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return Contractor.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<Contractor> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(Contractor))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Contractor>();
			}
			
			public static System.Collections.Generic.IList<Contractor> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return Contractor.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				Contractor o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Contractor>();
			}
			
			// Relation (EntityRegistry-Contractor): <Warehouse.Contractor>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<Contractor> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<Contractor>();
			}
			
			public static System.Collections.Generic.IList<Contractor> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<Contractor>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<Contractor> GetList(System.Linq.Expressions.Expression<Func<Contractor, bool>> predicate)
			{
				return Contractor.CreateCriteria().Add(Expression.Where<Contractor>(predicate)).List<Contractor>();
			}
			
			public static Contractor GetUnique(System.Linq.Expressions.Expression<Func<Contractor, bool>> predicate)
			{
				return Contractor.CreateCriteria().Add(Expression.Where<Contractor>(predicate)).UniqueResult<Contractor>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IPartnerCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner> { }
		
		public interface IPartner
		{
		}
		
		public partial class Partner : Warehouse.Contractor, IPartner, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject
		{
			public Partner()
			{
			}
			
			public virtual DateTime Since { get; set; }
			
			public static new Partner GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Partner>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<Partner>();
			}
			
			public static new System.Collections.Generic.IList<Partner> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Partner>();
			}
			
			public static new System.Collections.Generic.IList<Partner> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return Partner.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static new System.Collections.Generic.IList<Partner> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(Partner))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Partner>();
			}
			
			public static new System.Collections.Generic.IList<Partner> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return Partner.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public override void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static new ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<Partner>();
			}
			
			public static new System.Collections.Generic.IList<Partner> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<Partner>();
			}
			
			public static System.Collections.Generic.IList<Partner> GetList(System.Linq.Expressions.Expression<Func<Partner, bool>> predicate)
			{
				return Partner.CreateCriteria().Add(Expression.Where<Partner>(predicate)).List<Partner>();
			}
			
			public static Partner GetUnique(System.Linq.Expressions.Expression<Func<Partner, bool>> predicate)
			{
				return Partner.CreateCriteria().Add(Expression.Where<Partner>(predicate)).UniqueResult<Partner>();
			}
			
		}
		
		public interface IStoreDocCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc> { }
		
		public interface IStoreDoc
		{
		}
		
		public partial class StoreDoc : IStoreDoc, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public StoreDoc()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Iesi.Collections.Generic.ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine> Lines { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private string m_RefNum;
			public virtual string RefNum
			{
				get { return m_RefNum; }
				set { m_RefNum = value != null && value.Length > 16 ? value.Substring(0, 16) : value; }
			}
			private DateTime m_Created = DateTime.Now;
			public virtual DateTime Created
			{
				get { return m_Created; }
				set { m_Created = value; }
			}
			private string m_Name;
			public virtual string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static StoreDoc GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreDoc>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<StoreDoc>();
			}
			
			public static StoreDoc GetByRefNum(string refNum)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreDoc>()
					.Add(Expression.Eq("RefNum", refNum))
					.UniqueResult<StoreDoc>();
			}
			
			public static StoreDoc GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreDoc>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<StoreDoc>();
			}
			
			public static System.Collections.Generic.IList<StoreDoc> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreDoc>();
			}
			
			public static System.Collections.Generic.IList<StoreDoc> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return StoreDoc.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<StoreDoc> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(StoreDoc))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreDoc>();
			}
			
			public static System.Collections.Generic.IList<StoreDoc> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return StoreDoc.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				StoreDoc o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreDoc>();
			}
			
			// Relation (EntityRegistry-StoreDoc): <Warehouse.StoreDoc>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<StoreDoc> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<StoreDoc>();
			}
			
			public static System.Collections.Generic.IList<StoreDoc> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<StoreDoc>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<StoreDoc> GetList(System.Linq.Expressions.Expression<Func<StoreDoc, bool>> predicate)
			{
				return StoreDoc.CreateCriteria().Add(Expression.Where<StoreDoc>(predicate)).List<StoreDoc>();
			}
			
			public static StoreDoc GetUnique(System.Linq.Expressions.Expression<Func<StoreDoc, bool>> predicate)
			{
				return StoreDoc.CreateCriteria().Add(Expression.Where<StoreDoc>(predicate)).UniqueResult<StoreDoc>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IStoreDocLineCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine> { }
		
		public interface IStoreDocLine
		{
		}
		
		public partial class StoreDocLine : IStoreDocLine, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public StoreDocLine()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc Doc { get; set; }
			public virtual int Position { get; set; }
			#region Composite id override methods
			public override bool Equals(object o)
			{
				return o != null && (o as StoreDocLine) != null && this.Doc.Id == (o as StoreDocLine).Doc.Id && this.Position == (o as StoreDocLine).Position;
			}
			
			public override int GetHashCode()
			{
				return (this.Doc.Id.ToString() + "|" + this.Position.ToString()).GetHashCode();
			}
			#endregion
			
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product Product { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			public virtual int Quantity { get; set; }
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static StoreDocLine GetById(int storeDocId, int position)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreDocLine>()
					.Add(Expression.Eq("StoreDocId", storeDocId))
					.Add(Expression.Eq("Position", position))
					.UniqueResult<StoreDocLine>();
			}
			
			public static StoreDocLine GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreDocLine>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<StoreDocLine>();
			}
			
			public static System.Collections.Generic.IList<StoreDocLine> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreDocLine>();
			}
			
			public static System.Collections.Generic.IList<StoreDocLine> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return StoreDocLine.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<StoreDocLine> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(StoreDocLine))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreDocLine>();
			}
			
			public static System.Collections.Generic.IList<StoreDocLine> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return StoreDocLine.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int storeDocId, int position)
			{
				StoreDocLine o = GetById(storeDocId, position);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreDocLine>();
			}
			
			// Relation (Doc-Lines): <Warehouse.StoreDocLine>--M:1--<Warehouse.StoreDoc>
			public static System.Collections.Generic.IList<StoreDocLine> GetCollectionByStoreDocId(int storeDocId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Doc.Id", storeDocId))
					.List<StoreDocLine>();
			}
			
			// Relation (Product-StoreDocLineList): <Warehouse.StoreDocLine>--M:1--<Warehouse.Product>
			public static System.Collections.Generic.IList<StoreDocLine> GetCollectionByProduct(int productId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Product.Id", productId))
					.List<StoreDocLine>();
			}
			
			// Relation (EntityRegistry-StoreDocLine): <Warehouse.StoreDocLine>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<StoreDocLine> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<StoreDocLine>();
			}
			
			public static System.Collections.Generic.IList<StoreDocLine> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<StoreDocLine>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<StoreDocLine> GetList(System.Linq.Expressions.Expression<Func<StoreDocLine, bool>> predicate)
			{
				return StoreDocLine.CreateCriteria().Add(Expression.Where<StoreDocLine>(predicate)).List<StoreDocLine>();
			}
			
			public static StoreDocLine GetUnique(System.Linq.Expressions.Expression<Func<StoreDocLine, bool>> predicate)
			{
				return StoreDocLine.CreateCriteria().Add(Expression.Where<StoreDocLine>(predicate)).UniqueResult<StoreDocLine>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
		public interface IStoreTransactionCollection : ISet<Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction> { }
		
		public interface IStoreTransaction
		{
		}
		
		public partial class StoreTransaction : IStoreTransaction, Arbinada.GenieLamp.Warehouse.Persistence.IPersistentObject, Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry, Arbinada.GenieLamp.Warehouse.Patterns.IUsesAudit
		{
			public StoreTransaction()
			{
			}
			
			public virtual int Version { get; set; }
			public virtual int Id { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor Supplier { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store Store { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product Product { get; set; }
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor Customer { get; set; }
			private Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry m_EntityRegistry = null;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry EntityRegistry
			{
				get { return m_EntityRegistry; }
				set { if (m_EntityRegistry == null) m_EntityRegistry = value; }
			}
			private DateTime m_TxDate = DateTime.Now;
			public virtual DateTime TxDate
			{
				get { return m_TxDate; }
				set { m_TxDate = value; }
			}
			private Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Direction m_Direction = (Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Direction)0;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Direction Direction
			{
				get { return m_Direction; }
				set { m_Direction = value; }
			}
			private Arbinada.GenieLamp.Warehouse.Domain.Warehouse.State m_State = (Arbinada.GenieLamp.Warehouse.Domain.Warehouse.State)0;
			public virtual Arbinada.GenieLamp.Warehouse.Domain.Warehouse.State State
			{
				get { return m_State; }
				set { m_State = value; }
			}
			public virtual int Quantity { get; set; }
			private string m_CreatedBy;
			public virtual string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			public virtual string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			public virtual DateTime? LastModifiedDate { get; set; }
			
			public static StoreTransaction GetById(int id)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreTransaction>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<StoreTransaction>();
			}
			
			public static StoreTransaction GetByEntityRegistryId(int? entityRegistryId)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreTransaction>()
					.Add(Expression.Eq("EntityRegistryId", entityRegistryId))
					.UniqueResult<StoreTransaction>();
			}
			
			public static System.Collections.Generic.IList<StoreTransaction> GetByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreTransaction>();
			}
			
			public static System.Collections.Generic.IList<StoreTransaction> GetPageByHQL(string hql, Arbinada.GenieLamp.Warehouse.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return StoreTransaction.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<StoreTransaction> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(StoreTransaction))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<StoreTransaction>();
			}
			
			public static System.Collections.Generic.IList<StoreTransaction> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return StoreTransaction.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				StoreTransaction o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().CreateCriteria<StoreTransaction>();
			}
			
			// Relation (Supplier-Transactions): <Warehouse.StoreTransaction>--M:1--<Warehouse.Contractor>
			public static System.Collections.Generic.IList<StoreTransaction> GetCollectionBySupplierId(int supplierId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Supplier.Id", supplierId))
					.List<StoreTransaction>();
			}
			
			// Relation (Store-StoreTransactionList): <Warehouse.StoreTransaction>--M:1--<Warehouse.Store>
			public static System.Collections.Generic.IList<StoreTransaction> GetCollectionByStore(int storeId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Store.Id", storeId))
					.List<StoreTransaction>();
			}
			
			// Relation (Product-StoreTransactionList): <Warehouse.StoreTransaction>--M:1--<Warehouse.Product>
			public static System.Collections.Generic.IList<StoreTransaction> GetCollectionByProduct(int productId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Product.Id", productId))
					.List<StoreTransaction>();
			}
			
			// Relation (Customer-StoreTransactionList): <Warehouse.StoreTransaction>--M:1--<Warehouse.Contractor>
			public static System.Collections.Generic.IList<StoreTransaction> GetCollectionByCustomer(int customerId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Customer.Id", customerId))
					.List<StoreTransaction>();
			}
			
			// Relation (EntityRegistry-StoreTransaction): <Warehouse.StoreTransaction>--1:1--<Core.EntityRegistry>
			public static System.Collections.Generic.IList<StoreTransaction> GetCollectionByEntityRegistryId(int? entityRegistryId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("EntityRegistry.Id", entityRegistryId))
					.List<StoreTransaction>();
			}
			
			public static System.Collections.Generic.IList<StoreTransaction> GetPage(int page, int pageSize, Arbinada.GenieLamp.Warehouse.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(Arbinada.GenieLamp.Warehouse.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<StoreTransaction>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public virtual void Delete(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					Arbinada.GenieLamp.Warehouse.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<StoreTransaction> GetList(System.Linq.Expressions.Expression<Func<StoreTransaction, bool>> predicate)
			{
				return StoreTransaction.CreateCriteria().Add(Expression.Where<StoreTransaction>(predicate)).List<StoreTransaction>();
			}
			
			public static StoreTransaction GetUnique(System.Linq.Expressions.Expression<Func<StoreTransaction, bool>> predicate)
			{
				return StoreTransaction.CreateCriteria().Add(Expression.Where<StoreTransaction>(predicate)).UniqueResult<StoreTransaction>();
			}
			
			#region Implementation of IUsesRegistry
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityRegistry Arbinada.GenieLamp.Warehouse.Patterns.IUsesRegistry.Registry
			{
				get { return this.EntityRegistry; }
				set { this.EntityRegistry = value; }
			}
			
			#endregion
		}
		
	}
	#endregion
}

