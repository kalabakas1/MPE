using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Logging.Repository;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Repositories;
using Newtonsoft.Json;

namespace MPE.Pinger.Helpers
{
    internal class Configuration
    {
        private const int DefaultUpdateInMinutes = 10;

        private static ConfigurationFile _configuration;
        private static readonly object _lock = new object();
        private static DateTime _nextUpdate = DateTime.MinValue;

        public static T Get<T>(string key)
        {
            var appSettingRepository = new SettingsRepository();
            return appSettingRepository.Get<T>(key);
        }

        public static ConfigurationFile ReadConfigurationFile()
        {
            lock (_lock)
            {
                if (_configuration == null || _nextUpdate < DateTime.Now)
                {
                    var restConfigRepository = new ConfigurationRestRepository();

                    try
                    {
                        var fileData = File.ReadAllText(Get<string>(Constants.ConfigurationPath));
                        _configuration = JsonConvert.DeserializeObject<ConfigurationFile>(fileData);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Could not read configuration file", e);
                    }

                    var newConfiguration = restConfigRepository.RequestConfiguration(
                        Get<string>(Constants.RestEndpoint), 
                        Get<string>(Constants.ClientHost), 
                        Get<string>(Constants.ClientApiKey));

                    if (newConfiguration == null && _configuration != null)
                    {
                        LoggerFactory.Instance.Debug("Configuration update request sent");
                        restConfigRepository.UpdateConfiguration(_configuration);
                    }
                    else
                    {
                        LoggerFactory.Instance.Debug("Updating local configuration file");
                        File.WriteAllText(Get<string>(Constants.ConfigurationPath),
                            JsonConvert.SerializeObject(newConfiguration, Formatting.Indented));
                        _configuration = newConfiguration;
                    }

                    _nextUpdate = DateTime.Now.AddMinutes(DefaultUpdateInMinutes);
                }
            }

            return _configuration;
        }
    }
}
