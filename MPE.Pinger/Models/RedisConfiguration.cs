using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPE.Pinger.Models
{
    internal class RedisConfiguration
    {
        [JsonProperty("Host")]
        public string Host { get; set; }
        [JsonProperty("Port")]
        public int Port { get; set; }
        [JsonProperty("Metrics")]
        public string[] Metrics { get; set; }
    }
}
