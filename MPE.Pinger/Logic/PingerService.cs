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

namespace MPE.Pinger.Logic
{
    internal class PingerService
    {
        private List<Task> _tasks;
        private readonly IEnumerable<IConnectionTester> _testers;
        private ILogger _logger = new LoggerConfigurator().Generate();

        private RetryPolicy RetryPolicy =>
            Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    new[]
                    {
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(60),
                        TimeSpan.FromSeconds(180),
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
            var fileData = File.ReadAllText(ConfigurationManager.AppSettings["MPE.Pinger.Configuration.Path"]);
            return JsonConvert.DeserializeObject<List<Connection>>(fileData);
        }
    }
}