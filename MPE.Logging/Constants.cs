using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Logging
{
    internal class Constants
    {
        public const string MinimumLevel = "Logging.MinimumLevel";

        public const string Console_MinimumLevel = "Logging.Console.MinimumLevel";

        public const string File_FileName = "Logging.File.FileName";
        public const string File_MinimumLevel = "Logging.File.MinimumLevel";

        public const string Slack_Url = "Logging.Slack.Url";
        public const string Slack_MinimumLevel = "Logging.Slack.MinimumLevel";

        public const string Elastic_Url = "Logging.Elastic.Url";
        public const string Elastic_MinimumLevel = "Logging.Elastic.MinimumLevel";
        public const string Elastic_IndexFormat = "Logging.Elastic.IndexFormat";

        public const string Sentry_Dsn = "Logging.Sentry.Dsn";
    }
}
