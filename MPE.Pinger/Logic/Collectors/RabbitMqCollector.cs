using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;
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
            _configurationFile = ConfigurationService.Instance.ReadConfigurationFile();
            _isEnabled = _configurationFile.RabbitMq != null
                         && (_configurationFile.RabbitMq.Fields?.Any() ?? false);
        }

        public List<MetricResult> Collect()
        {
            if (!_isEnabled)
            {
                return new List<MetricResult>();
            }

            return MetricResultHelper.Generate($"{_configurationFile.Host}.RabbitMQ",
                _configurationFile.RabbitMq.Fields, RequestQueueApi());
        }

        private Dictionary<string, string> RequestQueueApi()
        {
            try
            {
                var host = string.IsNullOrEmpty(_configurationFile.RabbitMq.Host)
                    ? "localhost"
                    : _configurationFile.RabbitMq.Host;
                var port = _configurationFile.RabbitMq.Port == 0 ? 15672 : _configurationFile.RabbitMq.Port;
                var username = string.IsNullOrEmpty(_configurationFile.RabbitMq.Username)
                    ? "guest"
                    : _configurationFile.RabbitMq.Username;
                var password = string.IsNullOrEmpty(_configurationFile.RabbitMq.Password)
                    ? "guest"
                    : _configurationFile.RabbitMq.Password;

                var client = new RestClient($"http://{host}:{port}");
                client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest("/api/queues");
                var response = client.Execute(request);


                var result = new Dictionary<string, string>();
                var objects = JsonConvert.DeserializeObject<List<object>>(response.Content);
                if (objects != null && objects.Any())
                {
                    var converted = objects.Select(JsonObjectHelper.FlattenObject).ToList();
                    foreach (var convert in converted)
                    {
                        var name = convert["name"];
                        foreach (var key in convert.Keys.ToList())
                        {
                            if (key != "name")
                            {
                                var resultKey = $"{name}.{key}";
                                if (!result.ContainsKey(resultKey))
                                {
                                    result.Add(resultKey, convert[key]);
                                }
                            }
                        }
                    }

                    return result;
                }

                return result;
            }
            catch (Exception e)
            {
                LoggerFactory.Instance.Debug(e.Message);
                return new Dictionary<string, string>();
            }
        }
    }
}
