using System;
using System.Collections.Generic;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Pinger.Repositories
{
    internal class ElasticRestRepository<T> : IRepository<T>
    {
        private const string Host = "http://localhost:9200/";
        private RestClient _client;
        private string _prefix;
        public ElasticRestRepository()
        {
            _client = new RestClient("http://localhost:9200/");
            _prefix = typeof(T).Name.Replace("Result", string.Empty).ToLowerInvariant();
        }

        public void Write(List<T> results)
        {
            foreach (var metricResult in results)
            {
                Write(metricResult);
            }
        }

        public void Write(T result)
        {
            try
            {
                var request = new RestRequest($"{_prefix}-{DateTime.Now:yyyy-MM-dd}/{_prefix}/{Guid.NewGuid()}", Method.PUT);
                request.AddJsonBody(result);
                var response = _client.Execute(request);
            }
            catch (Exception e)
            {
                
            }
        }

        public T Pop()
        {
            throw new NotImplementedException();
        }

        public void DeleteIndex(DateTime date)
        {
            var request = new RestRequest($"{_prefix}-{date:yyyy-MM-dd}", Method.DELETE);
            var response = _client.Execute(request);
        }
    }
}
