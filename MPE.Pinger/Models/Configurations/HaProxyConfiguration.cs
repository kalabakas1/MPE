using Newtonsoft.Json;

namespace MPE.Pinger.Models.Configurations
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
