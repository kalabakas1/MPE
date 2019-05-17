using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Models.Results;

namespace MPE.Pinger.Logic.Listeners
{
    internal class EventLogListener
    {
        private ConfigurationFile _configurationFile;
        private List<EventLog> _eventLogs;
        private IRepository<EventLogResult> _tmpRepository;

        public EventLogListener(
            IRepository<EventLogResult> tmpRepository)
        {
            _tmpRepository = tmpRepository;
            _configurationFile = Configuration.ReadConfigurationFile();

            _eventLogs = new List<EventLog>();
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
                var eventLog = new EventLog(category);
                eventLog.EnableRaisingEvents = true;
                eventLog.EntryWritten += (sender, eventArgs) =>
                {
                    if (eventArgs.Entry.EntryType <= type)
                    {
                        var result = ConvertToResult(eventArgs.Entry);
                        _tmpRepository.Write(result);
                    }

                    eventArgs.Entry.Dispose();
                };

                _eventLogs.Add(eventLog);
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

        private EventLogResult ConvertToResult(EventLogEntry entry)
        {
            return new EventLogResult
            {
                Path = $"{_configurationFile.Host}.Log",
                Type = entry.EntryType.ToString(),
                Source = entry.Source,
                Message = entry.Message,
                Machine = entry.MachineName,
                Username = entry.UserName,
                Timestamp = entry.TimeGenerated
            };
        }
    }
}
