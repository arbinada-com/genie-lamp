using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GenieLamp.Examples.QuickStart.Domain.QuickStart;
using GenieLamp.Examples.QuickStart.Persistence;

namespace Domain.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Customer c1 = new Customer()
            {
                Code = "C001",
                Name = "BlueHat Inc.",
                Email = "contact@bluehatinc.com"
            };
            c1.Save();

            Product prod1 = new Product()
            {
                Reference = "P01",
                Name = "Apple jus 50cl"
            };
            prod1.Save();

            Product prod2 = new Product()
            {
                Reference = "P02",
                Name = "Orange jus 25cl"
            };
            prod2.Save();

            NHibernate.ITransaction tx = SessionManager.GetSession().BeginTransaction();
            try
            {
                PurchaseOrder po1 = new PurchaseOrder()
                {
                    Number = "PO0001",
                    Customer = c1,
                    IssueDate = DateTime.Now.Date
                };
                po1.Save(tx);

                PurchaseOrderLine line1 = new PurchaseOrderLine()
                {
                    PurchaseOrder = po1,
                    Position = 1,
                    Product = prod1,
                    Price = 10.5M,
                    Quantity = 10
                };
                line1.Save(tx);

                PurchaseOrderLine line2 = new PurchaseOrderLine()
                {
                    PurchaseOrder = po1,
                    Position = 2,
                    Product = prod2,
                    Price = 4.35M,
                    Quantity = 20
                };
                line2.Save(tx);
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
