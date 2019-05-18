using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Helpers;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Serilog;

namespace MPE.Pinger.Logic
{
    internal class TestConductor
    {
        private List<Task<MetricResult>> _tasks;
        private readonly IEnumerable<ITester> _testers;
        private readonly AlertHub _alertHub;
        private readonly HealingExecutor _healingExecutor;

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
            IEnumerable<ITester> testers,
            AlertHub alertHub,
            HealingExecutor healingExecutor)
        {
            _testers = testers;
            _alertHub = alertHub;
            _healingExecutor = healingExecutor;
            _tasks = new List<Task<MetricResult>>();
        }

        public List<MetricResult> Run()
        {
            LoggerFactory.Instance.Debug("Starting...");

            var results = new List<MetricResult>();

            var configuration = ConfigurationService.Instance.ReadConfigurationFile();
            foreach (var connection in configuration.Connections)
            {
                var task = Task<MetricResult>.Factory.StartNew(() =>
                {
                    var result = new MetricResult
                    {
                        Path = $"{configuration.Host}.Test.{connection.Type}.{connection.Alias}",
                        Timestamp = DateTime.Now,
                        Alias = $"{connection.Type}.{connection.Alias}",
                        Message = "Succeeded",
                        Value = 1
                    };

                    var tester = _testers.Where(x => x.CanTest(connection))
                        .Select(x => (ITester)Activator.CreateInstance(x.GetType())).FirstOrDefault();

                    if (tester == null)
                    {
                        LoggerFactory.Instance.Debug($"No testers found for {connection.Alias}");
                        return result;
                    }

                    try
                    {
                        LoggerFactory.Instance.Debug($"Test - Start: {result.Path}");
                        RetryPolicy.Execute(() => tester.Test(connection));
                        _alertHub.Abort(result.Path);
                        LoggerFactory.Instance.Debug($"Test - End: {result.Path}");
                    }
                    catch (Exception e)
                    {

                        try
                        {
                            if (_healingExecutor.CanHeal(connection))
                            {
                                _healingExecutor.Heal(connection);

                                Thread.Sleep(30000);

                                tester.Test(connection);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            var message = $"Pinger failed for: {connection.Alias}";
                            LoggerFactory.Instance.Debug(e, message);
                            result.Message = "Failed";
                            result.Value = 0;

                            if (!_alertHub.IsAlerting(result.Path))
                            {
                                _alertHub.Alert(result.Path);
                            }
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
            return int.Parse(ConfigurationService.Instance.Get<string>(string.Format(Constants.FailedPauseTemplate, failNumber)));
        }
    }
}