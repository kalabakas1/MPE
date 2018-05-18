using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;

namespace MPE.Pinger.Repositories
{
    internal class InMemoryRepository<T> : IRepository<T>
    {
        private static ConcurrentQueue<T> _dataStore = new ConcurrentQueue<T>();

        public void Write(List<T> results)
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

        public void Write(T result)
        {
            if (result == null)
            {
                return;
            }

            _dataStore.Enqueue(result);
        }

        public T Pop()
        {
            T metric;
            if (_dataStore.TryDequeue(out metric))
            {
                return metric;
            }
            
            throw new Exception("Not possible to pop");
        }
    }
}
