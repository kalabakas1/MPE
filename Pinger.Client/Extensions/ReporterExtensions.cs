using System;
using System.Collections.Generic;
using System.Linq;
using Pinger.Client.Models;

namespace Pinger.Client.Extensions
{
    public static class ReporterExtensions
    {
        public static void Enqueue(this Reporter reporter, string providerName, string endpoint, long value, string responseCode)
        {
            var builder = new MetricBuilder();

            var itemName = endpoint;
            if (!string.IsNullOrEmpty(providerName))
            {
                itemName = $"{providerName}.{endpoint}";
            }

            var metric = builder.WithTimestamp()
                .WithValue(value)
                .WithPath(GeneratePath(itemName))
                .WithAlias(itemName)
                .WithMessage(responseCode).Build();

            reporter.Enqueue(metric);
        }

        public static void Enqueue(this Reporter reporter, Metric metric)
        {
            reporter.Enqueue(metric);
        }

        public static void Enqueue(this Reporter reporter, string alias, Dictionary<string, object> data = null)
        {
            var builder = new MetricBuilder();
            builder.WithTimestamp()
                .WithAlias(alias)
                .WithPath(GeneratePath(alias));

            if (data != null && data.Any())
            {
                foreach (var pair in data)
                {
                    builder.WithData(pair.Key, pair.Value);
                }
            }

            reporter.Enqueue(builder.Build());
        }

        public static MetricBuilder GetBuilder(this Reporter reporter, string alias)
        {
            var builder = new MetricBuilder();
            builder.WithTimestamp()
                .WithPath(GeneratePath(alias))
                .WithAlias(alias);

            return builder;
        }

        public static void Enqueue(this Reporter reporter, MetricBuilder builder)
        {
            reporter.Enqueue(builder.Build());
        }

        private static string GeneratePath(string alias)
        {
            return
                $"{Constants.ProjectAppSettingName.GetSetting<string>()}.{Constants.EnvironmentAppSettingName.GetSetting<string>().RemoveSpecialCharacters()}.{alias}";
        }
    }
}
