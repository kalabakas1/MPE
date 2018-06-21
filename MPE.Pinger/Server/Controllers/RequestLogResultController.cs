using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;
using MPE.Pinger.Repositories;
using MPE.Pinger.Server.Attributes;

namespace MPE.Pinger.Server.Controllers
{
    public class RequestLogResultController : ApiController
    {
        private readonly IRepository<RequestLogResult> _tempRepository;
        public RequestLogResultController()
        {
            _tempRepository = new InMemoryRepository<RequestLogResult>();
        }

        [HttpPost]
        [ApiKeyAuthorize]
        public HttpResponseMessage Post(List<RequestLogResult> results)
        {
            _tempRepository.Write(results);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
