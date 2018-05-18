using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Pinger.Logic.Collectors
{
    internal class HaProxyCollector : ICollector
    {
        private ConfigurationFile _configurationFile;
        private bool _isEnabled;
        public HaProxyCollector()
        {
            _configurationFile = Configuration.ReadConfigurationFile();
            _isEnabled = _configurationFile.HaProxy != null &&
                         !string.IsNullOrEmpty(_configurationFile.HaProxy.Endpoint) &&
                         !string.IsNullOrEmpty(_configurationFile.HaProxy.Username) &&
                         !string.IsNullOrEmpty(_configurationFile.HaProxy.Password) &&
                         (_configurationFile.HaProxy.Fields?.Any() ?? false);
        }

        public List<MetricResult> Collect()
        {
            if (!_isEnabled)
            {
                return new List<MetricResult>();
            }

            var results = new List<MetricResult>();

            var csvData = QueryDataAsCsv();
            var headers = new List<string>();
            var rows = ConvertCsvToData(csvData, out headers);
            if (rows.Count == 0)
            {
                return results;
            }

            var pxNameIndex = headers.IndexOf("# pxname");
            var svNameIndex = headers.IndexOf("svname");
            foreach (var row in rows)
            {
                if (row.Count != headers.Count)
                {
                    continue;
                }

                foreach (var header in headers)
                {
                    if (header == "# pxname" || header == "svname")
                    {
                        continue;
                    }

                    var path = $"{_configurationFile.Host}.HaProxy.{row[pxNameIndex]}.{row[svNameIndex]}.{header}";
                    if (!_configurationFile.HaProxy.Fields.Any(x => Regex.IsMatch(path, x)))
                    {
                        continue;
                    }

                    var metricResult = new MetricResult
                    {
                        Timestamp = DateTime.Now,
                        Path = path,
                        Alias = header
                    };

                    var headerIndex = headers.IndexOf(header);
                    var strValue = row[headerIndex];
                    float flValue = 0;
                    if (float.TryParse(strValue, out flValue))
                    {
                        metricResult.Value = flValue;
                    }
                    else
                    {
                        metricResult.Message = strValue;
                    }
                    
                    results.Add(metricResult);
                }
            }

            return results;
        }

        private string QueryDataAsCsv()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            var uri = new Uri(_configurationFile.HaProxy.Endpoint);
            var client = new RestClient(uri.GetLeftPart(UriPartial.Authority));
            client.Authenticator = new HttpBasicAuthenticator(_configurationFile.HaProxy.Username, _configurationFile.HaProxy.Password);

            var request = new RestRequest(uri.PathAndQuery + "?stats;csv");
            var response = client.Execute(request);

            return response.Content;
        }

        private List<List<string>> ConvertCsvToData(string csvData, out List<string> headers)
        {
            headers = new List<string>();
            if (string.IsNullOrEmpty(csvData))
            {
                return new List<List<string>>();
            }

            var result = new List<List<string>>();

            var lines = csvData.Split('\n');
            if (lines.Length > 0)
            {
                headers = lines[0].Split(',').ToList();
            }

            for (int i = 1; i < lines.Length; i++)
            {
                result.Add(lines[i].Split(',').ToList());
            }

            return result;
        }
    }
}
