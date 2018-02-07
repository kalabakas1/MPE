using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Regtime.Outlook.App.Models;
using MPE.Regtime.Outlook.App.Models.Configurations;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Services
{
    internal class ConfigurationService
    {
        private const string DefaultConfigurationFileName = @"Regtime.json";
        private static Configuration _configuration;
        private readonly string _configurationFilePath;

        public ConfigurationService(
            string configurationPath = null)
        {
            _configurationFilePath = configurationPath ?? DefaultConfigurationFileName;
            if (_configuration == null)
            {
                _configuration = ReadConfiguration();
            }
        }

        public Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = ReadConfiguration();
                }
                return _configuration;
            }
        }

        public void SetPassword(string password)
        {
            _configuration.Password = password;
        }

        private Configuration ReadConfiguration()
        {
            var fileContent = File.ReadAllText(_configurationFilePath);
            var configuration = JsonConvert.DeserializeObject<Configuration>(fileContent);

            return configuration;
        }
    }
}
