using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using Newtonsoft.Json;
using RestSharp;

namespace MPE.Pinger.Repositories
{
    internal class RestRepository<T> : IRepository<T>
    {
        private ConfigurationFile _configurationFile;
        public RestRepository()
        {
            _configurationFile = Configuration.ReadConfigurationFile();
        }

        public void Write(List<T> results)
        {
            if (results == null || !results.Any())
            {
                return;
            }

            var request = new RestRequest("/api/" + typeof(T).Name);
            request.Method = Method.POST;
            request.AddHeader("Authorization", _configurationFile.ApiKey);
            request.AddParameter("application/json", JsonConvert.SerializeObject(results), ParameterType.RequestBody);


            var response = GetClient().Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                LoggerFactory.Instance.Debug(response.ErrorMessage);
                throw new Exception(response.ErrorMessage);
            }
        }

        public void Write(T result)
        {
            if (result == null)
            {
                return;
            }

            Write(new List<T> { result });
        }

        public T Pop()
        {
            return default(T);
        }

        private RestClient GetClient()
        {
            var client = new RestClient(_configurationFile.RestEndpoint);

            return client;
        }
    }
}
