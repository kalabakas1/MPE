using Newtonsoft.Json;

namespace MPE.Pinger.Models.Results
{
    public class MetricResult : BaseResult
    {
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("Value")]
        public float Value { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}
