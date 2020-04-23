// Genie Lamp Core (1.1.4798.27721)
// Genie of NHibernate framework (1.0.4798.27723)
// Starter application (1.1.4798.27722)
// This file was automatically generated at 2013-03-14 16:56:47
// Do not modify it manually.

using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

using NHibernate;
using NHibernate.Criterion;

namespace GenieLamp.Examples.QuickStart.Domain
{
	#region Enumerations
	#endregion
	#region Entities classes
	namespace QuickStart
	{
		public interface ICustomerCollection : ISet<GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer> { }
		
		public interface ICustomer
		{
		}
		
		public partial class Customer : ICustomer, GenieLamp.Examples.QuickStart.Persistence.IPersistentObject
		{
			public Customer()
			{
			}
			
			public virtual int Id { get; set; }
			public virtual Iesi.Collections.Generic.ISet<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder> Orders { get; set; }
			private string m_Code;
			public virtual string Code
			{
				get { return m_Code; }
				set { m_Code = value != null && value.Length > 10 ? value.Substring(0, 10) : value; }
			}
			private string m_Name;
			public virtual string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_Phone;
			public virtual string Phone
			{
				get { return m_Phone; }
				set { m_Phone = value != null && value.Length > 40 ? value.Substring(0, 40) : value; }
			}
			private string m_Email;
			public virtual string Email
			{
				get { return m_Email; }
				set { m_Email = value != null && value.Length > 255 ? value.Substring(0, 255) : value; }
			}
			
			public static Customer GetById(int id)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<Customer>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<Customer>();
			}
			
			public static Customer GetByCode(string code)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<Customer>()
					.Add(Expression.Eq("Code", code))
					.UniqueResult<Customer>();
			}
			
