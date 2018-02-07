using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{
    public class LogonTokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}