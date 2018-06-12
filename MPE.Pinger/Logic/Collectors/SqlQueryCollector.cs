using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        private readonly IRepository<MetricResult> _repository;
        private ConfigurationFile _configurationFile;
        private bool _isEnabled;
        private bool _isStarted;
        private List<Timer> _timers;

        public SqlQueryCollector(
            IRepository<MetricResult> repository)
        {
            _repository = repository;
            _configurationFile = Configuration.ReadConfigurationFile();
            _isEnabled = _configurationFile.SqlConfiguration != null
                         && (_configurationFile.SqlConfiguration.SqlQueries?.Any() ?? false)
                         && (_configurationFile.SqlConfiguration.ConnectionStrings?.Any() ?? false);

            if (!_isEnabled)
            {
                LoggerFactory.Instance.Debug("SqlQueryCollector is not enabled");
            }
        }

        private void Init()
        {
            _timers = new List<Timer>();

            foreach (var query in _configurationFile.SqlConfiguration.SqlQueries)
            {
                if (File.Exists(query.FilePath))
                {
                    var timer = new Timer(query.IntervalInSec * 1000);
                    timer.Elapsed += (sender, args) =>
                    {
                        try
                        {
                            using (var database =
                                new Database(
                                    _configurationFile.SqlConfiguration.ConnectionStrings[query.ConnectionString],
                                    "System.Data.SqlClient"))
                            {
                                var metrics = database.Fetch<MetricResult>(File.ReadAllText(query.FilePath));
                                var date = DateTime.Now;
                                foreach (var metric in metrics)
                                {
                                    metric.Path = $"{_configurationFile.Host}.Sql.{query.ConnectionString.RemoveSpecialCharacters()}.{query.Alias.RemoveSpecialCharacters()}.{metric.Alias.RemoveSpecialCharacters()}";
                                    metric.Alias = $"{query.ConnectionString}.{metric.Alias}".RemoveSpecialCharacters();
                                    metric.Message = $"{query.ConnectionString} - {metric.Message}";
                                    metric.Timestamp = date;
                                }

                                _repository.Write(metrics);
                            }
                        }
                        catch (Exception e)
                        {
                            LoggerFactory.Instance.Debug(
                                $"SqlQueryCollector - Failed to execute {query.FilePath} on {query.ConnectionString}");
                        }
                    };
                    timer.Start();

                    _timers.Add(timer);
                }
                else
                {
                    LoggerFactory.Instance.Debug($"SqlQueryCollector - File does not exist {query.FilePath}");
                }
            }

            _isStarted = true;
        }

        public List<MetricResult> Collect()
        {
            if (!_isStarted && _isEnabled)
            {
                Init();
            }

            return new List<MetricResult>();
        }
    }
}
