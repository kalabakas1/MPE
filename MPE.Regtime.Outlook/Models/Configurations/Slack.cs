using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Regtime.Outlook.App.Models.Configurations
{
    public class Slack
    {
        [JsonProperty("Token")]
        public string Token { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Channel")]
        public string Channel { get; set; }
        [JsonProperty("Account")]
        public string Account { get; set; }
    }
}
