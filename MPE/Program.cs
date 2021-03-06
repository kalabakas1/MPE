﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions.MonoHttp;

namespace MPE
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(0, 1000000, (x) =>
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync("https://www.smukfest.dk/media/4503/tn%C3%B8_smukfest_under_koncert_livecamp_man_cousin_2017_m1cp31ff33-1.jpga").Result;
                }
            });

            Console.ReadLine();
        }

        private class IisLogExecuter
        {
            public static void Run()
            {
                var path = @"C:\Users\mpe\Desktop\requestlogs\";
                var files = Directory.GetFiles(path, "*.log", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var repo = new RequestLogRepository(path);

                    var lines = repo.ReadLogRecords(2, Path.GetFileName(file));

                    var records = repo.ParseLines("RfProd", lines);

                    int index = 0;
                    int chunkSize = 256;
                    while (true)
                    {
                        var chunk = records.Skip(index * chunkSize).Take(chunkSize);

                        if (!chunk.Any())
                        {
                            break;
                        }

                        var client = new RestClient("http://195.215.240.99:8080/api/RequestLogResult");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("authorization", "");
                        request.AddHeader("content-type", "application/json");
                        request.AddParameter("application/json", JsonConvert.SerializeObject(chunk),
                            ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);

                        index++;
                    }
                }
            }
        }

        private class RequestLogRepository
        {
            private readonly string _requestLogFolder;

            public RequestLogRepository(string folder)
            {
                _requestLogFolder = folder;
            }

            public List<string> ReadLogRecords(int processId, string fileName)
            {
                var logFilePath = Path.Combine(_requestLogFolder, $"W3SVC{processId}", fileName);
                if (!File.Exists(logFilePath))
                {
                    throw new FileNotFoundException();
                }

                return File.ReadAllLines(logFilePath).ToList();
            }

            public List<RequestLogResult> ParseLines(string host, List<string> logRecordLines)
            {
                var headers = new List<string>();
                var records = new List<RequestLogResult>();
                foreach (var line in logRecordLines)
                {
                    if (line.StartsWith("#"))
                    {
                        var cmdLine = line.Replace("#", string.Empty);
                        if (cmdLine.StartsWith("Fields"))
                        {
                            headers = cmdLine.Replace("Fields: ", string.Empty).Split(' ').Select(x => x.Trim())
                                .ToList();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        var values = line.Split(' ').Select(x => x.Trim()).ToList();
                        var record = CreateRecord(host, headers, values);

                        records.Add(record);
                    }
                }

                return records;
            }

            private RequestLogResult CreateRecord(string host, List<string> headers, List<string> values)
            {
                var record = new RequestLogResult();
                if (headers.Contains("date") && headers.Contains("time"))
                {
                    record.Timestamp = DateTime.Parse($"{values[headers.IndexOf("date")]} {values[headers.IndexOf("time")]}").AddHours(2);
                }

                if (headers.Contains("c-ip"))
                {
                    record.Ip = values[headers.IndexOf("c-ip")];
                }

                if (headers.Contains("cs-method"))
                {
                    record.Method = values[headers.IndexOf("cs-method")];
                }

                if (headers.Contains("cs-uri-stem"))
                {
                    record.Uri = values[headers.IndexOf("cs-uri-stem")];
                }

                if (headers.Contains("cs-uri-query"))
                {
                    record.Query = values[headers.IndexOf("cs-uri-query")];

                    if (record.Query != null && record.Query.Length == 1)
                    {
                        record.Query = null;
                    }

                    if (!string.IsNullOrEmpty(record.Query))
                    {
                        try
                        {
                            var queryData = record.Query.Split('&').ToDictionary(x => x.Split('=')[0].ToLower(),
                                x => x.Split('=')[1].ToLower());

                            if (queryData.ContainsKey("apikey"))
                            {
                                record.QueryApiKey = queryData["apikey"];
                            }

                            if (queryData.ContainsKey("id"))
                            {
                                record.QueryId = queryData["id"];
                            }
                        }
                        catch { }
                    }
                }

                if (string.IsNullOrEmpty(record.Uri))
                {
                    throw new Exception("Not able to create path");
                }

                record.Path = $"{host}.Request.{record.Uri.Replace("/", ".")}".Replace("..", ".");

                return record;
            }

        }

        public class RequestLogResult
        {
            [JsonProperty("Timestamp")]
            public DateTime Timestamp { get; set; }
            [JsonProperty("Path")]
            public string Path { get; set; }
            [JsonProperty("Ip")]
            public string Ip { get; set; }
            [JsonProperty("Method")]
            public string Method { get; set; }
            [JsonProperty("Uri")]
            public string Uri { get; set; }
            [JsonProperty("Query")]
            public string Query { get; set; }
            [JsonProperty("QueryApiKey")]
            public string QueryApiKey { get; set; }
            [JsonProperty("QueryId")]
            public string QueryId { get; set; }
        }
    }
}
    