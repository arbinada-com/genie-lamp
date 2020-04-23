using System;
using System.Collections.Generic;

using NUnit.Framework;

using Arbinada.GenieLamp.Warehouse.Persistence;
using Arbinada.GenieLamp.Warehouse.Domain.Warehouse;


namespace Arbinada.GenieLamp.Warehouse.Domain.Test
{
	[TestFixture()]
	public class BasicTest
	{
		private const string ProductType1 = "InputDev";
		private const string ProductSubtype11 = "Keyboards";
		private const string ProductRef = "001";
		private const string ProductRef2 = "002";
		private const string ProductCaption = "Keyboard Std 103 key";
		private const string ProductCaption2 = "Keyboard Genius Multimedia 105 key";

		[TestFixtureSetUp]
		public void Init()
		{
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			SessionManager.CloseSession();
		}

		[SetUp]
		public void SetUp()
		{
			SessionManager.OpenSession();
		}

		[TearDown]
		public void TearDown()
		{
			SessionManager.CloseSession();
		}

		[Test()]
		public void TestSimpleCRUD()
		{
			Product product = new Product();
			product.RefCode = ProductRef;
			product.Caption = ProductCaption;

			Product product2 = new Product();
			product2.RefCode = ProductRef2;
			product2.Caption = ProductCaption2;

			UnitOfWork uow = new UnitOfWork();
			uow.Save(product);
			uow.Save(product2);
			uow.Commit();

			Assert.IsTrue(product.Id > 0, "Product Id was not updated");
			Assert.IsTrue(product2.Id > 0, "Product2 Id was not updated");

			Product p1 = Product.GetByRefCode(ProductRef);
			Assert.IsNotNull(p1);
			Assert.AreEqual(ProductCaption, p1.Caption);
			p1.Caption = ProductCaption2;
			p1.Save();
			
			Product p11 = Product.GetByRefCode(ProductRef);
			Assert.IsNotNull(p11);
			Assert.AreEqual(ProductCaption2, p11.Caption);

			Product p2 = Product.GetByRefCode(ProductRef2);
			Assert.IsNotNull(p2);

			UnitOfWork uow2 = new UnitOfWork();
			uow2.Delete(p11);
			uow2.Delete(p2);
			uow2.Commit();

			Assert.IsNull(Product.GetByRefCode(ProductRef));
			Assert.IsNull(Product.GetByRefCode(ProductRef2));
		}

		[Test]
		public void TestOneToMany()
		{
			string storeTypeName = "ST1";
			StoreType st1 = new StoreType();
			st1.Name = storeTypeName;

			Store s1 = new Store();
			s1.Code = "S01";
			s1.StoreType = st1;
			Store s2 = new Store();
			s2.Code = "S02";
			s2.StoreType = st1;

			UnitOfWork uow = new UnitOfWork();
			uow.Save(st1);
			uow.Save(s1);
			uow.Save(s2);
			uow.Commit();

			st1.Refresh();
			Assert.IsNotNull(st1.Stores);
			Assert.AreEqual(2, st1.Stores.Count);
		}

		[Test]
		public void TestOneToManySelfReferenced() // Hierarchy
		{
			ProductType pt1 = new ProductType();
			pt1.Code = ProductType1;
			pt1.Name = ProductType1;
			Assert.IsNull(pt1.EntityRegistry);
			pt1.Save();
			Assert.IsNotNull(pt1.EntityRegistry, "Registry was not created");
			Assert.AreEqual(pt1.GetType().FullName, pt1.EntityRegistry.EntityType.FullName, "Invalid entity type");

			ProductType pt11 = new ProductType();
			pt11.Code = ProductSubtype11;
			pt11.Parent = pt1;
			pt11.Save();
			Assert.IsNotNull(pt11.EntityRegistry);

			Assert.IsNull(pt1.ProductSubtypes);
			pt1.Refresh();
			Assert.IsNotNull(pt1.ProductSubtypes);

			pt1.ProductSubtypes.Remove(pt11);
			Assert.AreEqual(0, pt1.ProductSubtypes.Count);
			pt1.Save();
			pt11.Delete();
			pt1.Refresh();
			Assert.AreEqual(0, pt1.ProductSubtypes.Count);

			pt1.Delete();

			Assert.IsNull(ProductType.GetByCode(ProductSubtype11));
			Assert.IsNull(ProductType.GetByCode(ProductType1));
		}

