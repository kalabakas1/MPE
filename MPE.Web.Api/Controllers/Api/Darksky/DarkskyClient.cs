using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MPE.Web.Api.Controllers.Api.Models;
using RestSharp;

namespace MPE.Web.Api.Controllers.Api.Darksky
{
    public class DarkskyClient
    {
        private const string Endpoint = "https://api.darksky.net/";
        private RestClient _client;
        private string _key;

        public DarkskyClient(string key)
        {
            _client = new RestClient(Endpoint);
            _key = key;
        }

        public Weather Get(decimal longitude, decimal latitude, DateTime date)
        {
            var request = new RestRequest($"forecast/{_key}/{longitude.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture)},{latitude.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture)},{UnixTimeNow(date)}");
            var result = _client.Execute<Weather>(request);

            return result.Data;
        }

        public long UnixTimeNow(DateTime dt)
        {
            var timeSpan = (dt - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
    }
}