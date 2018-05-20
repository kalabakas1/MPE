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
            var categories = _configurationFile.LogCategories;

            if (categories == null || !categories.Any())
            {
                return;
            }

            foreach (var category in categories)
            {
                var eventLog = new EventLog(category);
                eventLog.EnableRaisingEvents = true;
                eventLog.EntryWritten += (sender, eventArgs) =>
                {
                    var result = ConvertToResult(eventArgs.Entry);
                    _tmpRepository.Write(result);
                };

                _eventLogs.Add(eventLog);
            }
        }

        private EventLogResult ConvertToResult(EventLogEntry entry)
        {
            return new EventLogResult
            {
                Path = $"{_configurationFile.Host}.Log.",
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
