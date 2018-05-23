using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Extensions;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;
using NPoco;

namespace MPE.Pinger.Logic.Collectors
{
    public class SqlQueryCollector : ICollector
    {
        private ConfigurationFile _configurationFile;
        private bool _isEnabled;
        public SqlQueryCollector()
        {
            _configurationFile = Configuration.ReadConfigurationFile();
            _isEnabled = _configurationFile.SqlConfiguration != null
                         && (_configurationFile.SqlConfiguration.SqlQueries?.Any() ?? false)
                         && (_configurationFile.SqlConfiguration.ConnectionStrings?.Any() ?? false);

            if (!_isEnabled)
            {
                LoggerFactory.Instance.Debug("SqlQueryCollector is not enabled");
            }
        }
        public List<MetricResult> Collect()
        {
            var result = new List<MetricResult>();

            foreach (var query in _configurationFile.SqlConfiguration.SqlQueries)
            {
                if (File.Exists(query.FilePath))
                {
                    try
                    {
                        using (var database =
                            new Database(_configurationFile.SqlConfiguration.ConnectionStrings[query.ConnectionString], "System.Data.SqlClient"))
                        {
                            var metrics = database.Fetch<MetricResult>(File.ReadAllText(query.FilePath));
                            foreach (var metric in metrics)
                            {
                                metric.Path = $"{_configurationFile.Host}.Sql.{query.Alias.RemoveSpecialCharacters()}.{metric.Alias.RemoveSpecialCharacters()}";
                            }

                            result.AddRange(metrics);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerFactory.Instance.Debug($"SqlQueryCollector - Failed to execute {query.FilePath} on {query.ConnectionString}");
                    }
                }
                else
                {
                    LoggerFactory.Instance.Debug($"SqlQueryCollector - File does not exist {query.FilePath}");
                }
            }

            return result;
        }
    }
}
