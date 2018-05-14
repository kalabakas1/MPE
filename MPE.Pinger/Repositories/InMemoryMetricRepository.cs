using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;

namespace MPE.Pinger.Repositories
{
    internal class InMemoryMetricRepository : IMetricRepository
    {
        private static ConcurrentQueue<MetricResult> _dataStore = new ConcurrentQueue<MetricResult>();

        public void Write(List<MetricResult> results)
        {
            if (results == null || !results.Any())
            {
                return;
            }

            foreach (var metricResult in results)
            {
                Write(metricResult);
            }
        }

        public void Write(MetricResult result)
        {
            if (result == null)
            {
                return;
            }

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
