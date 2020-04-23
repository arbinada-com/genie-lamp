using System;

using NUnit.Core;
using NUnit.Framework;

namespace Arbinada.GenieLamp.Warehouse.Domain.Test
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			CoreExtensions.Host.InitializeService();
			TestPackage package = new TestPackage("Test");
			package.Assemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);
			SimpleTestRunner runner = new SimpleTestRunner();
			if (runner.Load(package))
			{
				TestResult result = runner.Run(new NullListener(), TestFilter.Empty, false, LoggingThreshold.Error);
				if (!result.IsSuccess)
					throw new Exception(result.Message);
			}
		}
	}
}
