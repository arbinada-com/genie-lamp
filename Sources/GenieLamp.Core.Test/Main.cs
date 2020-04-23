using System;

using NUnit.Core;
using NUnit.Framework;

namespace GenieLamp.Core.Test
{
	public class EntryPoint
	{
		public static void Main(string[] args)
		{
			CoreExtensions.Host.InitializeService();
			TestPackage package = new TestPackage("Test");
			package.Assemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);
			SimpleTestRunner runner = new SimpleTestRunner();
			if (runner.Load(package))
			{
				TestResult result = runner.Run(new NullListener(), TestFilter.Empty, true, LoggingThreshold.Error);
				if (!result.IsSuccess)
					throw new Exception(result.Message);
			}
		}
	}
}

