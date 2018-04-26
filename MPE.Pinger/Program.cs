using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Logic;
using Topshelf;

namespace MPE.Pinger
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var rc = HostFactory.Run(x =>
            {
                x.Service<TimedPingerService>(s =>
                {
                    s.ConstructUsing(name => new TimedPingerService());
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
