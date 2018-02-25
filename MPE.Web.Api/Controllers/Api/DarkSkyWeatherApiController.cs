using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using MPE.Api.Attributes;
using MPE.Web.Api.Controllers.Api.Darksky;
using MPE.Web.Api.Controllers.Api.Models;
using RestSharp;

namespace MPE.Web.Api.Controllers.Api
{
    [RoutePrefix("api/darksky")]
    public class DarkskyWeatherApiController : ApiController
    {
        [RestrictedMethod("Darksky:GetByDate")]
        [Route("{key}/long/{longitude:decimal}/lat/{latitude:decimal}/date/{dt:datetime}")]
        public object GetByDate(string key, decimal longitude, decimal latitude, DateTime dt)
        {
            var client = new DarkskyClient(key);
            var weather = client.Get(longitude, latitude, dt);
            return Ok(weather);
        }

        [RestrictedMethod("Darksky:GetByDateAarhus")]
        [Route("{key}/aarhus-dk/date/{dt:datetime}")]
        public object GetByDateAarhus(string key, DateTime dt)
        {
            //The coordinates for Aarhus city
            decimal latitude = (decimal)10.2039;
            decimal longitude = (decimal)56.1629;
            var client = new DarkskyClient(key);
            var weather = client.Get(longitude, latitude, dt);
            return Ok(weather);
        }
    }
}