using System;
using System.Configuration;
using System.Linq;
using MPE.Logging.Interfaces;

namespace MPE.Logging.Repository
{
    internal class AppSettingRepository : IAppSettingRepository
    {
        public T Get<T>(string key)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                throw new Exception($"No AppSetting with key: {key}");
            }

            var rawData = ConfigurationManager.AppSettings[key];

            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), rawData);
            }
            else
            {
                return (T)Convert.ChangeType(rawData, typeof(T));
            }
        }
    }
}
