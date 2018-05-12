using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;

namespace MPE.Pinger.Logic
{
    internal class TimedMetricExecutor
    {
        private readonly IMetricWriter _metricWriter;
        private readonly List<ICollector> _collectors;
        private readonly Timer _timer;


        public TimedMetricExecutor(
            List<ICollector> collectors,
            IMetricWriter metricWriter)
        {
            _metricWriter = metricWriter;
            _collectors = collectors;
            _timer = new Timer(5000);

            _timer.Elapsed += (sender, args) => Execute();
        }

        private void Execute()
        {
            var tasks = new List<Task<List<MetricResult>>>();
            foreach (var collector in _collectors)
            {
                var task = Task<List<MetricResult>>.Factory.StartNew(() =>
                {
                    return collector.Collect();
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            var results = tasks.SelectMany(x => x.Result);
            _metricWriter.Write(results.ToList());
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
