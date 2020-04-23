using NUnit.Framework;
using System;
using System.Collections.Generic;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Patterns;

namespace GenieLamp.Core.Test
{
	[TestFixture()]
	public class PatternsTest
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
		public void ApplyToTest()
		{

			RegistryPattern registry = lamp.Config.Patterns.Registry;
			StateVersionPattern stateVersion = lamp.Config.Patterns.StateVersion;
			Assert.AreEqual("Warehouse", registry.Schema, "Schema where to implement");
			Assert.IsTrue(stateVersion.PatternApply.Count > 1, "Registry pattern entities are not excluded");

			HashSet<string> excludedEntities = new HashSet<string>();
			excludedEntities.Add(registry.TypesEntity.Entity.FullName);
			excludedEntities.Add(registry.RegistryEntity.Entity.FullName);
			foreach(PatternApplyItem ai in stateVersion.PatternApply)
			{
				if (ai.ApplyMode == PatternApplyMode.Exclude && ai.ItemType == PatternApplyItemType.Entity)
					excludedEntities.Add(ai.Entity.FullName);
			}


			foreach(Entity entity in lamp.Model.Entities)
			{
				bool excluded = excludedEntities.Contains(entity.FullName) || entity.HasSupertype; // StateVersion applied only to superclasses
				bool applied = stateVersion.AppliedTo(entity);
				Assert.AreEqual(excluded, !applied, "Entity {0}; excluded: {1}, applied: {2}", entity.FullName, excluded, applied);

				excluded = excludedEntities.Contains(entity.FullName);
				applied = stateVersion.AppliedToEntityOrSupertypes(entity);
				Assert.AreEqual(excluded, !applied, "Entity {0}; excluded: {1}, applied: {2}", entity.FullName, excluded, applied);
			}
		}
	}
}

