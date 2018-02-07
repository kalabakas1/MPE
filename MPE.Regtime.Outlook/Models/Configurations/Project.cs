using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.Configurations
{
    public class Project
    {
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}