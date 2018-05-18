using System;
using Newtonsoft.Json;

namespace MPE.Pinger.Models.Results
{
    public class EventLogResult : BaseResult 
    {
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Source")]
        public string Source { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
        [JsonProperty("Machine")]
        public string Machine { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
    }
}
