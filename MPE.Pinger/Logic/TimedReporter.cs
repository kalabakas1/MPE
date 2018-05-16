using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Repositories;

namespace MPE.Pinger.Logic
{

    internal class TimedReporter : IProcess
    {
        private const int BulkSize = 256;
        private bool IsRunning = false;
        private readonly IMetricRepository _tempMetricRepository;
        private readonly IMetricRepository _persitanceRepository;
        private Timer _timer;
        public TimedReporter(
            IMetricRepository tempMetricRepository,
            IMetricRepository persitanceRepository)
        {
            _tempMetricRepository = tempMetricRepository;
            _persitanceRepository = persitanceRepository;
            _timer = new Timer(Configuration.Get<int>(Constants.ReportIntevalSec) * 1000);
            _timer.Elapsed += (sender, args) => ReportMetrics();
            _timer.AutoReset = true;
        }

        private void ReportMetrics()
        {
            LoggerFactory.Instance.Debug($"Reporting stating...");

            var run = true;
            var metrics = new List<MetricResult>();

            var count = 0;
            while (run)
            {
                while (run && count < BulkSize)
                {
                    try
                    {
                        metrics.Add(_tempMetricRepository.Pop());
                    }
                    catch (Exception e)
                    {
                        run = false;
                    }
                    count++;
                }

                try
                {
                    _persitanceRepository.Write(metrics);
                }
                catch (Exception e)
                {
                    LoggerFactory.Instance.Debug("Failed to write to Persistance storage", e);
                    _tempMetricRepository.Write(metrics);
                    run = false;
                }

                metrics = new List<MetricResult>();

                count = 0;
            }

            LoggerFactory.Instance.Debug($"Reporting ended...");

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
