﻿using System;
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
        public ConfigurationFile RequestConfiguration(string restEndpoint, string host, string apiKey)
        {
            var restClient = new RestClient(restEndpoint);
            var request = new RestRequest("/api/Configuration");
            request.AddQueryParameter("host", host);
            request.AddHeader(Constants.AuthenticationHeaderName, apiKey);
            request.Method = Method.GET;
            request.Timeout = 5000;

            var result = restClient.Execute(request);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ConfigurationFile>(result.Content);
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
            request.Timeout = 5000;

            var result = restClient.Execute(request);
        }
    }
}
