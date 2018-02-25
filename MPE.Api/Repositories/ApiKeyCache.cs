using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Functional.Maybe;
using MPE.Api.Models;

namespace MPE.Api.Repositories
{
    internal class ApiKeyCache
    {
        private const int ExpirationInSeconds = 10;
        public string GenerateCacheKey(string key)
        {
            return $"{typeof(ApiKey).Name}_{key}";
        }

        public Maybe<ApiKey> Get(string key)
        {
            var cacheItem = MemoryCache.Default.Get(GenerateCacheKey(key));
            if (cacheItem != null)
            {
                return ((ApiKey)cacheItem).ToMaybe();
            }
            return ((ApiKey)null).ToMaybe();
        }

        public void Add(ApiKey key)
        {
            MemoryCache.Default.Add(GenerateCacheKey(key.Key), key, new DateTimeOffset(DateTime.Now.AddSeconds(ExpirationInSeconds)));
        }
    }
}
