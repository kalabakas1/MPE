using MPE.Pinger.Logic;

namespace MPE.Pinger
{
    internal class Startup
    {
        private readonly TimedTestExecutor _testExecutor;
        private readonly MetricConductor _metricConductor;
        public Startup()
        {
            _testExecutor = new TimedTestExecutor();
        }

        public void Start()
        {
            _testExecutor.Start();
        }

        public void Stop()
        {
            _testExecutor.Stop();
        }
    }
}
