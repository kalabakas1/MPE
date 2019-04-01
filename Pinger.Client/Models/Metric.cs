using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pinger.Client.Models
{
    public class Metric
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
        [JsonProperty("Data")]
        public Dictionary<string, string> Data { get; set; }

        public void AddData(string key, object value)
        {
            if (Data.ContainsKey(key))
            {
                Data[key] = value?.ToString();
            }
            else
            {
                Data.Add(key, value?.ToString());
            }
        }
    }
}
