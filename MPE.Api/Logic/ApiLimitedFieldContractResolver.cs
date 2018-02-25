using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Functional.Maybe;
using MPE.Api.Attributes;
using MPE.Api.Interfaces;
using MPE.Api.Models;
using MPE.Api.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Api.Logic
{
    public class ApiLimitedFieldContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly HashSet<Type> _types;
        private object _lock = new object();
        private IApiKeyRepository _apiKeyRepository;

        public ApiLimitedFieldContractResolver()
        {
            lock (_lock)
            {
                if (_types == null)
                {
                    _types = GetMarkedTypes();
                }
            }

            _apiKeyRepository = new ApiKeyRepository();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var declaringType = member.DeclaringType;
            if (_types.Contains(declaringType) && declaringType != null)
            {
                property.ShouldSerialize = s =>
                {
                    var maybeApiKey = GetApiKey();
                    if (maybeApiKey.HasValue)
                    {
                        var key = maybeApiKey.Value;
                        var field = key.Fields.FirstOrDefault(x =>
                            x.FieldName != null && x.FieldName.Equals(member.Name, StringComparison.InvariantCultureIgnoreCase)
                            && x.TypePath.Equals(declaringType.FullName, StringComparison.InvariantCultureIgnoreCase));

                        if (field != null)
                        {
                            return true;
                        }

                        return key.Fields.Any(x =>
                            x.TypePath.Equals(declaringType.FullName, StringComparison.InvariantCultureIgnoreCase)
                            && string.IsNullOrEmpty(x.FieldName))
                            || key.Admin;
                    }
                    return true;
                };
            }

            return property;
        }

        private Maybe<ApiKey> GetApiKey()
        {
            var header = HttpContext.Current.Request.Headers.Get(ApiConstants.AuthorizationHeaderName);
            var maybeKey = _apiKeyRepository.Get(header);
            return maybeKey;
        }

        private HashSet<Type> GetMarkedTypes()
        {
            var types = new HashSet<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = type.GetCustomAttribute<RestrictSerializationAttribute>();
                    if (attribute != null)
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }
    }
}
