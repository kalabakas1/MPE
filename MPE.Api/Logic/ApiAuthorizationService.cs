using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MPE.Api.Attributes;
using MPE.Api.Interfaces;
using MPE.Api.Repositories;

namespace MPE.Api.Logic
{
    internal class ApiAuthorizationService : IApiAuthorizationService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        public ApiAuthorizationService()
        {
            _apiKeyRepository = new ApiKeyRepository();
        }

        public bool ShouldFieldBeSerialized(string key, Type type, string fieldName)
        {
            var maybeApiKey = _apiKeyRepository.Get(key);
            if (maybeApiKey.HasValue)
            {
                var typeAlias = type.GetCustomAttribute<RestrictSerializationAttribute>()
                    .TypeAlias;
                var apiKey = maybeApiKey.Value;
                var field = apiKey.Fields.FirstOrDefault(x =>
                    x.FieldName != null && x.FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)
                    && x.TypeAlias.Equals(typeAlias, StringComparison.InvariantCultureIgnoreCase));

                if (field != null)
                {
                    return true;
                }

                return apiKey.Fields.Any(x =>
                           x.TypeAlias.Equals(typeAlias, StringComparison.InvariantCultureIgnoreCase)
                           && string.IsNullOrEmpty(x.FieldName))
                       || apiKey.Admin;
            }

            return false;
        }
    }
}