		[Test]
		public void TestOneToOne()
		{
			string exampleName = "EX1";
			string typeExtCaption = "Extension";

			ExampleOneToOne et1 = new ExampleOneToOne();
			et1.Name = exampleName;
			et1.Save();
			Assert.IsNotNull(et1.EntityRegistry);

			ExampleOneToOneEx etx1 = new ExampleOneToOneEx();
			etx1.Caption = typeExtCaption;
			etx1.ExampleOneToOne = et1;
			etx1.Save();

			et1.Refresh();
			Assert.IsNotNull(et1.Extension);
			Assert.IsTrue(et1.Extension.Id > 0);
			Assert.IsTrue(etx1.Id > 0);
			Assert.AreEqual(typeExtCaption, et1.Extension.Caption);
			int extId = et1.Extension.Id;
			et1.Extension.Delete();

			Assert.IsNull(ExampleOneToOneEx.GetById(extId));

			et1.Delete();
			Assert.IsNull(ExampleOneToOne.GetByName(exampleName));
		}

		[Test]
		public void TestCompositeIdAndCascade()
		{
			const string docRef = "123";

			Product product = new Product();
			product.RefCode = "PROD" + docRef;
			product.Caption = ProductCaption;
			product.Save();

			StoreDoc doc = new StoreDoc();
			doc.RefNum = docRef;

			StoreDocLine line1 = new StoreDocLine();
			line1.Doc = doc;
			line1.Position = 1;
			line1.Product = product;
			line1.Quantity = 10;

			StoreDocLine line2 = new StoreDocLine();
			line2.Doc = doc;
			line2.Position = 2;
			line2.Product = product;
			line2.Quantity = 15;

			UnitOfWork uow = new UnitOfWork();
			uow.Save(doc);
			uow.Save(line1);
			uow.Save(line2);
			uow.Commit();

			doc.Refresh();
			Assert.AreEqual(2, doc.Lines.Count);

			SessionManager.CloseSession();
			StoreDoc doc2 = StoreDoc.GetByRefNum(docRef);
			Assert.IsNotNull(doc2, "Document was not saved. Ref: {0}", docRef);
			Assert.AreEqual(2, doc2.Lines.Count, "Document lines was not saved. Ref: {0}", docRef);
			int docId = doc2.Id;
			doc2.Delete();

			SessionManager.CloseSession();
			StoreDoc doc3 = StoreDoc.GetByRefNum(docRef);
			Assert.IsNull(doc3, "Document was not deleted. Ref: {0}", docRef);
			IList<StoreDocLine> lines3 = StoreDocLine.GetCollectionByStoreDocId(docId);
			Assert.IsNotNull(lines3, "Collection is null");
			Assert.AreEqual(0, lines3.Count, "Collection is not empty");
		}


