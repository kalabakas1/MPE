using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Pinger.Repositories
{
    internal class ElasticRestRepository<T> : IRepository<T>
    {
        private readonly IRepository<T> _inMemoryRepository;
        private const string Host = "http://localhost:9200/";
        private RestClient _client;
        private string _prefix;
        public ElasticRestRepository(
            IRepository<T> inMemoryRepository)
        {
            _inMemoryRepository = inMemoryRepository;
            _client = new RestClient("http://localhost:9200/");
            _prefix = typeof(T).Name.Replace("Result", string.Empty).ToLowerInvariant();
        }

        public void Write(List<T> results)
        {
            Parallel.ForEach(results, Write);
        }

        public void Write(T result)
        {
            try
            {
                var request = new RestRequest($"{_prefix}-{DateTime.Now:yyyy-MM-dd}/{_prefix}/{Guid.NewGuid()}", Method.PUT);
                request.AddJsonBody(result);
                var response = _client.Execute(request);

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    LoggerFactory.Instance.Debug($"IndexResponse: {response.StatusCode} - {response.Content}");
                    _inMemoryRepository.Write(result);
                }
            }
            catch (Exception e)
            {
                LoggerFactory.Instance.Debug(e.Message);
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
