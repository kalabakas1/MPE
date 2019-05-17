using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MPE.Pinger.Models.Configurations;
using MPE.Pinger.Repositories;
using MPE.Pinger.Server.Attributes;

namespace MPE.Pinger.Server.Controllers
{
    [ApiKeyAuthorize]
    public class ConfigurationController : ApiController
    {
        private ConfigurationServerRepository _configurationServerRepository;

        public ConfigurationController()
        {
            _configurationServerRepository = new ConfigurationServerRepository();    
        }

        [HttpGet]
        public HttpResponseMessage Get(string host)
        {
            IEnumerable<string> headerValues = new List<string>();
            if (!Request.Headers.TryGetValues(Constants.AuthenticationHeaderName, out headerValues))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No auth header present");
            }

            var key = headerValues.FirstOrDefault();
            if (!_configurationServerRepository.HaveConfiguration(host, key))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            ConfigurationFile file = null;
            try
            {
                file = _configurationServerRepository.Get(host, key);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, file);
        }

        [HttpPost]
        public HttpResponseMessage Post(ConfigurationFile configuration)
        {
            if (configuration == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No file body located");
            }

            IEnumerable<string> headerValues = new List<string>();
            if (!Request.Headers.TryGetValues(Constants.AuthenticationHeaderName, out headerValues))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No auth header present");
            }

            var key = headerValues.FirstOrDefault();
            if (_configurationServerRepository.HaveConfiguration(configuration.Host, key))
            {
                return Request.CreateResponse(HttpStatusCode.Found);
            }

            if (key != null && key.Equals(configuration.ApiKey))
            {
                _configurationServerRepository.Save(configuration);
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
