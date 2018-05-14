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
using MPE.Pinger.Repositories;

namespace MPE.Pinger.Server
{
    internal class ServerStartup : IProcess
    {
        private HttpSelfHostServer _server;
        private TimedReporter _reporter;
        private TimedElasticSearchRetentionPolicy _retentionPolicy;

        public ServerStartup()
        {
            var config = new HttpSelfHostConfiguration($"http://{Configuration.Get<string>(Constants.ServerHost)}:{Configuration.Get<string>(Constants.ServerPort)}");
            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);

            _reporter = new TimedReporter(new InMemoryMetricRepository(), new ElasticRestMetricRepository());

            _retentionPolicy = new TimedElasticSearchRetentionPolicy();
        }

        public void Start()
        {
            _retentionPolicy.Start();
            _reporter.Start();
            _server.OpenAsync().Wait();
        }

        public void Stop()
        {
            _retentionPolicy.Stop();
            _reporter.Stop();
            _server.Dispose();
        }
    }
}
