using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Models.Configurations;
using Newtonsoft.Json;
using RestSharp;

namespace MPE.Pinger.Repositories
{
    internal class ConfigurationRestRepository
    {
        public ConfigurationFile RequestConfiguration(ConfigurationFile current)
        {
            var restClient = new RestClient(current.RestEndpoint);
            var request = new RestRequest("/api/Configuration");
            request.AddQueryParameter("host", current.Host);
            request.AddHeader(Constants.AuthenticationHeaderName, current.ApiKey);
            request.Method = Method.GET;

            var result = restClient.Execute<ConfigurationFile>(request);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return result.Data;
            }

            return null;
        }

        public void UpdateConfiguration(ConfigurationFile configuration)
        {
            var restClient = new RestClient(configuration.RestEndpoint);
            var request = new RestRequest("/api/Configuration");
            request.AddHeader(Constants.AuthenticationHeaderName, configuration.ApiKey);
            request.AddParameter("application/json", JsonConvert.SerializeObject(configuration), ParameterType.RequestBody);
            request.Method = Method.POST;

            var result = restClient.Execute(request);
        }
    }
}
