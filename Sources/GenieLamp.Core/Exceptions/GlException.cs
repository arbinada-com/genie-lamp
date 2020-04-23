using System;

namespace GenieLamp.Core.Exceptions
{
	public class GlException : ApplicationException
	{
		public GlException(string message) 
			: base(message)
		{ }
		
		public GlException(string message, Exception inner)
			: base(message, inner)
		{ }
		
		public GlException(string format, params object[] args) 
			: base(String.Format(format, args))
		{ }
	}
}

