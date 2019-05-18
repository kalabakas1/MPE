using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Helpers;

namespace MPE.Pinger.Repositories
{
    internal class ApiKeyRepository
    {
        private static HashSet<string> _apiKeys;
        private static object _lock = new object();
        public ApiKeyRepository()
        {
            lock (_lock)
            {
                if (_apiKeys == null)
                {
                    var apiKeyPath = ConfigurationService.Instance.Get<string>(Constants.ApiKeysPath);
                    var keys = ReadKeys(apiKeyPath);
                    _apiKeys = new HashSet<string>(keys);
                }
            }
        }

        public bool IsValid(string apiKey)
        {
            return _apiKeys.Contains(apiKey);
        }

        private List<string> ReadKeys(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllLines(path).ToList();
            }
            return new List<string>();
        }
    }
}
