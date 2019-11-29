using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using RestSharp;

namespace MPE.Sitemap.Warmup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Mode:");
            var mode = Console.ReadLine();

            switch (mode)
            {
                case "sitemap":
                    Sitemap();
                    break;

                case "spam":
                    Spam();
                    break;
            }

            Console.ReadLine();
        }

        private static void Spam()
        {
            Console.WriteLine("Endpoint:");
            var endpoint = Console.ReadLine();

            Console.WriteLine("Requests:");
            var requests = int.Parse(Console.ReadLine());

            Parallel.For(0, requests,new ParallelOptions { MaxDegreeOfParallelism = 1}, i =>
            {
                var client = new HttpClient();
                var sw = Stopwatch.StartNew();
                var response = client.GetAsync(endpoint).Result;
                sw.Stop();
                Console.WriteLine($"{response.StatusCode} {string.Empty.PadRight((int)sw.ElapsedMilliseconds / 100, '+')}");
                client.Dispose();
            });
        }

        private static void Sitemap()
        {
            Console.WriteLine("Please supply a csv domain list: ");
            var domains = Console.ReadLine().Split(',');

            var threads = 0;
            Console.WriteLine("Define number of threads: ");
            threads = int.Parse(Console.ReadLine());

            var containsWord = string.Empty;
            Console.WriteLine("Paths contains:");
            containsWord = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Sleep total:");
            var sleep = int.Parse(Console.ReadLine());

            Console.WriteLine($"Started at {DateTime.Now}");

            var entries = new List<SitemapEntry>();
            foreach (var domain in domains)
            {
                var sitemapUrl = $"{domain}/sitemap.xml";
                entries.AddRange(ReadSitemap(sitemapUrl).Where(x => x.Url.Contains(containsWord)).ToList());
            }

            Console.WriteLine($"Total entries: {entries.Count}");

            var tasks = new Task[threads];
            var entriesPerThread = entries.Count / threads;
            for (int i = 0; i < threads; i++)
            {
                var taskEntries = entries.Skip(i * entriesPerThread).Take(entriesPerThread).ToList();
                tasks[i] = ExecuteDriver(taskEntries.Select(x => x.Url).ToList(), sleep);
            }

            Task.WaitAll(tasks);

            Console.WriteLine($"Ended at {DateTime.Now}");
        }

        private static Task ExecuteDriver(List<string> entries, int sleep)
        {
            return Task.Factory.StartNew(() =>
            {
                var count = 0;
                ChromeDriver driver = null;
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("--headless", "--no-sandbox", "--disable-web-security", "--disable-gpu", "--incognito", "--log-level=OFF", "--hide-scrollbars");
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                foreach (var entry in entries)
                {
                    if (count % 10 == 0 || driver == null)
                    {
                        if (driver != null)
                        {
                            driver.Dispose();
                        }

                        driver = new ChromeDriver(service, chromeOptions);
                    }

                    try
                    {
                        driver.Url = entry;
                        driver.Manage();

                        Console.WriteLine(entry);

                        for (var i = 0; i < 5; i++)
                        {
                            IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
                            js.ExecuteScript("window.scrollBy(0,1000)");

                            Thread.Sleep(sleep / 5);
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    count++;
                }
            });
        }

        private static List<SitemapEntry> ReadSitemap(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var rawData = client.DownloadString(url);
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(rawData);

                var nodes = xmlDocument.GetElementsByTagName("url");

                var list = new List<SitemapEntry>();
                foreach (XmlNode node in nodes)
                {
                    list.Add(new SitemapEntry
                    {
                        Url = node["loc"]?.InnerText,
                        Priority = node["priority"]?.InnerText,
                        LastModified = node["lastmod"]?.InnerText,
                        ChangeFrequency = node["changefreq"]?.InnerText
                    });
                }

                return list;
            }
        }

        private class SitemapEntry
        {
            public string Url { get; set; }
            public string Priority { get; set; }
            public string LastModified { get; set; }
            public string ChangeFrequency { get; set; }
        }
    }
}
