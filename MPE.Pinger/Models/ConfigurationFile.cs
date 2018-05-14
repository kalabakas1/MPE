using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Pinger.Models
{
    internal class ConfigurationFile
    {
        [JsonProperty("Host")]
        public string Host { get; set; }
        [JsonProperty("ServerHost")]
        public string ServerHost { get; set; }
        [JsonProperty("ServerPort")]
        public string ServerPort { get; set; }
        [JsonProperty("Connections")]
        public List<Connection> Connections { get; set; }
        [JsonProperty("Metrics")]
        public List<Metric> Metrics { get; set; }
        [JsonProperty("Redis")]
        public RedisConfiguration Redis { get; set; }
    }
}
