using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Regtime.Outlook.App.Models;

namespace MPE.Regtime.Outlook.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting execution...");
            foreach (var s in args ?? new string[0])
            {
                Console.WriteLine(s);
            }

            var options = new ConsoleOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var regtimeConsole = new RegtimeConsole(options.ConfigurationFilePath);
                regtimeConsole.Run(options.Command, options.Date);
            }
        }
    }
}
