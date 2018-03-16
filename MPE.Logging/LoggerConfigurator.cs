using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging.Interfaces;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Slack;

namespace MPE.Logging
{
    public class LoggerConfigurator
    {
        private const string FilePathDefault = "../Logs/log-{Date}.txt";

        private readonly IAppSettingRepository _appSettingRepository;

        public LoggerConfigurator(
            IAppSettingRepository appSettingRepository)
        {
            _appSettingRepository = appSettingRepository;
        }

        public Logger Generate()
        {
            var configuration = new LoggerConfiguration()
                .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyName)
                .MinimumLevel.Is(Get(Constants.MinimumLevel, LogEventLevel.Debug))
                .WriteTo.Console(Get(Constants.Console_MinimumLevel, LogEventLevel.Debug))
                .WriteTo.RollingFile(Get(Constants.File_FileName, FilePathDefault), Get(Constants.File_MinimumLevel, LogEventLevel.Debug));

            var slackUrl = Get(Constants.Slack_Url, string.Empty);
            if (!string.IsNullOrEmpty(slackUrl))
            {
                configuration.WriteTo.Slack(slackUrl, restrictedToMinimumLevel: Get(Constants.Slack_MinimumLevel, LogEventLevel.Error));
            }

            var elasticUrl = Get(Constants.Elastic_Url, string.Empty);
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                configuration.WriteTo.Elasticsearch(elasticUrl,
                    restrictedToMinimumLevel: Get(Constants.Elastic_MinimumLevel, LogEventLevel.Debug), 
                    indexFormat: Get(Constants.Elastic_IndexFormat, "log_{0:yyyy.MM}"));
            }

            return configuration.CreateLogger();
        }

        private T Get<T>(string key, T fallback)
        {
            try
            {
                return _appSettingRepository.Get<T>(key);
            }
            catch
            {
                return fallback;
            }
        }
    }
}
