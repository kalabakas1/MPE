using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;

namespace MPE.Pinger.Logic
{
    internal class TimedTestExecutor : IProcess
    {
        private readonly List<ITester> _testers;
        private readonly IRepository<MetricResult> _repository;
        private readonly AlertHub _alertHub;
        private readonly HealingExecutor _healingExecutor;
        private Timer _timer;

        public TimedTestExecutor(
            IEnumerable<ITester> testers,
            IRepository<MetricResult> repository,
            AlertHub alertHub,
            HealingExecutor healingExecutor)
        {
            _testers = testers.ToList();
            _repository = repository;
            _alertHub = alertHub;
            _healingExecutor = healingExecutor;
            _timer = new Timer(ConfigurationService.Instance.Get<int>(Constants.WaitBetweenTestsSec) * 1000);
            _timer.Elapsed += (sender, args) => Execute();
            _timer.AutoReset = false;
        }

        private void Execute()
        {
            var fromTime = TimeSpan.Parse(ConfigurationService.Instance.Get<string>(Constants.TestEnableFromTime));
            var toTime = TimeSpan.Parse(ConfigurationService.Instance.Get<string>(Constants.TestEnableToTime));
            var now = DateTime.Now.TimeOfDay;
            if (fromTime <= now && toTime >= now)
            {
                var results = new TestConductor(_testers, _alertHub, _healingExecutor).Run();
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
