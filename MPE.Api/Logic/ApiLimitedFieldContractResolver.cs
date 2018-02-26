using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Functional.Maybe;
using MPE.Api.Attributes;
using MPE.Api.Helpers;
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
        private readonly IApiAuthorizationService _authorizationService;

        public ApiLimitedFieldContractResolver()
        {
            lock (_lock)
            {
                if (_types == null)
                {
                    _types = ReflectionHelper.GetTypesDecoratedByAttribute(typeof(RestrictSerializationAttribute));
                }
            }

            _authorizationService = new ApiAuthorizationService();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var declaringType = member.DeclaringType;
            if (_types.Contains(declaringType) && declaringType != null)
            {
                property.ShouldSerialize = s =>
                {
                    var header = HttpContext.Current.Request.Headers.Get(ApiConstants.AuthorizationHeaderName);
                    return _authorizationService.ShouldFieldBeSerialized(header, member.DeclaringType, member.Name);
                };
            }

            return property;
        }
    }
}
