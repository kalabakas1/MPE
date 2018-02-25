using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Functional.Maybe;
using MPE.Api.Interfaces;
using MPE.Api.Models;
using NPoco;

namespace MPE.Api.Repositories
{
    internal class ApiKeyRepository : IApiKeyRepository
    {
        private readonly ApiKeyCache _apiKeyCache;
        public ApiKeyRepository()
        {
            _apiKeyCache = new ApiKeyCache();
        }

        private Database GetClient()
        {
            return new Database(ApiConstants.ConnectionStringName);
        }

        public Maybe<ApiKey> Get(string key)
        {
            var fromCache = _apiKeyCache.Get(key);
            if (fromCache.HasValue)
            {
                return fromCache;
            }

            using (var client = GetClient())
            {
                var apiKey =
                    client.FirstOrDefault<ApiKey>(@"SELECT * FROM Api_Key WHERE Deleted = 0 AND [Key] = @0", key);
                if (apiKey != null)
                {
                    apiKey.Methods =
                        client.Fetch<ApiKeyMethod>(@"SELECT * FROM Api_KeyMethod WHERE Deleted = 0 AND KeyID = @0",
                            apiKey.Id);
                    apiKey.Fields =
                        new HashSet<ApiKeyField>(client.Fetch<ApiKeyField>(@"SELECT * FROM Api_KeyField WHERE Deleted = 0 AND KeyID = @0",
                            apiKey.Id));

                    _apiKeyCache.Add(apiKey);
                }

                return apiKey.ToMaybe();
            }
        }
    }
}
