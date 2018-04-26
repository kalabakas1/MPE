using System;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;

namespace MPE.Logging.Sinks
{
    internal class CoolSmsSink : ILogEventSink
    {
        private const int MaxTextMessageLength = 160;
        private const string ApiEndpoint = "http://api.linkmobility.dk/";

        private readonly string _apiKey;
        private readonly string _fromName;
        private readonly string[] _phoneNumbers;

        public CoolSmsSink(
            string apiKey, 
            string fromName,
            string[] phoneNumbers)
        {
            _apiKey = apiKey;
            _fromName = fromName;
            _phoneNumbers = phoneNumbers;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level >= LogEventLevel.Fatal 
                && logEvent.Exception != null)
            {
                Send(logEvent.Exception.Message);
            }
        }

        private void Send(string message)
        {
            var client = new RestClient(ApiEndpoint);

            var request = new RestRequest("v2/message.json");
            request.AddQueryParameter("apikey", _apiKey);
            request.AddJsonBody(new
            {
                message = new
                {
                    format = "GSM",
                    recipients = string.Join(",", _phoneNumbers.Where(ValidatePhoneNumber)),
                    sender = _fromName,
                    message = message.Substring(0, Math.Min(message.Length, MaxTextMessageLength))
                }
            });
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json");

            var response = client.Execute(request);
        }

        private bool ValidatePhoneNumber(string number)
        {
            if (number.Length > 0)
            {
                long parsedRest = 0;
                if (number.First() == 'c'
                    || number.First() == '+')
                {
                    var rest = number.Substring(1, number.Length - 1);
                    return long.TryParse(rest, out parsedRest);
                }
            }

            return false;
        }
    }

    internal static class CoolSmsSinkExtensions
    {
        public static LoggerConfiguration CoolSmsSink(
            this LoggerSinkConfiguration loggerSinkConfiguration, string apiKey,
            string fromName, params string[] phoneNumbers)
        {
            return loggerSinkConfiguration.Sink(new CoolSmsSink(apiKey, fromName, phoneNumbers));
        }
    }
}
