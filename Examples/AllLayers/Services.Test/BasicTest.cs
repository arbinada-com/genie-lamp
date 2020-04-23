using NUnit.Framework;
using System;

using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;

using Arbinada.GenieLamp.Warehouse.Services;
using Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse;
using Arbinada.GenieLamp.Warehouse.Services.Interfaces;


namespace Arbinada.GenieLamp.Warehouse.Services.Test
{
	[TestFixture]
	public class BasicTest
	{
		private const string ProductRef1 = "001";
		private const string ProductRef2 = "002";
		private const string ProductCaption1 = "Keyboard Std 103 key";
		private const string ProductCaption2 = "Keyboard Genius Multimedia 105 key";
		private JsonServiceClient client;

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

#if SSTACK
		[Test]
		public void TestServiceStackREST()
		{
			client = new JsonServiceClient(Environment.ServiceUrl);

			Environment.Log.Info("TestServiceStackREST");

			ProductDTO product1 = new ProductDTO();
			product1.RefCode = ProductRef1;
			product1.Caption = ProductCaption1;
			ProductRequest prq1 = new ProductRequest();
			prq1.ProductDTO = product1;
			ProductResponse prr1 = client.Post<ProductResponse>("/ProductService", prq1);
			Assert.IsFalse(prr1.CommitResult.HasError, prr1.CommitResult.Message);
			product1 = prr1.ProductDTO;
			Assert.AreNotEqual(0, product1.Id, "Invalid Id");

			Environment.Log.InfoFormat("1: product1.Id: {0}", product1.Id);

			ProductResponse prr2 = client.Delete<ProductResponse>(String.Format("/ProductService/Id/{0}", product1.Id));
			Assert.IsFalse(prr2.CommitResult.HasError, prr2.CommitResult.Message);

			ProductDTO product2 = new ProductDTO();
			product2.RefCode = ProductRef2;
			product2.Caption = ProductCaption2;
			UnitOfWorkDTO uow = new UnitOfWorkDTO();
			uow.Save(product2);
			Environment.Log.InfoFormat("1: product2.Id: {0}", product2.Id);
			PersistenceRequest pr2 = new PersistenceRequest();
			pr2.UnitOfWork = uow;
			PersistenceResponse ps2 = client.Post<PersistenceResponse>("/Persistence", pr2);
			Assert.IsFalse(ps2.CommitResult.HasError, ps2.CommitResult.Message);
			ps2.UpdatedObjects.Update<ProductDTO>(ref product2);
			Assert.AreNotEqual(0, product2.Id, "Invalid Id");


			ProductResponse prr3 = client.Get<ProductResponse>("/ProductService/Id/" + product2.Id);
			Assert.IsNotNull(prr3.ProductDTO);

			ProductListResponse prodList = client.Get<ProductListResponse>("/ProductService");
			foreach(ProductDTO prod in prodList.ProductDTOList)
			{
				if (prod.RefCode == ProductRef1 || prod.RefCode == ProductRef2)
					client.Delete<ProductResponse>(String.Format("/ProductService/Id/{0}", prod.Id));
			}

			client.Dispose();
		}
#endif

#if WCF
		[Test()]
		public void TestServiceStackREST()
		{
			/*
			DomainServices services = new DomainServices();
			ProductDTO product1 = (services as IProductService).GetProductByRefCode(ProductRef);
			Assert.IsNotNull(product1, "Product was not created");
			Assert.AreEqual(ProductCaption, product1.Caption);

			product1.Caption = ProductCaption2;
			UnitOfWork uof1 = new UnitOfWork();
			uof1.Save(product1);
			uof1.Commit();
			
			ProductDTO product2 = (services as IProductService).GetProductByRefCode(ProductRef);
			Assert.IsNotNull(product2);
			Assert.AreEqual(ProductCaption2, product2.Caption, "Product was not updated");
			Assert.AreNotEqual(0, product2.Id, "Invalid Id");
			
			UnitOfWork uof2 = new UnitOfWork();
			uof2.Delete(product2);
			uof2.Commit();
			
			ProductDTO product3 = (services as IProductService).GetProductByRefCode(ProductRef);
			Assert.IsNull(product3, "Product was not deleted");
			
			ProductTypeProxy pt1 = (services as IProductTypeService).CreateProductType();
			pt1.Code = ProductType;
			pt1.Name = String.Format("Type {0}", pt1.Code);
			UnitOfWork uow3 = new UnitOfWork();
			uow3.Save(pt1);
			cr = uow3.Commit();
			Assert.IsFalse(cr.HasError, cr.Message);
			
			ProductTypeProxy pt2 = (services as IProductTypeService).GetProductTypeByCode(ProductType);
			Assert.IsNotNull(pt2, "Product type was not created");
			Assert.AreEqual(ProductType, pt2.Code);
			
			
			UnitOfWork uow4 = new UnitOfWork();
			uow4.Delete(pt2);
			uow4.Commit();
			Assert.IsNull((services as IProductTypeService).GetProductTypeByCode(ProductType), "Product type was not deleted");
			*/
			/*
			ProductDTO product = ProductDTO.GetByRefCode(ProductRef);
			if (product != null)
			{
				UnitOfWork uof = new UnitOfWork();
				uof.Delete(product);
				CommitResult cr = uof.Commit();
				Assert.IsFalse(cr.HasError, cr.Message);
			}
			
			ProductTypeProxy pt1 = ProductTypeProxy.GetByCode(ProductType);
			if (pt1 != null)
			{
				UnitOfWork uof = new UnitOfWork();
				uof.Delete(pt1);
				CommitResult cr = uof.Commit();
				Assert.IsFalse(cr.HasError, cr.Message);
			}
			*/
		}
#endif

	}
}