			public static System.Collections.Generic.IList<Customer> GetByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Customer>();
			}
			
			public static System.Collections.Generic.IList<Customer> GetPageByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return Customer.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<Customer> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(Customer))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Customer>();
			}
			
			public static System.Collections.Generic.IList<Customer> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return Customer.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				Customer o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<Customer>();
			}
			
			public static System.Collections.Generic.IList<Customer> GetPage(int page, int pageSize, GenieLamp.Examples.QuickStart.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(GenieLamp.Examples.QuickStart.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<Customer>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
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
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<Customer> GetList(System.Linq.Expressions.Expression<Func<Customer, bool>> predicate)
			{
				return Customer.CreateCriteria().Add(Expression.Where<Customer>(predicate)).List<Customer>();
			}
			
			public static Customer GetUnique(System.Linq.Expressions.Expression<Func<Customer, bool>> predicate)
			{
				return Customer.CreateCriteria().Add(Expression.Where<Customer>(predicate)).UniqueResult<Customer>();
			}
			
		}
		
		public interface IProductCollection : ISet<GenieLamp.Examples.QuickStart.Domain.QuickStart.Product> { }
		
		public interface IProduct
		{
		}
		
		public partial class Product : IProduct, GenieLamp.Examples.QuickStart.Persistence.IPersistentObject
		{
			public Product()
			{
			}
			
			public virtual int Id { get; set; }
			private string m_Reference;
			public virtual string Reference
			{
				get { return m_Reference; }
				set { m_Reference = value != null && value.Length > 10 ? value.Substring(0, 10) : value; }
			}
			private string m_Name;
			public virtual string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			
			public static Product GetById(int id)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<Product>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<Product>();
			}
			
			public static Product GetByReference(string reference)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<Product>()
					.Add(Expression.Eq("Reference", reference))
					.UniqueResult<Product>();
			}
			
			public static System.Collections.Generic.IList<Product> GetByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<Product>();
			}
			
			public static System.Collections.Generic.IList<Product> GetPageByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return Product.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<Product> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
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
				GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<Product>();
			}
			
			public static System.Collections.Generic.IList<Product> GetPage(int page, int pageSize, GenieLamp.Examples.QuickStart.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(GenieLamp.Examples.QuickStart.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<Product>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
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
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Delete(this);
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
			
		}
		
		public interface IPurchaseOrderCollection : ISet<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder> { }
		
		public interface IPurchaseOrder
		{
			/// <summary>
			/// <Text lang="en">Return the total amount of the order</Text>
			/// </summary>
			decimal TotalAmount { get; }
			/// <summary>
			/// <Text lang="en">Set purchase order state "Validated" to true</Text>
			/// </summary>
			void Validate();
		}
		
		public partial class PurchaseOrder : IPurchaseOrder, GenieLamp.Examples.QuickStart.Persistence.IPersistentObject
		{
			public PurchaseOrder()
			{
			}
			
			public virtual int Id { get; set; }
			public virtual GenieLamp.Examples.QuickStart.Domain.QuickStart.Customer Customer { get; set; }
			public virtual Iesi.Collections.Generic.ISet<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine> Lines { get; set; }
			private string m_Number;
			public virtual string Number
			{
				get { return m_Number; }
				set { m_Number = value != null && value.Length > 15 ? value.Substring(0, 15) : value; }
			}
			public virtual DateTime IssueDate { get; set; }
			private bool m_Validated = false;
			public virtual bool Validated
			{
				get { return m_Validated; }
				set { m_Validated = value; }
			}
			public virtual DateTime? ShipmentDate { get; set; }
			
			public static PurchaseOrder GetById(int id)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<PurchaseOrder>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<PurchaseOrder>();
			}
			
			public static PurchaseOrder GetByNumber(string number)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<PurchaseOrder>()
					.Add(Expression.Eq("Number", number))
					.UniqueResult<PurchaseOrder>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrder> GetByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<PurchaseOrder>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrder> GetPageByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return PurchaseOrder.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<PurchaseOrder> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(PurchaseOrder))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<PurchaseOrder>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrder> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return PurchaseOrder.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				PurchaseOrder o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<PurchaseOrder>();
			}
			
			// Relation (Orders-Customer): <QuickStart.Customer>--1:M--<QuickStart.PurchaseOrder>
			public static System.Collections.Generic.IList<PurchaseOrder> GetCollectionByCustomerId(int customerId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Customer.Id", customerId))
					.List<PurchaseOrder>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrder> GetPage(int page, int pageSize, GenieLamp.Examples.QuickStart.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(GenieLamp.Examples.QuickStart.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<PurchaseOrder>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
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
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<PurchaseOrder> GetList(System.Linq.Expressions.Expression<Func<PurchaseOrder, bool>> predicate)
			{
				return PurchaseOrder.CreateCriteria().Add(Expression.Where<PurchaseOrder>(predicate)).List<PurchaseOrder>();
			}
			
			public static PurchaseOrder GetUnique(System.Linq.Expressions.Expression<Func<PurchaseOrder, bool>> predicate)
			{
				return PurchaseOrder.CreateCriteria().Add(Expression.Where<PurchaseOrder>(predicate)).UniqueResult<PurchaseOrder>();
			}
			
		}
		
		public interface IPurchaseOrderLineCollection : ISet<GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrderLine> { }
		
		public interface IPurchaseOrderLine
		{
		}
		
		public partial class PurchaseOrderLine : IPurchaseOrderLine, GenieLamp.Examples.QuickStart.Persistence.IPersistentObject
		{
			public PurchaseOrderLine()
			{
			}
			
			public virtual int Id { get; set; }
			public virtual GenieLamp.Examples.QuickStart.Domain.QuickStart.PurchaseOrder PurchaseOrder { get; set; }
			public virtual GenieLamp.Examples.QuickStart.Domain.QuickStart.Product Product { get; set; }
			public virtual short Position { get; set; }
			public virtual decimal Price { get; set; }
			public virtual int Quantity { get; set; }
			
			public static PurchaseOrderLine GetById(int id)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<PurchaseOrderLine>()
					.Add(Expression.Eq("Id", id))
					.UniqueResult<PurchaseOrderLine>();
			}
			
			public static PurchaseOrderLine GetByPurchaseOrderIdPosition(int purchaseOrderId, short position)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<PurchaseOrderLine>()
					.Add(Expression.Eq("PurchaseOrder.Id", purchaseOrderId))
					.Add(Expression.Eq("Position", position))
					.UniqueResult<PurchaseOrderLine>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Queries.QueryFactory.CreateQuery(hql, hqlParams)
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<PurchaseOrderLine>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetPageByHQL(string hql, GenieLamp.Examples.QuickStart.Queries.DomainQueryParams hqlParams, int page = 0, int pageSize = 20)
			{
				return PurchaseOrderLine.GetByHQL(hql, hqlParams, page * pageSize, pageSize);
			}
			
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetBySQL(string sql, int firstResult = 0, int maxResult = 100)
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateSQLQuery(sql)
					.AddEntity(typeof(PurchaseOrderLine))
					.SetFirstResult(firstResult < 0 ? 0 : firstResult)
					.SetMaxResults(maxResult < firstResult ? 100 : maxResult)
					.List<PurchaseOrderLine>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetPageBySQL(string sql, int page = 0, int pageSize = 20)
			{
				return PurchaseOrderLine.GetPageBySQL(sql, page * pageSize, pageSize);
			}
			
			public static void DeleteById(int id)
			{
				PurchaseOrderLine o = GetById(id);
				if (o != null) o.Delete();
			}
			
			public virtual void Refresh()
			{
				GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Refresh(this);
			}
			
			public static ICriteria CreateCriteria()
			{
				return GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().CreateCriteria<PurchaseOrderLine>();
			}
			
			// Relation (PurchaseOrder-Lines): <QuickStart.PurchaseOrderLine>--M:1--<QuickStart.PurchaseOrder>
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetCollectionByPurchaseOrderId(int purchaseOrderId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("PurchaseOrder.Id", purchaseOrderId))
					.List<PurchaseOrderLine>();
			}
			
			// Relation (Product-PurchaseOrderLineList): <QuickStart.PurchaseOrderLine>--M:1--<QuickStart.Product>
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetCollectionByProductId(int productId)
			{
				return CreateCriteria()
					.Add(Expression.Eq("Product.Id", productId))
					.List<PurchaseOrderLine>();
			}
			
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetPage(int page, int pageSize, GenieLamp.Examples.QuickStart.Queries.SortOrder[] sortOrders)
			{
				int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);
				ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);
				if (sortOrders != null)
					foreach(GenieLamp.Examples.QuickStart.Queries.SortOrder sort in sortOrders)
						criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));
				return criteria.List<PurchaseOrderLine>();
			}
			
			public virtual void Save(ITransaction outer = null)
			{
				ITransaction tx = null;
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().SaveOrUpdate(this);
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
				if (outer == null) tx = GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().BeginTransaction();
				try
				{
					GenieLamp.Examples.QuickStart.Persistence.SessionManager.GetSession().Delete(this);
					if (outer == null) tx.Commit();
				}
				catch(Exception)
				{
					if (outer == null) tx.Rollback();
					throw;
				}
			}
			
			public static System.Collections.Generic.IList<PurchaseOrderLine> GetList(System.Linq.Expressions.Expression<Func<PurchaseOrderLine, bool>> predicate)
			{
				return PurchaseOrderLine.CreateCriteria().Add(Expression.Where<PurchaseOrderLine>(predicate)).List<PurchaseOrderLine>();
			}
			
			public static PurchaseOrderLine GetUnique(System.Linq.Expressions.Expression<Func<PurchaseOrderLine, bool>> predicate)
			{
				return PurchaseOrderLine.CreateCriteria().Add(Expression.Where<PurchaseOrderLine>(predicate)).UniqueResult<PurchaseOrderLine>();
			}
			
		}
		
	}
	#endregion
}

