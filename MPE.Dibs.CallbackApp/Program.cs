using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MPE.Dibs.CallbackApp.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Dibs.CallbackApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Console.WriteLine("Dibs Callback App");

            ConfigurationFile config = null;
            List<string> transactionIds = new List<string>();
            try
            {
                config = ReadConfig(out transactionIds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }

            foreach (var transactionId in transactionIds)
            {
                var dibsClient = new RestClient("https://payment.architrade.com");
                dibsClient.Authenticator = new HttpBasicAuthenticator(config.ApiUser, config.Password);

                var dibsRequest = new RestRequest("/cgi-adm/callback.cgi", Method.POST);
                dibsRequest.AddParameter("merchant", config.Merchant);
                dibsRequest.AddParameter("transact", transactionId);

                var response = dibsClient.Execute(dibsRequest);

                var rawData = response.Content;
                var data = rawData.Split('&').ToDictionary(x => x.Split('=')[0], x => HttpUtility.UrlDecode(x.Split('=')[1]));

                var oldHash = string.Empty;
                var calcVal = string.Empty;
                if (data.ContainsKey("MAC"))
                {
                    oldHash = data["MAC"];
                    data.Remove("MAC");

                    calcVal = CalculateMac(data, config.Hmac).ToLower();
                    data.Add("MAC", calcVal);
                }
                else if (data.ContainsKey("md5key"))
                {
                    oldHash = data["md5key"];
                    data.Remove("md5key");

                    calcVal = CalculateMd5(data, config.Key1, config.Key2).ToLower();
                    data.Add("md5key", calcVal);
                }

                if (string.IsNullOrEmpty(calcVal))
                {
                    Console.WriteLine("MAC or md5key not pressent");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                var url = new Uri(config.CallbackUrl);
                var venmaClient = new RestClient(url.Scheme + "://" + url.Host);
                var venmaRequest = new RestRequest(url.PathAndQuery);
                venmaRequest.Method = Method.POST;
                venmaRequest.AddHeader("Cache-Control", "no-cache");
                venmaRequest.AddHeader("Content-Type", "multipart/form-data");

                foreach (var pair in data)
                {
                    venmaRequest.AddParameter(pair.Key, pair.Value, ParameterType.GetOrPost);
                }

                Console.WriteLine(data["billingEmail"]);

                var callbackResponse = venmaClient.Execute(venmaRequest);

                Console.WriteLine(callbackResponse.Content);
            }

            Console.WriteLine("DONE...");
            Console.ReadLine();
        }

        private static ConfigurationFile ReadConfig(out List<string> transactions)
        {
            var filePath = ConfigurationManager.AppSettings["MPE.Config.Path"]; 
            ConfigurationFile config = null;
            transactions = new List<string>();
            if (File.Exists(filePath))
            {
                config = JsonConvert.DeserializeObject<ConfigurationFile>(File.ReadAllText(filePath));
            }
            else
            {
                throw new Exception("Config file not found: " + filePath);
            }

            if (File.Exists(config.TransctionFilePath))
            {
                transactions = File.ReadAllLines(config.TransctionFilePath).ToList();
            }
            else
            {
                throw new Exception("Could not read transaction file: " + config.TransctionFilePath);
            }

            return config;
        }

        public static string CalculateMd5(Dictionary<string, string> formFields, string key1, string key2)
        {

            var md5Key = GenerateMd5(
                string.Format("merchant={0}&orderid={1}&currency={2}&amount={3}",
                    formFields["merchant"], formFields["orderid"], formFields["currency"], formFields["amount"]), key1, key2);

            return md5Key;
        }

        public static string GenerateMd5(string input, string key1, string key2)
        {
            using (MD5 cs = MD5.Create())
            {
                var sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.ASCII.GetBytes(key1
                                                              + input));
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                hash = cs.ComputeHash(Encoding.ASCII.GetBytes(key2 + sb));
                sb.Length = 0;
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static string CalculateMac(Dictionary<string, string> formFields, string macKey)
        {
            var keys = formFields.Keys.ToList();
            keys.Sort(StringComparer.Ordinal);
            string msg = "";
            foreach (var key in keys)
            {
                if (key != keys[0]) msg += "&";
                msg += key + "=" + formFields[key];
            }

            //Decoding the secret Hex encoded key and getting the bytes for MAC calculation
            var K_bytes = new byte[macKey.Length / 2];
            for (int i = 0; i < K_bytes.Length; i++)
            {
                K_bytes[i] = byte.Parse(macKey.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            //Getting bytes from message
            var encoding = new UTF8Encoding();
            byte[] msg_bytes = encoding.GetBytes(msg);

            //Calculate MAC key
            var hash = new HMACSHA256(K_bytes);
            byte[] mac_bytes = hash.ComputeHash(msg_bytes);
            string mac = BitConverter.ToString(mac_bytes).Replace("-", "").ToLower();

            return mac;
        }

        public static string GetFormString(Dictionary<string, string> dic)
        {
            var macString = "";
            var keysSortedList = dic.Keys.OrderBy(x => x).ToArray();

            for (var keyIndex = 0; keyIndex < keysSortedList.Count(); keyIndex++)
            {
                var key = keysSortedList[keyIndex];
                macString += key + "=" + dic[key];
                if (keyIndex < keysSortedList.Count() - 1)
                {
                    macString += "&";
                }
            }
            return macString;
        }

        public static string HexToString(string hex)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < hex.Length; i += 2)
            {
                string hexdec = hex.Substring(i, 2);
                int number = int.Parse(hexdec, NumberStyles.HexNumber);
                char charToAdd = (char)number;
                sb.Append(charToAdd);
            }
            return sb.ToString();
        }
    }
}
