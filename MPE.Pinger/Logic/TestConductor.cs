using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;
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
            LoggerFactory.Instance.Debug("Starting...");

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
                            LoggerFactory.Instance.Debug($"Test - Start :{result.Path}");
                            RetryPolicy.Execute(() => tester.Test(connection));
                            LoggerFactory.Instance.Debug($"Test - End :{result.Path}");
                        }
                        catch (Exception e)
                        {
                            LoggerFactory.Instance.Fatal(e, $"Pinger failed for: {connection.Alias}");
                            result.Message = "Failed";
                        }
                    }

                    return result;
                });

                _tasks.Add(task);
            }

            Task.WaitAll(_tasks.ToArray());

            results.AddRange(_tasks.Select(x => x.Result));

            LoggerFactory.Instance.Debug("Done...");

            return results;
        }

        private int GetFailWaitInSec(int failNumber)
        {
            return int.Parse(Configuration.Get<string>(string.Format(Constants.FailedPauseTemplate, failNumber)));
        }
    }
}