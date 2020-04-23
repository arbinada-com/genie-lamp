using NUnit.Framework;
using System;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Test
{
	[TestFixture()]
	public class TypesTest
	{
		[Test()]
		public void MaxValueTest()
		{
			GenieLamp lamp = InitializationTest.LoadTestProject("TestProject.xml");
			lamp.Init();

			TypeDefinition tdFixed = new TypeDefinition(false, 1, false, 0, false, true, false, false, false, "");

			StdType t1 = new StdType(lamp.Model, "", "testType1", BaseType.TypeInt, tdFixed);
			Assert.IsNotNull(t1.MaxValue, "Has not max value");
			Assert.AreEqual(byte.MaxValue, t1.MaxValue.Value);

			tdFixed.Length = 2;
			StdType t2 = new StdType(lamp.Model, "", "testType2", BaseType.TypeInt, tdFixed);
			Assert.IsNotNull(t2.MaxValue, "Has not max value");
			Assert.AreEqual(Int16.MaxValue, t2.MaxValue.Value);

			tdFixed.Length = 4;
			StdType t3 = new StdType(lamp.Model, "", "testType3", BaseType.TypeInt, tdFixed);
			Assert.IsNotNull(t3.MaxValue, "Has not max value");
			Assert.AreEqual(Int32.MaxValue, t3.MaxValue.Value);

			tdFixed.Length = 8;
			StdType t4 = new StdType(lamp.Model, "", "testType4", BaseType.TypeInt, tdFixed);
			Assert.IsNotNull(t4.MaxValue, "Has not max value");
			Assert.AreEqual(Int64.MaxValue, t4.MaxValue.Value);

			tdFixed.Length = 20;
			StdType t5 = new StdType(lamp.Model, "", "testType5", BaseType.TypeDecimal, tdFixed);
			Assert.IsNotNull(t5.MaxValue, "Has not max value");
			Assert.AreEqual(Convert.ToDecimal(Math.Pow(10, 20)), t5.MaxValue.Value);

			StdType t6 = new StdType(lamp.Model, "", "testType6", BaseType.TypeCurrency, tdFixed);
			Assert.IsNull(t6.MaxValue, "Has max value");
		}
	}
}

