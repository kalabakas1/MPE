using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Serilog;
using Configuration = MPE.Pinger.Helpers.Configuration;

namespace MPE.Pinger.Logic
{
    internal class TestConductor
    {
        private List<Task<MetricResult>> _tasks;
        private readonly IEnumerable<IConnectionTester> _testers;
        private ILogger _logger = new LoggerFactory().Generate();

        private RetryPolicy RetryPolicy =>
            Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    new[]
                    {
                        TimeSpan.FromSeconds(GetFailWaitInSec(1)),
                        TimeSpan.FromSeconds(GetFailWaitInSec(2)),
                        TimeSpan.FromSeconds(GetFailWaitInSec(3)),
                    });

        public TestConductor(
            IEnumerable<IConnectionTester> testers)
        {
            _testers = testers;
            _tasks = new List<Task<MetricResult>>();
        }

        public List<MetricResult> Run()
        {
            _logger.Debug("Starting...");

            var results = new List<MetricResult>();

            var configuration = Configuration.ReadConfigurationFile();
            foreach (var connection in configuration.Connections)
            {
                var task = Task<MetricResult>.Factory.StartNew(() =>
                {
                    var result = new MetricResult
                    {
                        Path = $"{configuration.Host}.Test.{connection.Type}.{connection.Alias}",
                        Timestamp = DateTime.Now,
                        Alias = $"{connection.Type}.{connection.Alias}" ,
                        Message = "Succeeded"
                    };

                    var testers = _testers.Where(x => x.CanTest(connection))
                        .Select(x => (IConnectionTester)Activator.CreateInstance(x.GetType())).ToList();

                    foreach (var tester in testers)
                    {
                        try
                        {
                            RetryPolicy.Execute(() => tester.Test(connection));
                        }
                        catch (Exception e)
                        {
                            _logger.Fatal(e, $"Pinger failed for: {connection.Alias}");
                            result.Message = "Failed";
                        }
                    }

                    return result;
                });

                _tasks.Add(task);
            }

            Task.WaitAll(_tasks.ToArray());

            results.AddRange(_tasks.Select(x => x.Result));

            _logger.Debug("Done...");

            return results;
        }

        private int GetFailWaitInSec(int failNumber)
        {
            return int.Parse(Configuration.Get<string>(string.Format(Constants.FailedPauseTemplate, failNumber)));
        }
    }
}