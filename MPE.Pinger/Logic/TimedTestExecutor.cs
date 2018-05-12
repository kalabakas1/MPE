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
    internal class TimedTestExecutor
    {
        private readonly List<IConnectionTester> _testers;
        private readonly IMetricWriter _metricWriter;
        private Timer _timer;

        public TimedTestExecutor(
            IEnumerable<IConnectionTester> testers,
            IMetricWriter metricWriter)
        {
            _testers = testers.ToList();
            _metricWriter = metricWriter;
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
                var results = new TestConductor(_testers).Run();
                _metricWriter.Write(results);
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
