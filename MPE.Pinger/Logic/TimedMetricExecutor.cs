﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;

namespace MPE.Pinger.Logic
{
    internal class TimedMetricExecutor : IProcess
    {
        private readonly IRepository<MetricResult> _repository;
        private readonly List<ICollector> _collectors;
        private readonly Timer _timer;

        public TimedMetricExecutor(
            List<ICollector> collectors,
            IRepository<MetricResult> repository)
        {
            _repository = repository;
            _collectors = collectors;
            _timer = new Timer(ConfigurationService.Instance.Get<int>(Constants.MetricIntevalSec) * 1000);

            _timer.Elapsed += (sender, args) => Execute();
            _timer.AutoReset = false;
        }

        private void Execute()
        {
            var tasks = new List<Task<List<MetricResult>>>();
            foreach (var collector in _collectors)
            {
                var task = Task<List<MetricResult>>.Factory.StartNew(() =>
                {
                    try
                    {
                        return collector.Collect();
                    }
                    catch (Exception e)
                    {
                        LoggerFactory.Instance.Debug($"Collector failed: {collector.GetType().Name} - {e.Message}");
                        return new List<MetricResult>();
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            var results = tasks.SelectMany(x => x.Result);
            _repository.Write(results.ToList());
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
