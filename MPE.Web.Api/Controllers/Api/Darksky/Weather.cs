using Newtonsoft.Json;

namespace MPE.Web.Api.Controllers.Api.Models
{
    public class Weather
    {
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("currently")]
        public Currently Currently { get; set; }
    }
}