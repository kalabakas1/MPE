﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace MPE.Pinger.Models.Configurations
{
    public class ConfigurationFile
    {
        [JsonProperty("Host")]
        public string Host { get; set; }
        [JsonProperty("RestEndpoint")]
        public string RestEndpoint { get; set; }
        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }
        [JsonProperty("EventLogging")]
        public EventLogConfiguration EventLogging { get; set; }
        [JsonProperty("Connections")]
        public List<Connection> Connections { get; set; }
        [JsonProperty("Metrics")]
        public List<Metric> Metrics { get; set; }
        [JsonProperty("Redis")]
        public RedisConfiguration Redis { get; set; }
        [JsonProperty("RabbitMQ")]
        public RabbitMqConfiguration RabbitMq { get; set; }
        [JsonProperty("ElasticSearch")]
        public ElasticSearchConfiguration ElasticSearch { get; set; }
        [JsonProperty("HaProxy")]
        public HaProxyConfiguration HaProxy { get; set; }
        [JsonProperty("Sql")]
        public SqlConfiguration SqlConfiguration { get; set; }
    }
}
