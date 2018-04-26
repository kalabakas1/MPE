using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using MPE.Logging.Interfaces;
using Newtonsoft.Json;

namespace MPE.Logging.Repository
{
    public class SettingsRepository : IAppSettingRepository
    {
        private static Dictionary<string, string> _configuration;
        public SettingsRepository()
        {
            if (_configuration == null)
            {
                _configuration = ReadConfiguration();
            }
        }

        public T Get<T>(string key)
        {
            if (!_configuration.Keys.Contains(key))
            {
                throw new Exception($"No configuration with key: {key}");
            }

            var rawData = _configuration[key];

            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), rawData);
            }
            else
            {
                return (T)Convert.ChangeType(rawData, typeof(T));
            }
        }

        private Dictionary<string, string> ReadConfiguration()
        {
            try
            {
                var data = File.ReadAllText(ConfigurationManager.AppSettings["MPE.Configuration"]);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
