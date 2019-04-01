using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinger.Client.Models;

namespace Pinger.Client
{
    public class MetricBuilder
    {
        private Metric _metric;
        public MetricBuilder()
        {
            _metric = new Metric();
        }

        public Metric Build()
        {
            return _metric;
        }

        public MetricBuilder WithTimestamp()
        {
            _metric.Timestamp = DateTime.Now;
            return this;
        }

        public MetricBuilder WithValue(long value)
        {
            _metric.Value = value;
            return this;
        }

        public MetricBuilder WithPath(string path)
        {
            _metric.Path = path;
            return this;
        }

        public MetricBuilder WithAlias(string alias)
        {
            _metric.Alias = alias;
            return this;
        }

        public MetricBuilder WithMessage(string message)
        {
            _metric.Message = message;
            return this;
        }

        public MetricBuilder WithData(string key, string value)
        {
            _metric.AddData(key, value);
            return this;
        }
    }
}
