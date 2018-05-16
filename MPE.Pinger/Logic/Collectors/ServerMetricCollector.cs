using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using Serilog;

namespace MPE.Pinger.Logic.Collectors
{
    internal class ServerMetricCollector : ICollector
    {
        private readonly List<Metric> _counters;
        private ILogger _logger = new LoggerFactory().Generate();
        private ConfigurationFile _configurationFile;
        private bool _isEnabled;

        public ServerMetricCollector()
        {
            _counters = new List<Metric>();
        }

        private void InitCounters()
        {
            if (_isEnabled)
            {
                return;
            }

            _configurationFile = Configuration.ReadConfigurationFile();

            foreach (var metric in _configurationFile.Metrics)
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

                        metric.Alias = metric.Alias;
                        _counters.Add(metric);
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug("Failed to create counter", e);
                }
            }

            _isEnabled = true;
        }

        public List<MetricResult> Collect()
        {
            InitCounters();

            return _counters.Select(x => new MetricResult
            {
                Path = _configurationFile.Host + "." + x.Alias,
                Alias = x.Alias,
                Timestamp = DateTime.Now,
                Value = x.Counters.Sum(z =>
                {
                    try
                    {
                        var cs1 = z.NextSample();
                        Thread.Sleep(500);
                        var cs2 = z.NextSample();
                        return CounterSample.Calculate(cs1, cs2);
                    }
                    catch (Exception e)
                    {
                        _logger.Debug("Failed to get metric", e);
                        return 0;
                    }
                })
            }).ToList();
        }
    }
}
