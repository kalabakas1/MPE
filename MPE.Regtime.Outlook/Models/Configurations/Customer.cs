using System.Linq;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.Configurations
{
    public class Customer
    {
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Projects")]
        public Project[] Projects { get; set; }
        [JsonProperty("Fogbugz")]
        public string Fogbugz { get; set; }
        public Project GetProject(string alias)
        {
            return Projects.FirstOrDefault(x => x.Alias.ToUpper() == alias?.ToUpper());
        }
    }
}