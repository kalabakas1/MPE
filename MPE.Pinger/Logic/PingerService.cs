﻿using System;
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
    internal class PingerService
    {
        private List<Task> _tasks;
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

        public PingerService(
            IEnumerable<IConnectionTester> testers)
        {
            _testers = testers;
            _tasks = new List<Task>();
        }

        public void Run()
        {
            _logger.Debug("Starting...");

            var connections = ReadFromFile();
            foreach (var connection in connections)
            {
                var task = Task.Run(() =>
                {
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
                        }
                    }
                });

                _tasks.Add(task);
            }

            Task.WaitAll(_tasks.ToArray());

            _logger.Debug("Done...");
        }

        private List<Connection> ReadFromFile()
        {
            var fileData = File.ReadAllText(Configuration.Get<string>("MPE.Pinger.Configuration.Path"));
            return JsonConvert.DeserializeObject<List<Connection>>(fileData);
        }

        private int GetFailWaitInSec(int failNumber)
        {
            return int.Parse(Configuration.Get<string>($"MPE.Pinger.Fail{failNumber}.Pause.Secs"));
        }
    }
}