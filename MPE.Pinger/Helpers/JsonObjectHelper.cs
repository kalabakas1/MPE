using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MPE.Pinger.Helpers
{
    internal class JsonObjectHelper
    {
        public static Dictionary<string, string> FlattenObject(object obj)
        {
            return FlattenObject(JsonConvert.SerializeObject(obj));
        }

        public static Dictionary<string, string> FlattenObject(string json)
        {
            var jsonObject = JObject.Parse(json);
            var jTokens = jsonObject.Descendants().Where(p => p.Count() == 0);
            var results = jTokens.Aggregate(new Dictionary<string, string>(), (properties, jToken) =>
            {
                properties.Add(jToken.Path, jToken.ToString());
                return properties;
            });

            return results;
        }
    }
}
