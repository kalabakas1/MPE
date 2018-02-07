using System.Linq;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.Configurations
{
    internal class Configuration
    {
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
        [JsonProperty("Customers")]
        public Customer[] Customers { get; set; }

        public Customer GetByAlias(string customerAlias)
        {
            return Customers.FirstOrDefault(x => x.Alias.ToUpper() == customerAlias?.ToUpper());
        }
    }
}
