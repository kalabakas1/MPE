using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPE
{
    internal class CallbackTester
    {
        private static HttpClient _client = new HttpClient();

        public static void Run()
        {
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                File.ReadAllText(@"C:\Users\mpe\Desktop\data.json"));

            Console.WriteLine("Data read...");


            int count = 0;
            foreach (var d in data)
            {
                try
                {
                    Send("", d);
                    Thread.Sleep(50);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                count++;

                if (count % 100 == 0)
                {
                    Console.WriteLine(count);
                }
            }

            Console.WriteLine("done...");
            Console.ReadLine();
        }

        private static async void Send(string url, Dictionary<string, string> data)
        {
            var fields = new NameValueCollection();
            foreach (KeyValuePair<string, string> d in data)
            {
                fields.Add(d.Key, d.Value.ToString());
            }

            try
            {
                var response = await _client.PostAsync(url, new FormUrlEncodedContent(data.Select(x => x)));
                var strData = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(strData))
                {
                    Console.WriteLine(strData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
