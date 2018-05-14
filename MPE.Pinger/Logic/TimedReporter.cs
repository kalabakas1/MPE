using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Repositories;

namespace MPE.Pinger.Logic
{

    internal class TimedReporter : IProcess
    {
        private readonly IMetricRepository _metricRepository;
        private Timer _timer;
        private LocalElasticRestMetricRepository _localElasticRestMetricRepository;
        public TimedReporter(
            IMetricRepository metricRepository)
        {
            _metricRepository = metricRepository;
            _timer = new Timer(Configuration.Get<int>(Constants.ReportIntevalSec) * 1000);
            _timer.Elapsed += (sender, args) => ReportMetrics();
            _localElasticRestMetricRepository = new LocalElasticRestMetricRepository();
        }

        private void ReportMetrics()
        {
            var run = true;
            while (run)
            {
                try
                {
                    var metric = _metricRepository.Pop();
                    _localElasticRestMetricRepository.Write(metric);
                }
                catch (Exception e)
                {
                    run = false;
                }
            }
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
