using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{
    internal class SearchRequest : CmdRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("q")]
        public string Query { get; set; }
        [JsonProperty("max")]
        public int Max { get; set; }
        [JsonProperty("cols")]
        public string[] Columns { get; set; }
    }
}
