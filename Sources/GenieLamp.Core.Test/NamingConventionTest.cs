using NUnit.Framework;
using System;

using GenieLamp.Core;
using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Test
{
	[TestFixture()]
	public class NamingConventionTest
	{
		[Test()]
		public void TestConvert()
		{
			string name1 = "_DFC.EntityType__Root_SUFFIX_term10";
			string name2 = "_DFC_ENTITY_Type_Root_SUFFIX_TERM10";
			string name3 = "_Dfc_Entity_type_RootSuffix_term10";
			
			NamingConvention c = new NamingConvention(NamingStyle.LowerCase);
			string nameLC = "_dfc_entity_type_root_suffix_term10";
			Assert.AreEqual(nameLC, c.Convert(name1));
			Assert.AreEqual(nameLC, c.Convert(name2));
			Assert.AreEqual(nameLC, c.Convert(name3));
			
			c = new NamingConvention(NamingStyle.UpperCase);
			string nameUC = "_DFC_ENTITY_TYPE_ROOT_SUFFIX_TERM10";
			Assert.AreEqual(nameUC, c.Convert(name1));
			Assert.AreEqual(nameUC, c.Convert(name2));
			Assert.AreEqual(nameUC, c.Convert(name3));
			
			c = new NamingConvention(NamingStyle.CamelCase);
			string nameCC = "DfcEntityTypeRootSuffixTerm10";
			Assert.AreEqual(nameCC, c.Convert(name1));
			Assert.AreEqual(nameCC, c.Convert(name2));
			Assert.AreEqual(nameCC, c.Convert(name3));
		}
		
		[Test()]
		public void TestParseStyle()
		{
			Assert.AreEqual(NamingStyle.UpperCase, NamingConvention.ParseStyle("UppErCase"));
			Assert.AreEqual(NamingStyle.LowerCase, NamingConvention.ParseStyle("lOwErCaSe"));
			Assert.AreEqual(NamingStyle.CamelCase, NamingConvention.ParseStyle("CAMELCase"));
		}

		[Test]
		public void TestTruncating()
		{
			NamingConvention c = new NamingConvention(NamingStyle.UpperCase);
			c.MaxLength = 20;
			//              12345678901234567890123456789012345
			string name1 = "_dfc_entity_type_root_suffix_term10";
			string name2 = "_dfc_entity_type_root_suffix";
			string name3 = "_dfc_entity_type";
			//               12345678901234567890
			Assert.AreEqual("_dfc_entity_type_r10", c.TruncateName(name1));
			Assert.AreEqual("_dfc_entity_type_roo", c.TruncateName(name2));
			Assert.AreEqual("_dfc_entity_type"    , c.TruncateName(name3));
		}
	}
}

