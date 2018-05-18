using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;

namespace MPE.Pinger.Helpers
{
    internal class MetricResultHelper
    {
        public static List<MetricResult> Generate(string prefix, string[] fields, Dictionary<string, string> data)
        {
            var result = new List<MetricResult>();
            foreach (var rabbitMqField in fields)
            {
                var pairs = data.Where(x => Regex.IsMatch(x.Key, rabbitMqField));
                result.AddRange(pairs.Select(x =>
                {
                    var metric = new MetricResult
                    {
                        Path = $"{prefix}.{x.Key}".Replace("-", string.Empty).Replace("_", string.Empty),
                        Alias = x.Key,
                        Timestamp = DateTime.Now
                    };

                    float value = 0;
                    var strVal = x.Value;
                    if (float.TryParse(strVal, out value))
                    {
                        metric.Value = value;
                    }

                    return metric;
                }));
            }

            return result;
        }
    }
}
