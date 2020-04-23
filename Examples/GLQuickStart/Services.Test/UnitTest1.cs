using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenieLamp.Examples.QuickStart.Services.Adapters.QuickStart;
using GenieLamp.Examples.QuickStart.Services.Adapters;

namespace Services.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Preparing connection to services
            WebClientFactory.AuthRequired = false;
            WebClientFactory.ServiceUrl = "http://localhost:8080/";
            // Do tests
            PurchaseOrder po = PurchaseOrder.GetByNumber("PO0001");
            PurchaseOrderLineCollection lines =
                PurchaseOrderLineCollection.GetCollectionByPurchaseOrderId(po.Id);
            decimal total = 0.0M;
            foreach (PurchaseOrderLine line in lines)
            {
                total += line.Price * line.Quantity;
            }
            Assert.AreEqual(total, po.TotalAmount, "Invalid total amount");
            Assert.IsFalse(po.Validated, "Order is already validated");
            po.Validate();
            Assert.IsTrue(po.Validated, "Order is not validated");
        }
    }
}
