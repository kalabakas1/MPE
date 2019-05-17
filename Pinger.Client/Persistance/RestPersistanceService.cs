using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;

namespace Pinger.Client.Persistance
{
    internal class RestPersistanceService
    {
        private string _apiKey;
        private string _host;

        private RestClient _client;
        public RestPersistanceService(
            string apiKey,
            string host)
        {
            _apiKey = apiKey;
            _host = host;

            _client = new RestClient($"{_host}");
        }

        public void Persist<T>(List<T> metrics)
        {
            var endpoint = typeof(T).GetAttribute<DescriptionAttribute>().Description;

            var request = new RestRequest($"/api/{endpoint}");
            request.Method = Method.POST;
            request.AddHeader("Authorization", _apiKey);
            request.Timeout = 2000;
            request.AddParameter("application/json", JsonConvert.SerializeObject(metrics), ParameterType.RequestBody);

            var response = _client.Execute(request);

            Console.WriteLine($"Persistance response: " +response.StatusCode);
        }
    }
}
