using System;
using CommandLine;

namespace MPE.Regtime.Outlook.App.Models
{
    internal class ConsoleOptions
    {
        [Option(DefaultValue = "Regtime.json", HelpText = "Path to configuration file")]
        public string ConfigurationFilePath { get; set; }
        [Option(HelpText = "Command to be executed - can be validate, register, single, config or clear")]
        public string Command { get; set; }
        [Option(HelpText = "Date used for synchronization and validation")]
        public DateTime Date { get; set; }
    }
}
