using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Metamodel.Physical;

namespace GenieLamp.Core.Test
{
	[TestFixture()]
	public class InitializationTest
	{

		public static GenieLamp LoadTestProject(string projectFileName)
		{
			SpellConfigMock config = new SpellConfigMock();
			config.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../Models/" + projectFileName);
			LoggerMock logger = new LoggerMock();
			GenieLamp lamp = GenieLamp.CreateGenieLamp(config, logger);
			Assert.IsNotNull(lamp, "Lamp was not created");
			try
			{
				lamp.Init();
			}
			catch(Exception e)
			{
				throw new Exception(logger.Lines.ToString(), e);
			}
			return lamp;
		}

		[Test()]
		public void LoadTest()
		{
			GenieLamp lamp = InitializationTest.LoadTestProject("TestProject.xml");
			Assert.AreEqual("Test", lamp.ProjectName);
			Assert.AreEqual("Warehouse", lamp.Model.DefaultSchema);
		}


		[Test()]
		public void ImplicitlyCreatedMetaobjectsTest()
		{
			GenieLamp lamp = InitializationTest.LoadTestProject("TestProject.xml");
			Model model = lamp.Model;

			Entity storeDoc = model.Entities.GetByName(QualName.MakeFullName(model.DefaultSchema, "StoreDoc"));
			Assert.IsNotNull(storeDoc, "Entity not exists");
			Generator gen = model.Generators.GetByName(QualName.MakeFullName(storeDoc.Schema, Generator.DefaultName(storeDoc)));
			Assert.IsNotNull(gen, "Implicit generator");
			Assert.AreEqual("SEQ_STORE_DOC", gen.Persistence.Name, "Implicit generator persistence name");
			Assert.AreEqual(Int32.MaxValue, gen.MaxValue, "Max value");
		}
		
		[Test()]
		public void ForeignKeyAndIndexPersistentNameTest()
		{
			GenieLamp lamp = InitializationTest.LoadTestProject("TestProject.xml");
			Model model = lamp.Model;

			Relation r = null;
			foreach(Relation r2 in model.Relations)
			{
				if (r2.Name == "Doc" && r2.Name2 == "Lines")
				{
					r = r2;
					break;
				}
			}
			Assert.IsNotNull(r, "Relation not found");
			Assert.AreEqual("FK_CUSTOM_NAME", r.Persistence.Name, "Relation persistent name was changed");
			Assert.IsNotNull(r.ForeignKey, "Relation does not have FK");
			Assert.AreEqual(r.Persistence.Name, r.ForeignKey.Name, "Foreign key custom name is not set correctly");

			Assert.AreEqual("IX_CUSTOM_NAME", r.IndexName, "Relation index name was changed");
			Assert.IsTrue(r.ForeignKey.HasIndex, "Foreign key does not have index");
			Assert.AreEqual(r.IndexName, r.ForeignKey.Index.Name, "Foreign key custom index name is not set corectly");
		}

		[Test()]
		public void ConstraintIndexPersistentNameTest()
		{
			GenieLamp lamp = InitializationTest.LoadTestProject("TestProject.xml");
			Model model = lamp.Model;

			Entity en = model.Entities.GetByName(QualName.MakeFullName(model.DefaultSchema, "ProductType"));
			Assert.IsNotNull(en, "ProductType entity not exists");
			UniqueId u = null;
			foreach(UniqueId u2 in en.Constraints.UniqueIds)
			{
				if (u2.Attributes.Count == 1 && u2.Attributes[0].Name == "Code")
				{
					u = u2;
					break;
				}
			}
			Assert.IsNotNull(u, "Constraint not found");
			Assert.IsTrue(u.Persistence.NameDefined, "Constraint does not have index name");
			Assert.AreEqual("IX_CUSTOM_PRODUCT_TYPE", u.Persistence.Name, "Constraint index name was changed");
			Assert.IsNotNull(u.Index, "Relation does not have index");
			Assert.AreEqual(u.Persistence.Name, u.Index.Name, "Relation persistent name was changed");
		}

		[Test()]
		public void IndexesTest()
		{
			GenieLamp lamp = InitializationTest.LoadTestProject("TestProject.xml");
			Model model = lamp.Model;

			Entity en = model.Entities.GetByName(QualName.MakeFullName(model.DefaultSchema, "StoreDoc"));
			Assert.IsNotNull(en, "Entity not exists");
			int indexCount = 0;
			bool hasCompositeIndex = false;
			foreach(Index ix in model.PhysicalModel.Indexes.GetByEntity(en))
			{
				if (ix.DefinedInModel)
				{
					Assert.IsTrue(ix.Generate, "Index should be generated");
					indexCount++;
					if (ix.Columns.Count == 1)
					{
						if (ix.Columns[0].Attribute.Name == "RefNum")
						{
							Assert.IsTrue(ix.Unique, "Index should be unique");
							Assert.AreEqual(ColumnOrder.Desc, ix.Columns[0].Order);
						}
						else if (ix.Columns[0].Attribute.Name == "Name")
						{
							Assert.IsFalse(ix.Unique, "Index should be not unique");
							Assert.AreEqual(ColumnOrder.Asc, ix.Columns[0].Order);
						}
					}
					else if (ix.Columns.Count > 1)
					{
						hasCompositeIndex = true;
					}
				}

			}
			Assert.AreEqual(3, indexCount, "Invalid index count");
			Assert.IsTrue(hasCompositeIndex, "Composite index absent");
		}
	}
}

