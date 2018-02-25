using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MPE.Api.Repositories;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MPE.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestrictedMethodAttribute : AuthorizationFilterAttribute
    {
        private const string AuthorizationHeaderName = "Authorization";
        public string Method { get; set; }
        public RestrictedMethodAttribute(string method)
        {
            Method = method;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var apiKeyRepository = new ApiKeyRepository();

            var header = actionContext.Request.Headers.FirstOrDefault(x => x.Key == AuthorizationHeaderName);
            if (header.Value == null || !header.Value.Any())
            {
                throw new HttpResponseException(new HttpResponseMessage {StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = $"Missing header {AuthorizationHeaderName}"});
            }

            var maybeApiKey = apiKeyRepository.Get(header.Value.FirstOrDefault());
            if (!maybeApiKey.HasValue)
            {
                throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = $"No such API key exists" });
            }

            var apiKey = maybeApiKey.Value;
            if (!apiKey.Methods.Select(x => x.Method).Contains(Method))
            {
                throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = $"API key {apiKey.Key} not allowed to execute method {Method}" });
            }
        }
    }
}
