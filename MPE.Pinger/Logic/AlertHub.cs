using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging;

namespace MPE.Pinger.Logic
{
    public class AlertHub
    {
        private static object _lock = new object();
        private static HashSet<string> _alerts;
        public AlertHub()
        {
            lock (_lock)
            {
                if (_alerts == null)
                {
                    _alerts = new HashSet<string>();
                }                
            }
        }

        public void Alert(string alias)
        {
            lock (_lock)
            {
                if (!_alerts.Contains(alias))
                {
                    _alerts.Add(alias);
                    LoggerFactory.Instance.Fatal(alias, new Exception($"Alert: {alias}"));
                }
            }
        }

        public void Abort(string alias)
        {
            lock (_lock)
            {
                if (_alerts.Contains(alias))
                {
                    _alerts.Remove(alias);
                    LoggerFactory.Instance.Fatal(alias, new Exception($"Abort: {alias}"));
                }
            }
        }
    }
}
