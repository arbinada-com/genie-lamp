using System;

using NUnit.Framework;

using Arbinada.GenieLamp.Warehouse.Persistence;
using Arbinada.GenieLamp.Warehouse.Domain.Warehouse;

namespace Arbinada.GenieLamp.Warehouse.Domain.Test
{
	public class PatternsTest
	{
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

		[Test]
		public void TestStateVersion()
		{
			string productRef = "PR" + DateTime.Now.ToString("HHMMss");
			Product p1 = new Product();
			Assert.IsFalse(p1.Version > 0, "Version was set before saving");
			p1.RefCode = productRef;
			p1.Caption = "State version test";
			p1.Save();
			Assert.IsTrue(p1.Version > 0, "Version was not set");

			SessionManager.CloseSession();

			Product p2 = Product.GetByRefCode(productRef);
			Assert.IsNotNull(p2, "New product was not saved");
			int version = p2.Version;
			p2.Caption = p2.Caption + ". Modified";
			p2.Save();
			Assert.IsTrue(p2.Version > version, "Version was not refreshed");

			SessionManager.CloseSession();

			Product p3 = Product.GetByRefCode(productRef);
			Assert.IsNotNull(p3, "Modified product was not saved");
			Assert.IsTrue(p3.Version > version, "Version was not incremented");
		}

		[Test]
		public void TestAuditSimpleAndRegistry()
		{
			Partner p1 = new Partner();
			p1.Name = "Free Software Foundation";
			p1.Address = "51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA";
			p1.Email = "mail@fsf.org";
			p1.Save();

			Assert.IsNotNull(p1.EntityRegistry, "Registry was not created");
			int registryId = p1.EntityRegistry.Id;
			int partnerId = p1.Id;

			SessionManager.CloseSession();

			Partner p2 = Partner.GetById(partnerId);
			Assert.IsNotNull(p2, "Partner was not persisted");
			Assert.IsNotNull(p2.CreatedBy, "Creator was not set");
			Assert.IsNotNull(p2.CreatedDate, "Creation date was not set");
			Assert.IsNotNull(p2.EntityRegistry, "Registry was not associated");
			Core.EntityRegistry r1 = Core.EntityRegistry.GetById(registryId);
			Assert.IsNotNull(r1, "Registry was not persisted");
			DateTime created = p2.CreatedDate.Value;
			p2.Phone = "123456";
			p2.Save();

			SessionManager.CloseSession();

			Partner p3 = Partner.GetById(partnerId);
			Assert.IsNotNull(p3.LastModifiedBy, "Updater was not set");
			Assert.IsNotNull(p3.LastModifiedDate, "Update date was not set");
			Assert.IsTrue(created < p3.LastModifiedDate.Value, "Creation date is greater than update one");

			p3.Delete();
			SessionManager.CloseSession();

			Partner p4 = Partner.GetById(partnerId);
			Assert.IsNull(p4, "Partner was not deleted");
			Core.EntityRegistry r4 = Core.EntityRegistry.GetById(registryId);
			Assert.IsNull(r4, "Registry was not deleted. RegistryId {0}", registryId);
		}
	}
}

