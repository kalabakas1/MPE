using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{
    public class Case
    {
        [JsonProperty("ixBug")]
        public int IxBug { get; set; }
        [JsonProperty("operations")]
        public string[] Operations { get; set; }
        [JsonProperty("sProject")]
        public string Project { get; set; }

        [JsonProperty("sTitle")]
        public string Title { get; set; }
        [JsonProperty("hrsCurrEst")]
        public decimal CurrentEstimate { get; set; }
        [JsonProperty("hrsOrigEst")]
        public decimal OriginalEstimate { get; set; }
        [JsonProperty("hrsElapsed")]
        public decimal HoursElapsed { get; set; }
    }
}