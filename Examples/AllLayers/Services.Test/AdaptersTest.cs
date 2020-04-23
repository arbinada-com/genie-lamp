using NUnit.Framework;
using System;

using Arbinada.GenieLamp.Warehouse.Services.Interfaces;
using Arbinada.GenieLamp.Warehouse.Services.Adapters;
using Arbinada.GenieLamp.Warehouse.Services.Adapters.Warehouse;

namespace Arbinada.GenieLamp.Warehouse.Services.Test
{
	[TestFixture]
	public class AdaptersTest
	{
		private const string ProductRef = "010";
		private const string ProductTypeCode = "KBD";
		private const string ProductCaption = "Keyboard Std 103 key";
		private const string ProductCaption2 = "Keyboard Genius Multimedia 105 key";

		[SetUp]
		public void InitTest()
		{
//			Environment.Locker.WaitOne();
		}

		[TearDown]
		public void CleanupTest()
		{
//			Environment.Locker.ReleaseMutex();
		}

		[Test]
		public void TestObjectLifecycle()
		{
			Environment.Log.Info("TestObjectLifecycle");

			ProductType prodType1 = new ProductType();
			prodType1.Code = ProductTypeCode;
			prodType1.Name = "Keyboard";

			Product product1 = new Product();
			product1.RefCode = ProductRef;
			product1.Caption = ProductCaption;
			//product1.Id = 21;

			UnitOfWork uow = new UnitOfWork();
			uow.Save(prodType1);
			uow.Save(product1);
			uow.Commit();

			Assert.IsTrue(prodType1.Id > 0, "ProdType was not saved");
			Assert.IsTrue(product1.Id > 0, "Product was not saved");
			Environment.Log.InfoFormat("2: prodType1.Id: {0}", prodType1.Id);
			Environment.Log.InfoFormat("2: product1.Id: {0}", product1.Id);
			Assert.IsFalse(product1.TypeId.HasValue, "Relation was set");

			product1.TypeId = prodType1.Id;
			product1.Save(true);

			Product product2 = Product.GetById(product1.Id);
			Assert.AreEqual(prodType1.Id, product2.TypeId, "Relation was not set");

			product2.Delete(true);
			Assert.IsNull(Product.GetById(product1.Id), "Product was not deleted");

			prodType1.Delete(true);
			Assert.IsNull(ProductType.GetById(prodType1.Id), "Product type was not deleted");
		}

	}
}

