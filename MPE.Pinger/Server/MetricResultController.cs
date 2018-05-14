using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MPE.Pinger.Models;
using MPE.Pinger.Repositories;

namespace MPE.Pinger.Server
{
    public class MetricResultController : ApiController
    {
        private const string AuthenticationHeaderName = "Authorization";

        private ApiKeyRepository _apiKeyRepository;
        public MetricResultController()
        {
            _apiKeyRepository = new ApiKeyRepository();
        }

        public HttpResponseMessage Post(List<MetricResult> results)
        {
            if (!IsAuthentic(Request))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Api key not valid");
            }



            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private bool IsAuthentic(HttpRequestMessage message)
        {
            if (message.Headers.Contains(AuthenticationHeaderName))
            {
                var maybeApiKey = message.Headers.GetValues(AuthenticationHeaderName).ToList();
                if (maybeApiKey.Count() != 1)
                {
                    return false;
                }
                else
                {
                    return _apiKeyRepository.IsValid(maybeApiKey.First());
                }
            }

            return false;
        }
    }
}
