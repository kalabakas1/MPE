using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Pinger.Models
{
    internal class MetricResult
    {
        public MetricResult()
        {
            Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; set; }
        public string Path { get; set; }
        public string Alias { get; set; }
        public float Value { get; set; }
        public string Message { get; set; }
    }
}
