using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Pinger.Models
{
    internal class ConfigurationFile
    {
        public string Host { get; set; }
        public List<Connection> Connections { get; set; }
        public List<Metric> Metrics { get; set; }
    }
}
