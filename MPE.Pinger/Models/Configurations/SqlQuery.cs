using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Pinger.Models.Configurations
{
    public class SqlQuery
    {
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("ConnectionString")]
        public string ConnectionString { get; set; }
        [JsonProperty("FilePath")]
        public string FilePath { get; set; }
    }
}
