using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Newtonsoft.Json;

namespace MPE
{
    class Program
    {
        static void Main(string[] args)
        {

            //var names = PerformanceCounterCategory
            //    .GetCategories()
            //    .Select(cat =>
            //        cat.GetInstanceNames().Any()
            //            ? cat.GetInstanceNames().Select(i => cat.GetCounters(i)).SelectMany(counter => counter)
            //            : cat.GetCounters("")).SelectMany(counter => counter)
            //    .Select(counter => string.Format("{0} : {1}.{2}", counter.InstanceName, counter.CategoryName,
            //        counter.CounterName));

            //foreach (var name in names)
            //{
            //    Console.WriteLine(name);
            //}

            //Console.ReadLine();


            //PerformanceCounter memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            //PerformanceCounter processorCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            //var timer = new Timer(5000);
            //timer.Elapsed += (sender, eventArgs) =>
            //{
            //    var mem = memoryCounter.NextValue();
            //    Console.WriteLine(mem + "% RAM");

            //    var pro = processorCounter.NextValue();
            //    Console.WriteLine(pro + "% CPU");

            //    Console.WriteLine();
            //};

            //timer.Start();

            CallbackTester.Run();



            Console.ReadLine();
        }
    }
}
    