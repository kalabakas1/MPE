using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Pinger.Models.Configurations
{
    public class SqlConfiguration
    {
        [JsonProperty("ConnectionStrings")]
        public Dictionary<string, string> ConnectionStrings { get; set; }
        [JsonProperty("Queries")]
        public List<SqlQuery> SqlQueries { get; set; }
    }
}
