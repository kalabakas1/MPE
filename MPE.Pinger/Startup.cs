using System.Collections.Generic;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Logic;
using MPE.Pinger.Logic.Collectors;
using MPE.Pinger.Testers;
using MPE.Pinger.Writers;

namespace MPE.Pinger
{
    internal class Startup
    {
        private readonly TimedTestExecutor _testExecutor;
        private readonly TimedMetricExecutor _metricCollector;
        public Startup()
        {
            var writer = new ElasticRestWriter();
            _testExecutor = new TimedTestExecutor(new List<IConnectionTester>
            {
                new TcpTester(),
                new WebTester(),
                new ServiceTester()
            }, writer);

            _metricCollector = new TimedMetricExecutor(new List<ICollector>
            {
                new ServerMetricCollector(),
                new RedisMetricCollector()
            }, writer);
        }

        public void Start()
        {
            _metricCollector.Start();
            _testExecutor.Start();
        }

        public void Stop()
        {
            _metricCollector.Stop();
            _testExecutor.Stop();
        }
    }
}
