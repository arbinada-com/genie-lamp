using System;

using NUnit.Core;
using NUnit.Framework;

namespace Arbinada.GenieLamp.Warehouse.Services.Test
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
				runner.Run(new NullListener(), TestFilter.Empty, false, LoggingThreshold.Error);
			}
		}
	}
}
