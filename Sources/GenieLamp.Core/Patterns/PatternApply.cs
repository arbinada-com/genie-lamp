using System;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Patterns
{
	public enum PatternApplyMode
	{
		Exclude,
		Include
	}


	class PatternApply : IEnumerable<PatternApplyItem>
	{
		private List<PatternApplyItem> applyItems = new List<PatternApplyItem>();
		private GenieLampConfig owner;
		private Dictionary<int, PatternApplyItem> index = new Dictionary<int, PatternApplyItem>();

		#region Constructors
		public PatternApply(GenieLampConfig owner)
		{
			this.owner = owner;
		}
		#endregion

		public Model Model
		{
			get { return owner.Lamp.Model; }
		}

		public int Count
		{
			get { return this.applyItems.Count; }
		}

		public PatternApplyItem AddItem(PatternApplyItemType itemType,
		                                string itemName,
		                                PatternApplyMode applyMode)
		{
			PatternApplyItem item = new PatternApplyItem(itemType, itemName, applyMode);
			this.applyItems.Add(item);
			return item;
		}

		public void Update()
		{
			foreach(PatternApplyItem item in applyItems)
			{
				switch(item.ItemType)
				{
				case PatternApplyItemType.Entity:
					item.Entity = GetEntity(item.Name, "Entity not found");
					break;
				}
			}
			Reindex();
		}

		private void Reindex()
		{
			this.index.Clear();
			foreach(PatternApplyItem item in applyItems)
				index.Add(item.GetHashCode(), item);
		}

		public Entity GetEntity(string name, string message)
		{
			QualName qname = new QualName(name, Model.DefaultSchema);
			Entity entity =  Model.Entities.GetByName(qname.FullName, false);
			if (entity == null)
				throw new GlException("{0}. Name: {1}", message, qname.FullName);
			return entity;
		}

		public void Exclude(Entity entity)
		{
			AddMetaObject(entity, PatternApplyItemType.Entity, PatternApplyMode.Exclude);
		}

		public void Include(Entity entity)
		{
			AddMetaObject(entity, PatternApplyItemType.Entity, PatternApplyMode.Include);
		}

		private void AddMetaObject(MetaObject o, PatternApplyItemType itemType, PatternApplyMode applyMode)
		{
			PatternApplyItem item = new PatternApplyItem(itemType, o.FullName, applyMode);
			item.MetaObject = o;
			if (!this.index.ContainsKey(item.GetHashCode()))
			{
				this.applyItems.Add(item);
			    this.index.Add(item.GetHashCode(), item);
			}
		}

		public bool IsExcluded(PatternApplyItemType itemType, string itemName)
		{
			return this.index.ContainsKey(PatternApplyItem.GetHashCode(itemType, itemName, PatternApplyMode.Exclude));
		}

		public bool IsIncluded(PatternApplyItemType itemType, string itemName)
		{
			return this.index.ContainsKey(PatternApplyItem.GetHashCode(itemType, itemName, PatternApplyMode.Include));
		}

		#region IEnumerable[PatternApplyItem] implementation
		IEnumerator<PatternApplyItem> IEnumerable<PatternApplyItem>.GetEnumerator()
		{
			return this.applyItems.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.applyItems.GetEnumerator();
		}
		#endregion
	}
}

