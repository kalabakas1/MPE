using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;

namespace MPE.Pinger.Server
{
    internal class ServerStartup : IProcess
    {
        private readonly ConfigurationFile _configurationFile;
        private HttpSelfHostServer _server;
        public ServerStartup()
        {
            _configurationFile = Configuration.ReadConfigurationFile();

            var config = new HttpSelfHostConfiguration($"http://{_configurationFile.ServerHost}:{_configurationFile.ServerPort}");
            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);
        }

        public void Start()
        {
            _server.OpenAsync().Wait();
        }

        public void Stop()
        {
            _server.Dispose();
        }
    }
}
