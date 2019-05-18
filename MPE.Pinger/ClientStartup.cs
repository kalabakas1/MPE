using System.Collections.Generic;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Logic;
using MPE.Pinger.Logic.Collectors;
using MPE.Pinger.Logic.Listeners;
using MPE.Pinger.Logic.Testers;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;
using MPE.Pinger.Repositories;

namespace MPE.Pinger
{
    internal class ClientStartup
    {
        private readonly TimedTestExecutor _testExecutor;
        private readonly TimedMetricExecutor _metricCollector;
        private readonly TimedReporter<MetricResult> _metricTimedReporter;

        private readonly EventLogListener _eventLogListener;

        public ClientStartup()
        {
            var metricMemoryRepository = new InMemoryRepository<MetricResult>();
            var alertHub = new AlertHub();
            var healingExecutor = new HealingExecutor();
            
            _testExecutor = new TimedTestExecutor(new List<ITester>
            {
                new TcpTester(),
                new WebTester(),
                new ServiceTester(),
                new SslTester(),
                new PowershellTest()
            }, metricMemoryRepository,
            alertHub,
            healingExecutor);

            _metricCollector = new TimedMetricExecutor(new List<ICollector>
            {
                new ServerMetricCollector(),
                new RedisMetricCollector(),
                new RabbitMqCollector(),
                new ElasticSearchCollector(),
                new HaProxyCollector(),
                new SqlQueryCollector(metricMemoryRepository)
            }, metricMemoryRepository);

            var restMetricRepository = new RestRepository<MetricResult>();
            _metricTimedReporter = new TimedReporter<MetricResult>(metricMemoryRepository, restMetricRepository);

            _eventLogListener = new EventLogListener(metricMemoryRepository);
            _eventLogListener.Init();
        }

        public void Start()
        {
            _metricCollector.Start();
            _testExecutor.Start();
            _metricTimedReporter.Start();
        }

        public void Stop()
        {
            _metricCollector.Stop();
            _testExecutor.Stop();
            _metricTimedReporter.Stop();
        }
    }
}
