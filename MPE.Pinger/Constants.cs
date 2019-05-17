using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Pinger
{
    internal class Constants
    {
        public const string ConfigurationPath = "MPE.Pinger.Configuration.Path";
        public const string FailedPauseTemplate = "MPE.Pinger.Fail{0}.Pause.Secs";
        public const string MetricIntevalSec = "MPE.Pinger.Metric.Inteval.Sec";
        public const string ReportIntevalSec = "MPE.Pinger.Report.Inteval.Sec";
        public const string WaitBetweenTestsSec = "MPE.Pinger.WaitBetweenTest.Secs";
        public const string TestEnableFromTime = "MPE.Pinger.TimeSpan.From";
        public const string TestEnableToTime = "MPE.Pinger.TimeSpan.To";
        public const string ApiKeysPath = "MPE.Pinger.ApiKeys.Path";
        public const string ConfigurationFolder = "MPE.Pinger.ConfigurationFolder";
        public const string ServerPort = "MPE.Pinger.Server.Port";
        public const string ServerHost = "MPE.Pinger.Server.Host";
        public const string RetentionInDays = "MPE.Pinger.RetentionInDays";
        public const string SlackToken = "MPE.Pinger.SlackToken";
        public const string ClientHost = "MPE.Pinger.Client.Host";
        public const string ClientApiKey = "MPE.Pinger.Client.ApiKey";
        public const string RestEndpoint = "MPE.Pinger.RestEndpoint";

        public const string AuthenticationHeaderName = "Authorization";
    }
}
