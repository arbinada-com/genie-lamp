using System;
using System.Diagnostics;

using Arbinada.GenieLamp.Warehouse.Patterns;

namespace Arbinada.GenieLamp.Warehouse.Domain.Warehouse
{
	public class TestCustomMethods
	{
		public int Validate { get; set; }
		public bool ThrowOnValidate { get; set; }
		public int OnSave { get; set; }
		public bool ThrowOnSave { get; set; }
		public int OnDelete { get; set; }
		public bool ThrowOnDelete { get; set; }
		public int OnFlush { get; set; }
		public bool ThrowOnFlush { get; set; }

		public TestCustomMethods()
		{
			Clear();
		}

		public void Clear()
		{
			Validate = OnSave = OnDelete = OnFlush = 0;
			ThrowOnValidate = ThrowOnSave = ThrowOnDelete = ThrowOnFlush = false;
		}

		public override string ToString()
		{
			return string.Format("[TestCustomMethods: Validate={0}, ThrowOnValidate={1}, OnSave={2}, ThrowOnSave={3}, OnDelete={4}, ThrowOnDelete={5}, OnFlush={6}, ThrowOnFlush={7}]", Validate, ThrowOnValidate, OnSave, ThrowOnSave, OnDelete, ThrowOnDelete, OnFlush, ThrowOnFlush);
		}
	}


	public partial class Store
	{
		public virtual TestCustomMethods Tester { get; set; }


		public virtual void Check()
		{
			throw new NotImplementedException();
		}

		public virtual bool RecordReceived(int productId, int qty, decimal price, DateTime date)
		{
			throw new NotImplementedException();
		}

		public virtual int[] GetQuantity(System.Collections.Generic.IList<int> product, DateTime date)
		{
			throw new NotImplementedException();
		}

		#region IValidatable implementation
		public virtual void Validate()
		{
			if (Tester != null)
			{
				Tester.Validate++;
				if (Tester.ThrowOnValidate)
					throw new Exception("Store.Validate()");
			}
		}
		#endregion

		#region IOnSave implementation
		public virtual void OnSave(EventHandlerBase sender, NHibernate.Event.SaveOrUpdateEvent e)
		{
			if (Tester != null)
			{
				Tester.OnSave++;
				if (Tester.ThrowOnSave)
					throw new Exception("Store.OnSave()");
			}
		}
		#endregion

		#region IOnDelete implementation
		public virtual void OnDelete(EventHandlerBase sender, NHibernate.Event.DeleteEvent e, Iesi.Collections.ISet transientEntities)
		{
			if (Tester != null)
			{
				Tester.OnDelete++;
				if (Tester.ThrowOnDelete)
					throw new Exception("Store.OnDelete()");
			}
		}
		#endregion

		#region IOnFlush implementation
		public virtual void OnFlush(EventHandlerBase sender, NHibernate.Event.FlushEntityEvent e)
		{
			if (Tester != null)
			{
				Tester.OnFlush++;
				if (Tester.ThrowOnFlush)
					throw new Exception("Store.OnFlush()");
			}
		}
		#endregion
	}
}