		[Test]
		public void TestEventHandlers()
		{
			string storeCode = "S10";
			TestCustomMethods tester = new TestCustomMethods();

			StoreType st1 = new StoreType() { Name = "INT", Caption = "Internal store" };
			st1.Save();
			Store s1 = new Store() { Code = storeCode, StoreType = st1, Caption = "Store 1", Tester = tester };

			Assert.AreEqual(0, tester.Validate);
			Assert.AreEqual(0, tester.OnSave);
			Assert.AreEqual(0, tester.OnFlush);
			Assert.AreEqual(0, tester.OnDelete);

			s1.Save();

			Assert.AreEqual(3, tester.Validate);
			Assert.AreEqual(1, tester.OnSave);
			Assert.AreEqual(2, tester.OnFlush); // Because of registry added
			Assert.AreEqual(0, tester.OnDelete);

			s1.Caption = "Changed";
			s1.Save();
			Assert.AreEqual(4, tester.Validate);
			Assert.AreEqual(2, tester.OnSave);
			Assert.AreEqual(3, tester.OnFlush);
			Assert.AreEqual(0, tester.OnDelete);

			s1.Delete();
			Assert.AreEqual(4, tester.Validate);
			Assert.AreEqual(2, tester.OnSave);
			Assert.AreEqual(3, tester.OnFlush);
			Assert.AreEqual(1, tester.OnDelete);

			SessionManager.CloseSession();

			Assert.IsNull(Store.GetByCode(storeCode), "Store was not deleted");
			TestCustomMethods tester2 = new TestCustomMethods();
			Store st2 = new Store() { Code = storeCode, StoreType = st1, Caption = "Store 2", Tester = tester2 };

			bool exOccurred = false;
			tester2.ThrowOnValidate = true;
			try
			{
				st2.Save();
			}
			catch(Exception e)
			{
				Assert.AreEqual("Store.Validate()", e.Message);
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred);
			Assert.IsNull(Store.GetByCode(storeCode), "Store was saved but Validate exception");

			tester2.Clear();
			tester2.ThrowOnSave = true;
			exOccurred = false;
			try
			{
				st2.Save();
			}
			catch(Exception e)
			{
				Assert.AreEqual("Store.OnSave()", e.Message);
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred);
			Assert.IsNull(Store.GetByCode(storeCode), "Store was saved but OnSave exception");

			tester2.Clear();
			tester2.ThrowOnFlush = true;
			exOccurred = false;
			try
			{
				st2.Save();
			}
			catch(Exception e)
			{
				Assert.AreEqual("Store.OnFlush()", e.Message);
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred);
			Assert.IsNull(Store.GetByCode(storeCode), "Store was saved but OnFlush exception");

			tester2.Clear();
			exOccurred = false;
			try
			{
				st2.Save();
			}
			catch(Exception)
			{
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred, "Saved after exception OnFlush");

			SessionManager.CloseSession();
			Store st3 = new Store() { Code = storeCode, StoreType = st1, Caption = "Store 2", Tester = tester2 };
			st3.Save();
			SessionManager.CloseSession();

			Store st4 = Store.GetByCode(storeCode);
			Assert.IsNotNull(st4, "Store was not saved");
			st4.Tester = tester2;

			string caption = st4.Caption;
			st4.Caption = "555";
			tester2.Clear();
			tester2.ThrowOnValidate = true;
			exOccurred = false;
			try
			{
				st4.Save();
			}
			catch(Exception e)
			{
				Assert.AreEqual("Store.Validate()", e.Message);
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred);

			SessionManager.CloseSession();
			Store st5 = Store.GetByCode(storeCode);
			Assert.AreEqual(caption, st5.Caption, "Store was updated but Validate exception");
			st5.Tester = tester2;
			st5.Caption = "555";
			tester2.Clear();
			tester2.ThrowOnSave = true;
			exOccurred = false;
			try
			{
				st5.Save();
			}
			catch(Exception e)
			{
				Assert.AreEqual("Store.OnSave()", e.Message);
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred);

			SessionManager.CloseSession();
			Store st6 = Store.GetByCode(storeCode);
			Assert.AreEqual(caption, st6.Caption, "Store was updated but OnSave exception");
			st6.Tester = tester2;
			st6.Caption = "555";
			tester2.Clear();
			tester2.ThrowOnFlush = true;
			exOccurred = false;
			try
			{
				st6.Save();
			}
			catch(Exception e)
			{
				Assert.AreEqual("Store.OnFlush()", e.Message);
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred);

			SessionManager.CloseSession();
			Store st7 = Store.GetByCode(storeCode);
			Assert.AreEqual(caption, st7.Caption, "Store was updated but OnFlush exception");
			st7.Tester = tester2;

			tester2.Clear();
			tester2.ThrowOnDelete = true;
			exOccurred = false;
			try
			{
				st7.Delete();
			}
			catch(Exception e)
			{
				Assert.AreEqual("Store.OnDelete()", e.Message);
				exOccurred = true;
			}
			Assert.IsTrue(exOccurred);

			SessionManager.CloseSession();
			Store st8 = Store.GetByCode(storeCode);
			Assert.IsNotNull(st8, "Store was deleted but OnDelete exception");
			st8.Tester = tester2;

			tester2.Clear();
			tester2.ThrowOnFlush = true;
			tester2.ThrowOnValidate = true;
			exOccurred = false;
			try
			{
				st8.Delete();
			}
			catch(Exception)
			{
				exOccurred = true;
			}
			Assert.IsFalse(exOccurred, "Unexpected exception on delete. Tester: {0}", tester2.ToString());

			SessionManager.CloseSession();
			Assert.IsNull(Store.GetByCode(storeCode), "Store was not deleted");
		}
	}
}

