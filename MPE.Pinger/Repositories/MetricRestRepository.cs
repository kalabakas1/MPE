using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using Newtonsoft.Json;
using RestSharp;

namespace MPE.Pinger.Repositories
{
    internal class MetricRestRepository : IMetricRepository
    {
        private ConfigurationFile _configurationFile;
        public MetricRestRepository()
        {
            _configurationFile = Configuration.ReadConfigurationFile();
        }

        public void Write(List<MetricResult> results)
        {
            var request = new RestRequest("/api/MetricResult");
            request.Method = Method.POST;
            request.AddHeader("Authorization", _configurationFile.ApiKey);
            request.AddBody(JsonConvert.SerializeObject(results));

            var response = GetClient().Execute(request);
        }

        public void Write(MetricResult result)
        {
            var request = new RestRequest("/api/MetricResult");
            request.Method = Method.POST;
            request.AddHeader("Authorization", _configurationFile.ApiKey);
            request.AddJsonBody(new List<MetricResult> {result});

            var response = GetClient().Execute(request);
        }

        public MetricResult Pop()
        {
            return null;
        }

        private RestClient GetClient()
        {
            var client = new RestClient (_configurationFile.RestEndpoint);

            return client;
        }
    }
}
