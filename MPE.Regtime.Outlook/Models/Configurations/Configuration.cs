using System.Linq;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.Configurations
{
    internal class Configuration
    {
        [JsonProperty("RegtimeEndpoint")]
        public string RegtimeEndpoint { get; set; }
        [JsonProperty("DefaultFogbugz")]
        public string DefaultFogbugz { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("FbPassword")]
        public string FogbugzPassword { get; set; }
        [JsonProperty("Calendar")]
        public string Calendar { get; set; }
        [JsonProperty("TextMessageApiKey")]
        public string TextMessageApiKey { get; set; }
        [JsonProperty("Mobile")]
        public string Mobile { get; set; }
        [JsonProperty("Slack")]
        public Slack Slack { get; set; }
        [JsonProperty("Customers")]
        public Customer[] Customers { get; set; }

        public Customer GetByAlias(string customerAlias)
        {
            return Customers.FirstOrDefault(x => x.Alias.ToUpper() == customerAlias?.ToUpper());
        }
    }
}
