using System;

using log4net;
using log4net.Config;

namespace Arbinada.GenieLamp.Warehouse.Services.Host.ConsoleApp
{
	class MainClass
	{
        private const string ListeningOn = "http://localhost:8080/";

        protected static readonly ILog log = LogManager.GetLogger(typeof(MainClass));

		public static void Main(string[] args)
		{
            log4net.Config.XmlConfigurator.Configure();
            log.Warn("Service starting...");


			DomainServicesHost host = new DomainServicesHost();
            host.Init();
            host.Start(ListeningOn);

            Console.WriteLine("Started listening on: " + ListeningOn);
            Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, ListeningOn);
            log.Warn("Service started");

            Console.WriteLine("Press 'Q' or Ctrl+C to stop program");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(
            delegate(object sender, ConsoleCancelEventArgs e)
            {
                host.Stop();
            });
            while (Console.ReadKey().KeyChar != 'Q');
            host.Stop();
            log.Warn("Service is down");
		}
	}
}
