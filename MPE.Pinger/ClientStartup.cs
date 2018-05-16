﻿using System.Collections.Generic;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Logic;
using MPE.Pinger.Logic.Collectors;
using MPE.Pinger.Logic.Testers;
using MPE.Pinger.Repositories;

namespace MPE.Pinger
{
    internal class ClientStartup
    {
        private readonly TimedTestExecutor _testExecutor;
        private readonly TimedMetricExecutor _metricCollector;
        private readonly TimedReporter _reporter;
        public ClientStartup()
        {
            var writer = new InMemoryMetricRepository();
            _testExecutor = new TimedTestExecutor(new List<IConnectionTester>
            {
                new TcpTester(),
                new WebTester(),
                new ServiceTester()
            }, writer);

            _metricCollector = new TimedMetricExecutor(new List<ICollector>
            {
                new ServerMetricCollector(),
                new RedisMetricCollector(),
                new RabbitMqCollector()
            }, writer);

            _reporter = new TimedReporter(writer, new MetricRestRepository());
        }

        public void Start()
        {
            _metricCollector.Start();
            _testExecutor.Start();
            _reporter.Start();
        }

        public void Stop()
        {
            _metricCollector.Stop();
            _testExecutor.Stop();
            _reporter.Stop();
        }
    }
}