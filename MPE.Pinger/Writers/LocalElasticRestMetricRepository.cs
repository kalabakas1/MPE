using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Pinger.Writers
{
    internal class LocalElasticRestMetricRepository
    {
        private const string Host = "http://localhost:9200/";

        public void Write(MetricResult result)
        {
            var client = GetClient();
            var request = new RestRequest($"metric/metric/{Guid.NewGuid()}", Method.PUT);
            request.AddJsonBody(result);
            var response = client.Execute(request);
        }

        private RestClient GetClient()
        {
            var client = new RestClient("http://localhost:9200/");
            client.Authenticator = new HttpBasicAuthenticator("elastic", "hgysi7fg6A09ofLpFnjl");

            return client;
        }
    }
}
