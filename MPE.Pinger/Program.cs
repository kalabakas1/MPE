using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Logic;
using MPE.Pinger.Logic.Collectors;
using MPE.Pinger.Models;
using MPE.Pinger.Writers;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using StackExchange.Redis;
using Topshelf;
using Timer = System.Timers.Timer;

namespace MPE.Pinger
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var rc = HostFactory.Run(x =>
            {
                x.Service<Startup>(s =>
                {
                    s.ConstructUsing(name => new Startup());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Service to call different endpoint to check for life");
                x.SetDisplayName("MPE_Pinger");
                x.SetServiceName("MPE_Pinger");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
