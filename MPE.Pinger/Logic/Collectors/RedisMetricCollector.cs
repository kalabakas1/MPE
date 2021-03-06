﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;
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
            _configurationFile = ConfigurationService.Instance.ReadConfigurationFile();
        }

        public List<MetricResult> Collect()
        {
            InitiateServer();

            if (!IsEnabled)
            {
                return new List<MetricResult>();    
            }
            
            var info = _server.InfoRaw().Split('\n').Where(x => x.Contains(":")).Select(x => x.Split(':')).ToDictionary(x => x[0].ToLowerInvariant(), x => x[1]);

            var redisMetrics = _configurationFile.Redis.Metrics;
            var metrics = new List<MetricResult>();
            foreach (var metric in redisMetrics)
            {
                var keys = info.Keys.Where(x => Regex.IsMatch(x, metric));
                foreach (var key in keys)
                {
                    var strValue = info[key];
                    var result = new MetricResult
                    {
                        Path = $"{_configurationFile.Host}.Redis.{key}",
                        Alias = metric,
                        Timestamp = DateTime.Now
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

        private void InitiateServer()
        {
            IsEnabled = _configurationFile.Redis != null && _configurationFile.Redis.Metrics != null;

            if (IsEnabled)
            {
                try
                {
                    var options = new ConfigurationOptions
                    {
                        ConnectTimeout = 120000,
                        ConnectRetry = Retries,
                        EndPoints = { { _configurationFile.Redis.Host, _configurationFile.Redis.Port } },
                        AllowAdmin = true
                    };

                    if (_server == null || !_server.IsConnected)
                    {
                        var redis = ConnectionMultiplexer.Connect(options);
                        _server = redis.GetServer(_configurationFile.Redis.Host, _configurationFile.Redis.Port);
                    }
                }
                catch(Exception e)
                {
                    IsEnabled = false;
                }
            }
        }
    }
}
