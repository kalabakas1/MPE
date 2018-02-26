using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MPE.Api.Attributes;
using MPE.Api.Helpers;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;

namespace MPE.Api.Controllers
{
    [RoutePrefix("api/MPE")]
    public class SchemaApiController : ApiController
    {
        private object _lock = new Object();
        private HashSet<Type> _restrictedTypes;

        public SchemaApiController()
        {
            lock (_lock)
            {
                if (_restrictedTypes == null)
                {
                    _restrictedTypes =
                        ReflectionHelper.GetTypesDecoratedByAttribute(typeof(RestrictSerializationAttribute));
                }
            }
        }

        [HttpGet]
        [RestrictedMethod("MPE-RestrictedTypes")]
        [Route("RestrictedTypes")]
        public IHttpActionResult GetRestrictedTypes()
        {
            return Ok(_restrictedTypes.ToList()
                .Select(x => x.GetCustomAttribute<RestrictSerializationAttribute>().TypeAlias).OrderBy(x => x)
                .ToList());
        }

        [HttpGet]
        [RestrictedMethod("MPE-RestrictedTypes-Schema")]
        [Route("RestrictedTypes/{typeAlias}/Schema")]
        public IHttpActionResult GetRestrictedTypeSchema(string typeAlias)
        {
            var generator = new JsonSchemaGenerator();
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var type = _restrictedTypes.FirstOrDefault(x =>
                x.GetCustomAttribute<RestrictSerializationAttribute>().TypeAlias
                    .Equals(typeAlias, StringComparison.InvariantCultureIgnoreCase));

            if (type == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Ok(generator.Generate(type));
        }
    }
}
