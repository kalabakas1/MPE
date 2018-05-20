using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;
using Configuration = MPE.Pinger.Helpers.Configuration;

namespace MPE.Pinger.Logic
{
    internal class TimedTestExecutor : IProcess
    {
        private readonly List<ITester> _testers;
        private readonly IRepository<MetricResult> _repository;
        private readonly AlertHub _alertHub;
        private Timer _timer;

        public TimedTestExecutor(
            IEnumerable<ITester> testers,
            IRepository<MetricResult> repository,
            AlertHub alertHub)
        {
            _testers = testers.ToList();
            _repository = repository;
            _alertHub = alertHub;
            _timer = new Timer(Configuration.Get<int>(Constants.WaitBetweenTestsSec) * 1000);
            _timer.Elapsed += (sender, args) => Execute();
            _timer.AutoReset = false;
        }

        private void Execute()
        {
            var fromTime = TimeSpan.Parse(Configuration.Get<string>(Constants.TestEnableFromTime));
            var toTime = TimeSpan.Parse(Configuration.Get<string>(Constants.TestEnableToTime));
            var now = DateTime.Now.TimeOfDay;
            if (fromTime <= now && toTime >= now)
            {
                var results = new TestConductor(_testers, _alertHub).Run();
                _repository.Write(results);
            }
            _timer.Start();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
