using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Repositories;

namespace MPE.Pinger.Logic
{
    internal class TimedElasticSearchRetentionPolicy : IProcess
    {
        private int _retentionInDays;
        private Timer _timer;
        private ElasticRestMetricRepository _elasticRestMetricRepository;
        public TimedElasticSearchRetentionPolicy()
        {
            _elasticRestMetricRepository = new ElasticRestMetricRepository();
            _retentionInDays = Configuration.Get<int>(Constants.RetentionInDays);
            _timer = new Timer(60 * 60 * 1000);
            _timer.Elapsed += (sender, args) =>
                _elasticRestMetricRepository.DeleteIndex(DateTime.Now.AddDays(_retentionInDays * -1));
        }

        public void Start()
        {
            _elasticRestMetricRepository.DeleteIndex(DateTime.Now.AddDays(_retentionInDays * -1));
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
