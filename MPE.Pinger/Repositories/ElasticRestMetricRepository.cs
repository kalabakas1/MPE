using System;
using System.Collections.Generic;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Pinger.Repositories
{
    internal class ElasticRestMetricRepository : IMetricRepository
    {
        private const string Host = "http://localhost:9200/";

        public void Write(List<MetricResult> results)
        {
            foreach (var metricResult in results)
            {
                Write(metricResult);
            }
        }

        public void Write(MetricResult result)
        {
            var client = GetClient();
            var request = new RestRequest($"metric-{DateTime.Now:yyyy-MM-dd}/metric/{Guid.NewGuid()}", Method.PUT);
            request.AddJsonBody(result);
            var response = client.Execute(request);
        }

        public MetricResult Pop()
        {
            throw new NotImplementedException();
        }

        private RestClient GetClient()
        {
            var client = new RestClient("http://localhost:9200/");
            client.Authenticator = new HttpBasicAuthenticator("elastic", "hgysi7fg6A09ofLpFnjl");

            return client;
        }

        public void DeleteIndex(DateTime date)
        {
            var request = new RestRequest($"metric-{date:yyyy-MM-dd}", Method.DELETE);
            var response = GetClient().Execute(request);
        }
    }
}
