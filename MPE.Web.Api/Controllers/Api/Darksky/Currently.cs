using MPE.Api.Attributes;
using Newtonsoft.Json;

namespace MPE.Web.Api.Controllers.Api.Darksky
{
    [RestrictSerialization]
    public class Currently
    {
        [JsonProperty("time")]
        public int Time { get; set; }
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("nearestStormDistance")]
        public int NearestStormDistance { get; set; }
        [JsonProperty("precipIntensity")]
        public float PrecipIntensity { get; set; }
        [JsonProperty("precipIntensityError")]
        public float PrecipIntensityError { get; set; }
        [JsonProperty("precipProbability")]
        public float PrecipProbability { get; set; }
        [JsonProperty("precipType")]
        public string PrecipType { get; set; }
        [JsonProperty("temperature")]
        public float Temperature { get; set; }
        [JsonProperty("apparentTemperature")]
        public float ApparentTemperature { get; set; }
        [JsonProperty("dewPoint")]
        public float DewPoint { get; set; }
        [JsonProperty("humidity")]
        public float Humidity { get; set; }
        [JsonProperty("pressure")]
        public float Pressure { get; set; }
        [JsonProperty("windSpeed")]
        public float WindSpeed { get; set; }
        [JsonProperty("windGust")]
        public float WindGust { get; set; }
        [JsonProperty("windBearing")]
        public int WindBearing { get; set; }
        [JsonProperty("cloudCover")]
        public float CloudCover { get; set; }
        [JsonProperty("uvIndex")]
        public int UvIndex { get; set; }
        [JsonProperty("visibility")]
        public float Visibility { get; set; }
        [JsonProperty("ozone")]
        public float Ozone { get; set; }
    }
}