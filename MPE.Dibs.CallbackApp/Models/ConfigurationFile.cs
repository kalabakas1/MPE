using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MPE.Dibs.CallbackApp.Models
{
    internal class ConfigurationFile
    {
        [JsonProperty("Merchant")]
        public string Merchant { get; set; }
        [JsonProperty("ApiUser")]
        public string ApiUser { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("CallbackUrl")]
        public string CallbackUrl { get; set; }
        [JsonProperty("HMAC")]
        public string Hmac { get; set; }
        [JsonProperty("TransactionFilePath")]
        public string TransctionFilePath { get; set; }
    }
}
