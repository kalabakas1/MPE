using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;
using Newtonsoft.Json;

namespace MPE.Pinger.Logic.Listeners
{
    internal class EventLogListener
    {
        private ConfigurationFile _configurationFile;
        private List<EventLogWatcher> _eventLogs;
        private IRepository<MetricResult> _tmpRepository;

        public EventLogListener(
            IRepository<MetricResult> tmpRepository)
        {
            _tmpRepository = tmpRepository;
            _configurationFile = ConfigurationService.Instance.ReadConfigurationFile();

            _eventLogs = new List<EventLogWatcher>();
        }

        public void Init()
        {
            var eventLogging = _configurationFile.EventLogging;

            if (eventLogging?.Categories == null || !eventLogging.Categories.Any())
            {
                return;
            }

            var type = GetMinimumType(eventLogging.MinimumLevel);
            foreach (var category in eventLogging.Categories)
            {
                var query = new EventLogQuery(category, PathType.LogName);
                var watcher = new EventLogWatcher(query);
                watcher.EventRecordWritten += (sender, args) =>
                {
                    if (GetMinimumType(args.EventRecord.LevelDisplayName) <= type)
                    {
                        var result = new MetricResult
                        {
                            Path = $"{_configurationFile.Host}.Log",
                            Timestamp = args.EventRecord.TimeCreated ?? DateTime.Now,
                            Alias = $"Log.{args.EventRecord.LevelDisplayName}",
                            Message = JsonConvert.SerializeObject(args.EventRecord)
                        };

                        result.Data = new Dictionary<string, object>();
                        result.AddData("Level", args.EventRecord.LevelDisplayName);

                        _tmpRepository.Write(result);
                    }

                    args.EventRecord.Dispose();
                };

                watcher.Enabled = true;
                _eventLogs.Add(watcher);
            }
        }

        private EventLogEntryType GetMinimumType(string type)
        {
            var allowed = new List<string>
            {
                "Information",
                "Warning",
                "Error"
            };

            if (!allowed.Contains(type))
            {
                return EventLogEntryType.Information;
            }

            return (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), type);
        }
    }
}
