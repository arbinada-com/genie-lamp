using NUnit.Framework;
using System;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;


namespace GenieLamp.Core.Test
{
	[TestFixture()]
	public class SpellHintTest
	{
		private static GenieLamp lamp;

		[TestFixtureSetUp]
		public void TestSetup()
		{
			lamp = InitializationTest.LoadTestProject("TestProject.xml");
			Assert.IsNotNull(lamp, "Lamp was not created");
			lamp.Init();
		}

		[Test()]
		public void GetTextTest()
		{
			foreach(Entity entity in lamp.Model.Entities)
			{
				SpellHint tableHint = lamp.Model.SpellHints.Find("SQLServer", "2008", entity);
				Assert.IsNotNull(tableHint, "No entity hint found");
				if (entity.SpellHintParams.Count == 0)
				{
					Assert.AreEqual("ON DEFAULT", tableHint.GetText(entity));
				}
				else
				{
					ParamSimple param = entity.SpellHintParams.FindParam("SizeBigImg", "*");
					if (param != null)
					{
						Assert.AreEqual("ON MYFILEGROUP TEXTIMAGE_ON(MYFILEGROUP_TI)", tableHint.GetText(entity));
					}
					param = entity.SpellHintParams.FindParam("SizeBig", "*");
					if (param != null)
					{
						Assert.AreEqual("ON MYFILEGROUP", tableHint.GetText(entity));
					}
					param = entity.SpellHintParams.FindParam("Size", "Big");
					if (param != null)
					{
						Assert.AreEqual("ON MYFILEGROUP /*ParamBig*/", tableHint.GetText(entity));
					}
				}

				SpellHint tableHint2005 = lamp.Model.SpellHints.Find("SQLServer", "2005", entity);
				Assert.IsNotNull(tableHint2005, "No entity hint for SQL 2005 found");
				Assert.AreEqual("ON STORE_FG", tableHint2005.GetText(entity));
				Assert.IsNotNull(tableHint2005.SpellHintProperties.ParamByName("Name1", false), "No property found");
				Assert.AreEqual("Value1", tableHint2005.SpellHintProperties.ValueByName("Name1", String.Empty));
			}
		}

	}
}

