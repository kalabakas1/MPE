using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Testers;

namespace MPE.Pinger.Logic
{
    internal class TimedPingerService
    {
        private Timer _timer;

        public TimedPingerService()
        {
            _timer = new Timer(int.Parse(ConfigurationManager.AppSettings["MPE.Pinger.WaitBetweenTest.Secs"]) * 1000);
            _timer.Elapsed += (sender, args) => Execute();
        }

        private void Execute()
        {
            new PingerService(new List<IConnectionTester>
            {
                new TcpTester(),
                new WebTester()
            }).Run();
        }

        public void Start()
        {
            Execute();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
