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

namespace MPE.Pinger.Server.Controllers
{
    public class SlackWebhookController : ApiController
    {
        private readonly IRepository<MetricResult> _tempRepository;
        public SlackWebhookController()
        {
            _tempRepository = new InMemoryRepository<MetricResult>();
        }

        [HttpPost]
        public HttpResponseMessage Post(SlackMessage message)
        {
            if (Helpers.Configuration.Get<string>(Constants.SlackToken).Split(',').All(x => x != message.token))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Invalid Token");
            }

            var result = new MetricResult
            {
                Path = $"Story.{message.team_domain}.{message.channel_name.RemoveSpecialCharacters()}",
                Timestamp = DateTime.Now,
                Alias = $"Story.{message.team_domain}.{message.channel_name}.{message.user_name}",
                Message = $"{message.user_name}: {message.text.Replace(message.trigger_word, string.Empty)}"
            };
            
            _tempRepository.Write(new List<MetricResult> { result } );
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }

    public class SlackMessage : BaseResult
    {
        public string token { get; set; }
        public string team_id { get; set; }
        public string team_domain { get; set; }
        public string channel_id { get; set; }
        public string channel_name { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string text { get; set; }
        public string trigger_word { get; set; }
    }
}
