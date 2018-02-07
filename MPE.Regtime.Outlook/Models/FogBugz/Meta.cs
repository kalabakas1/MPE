using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MPE.Regtime.Outlook.App.Models.FogBugz
{
    public class Meta
    {
        [JsonProperty("jsdbInvalidator")]
        [XmlElement("jsdbInvalidator")]
        public string JsdbInvalidator { get; set; }
        [JsonProperty("clientVersionAllowed")]
        [XmlElement("clientVersionAllowed")]
        public Clientversionallowed ClientVersionAllowed { get; set; }
    }
}
