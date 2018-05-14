using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;

namespace MPE.Pinger.Repositories
{
    internal class MetricRepository : IMetricRepository
    {
        private static ConcurrentQueue<MetricResult> _dataStore = new ConcurrentQueue<MetricResult>();

        public void Write(List<MetricResult> results)
        {
            foreach (var metricResult in results)
            {
                Write(metricResult);
            }
        }

        public void Write(MetricResult result)
        {
            _dataStore.Enqueue(result);
        }

        public MetricResult Pop()
        {
            MetricResult metric;
            if (_dataStore.TryDequeue(out metric))
            {
                return metric;
            }
            
            throw new Exception("Not possible to pop");
        }
    }
}
