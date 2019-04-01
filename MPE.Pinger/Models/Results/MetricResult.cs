using System.Collections.Generic;
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
