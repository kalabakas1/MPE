using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Pinger.Models.Results
{
    public class RequestLogResult : BaseResult
    {
        [JsonProperty("Ip")]
        public string Ip { get; set; }
        [JsonProperty("Method")]
        public string Method { get; set; }
        [JsonProperty("Uri")]
        public string Uri { get; set; }
        [JsonProperty("Query")]
        public string Query { get; set; }
        [JsonProperty("QueryApiKey")]
        public string QueryApiKey { get; set; }
        [JsonProperty("QueryId")]
        public string QueryId { get; set; }
    }
}
