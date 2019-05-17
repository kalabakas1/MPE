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

    internal class TimedReporter<T> : IProcess
    {
        private const int BulkSize = 256;
        private bool IsRunning = false;
        private readonly IRepository<T> _tempRepository;
        private readonly IRepository<T> _persitanceRepository;
        private Timer _timer;
        private readonly string _type;

        public TimedReporter(
            IRepository<T> tempRepository,
            IRepository<T> persitanceRepository)
        {
            _tempRepository = tempRepository;
            _persitanceRepository = persitanceRepository;
            _type = typeof(T).Name;
            _timer = new Timer(Configuration.Get<int>(Constants.ReportIntevalSec) * 1000);
            _timer.Elapsed += (sender, args) => ReportMetrics();
            _timer.AutoReset = false;
        }

        private void ReportMetrics()
        {
            LoggerFactory.Instance.Debug($"Reporting stating - {_type}...");

            var run = true;
            var metrics = new List<T>();

            var count = 0;
            while (run)
            {
                while (run && count < BulkSize)
                {
                    try
                    {
                        metrics.Add(_tempRepository.Pop());
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
                    LoggerFactory.Instance.Debug($"Failed to write to storage - {_type}", e);
                    _tempRepository.Write(metrics);
                    run = false;
                }

                for (int i = 0; i < metrics.Count; i++)
                {
                    metrics[i] = default(T);
                }

                metrics = new List<T>();

                GC.Collect();

                count = 0;
            }

            LoggerFactory.Instance.Debug($"Reporting ended - {_type}...");

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
