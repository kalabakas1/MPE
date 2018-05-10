using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging.Repository;
using MPE.Pinger.Models;
using Newtonsoft.Json;

namespace MPE.Pinger.Helpers
{
    internal class Configuration
    {
        public static T Get<T>( string key)
        {
            var appSettingRepository = new SettingsRepository();
            return appSettingRepository.Get<T>(key);
        }

        public static ConfigurationFile ReadConfigurationFile()
        {
            var fileData = File.ReadAllText(Get<string>("MPE.Pinger.Configuration.Path"));
            return JsonConvert.DeserializeObject<ConfigurationFile>(fileData);
        }
    }
}
