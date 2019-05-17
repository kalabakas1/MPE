using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinger.Client.Extensions;

namespace Pinger.Client.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //Reporter.Instance.Enqueue("DataProviderFooBar", "GetData", 1234, "OK");

            //Reporter.Instance.Enqueue("GateProvider.UpdateTokens", new Dictionary<string, string>
            //{
            //    {"NFC",  "046FCAF2993780"},
            //    {"EAST", "True" },
            //    {"WEST", "False" }
            //});

            var builder = Reporter.Instance.GetBuilder("GateProvider.UpdateEntries");
            builder.WithData("Gate", 1000);
            Reporter.Instance.Enqueue(builder);

            Console.ReadLine();
        }
    }
}
