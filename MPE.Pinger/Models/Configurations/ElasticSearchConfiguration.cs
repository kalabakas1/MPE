using Newtonsoft.Json;

namespace MPE.Pinger.Models.Configurations
{
    public class ElasticSearchConfiguration
    {
        [JsonProperty("Host")]
        public string Host { get; set; }
        [JsonProperty("Port")]
        public int Port { get; set; }
        [JsonProperty("Fields")]
        public string[] Fields { get; set; }
    }
}
