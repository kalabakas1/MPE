using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging.Repository;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MPE.Logging
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurator = new LoggerConfigurator();

            var logger = configurator.Generate();

            //for (int i = 0; i < 10; i++)
            //{
            //   logger.Error(new Exception(Guid.NewGuid().ToString()), "An error was thrown.");
            //}

            logger.Error(new Exception(Guid.NewGuid().ToString()), "Test error");

            Console.ReadLine();
        }
    }
}
