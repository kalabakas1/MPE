using Newtonsoft.Json;

namespace MPE.Pinger.Models.Configurations
{
    public class Connection
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
        [JsonProperty(PropertyName = "Contains")]
        public string Contains { get; set; }
        [JsonProperty("DaysLeft")]
        public int DaysLeft { get; set; }
        [JsonProperty(PropertyName = "Healing")]
        public Healing Healing { get; set; }
        [JsonProperty(PropertyName = "Script")]
        public string Script { get; set; }
    }
}