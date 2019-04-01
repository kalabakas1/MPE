using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pinger.Client.Models
{
    [Description("MetricResult")]
    public class Metric
    {
        public Metric()
        {
            Data = new Dictionary<string, object>();
        }

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
        [JsonProperty("Data")]
        public Dictionary<string, object> Data { get; set; }

        public void AddData(string key, object value)
        {
            if (Data.ContainsKey(key))
            {
                Data[key] = value;
            }
            else
            {
                Data.Add(key, value);
            }
        }
    }
}
