using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Pinger.Logic.Collectors
{
    internal class RabbitMqCollector : ICollector
    {
        private ConfigurationFile _configurationFile;
        private bool _isEnabled;
        public RabbitMqCollector()
        {
            _configurationFile = Configuration.ReadConfigurationFile();
            _isEnabled = _configurationFile.RabbitMq != null
                         && (_configurationFile.RabbitMq.Fields?.Any() ?? false);
        }

        public List<MetricResult> Collect()
        {
            if (_isEnabled)
            {
                var data = RequestQueueApi();

                var result = new List<MetricResult>();

                foreach (var rabbitMqField in _configurationFile.RabbitMq.Fields)
                {
                    var pairs = data.Where(x => Regex.IsMatch(x.Key, rabbitMqField));
                    result.AddRange(pairs.Select(x =>
                    {
                        var metric = new MetricResult
                        {
                            Path = $"{_configurationFile.Host}.RabbitMQ.{x.Key}".Replace("-", "_"),
                            Alias = x.Key,
                            Timestamp = DateTime.Now
                        };

                        float value = 0;
                        var strVal = x.Value;
                        if (float.TryParse(strVal, out value))
                        {
                            metric.Value = value;
                        }

                        return metric;
                    }));
                }

                return result;
            }

            return new List<MetricResult>();
        }

        private Dictionary<string, string> RequestQueueApi()
        {
            var host = string.IsNullOrEmpty(_configurationFile.RabbitMq.Host) ? "localhost" : _configurationFile.RabbitMq.Host;
            var port = _configurationFile.RabbitMq.Port == 0 ? 15672 : _configurationFile.RabbitMq.Port;
            var username = string.IsNullOrEmpty(_configurationFile.RabbitMq.Username) ? "guest" : _configurationFile.RabbitMq.Username;
            var password = string.IsNullOrEmpty(_configurationFile.RabbitMq.Password) ? "guest" : _configurationFile.RabbitMq.Password;

            var client = new RestClient($"http://{host}:{port}");
            client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("/api/queues");
            var response = client.Execute(request);


            var result = new Dictionary<string, string>();
            var objects = JsonConvert.DeserializeObject<List<object>>(response.Content);
            if (objects != null && objects.Any())
            {
                var converted =  objects.Select(JsonObjectHelper.FlattenObject).ToList();
                foreach (var convert in converted)
                {
                    var name = convert["name"];
                    foreach (var key in convert.Keys.ToList())
                    {
                        if (key != "name")
                        {
                            result.Add($"{name}.{key}", convert[key]);
                        }
                    }
                }

                return result;
            }

            return result;
        }
    }
}
