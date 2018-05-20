using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Pinger.Models.Configurations
{
    public class EventLogConfiguration
    {
        [JsonProperty("MinimumLevel")]
        public string MinimumLevel { get; set; }
        [JsonProperty("Categories")]
        public string[] Categories { get; set; }
    }
}
