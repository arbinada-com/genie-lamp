using System;

namespace GenieLamp.Examples.QuickStart.Services.ConsoleHost
{
    public class Program
    {
        static void Main(string[] args)
        {
            string ServiceUrl = "http://localhost:8080/";
            DomainServicesHost host = new DomainServicesHost();
            host.Init();
            host.Start(ServiceUrl);

            Console.WriteLine("Started listening on: " + ServiceUrl);
            Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, ServiceUrl);

            Console.WriteLine("Press 'Q' or Ctrl+C to stop program");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(
            delegate(object sender, ConsoleCancelEventArgs e)
            {
                host.Stop();
            });
            while (Console.ReadKey().KeyChar != 'Q') ;
            host.Stop();
        }
    }
}
