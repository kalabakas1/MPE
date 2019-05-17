using System.Collections.Concurrent;

namespace Pinger.Client.Persistance
{
    internal class FixedConcurrentQueue<T> : ConcurrentQueue<T>
    {
        private static object Lock = new object();

        private int _maxItemx;
        public FixedConcurrentQueue(int maxItems)
        {
            _maxItemx = maxItems;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (Lock)
            {
                T item;
                while (Count > _maxItemx && TryDequeue(out item)) ;
            }
        }
    }
}
