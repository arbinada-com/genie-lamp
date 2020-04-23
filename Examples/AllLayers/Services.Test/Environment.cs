using NUnit.Framework;
using System;
using System.Configuration;

using log4net;

namespace Arbinada.GenieLamp.Warehouse.Services.Test
{
	[SetUpFixture]
	public class Environment
	{
        public static readonly ILog Log = LogManager.GetLogger(typeof(MainClass));

		public static System.Threading.Mutex Locker = new System.Threading.Mutex();

		public static string ServiceUrl
		{
			get
			{
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				return config.AppSettings.Settings["ServiceUrl"].Value.ToString();
			}
		}

		private DomainServicesHost host = null;

		[SetUp]
		public void TextFixtureSetUp()
		{
			Arbinada.GenieLamp.Warehouse.Domain.Test.Environment.Init();

			host = new DomainServicesHost();
			host.Init();
            host.Start(ServiceUrl);
            Console.WriteLine("Host created at {0}, listening on {1}", DateTime.Now, ServiceUrl);
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			if (host != null)
			{
				host.Stop();
				host.Dispose();
			}
			host = null;
		}
	}
}

