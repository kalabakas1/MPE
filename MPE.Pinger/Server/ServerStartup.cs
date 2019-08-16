using System;
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

        private TimedElasticSearchRetentionPolicy<MetricResult> _retentionPolicyMetrics;

        public ServerStartup()
        {
            var config = new HttpSelfHostConfiguration($"http://{ConfigurationService.Instance.Get<string>(Constants.ServerHost)}:{ConfigurationService.Instance.Get<string>(Constants.ServerPort)}");
            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            //config.MaxReceivedMessageSize = config.MaxReceivedMessageSize * 2;

            _server = new HttpSelfHostServer(config);

            _metricReporter = new TimedReporter<MetricResult>(new InMemoryRepository<MetricResult>(), new ElasticRestRepository<MetricResult>(new InMemoryRepository<MetricResult>()));

            _retentionPolicyMetrics = new TimedElasticSearchRetentionPolicy<MetricResult>();
        }

        public void Start()
        {
            _retentionPolicyMetrics.Start();
            _metricReporter.Start();
            _server.OpenAsync().Wait();
        }

        public void Stop()
        {
            _retentionPolicyMetrics.Stop();
            _metricReporter.Stop();
            _server.Dispose();
        }
    }
}
