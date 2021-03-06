﻿using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MPE.Pinger.Models.Configurations
{
    public class Metric
    {
        public Metric()
        {
            Counters = new List<PerformanceCounter>();
        }

        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("Category")]
        public string Category { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Instance")]
        public string Instance { get; set; }
        [JsonIgnore]
        public List<PerformanceCounter> Counters { get; set; }
    }
}
