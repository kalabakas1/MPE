using System;
using System.Collections.Generic;
using System.Linq;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using StackExchange.Redis;

namespace MPE.Pinger.Logic.Collectors
{
    internal class RedisMetricCollector : ICollector
    {
        private const int Retries = 10;

        private ConfigurationFile _configurationFile;
        private bool IsEnabled { get; set; }
        private IServer _server;

        public RedisMetricCollector()
        {
            _configurationFile = Configuration.ReadConfigurationFile();

            IsEnabled = _configurationFile.Redis != null && _configurationFile.Redis.Metrics != null;

            if (IsEnabled)
            {
                try
                {
                    var options = new ConfigurationOptions
                    {
                        ConnectTimeout = 120000,
                        ConnectRetry = Retries,
                        //ClientName = credentials.ClientName,
                        EndPoints = {{_configurationFile.Redis.Host, _configurationFile.Redis.Port}},
                        AllowAdmin = true
                    };

                    var redis = ConnectionMultiplexer.Connect(options);
                    _server = redis.GetServer(_configurationFile.Redis.Host, _configurationFile.Redis.Port);
                }
                catch
                {
                    IsEnabled = false;
                }
            }
        }

        public List<MetricResult> Collect()
        {
            if (!IsEnabled)
            {
                return new List<MetricResult>();    
            }
            
            var info = _server.InfoRaw().Split('\n').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Split(':')).ToDictionary(x => x[0].ToLowerInvariant(), x => x[1]);

            var redisMetrics = _configurationFile.Redis.Metrics;
            var metrics = new List<MetricResult>();
            foreach (var metric in redisMetrics)
            {
                if (info.ContainsKey(metric.ToLowerInvariant()))
                {
                    var strValue = info[metric.ToLowerInvariant()];
                    var result = new MetricResult
                    {
                        Path = $"{_configurationFile.Host}.Redis.{metric}",
                        Alias = metric
                    };

                    float flValue = 0;
                    if (float.TryParse(strValue, out flValue))
                    {
                        result.Value = flValue;
                    }

                    metrics.Add(result);
                }
            }

            return metrics;
        }
    }
}
