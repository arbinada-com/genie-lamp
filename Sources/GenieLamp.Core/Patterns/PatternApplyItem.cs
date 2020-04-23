using System;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Core.Patterns
{
	public enum PatternApplyItemType
	{
		Entity
	}

	class PatternApplyItem
	{
		public PatternApplyItem(PatternApplyItemType type,
		                        string name,
		                        PatternApplyMode mode)
		{
			this.Name = name;
			this.ApplyMode = mode;
			this.ItemType = type;
		}

		public string Name { get; set; }
		public PatternApplyMode ApplyMode { get; set; }
		public PatternApplyItemType ItemType { get; set; }
		public MetaObject MetaObject  { get; set; }

		public Entity Entity
		{
			get { return this.ItemType == PatternApplyItemType.Entity ? this.MetaObject as Entity : null; }
			set { this.MetaObject = value; }
		}

		public override int GetHashCode()
		{
			return PatternApplyItem.GetHashCode(
				this.ItemType,
				this.MetaObject != null ? this.MetaObject.FullName : this.Name,
				this.ApplyMode);
		}

		public static int GetHashCode(PatternApplyItemType itemType,
		                              string itemName,
		                              PatternApplyMode applyMode)
		{
			return (
				((int)applyMode).ToString() +
				((int)itemType).ToString() +
				itemName
				).GetHashCode();
		}
	}

}

