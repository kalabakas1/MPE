using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Models;
using Serilog;

namespace MPE.Pinger.Logic
{
    internal class MetricConductor
    {
        private readonly List<Metric> _counters;
        private ILogger _logger = new LoggerFactory().Generate();

        public MetricConductor()
        {
            _counters = new List<Metric>();
        }

        public void InitCounters()
        {
            var configurationFile = Configuration.ReadConfigurationFile();

            foreach (var metric in configurationFile.Metrics)
            {
                try
                {
                    var category = PerformanceCounterCategory.GetCategories().FirstOrDefault(x => x.CategoryName == metric.Category);
                    if (category != null)
                    {
                        var instanceNames = category.GetInstanceNames();
                        if (instanceNames.Any() && string.IsNullOrEmpty(metric.Instance))
                        {
                            foreach (var instanceName in instanceNames)
                            {
                                var counter = new PerformanceCounter(metric.Category, metric.Name, instanceName);
                                counter.NextValue();
                                metric.Counters.Add(counter);
                            }
                        }
                        else
                        {
                            var counter = new PerformanceCounter(metric.Category, metric.Name, metric.Instance);
                            metric.Counters.Add(counter);
                        }

                        metric.Alias = configurationFile.Host + "." + metric.Alias;
                        _counters.Add(metric);
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug("Failed to create counter", e);
                }
            }
        }

        public List<MetricCollected> Collect()
        {
            return _counters.Select(x => new MetricCollected
            {
                Alias = x.Alias,
                Timestamp = DateTime.Now,
                Value = x.Counters.Sum(z =>
                {
                    var cs1 = z.NextSample();
                    Thread.Sleep(100);
                    var cs2 = z.NextSample();
                    return CounterSample.Calculate(cs1, cs2);
                })
            }).ToList();
        }
    }
}
