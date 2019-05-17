using Newtonsoft.Json;

namespace MPE.Pinger.Models.Configurations
{
    public class RedisConfiguration
    {
        [JsonProperty("Host")]
        public string Host { get; set; }
        [JsonProperty("Port")]
        public int Port { get; set; }
        [JsonProperty("Metrics")]
        public string[] Metrics { get; set; }
    }
}
