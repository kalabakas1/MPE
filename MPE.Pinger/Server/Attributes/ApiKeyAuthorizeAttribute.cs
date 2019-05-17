using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using MPE.Pinger.Repositories;

namespace MPE.Pinger.Server.Attributes
{
    internal class ApiKeyAuthorizeAttribute : AuthorizeAttribute
    {
        private ApiKeyRepository _apiKeyRepository;

        public ApiKeyAuthorizeAttribute()
        {
            _apiKeyRepository = new ApiKeyRepository();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Contains(Constants.AuthenticationHeaderName))
            {
                var maybeApiKey = actionContext.Request.Headers.GetValues(Constants.AuthenticationHeaderName).ToList();
                if (maybeApiKey.Count() != 1)
                {
                    actionContext.Response =
                        new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    if (!_apiKeyRepository.IsValid(maybeApiKey.First()))
                    {
                        actionContext.Response =
                            new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                    }
                }
            }
            else
            {
                actionContext.Response =
                    new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
