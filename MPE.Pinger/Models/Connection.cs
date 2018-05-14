using Newtonsoft.Json;

namespace MPE.Pinger.Models
{
    internal class Connection
    {
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("Target")]
        public string Target { get; set; }
        [JsonProperty("Port")]
        public int Port { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Response")]
        public int[] Response { get; set; }
    }
}