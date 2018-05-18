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
    public class MetricResultController : ApiController
    {
        private readonly IRepository<MetricResult> _tempRepository;
        public MetricResultController()
        {
            _tempRepository = new InMemoryRepository<MetricResult>();
        }

        [HttpPost]
        [ApiKeyAuthorize]
        public HttpResponseMessage Post(List<MetricResult> results)
        {
            _tempRepository.Write(results);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
