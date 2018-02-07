using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{
    public abstract class Response
    {
        [JsonProperty("errors")]
        public object[] Errors { get; set; }
        [JsonProperty("warnings")]
        public object[] Warnings { get; set; }
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
        [JsonProperty("errorCodes")]
        public object ErrorCode { get; set; }
        [JsonProperty("maxCacheAge")]
        public object MaxCacheAge { get; set; }
    }
}
