﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Interfaces;
using Configuration = MPE.Pinger.Helpers.Configuration;

namespace MPE.Pinger.Logic
{
    internal class TimedTestExecutor : IProcess
    {
        private readonly List<IConnectionTester> _testers;
        private readonly IMetricRepository _metricRepository;
        private Timer _timer;

        public TimedTestExecutor(
            IEnumerable<IConnectionTester> testers,
            IMetricRepository metricRepository)
        {
            _testers = testers.ToList();
            _metricRepository = metricRepository;
            _timer = new Timer(Configuration.Get<int>(Constants.WaitBetweenTestsSec) * 1000);
            _timer.Elapsed += (sender, args) => Execute();
        }

        private void Execute()
        {
            var fromTime = TimeSpan.Parse(Configuration.Get<string>(Constants.TestEnableFromTime));
            var toTime = TimeSpan.Parse(Configuration.Get<string>(Constants.TestEnableToTime));
            var now = DateTime.Now.TimeOfDay;
            if (fromTime <= now && toTime >= now)
            {
                var results = new TestConductor(_testers).Run();
                _metricRepository.Write(results);
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
