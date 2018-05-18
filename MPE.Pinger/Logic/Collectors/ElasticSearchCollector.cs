using System.Collections.Generic;
using System.Linq;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;
using RestSharp;

namespace MPE.Pinger.Logic.Collectors
{
    public class ElasticSearchCollector : ICollector
    {
        private const string DefaultHost = "localhost";
        private const int DefaultPort = 9200;

        private ConfigurationFile _configurationFile;
        private bool _isEnabled;
        public ElasticSearchCollector()
        {
            _configurationFile = Configuration.ReadConfigurationFile();
            _isEnabled = _configurationFile.ElasticSearch != null &&
                         (_configurationFile.ElasticSearch.Fields?.Any() ?? false);
        }

        public List<MetricResult> Collect()
        {
            if (!_isEnabled)
            {
                return new List<MetricResult>();
            }

            return MetricResultHelper.Generate($"{_configurationFile.Host}.Elastic", _configurationFile.ElasticSearch.Fields, RequestData());
        }

        private Dictionary<string, string> RequestData()
        {
            var host = string.IsNullOrEmpty(_configurationFile.ElasticSearch.Host) ? DefaultHost : _configurationFile.ElasticSearch.Host;
            var port = _configurationFile.ElasticSearch.Port == 0 ? DefaultPort : _configurationFile.ElasticSearch.Port;

            var client = new RestClient($"http://{host}:{port}");
            var request = new RestRequest("/_nodes/stats");
            var response = client.Execute(request);

            var data = JsonObjectHelper.FlattenObject(response.Content);
            return data;
        }
    }
}
