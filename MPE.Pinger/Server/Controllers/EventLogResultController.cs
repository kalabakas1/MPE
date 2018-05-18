using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;
using MPE.Pinger.Repositories;
using MPE.Pinger.Server.Attributes;

namespace MPE.Pinger.Server.Controllers
{
    public class EventLogResultController : ApiController
    {
        private readonly IRepository<EventLogResult> _tempRepository;
        public EventLogResultController()
        {
            _tempRepository = new InMemoryRepository<EventLogResult>();
        }

        [HttpPost]
        [ApiKeyAuthorize]
        public HttpResponseMessage Post(List<EventLogResult> results)
        {
            _tempRepository.Write(results);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
