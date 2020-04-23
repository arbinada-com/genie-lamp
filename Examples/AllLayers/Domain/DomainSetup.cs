// Genie Lamp Core (1.1.4594.29523)
// Genie of NHibernate framework (1.0.4594.29782)
// Starter application (1.1.4594.29524)
// This file was automatically generated at 2012-07-30 16:36:49
// Do not modify it manually.

using System;

using Arbinada.GenieLamp.Warehouse.Persistence;

namespace Arbinada.GenieLamp.Warehouse
{
	public static class DomainSetup
	{
		public static void Init()
		{
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType1 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne).FullName);
			if (entityType1 == null)
			{
				entityType1 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType1.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne).FullName;
				entityType1.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOne).Name;
				entityType1.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType2 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx).FullName);
			if (entityType2 == null)
			{
				entityType2 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType2.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx).FullName;
				entityType2.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ExampleOneToOneEx).Name;
				entityType2.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType3 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType).FullName);
			if (entityType3 == null)
			{
				entityType3 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType3.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType).FullName;
				entityType3.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.ProductType).Name;
				entityType3.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType4 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product).FullName);
			if (entityType4 == null)
			{
				entityType4 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType4.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product).FullName;
				entityType4.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Product).Name;
				entityType4.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType5 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType).FullName);
			if (entityType5 == null)
			{
				entityType5 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType5.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType).FullName;
				entityType5.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreType).Name;
				entityType5.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType6 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store).FullName);
			if (entityType6 == null)
			{
				entityType6 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType6.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store).FullName;
				entityType6.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Store).Name;
				entityType6.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType7 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor).FullName);
			if (entityType7 == null)
			{
				entityType7 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType7.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor).FullName;
				entityType7.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Contractor).Name;
				entityType7.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType8 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner).FullName);
			if (entityType8 == null)
			{
				entityType8 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType8.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner).FullName;
				entityType8.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.Partner).Name;
				entityType8.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType9 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc).FullName);
			if (entityType9 == null)
			{
				entityType9 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType9.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc).FullName;
				entityType9.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDoc).Name;
				entityType9.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType10 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine).FullName);
			if (entityType10 == null)
			{
				entityType10 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType10.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine).FullName;
				entityType10.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreDocLine).Name;
				entityType10.Save();
			}
			Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType entityType11 = Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType.GetByFullName(typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction).FullName);
			if (entityType11 == null)
			{
				entityType11 = new Arbinada.GenieLamp.Warehouse.Domain.Core.EntityType();
				entityType11.FullName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction).FullName;
				entityType11.ShortName = typeof(Arbinada.GenieLamp.Warehouse.Domain.Warehouse.StoreTransaction).Name;
				entityType11.Save();
			}
		}
		
		public static void Cleanup()
		{
		}
		
	}
}

