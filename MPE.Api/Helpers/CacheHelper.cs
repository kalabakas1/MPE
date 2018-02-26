using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Functional.Maybe;

namespace MPE.Api.Helpers
{
    internal static class CacheHelper<T>
    {

        private const int ExpirationInSeconds = 10;
        public static Maybe<T> Get(string key)
        {
            var cacheItem = MemoryCache.Default.Get(GenerateCacheKey(key));
            if (cacheItem != null)
            {
                return ((T)cacheItem).ToMaybe();
            }
            return default(T).ToMaybe();
        }

        public static void Add(string key, T obj)
        {
            MemoryCache.Default.Add(GenerateCacheKey(key), obj, new DateTimeOffset(DateTime.Now.AddSeconds(ExpirationInSeconds)));
        }

        private static string GenerateCacheKey(string key)
        {
            return $"{typeof(T).Name}_{key}";
        }
    }
}
