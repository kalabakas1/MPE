using System;
using System.Configuration;

namespace Pinger.Client.Extensions
{
    internal static class AppSettingsExtensions
    {
        public static T GetAppSettingValue<T>(this string appSettingsName, T defaultValue)
        {
            T result = default(T);
            try
            {
                result = (T)Convert.ChangeType(ConfigurationManager.AppSettings[appSettingsName], typeof(T));
            }
            catch
            {
                result = defaultValue;
            }

            return result;
        }

        public static T GetSetting<T>(this string appSettingsName)
        {
            T result = default(T);
            try
            {
                result = (T)Convert.ChangeType(ConfigurationManager.AppSettings[appSettingsName], typeof(T));
            }
            catch
            {
                result = default(T);
            }

            return result;
        }
    }
}
