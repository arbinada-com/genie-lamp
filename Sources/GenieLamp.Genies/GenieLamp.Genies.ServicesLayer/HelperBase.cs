using System;

using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core;

namespace GenieLamp.Genies.ServicesLayer
{
	public class HelperBase
	{
		protected ICodeWriterCSharp cw;
		protected IEnvironmentHelper environment;

		public HelperBase(ICodeWriterCSharp cw, IEnvironmentHelper environment)
		{
			this.cw = cw;
			this.environment = environment;
		}
	}
}

