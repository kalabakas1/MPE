using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Pinger.Client.Extensions;
using Pinger.Client.Models;
using Pinger.Client.Persistance;

namespace Pinger.Client
{
    public class Reporter
    {
        private static Reporter _instance;
        private static object _lock = new object();

        private static Timer _timer;

        private static FixedConcurrentQueue<Metric> _metricStore;
        private static RestPersistanceService _restService;

        private const int DefaultTimerIntevalSec = 15;
        private const int DefaultMaxLocalItems = 1000000;
        private const int BulkSize = 256;
        
        private Reporter()
        {
            _metricStore = new FixedConcurrentQueue<Metric>(DefaultMaxLocalItems);

            _timer = new Timer(DefaultTimerIntevalSec * 1000);
            _timer.Elapsed += (sender, args) =>
            {
                FlushStore(_metricStore);
                _timer.Start();
            };
            _timer.AutoReset = false;
            _timer.Start();

            _restService = new RestPersistanceService(
                Constants.ApiKeyAppSettingName.GetSetting<string>(),
                Constants.Host);
        }

        public static Reporter Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Reporter();
                    }

                    return _instance;
                }
            }
        }

        private bool ValidateConfiguration()
        {
            return !string.IsNullOrEmpty(Constants.ApiKeyAppSettingName.GetSetting<string>())
                && !string.IsNullOrEmpty(Constants.EnvironmentAppSettingName.GetSetting<string>())
                && !string.IsNullOrEmpty(Constants.ProjectAppSettingName.GetSetting<string>());
        }

        public void Enqueue(Metric metric)
        {
            if (ValidateConfiguration())
            {
                _metricStore.Enqueue(metric);
            }
        }

        public bool IsStoreEmpty()
        {
            return _metricStore.IsEmpty;
        }

        private void FlushStore<T>(FixedConcurrentQueue<T> store)
        {
            var run = true;
            var records = new List<T>();

            var count = 0;
            while (run)
            {
                while (run && count < BulkSize)
                {
                    try
                    {
                        T item;
                        if (store.TryDequeue(out item))
                        {
                            records.Add(item);
                        }
                        else if (store.Count == 0)
                        {
                            run = false;
                        }
                    }
                    catch
                    {
                        run = false;
                    }
                    count++;
                }

                try
                {
                    if (records.Any())
                    {
                        _restService.Persist(records);
                    }
                    else
                    {
                        run = false;
                    }
                }
                catch
                {
                    records.ForEach(store.Enqueue);
                    run = false;
                }

                records = new List<T>();

                count = 0;
            }
        }
    }
}
