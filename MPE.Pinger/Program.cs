using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Logic;
using Newtonsoft.Json;
using Topshelf;

namespace MPE.Pinger
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var metrics = new MetricConductor();
            metrics.InitCounters();

            var timer = new Timer(5000);
            timer.Elapsed += (sender, eventArgs) =>
            {
                var collect = metrics.Collect();
                foreach (var metric in collect)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(metric));
                }

                Console.WriteLine();
            };

            timer.Start();


            Console.ReadLine();
            return;
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
