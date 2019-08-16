using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;
using Serilog;

namespace MPE.Pinger.Logic.Collectors
{
    internal class ServerMetricCollector : ICollector
    {
        private readonly List<Metric> _counters;
        private ILogger _logger = new LoggerFactory().Generate();
        private ConfigurationFile _configurationFile;
        private DateTime _renew;

        public ServerMetricCollector()
        {
            _counters = new List<Metric>();
            _renew = DateTime.MinValue;
        }

        private void InitCounters()
        {
            if (_renew > DateTime.Now)
            {
                return;
            }

            foreach (var counter in _counters)
            {
                if (counter?.Counters == null
                    || !counter.Counters.Any())
                {
                    continue;
                }

                for(int i = 0; i < counter.Counters.Count;i++)
                {
                    var performanceCounter = counter.Counters[i];
                    try
                    {
                        performanceCounter.Dispose();
                        counter.Counters.Remove(performanceCounter);
                        performanceCounter = null;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                counter.Counters.Clear();
            }

            _configurationFile = ConfigurationService.Instance.ReadConfigurationFile();

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

            _renew = DateTime.Now.AddMinutes(5);
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
                        _logger.Debug($"Failed to get metric - {x.Name}", e);
                        return 0;
                    }
                })
            }).ToList();
        }
    }
}
