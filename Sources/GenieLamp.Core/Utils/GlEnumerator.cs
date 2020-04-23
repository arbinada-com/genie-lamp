using System;
using System.Collections;
using System.Collections.Generic;

namespace GenieLamp.Core.Utils
{
	class GlEnumerator<IT, T> : IEnumerator<IT>
		where T : IT
	{
		IEnumerator<T> listEnumerator;
		
		public GlEnumerator(List<T> list)
		{ 
			listEnumerator = list.GetEnumerator();
		}

		public GlEnumerator(Dictionary<string, T>.ValueCollection list)
		{ 
			listEnumerator = list.GetEnumerator();
		}
		
		#region IEnumerator[IT] implementation
		IT IEnumerator<IT>.Current {
			get { return listEnumerator.Current; }
		}
		#endregion

		#region IEnumerator implementation
		bool IEnumerator.MoveNext()
		{
			return listEnumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			listEnumerator.Reset();
		}

		object IEnumerator.Current {
			get { return listEnumerator.Current; }
		}
		#endregion

		#region IDisposable implementation
		void IDisposable.Dispose()
		{
			listEnumerator.Dispose();
		}
		#endregion
	}
}

