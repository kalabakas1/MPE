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

        private readonly TimedReporter<EventLogResult> _eventLogTimedReporter;
        private readonly EventLogListener _eventLogListener;

        public ClientStartup()
        {
            var metricMemoryRepository = new InMemoryRepository<MetricResult>();
            var alertHub = new AlertHub();
            _testExecutor = new TimedTestExecutor(new List<ITester>
            {
                new TcpTester(),
                new WebTester(),
                new ServiceTester(),
                new SslTester()
            }, metricMemoryRepository,
            alertHub);

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

            var eventMemoryRepository = new InMemoryRepository<EventLogResult>();
            _eventLogListener = new EventLogListener(eventMemoryRepository);
            _eventLogListener.Init();
            
            var restEventLogRepository = new RestRepository<EventLogResult>();
            _eventLogTimedReporter = new TimedReporter<EventLogResult>(eventMemoryRepository, restEventLogRepository);
        }

        public void Start()
        {
            _metricCollector.Start();
            _testExecutor.Start();
            _metricTimedReporter.Start();
            _eventLogTimedReporter.Start();
        }

        public void Stop()
        {
            _metricCollector.Stop();
            _testExecutor.Stop();
            _metricTimedReporter.Stop();
            _eventLogTimedReporter.Stop();
        }
    }
}
