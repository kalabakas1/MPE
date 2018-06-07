﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Http;
using System.Web.Http.SelfHost;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Logic;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;
using MPE.Pinger.Repositories;

namespace MPE.Pinger.Server
{
    internal class ServerStartup : IProcess
    {
        private HttpSelfHostServer _server;
        private TimedReporter<MetricResult> _metricReporter;
        private TimedReporter<EventLogResult> _eventLogReporter;
        private TimedElasticSearchRetentionPolicy<MetricResult> _retentionPolicyMetrics;
        private TimedElasticSearchRetentionPolicy<EventLogResult> _retentionPolicyEventLogs;

        public ServerStartup()
        {
            var config = new HttpSelfHostConfiguration($"http://{Configuration.Get<string>(Constants.ServerHost)}:{Configuration.Get<string>(Constants.ServerPort)}");
            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);

            _metricReporter = new TimedReporter<MetricResult>(new InMemoryRepository<MetricResult>(), new ElasticRestRepository<MetricResult>(new InMemoryRepository<MetricResult>()));
            _eventLogReporter = new TimedReporter<EventLogResult>(new InMemoryRepository<EventLogResult>(), new ElasticRestRepository<EventLogResult>(new InMemoryRepository<EventLogResult>()));

            _retentionPolicyMetrics = new TimedElasticSearchRetentionPolicy<MetricResult>();
            _retentionPolicyEventLogs = new TimedElasticSearchRetentionPolicy<EventLogResult>();
        }

        public void Start()
        {
            _retentionPolicyMetrics.Start();
            _retentionPolicyEventLogs.Start();
            _metricReporter.Start();
            _eventLogReporter.Start();
            _server.OpenAsync().Wait();
        }

        public void Stop()
        {
            _retentionPolicyMetrics.Stop();
            _retentionPolicyEventLogs.Stop();
            _metricReporter.Stop();
            _eventLogReporter.Stop();
            _server.Dispose();
        }
    }
}
