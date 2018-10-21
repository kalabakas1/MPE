using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Pinger.Models.Configurations
{
    public class Healing
    {
        [JsonProperty(PropertyName = "Script")]
        public string Script { get; set; }
    }
}
