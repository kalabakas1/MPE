using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{
    public class Clientversionallowed
    {
        [JsonProperty("min")]
        [XmlElement("min")]
        public int Min { get; set; }
        [JsonProperty("max")]
        [XmlElement("max")]
        public int Max { get; set; }
    }
}