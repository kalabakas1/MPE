using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Pinger.Models
{
    public class HaProxyConfiguration
    {
        [JsonProperty("Endpoint")]
        public string Endpoint { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("Fields")]
        public string[] Fields { get; set; }
    }
}
