using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        private Database GetClient()
        {
            return new Database(ApiConstants.ConnectionStringName);
        }

        public Maybe<ApiKey> Get(string key)
        {
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
                        client.Fetch<ApiKeyField>(@"SELECT * FROM Api_KeyField WHERE Deleted = 0 AND KeyID = @0",
                            apiKey.Id);
                }

                return apiKey.ToMaybe();
            }
        }
    }
}
