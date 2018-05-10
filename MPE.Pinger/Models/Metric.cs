using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPE.Pinger.Models
{
    internal class Metric
    {
        public Metric()
        {
            Counters = new List<PerformanceCounter>();
        }

        public string Alias { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Instance { get; set; }
        [JsonIgnore]
        public List<PerformanceCounter> Counters { get; set; }
    }
}
