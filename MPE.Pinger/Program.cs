using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using MPE.Pinger.Server;
using Newtonsoft.Json;
using Topshelf;

namespace MPE.Pinger
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var mode = ConfigurationManager.AppSettings["MPE.Pinger.Mode"]?.ToLowerInvariant();
            TopshelfExitCode rc = TopshelfExitCode.Ok;
            switch (mode)
            {
                case "server":
                    rc = HostFactory.Run(x =>
                    {
                        x.Service<ServerStartup>(s =>
                        {
                            s.ConstructUsing(name => new ServerStartup());
                            s.WhenStarted(tc => tc.Start());
                            s.WhenStopped(tc => tc.Stop());
                        });
                        x.RunAsLocalSystem();

                        x.SetDescription("Server to receive metric results");
                        x.SetDisplayName("MPE_Pinger_Server");
                        x.SetServiceName("MPE_Pinger_Server");
                    });
                    break;

                case "client":
                    rc = HostFactory.Run(x =>
                    {
                        x.Service<ClientStartup>(s =>
                        {
                            s.ConstructUsing(name => new ClientStartup());
                            s.WhenStarted(tc => tc.Start());
                            s.WhenStopped(tc => tc.Stop());
                        });
                        x.RunAsLocalSystem();

                        x.SetDescription("Service to call different endpoint to check for life");
                        x.SetDisplayName("MPE_Pinger");
                        x.SetServiceName("MPE_Pinger");
                    });
                    break;
            }

            Console.ReadLine();

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
