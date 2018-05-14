using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPE.Pinger.Models
{
    public class MetricResult
    {
        [JsonProperty("Timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("Path")]
        public string Path { get; set; }
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("Value")]
        public float Value { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}
