using System.Collections.Generic;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{
    public class Data
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("totalHits")]
        public int TotalHits { get; set; }
        [JsonProperty("cases")]
        public List<Case> Cases { get; set; }
    }
}