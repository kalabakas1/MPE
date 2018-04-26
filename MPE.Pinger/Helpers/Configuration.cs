using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging.Repository;

namespace MPE.Pinger.Helpers
{
    internal class Configuration
    {
        public static T Get<T>( string key)
        {
            var appSettingRepository = new SettingsRepository();
            return appSettingRepository.Get<T>(key);
        }
    }
}
