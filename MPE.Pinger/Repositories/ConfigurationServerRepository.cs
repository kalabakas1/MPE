using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Helpers;
using MPE.Pinger.Models.Configurations;
using Newtonsoft.Json;

namespace MPE.Pinger.Repositories
{
    internal class ConfigurationServerRepository
    {
        public bool HaveConfiguration(string host, string key)
        {
            var path = GeneratePath(host, key);
            return File.Exists(path);
        }

        public ConfigurationFile Get(string host, string key)
        {
            if (!HaveConfiguration(host, key))
            {
                throw new Exception("No configurationfile located");
            }

            ConfigurationFile configuration = null;
            try
            {
                var rawConfigurationFile = File.ReadAllText(GeneratePath(host, key));
                configuration = JsonConvert.DeserializeObject<ConfigurationFile>(rawConfigurationFile);
            }
            catch (Exception e)
            {
                throw new Exception("Not possible to read or convert the file", e);
            }

            return configuration;
        }

        public void Save(ConfigurationFile configuration)
        {
            var path = GeneratePath(configuration.Host, configuration.ApiKey);
            File.WriteAllText(path, JsonConvert.SerializeObject(configuration, Formatting.Indented));
        }

        private string GeneratePath(string host, string key)
        {
            var name = GenerateName(host, key);
            var configurationFolder = ConfigurationService.Instance.Get<string>(Constants.ConfigurationFolder);

            var path = Path.Combine(configurationFolder, name);

            return path;
        }

        private string GenerateName(string host, string key)
        {
            return $"{host}_{key}.json";
        }
    }
}
