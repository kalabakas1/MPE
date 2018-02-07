using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{

    public class SearchResponse : Response
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
