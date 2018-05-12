using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Pinger.Models
{
    internal class RedisConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string[] Metrics { get; set; }
    }
}
