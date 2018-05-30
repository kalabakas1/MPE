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
                var oldMac = data["MAC"];
                data.Remove("MAC");

                var calcVal = CalculateMacForDibsPayment(data, HexToString(config.Hmac)).ToLower();
                data.Add("MAC", calcVal);

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
                Console.ReadLine();
            }
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

        public static string CalculateMacForDibsPayment(Dictionary<string, string> formFields, string macKey)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(macKey);
            byte[] messageBytes = encoding.GetBytes(GetFormString(formFields));

            var hmacsha256 = new HMACSHA256(keyByte);
            var hashmessage = hmacsha256.ComputeHash(messageBytes);
            string sbinary = "";

            for (int i = 0; i < hashmessage.Length; i++)
            {
                sbinary += hashmessage[i].ToString("X2");
            }

            return sbinary;
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
