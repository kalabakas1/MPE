using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MPE.Pinger.Extensions;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models.Results;
using MPE.Pinger.Repositories;
using MPE.Pinger.Server.Attributes;

namespace MPE.Pinger.Server.Controllers
{
    public class StoryController : ApiController
    {
        private readonly IRepository<MetricResult> _tempRepository;
        public StoryController()
        {
            _tempRepository = new InMemoryRepository<MetricResult>();
        }

        [ApiKeyAuthorize]
        [HttpPost]
        public HttpResponseMessage Post(MetricResult input)
        {
            var result = new MetricResult
            {
                Path = $"Story.{input.Path}",
                Alias = input.Alias,
                Message = input.Message,
                Timestamp = DateTime.Now
            };

            _tempRepository.Write(new List<MetricResult> {result});

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
