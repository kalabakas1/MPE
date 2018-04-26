using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Testers;
using Configuration = MPE.Pinger.Helpers.Configuration;

namespace MPE.Pinger.Logic
{
    internal class TimedPingerService
    {
        private Timer _timer;

        public TimedPingerService()
        {
            _timer = new Timer(Configuration.Get<int>("MPE.Pinger.WaitBetweenTest.Secs") * 1000);
            _timer.Elapsed += (sender, args) => Execute();
        }

        private void Execute()
        {
            var fromTime = TimeSpan.Parse(Configuration.Get<string>("MPE.Pinger.TimeSpan.From"));
            var toTime = TimeSpan.Parse(Configuration.Get<string>("MPE.Pinger.TimeSpan.To"));
            var now = DateTime.Now.TimeOfDay;
            if (fromTime <= now && toTime >= now)
            {
                new PingerService(new List<IConnectionTester>
                {
                    new TcpTester(),
                    new WebTester()
                }).Run();
            }
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
