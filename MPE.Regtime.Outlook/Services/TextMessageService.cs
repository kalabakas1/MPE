using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace MPE.Regtime.Outlook.App.Services
{
    internal class TextMessageService
    {
        private const string ApiEndPoint = "http://api.linkmobility.dk/v2/";
        private readonly ConfigurationService _configurationService;
        public TextMessageService(
            ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public void Send(string to, string message)
        {
            var client = new RestClient(ApiEndPoint);
            var request = new RestRequest("message.json", Method.POST);
            request.Timeout = 30000;
            request.AddJsonBody(new
            {
                message = new
                {
                    format = "GSM",
                    recipients = to,
                    sender = "Regtime",
                    message
                }
            });
            request.AddQueryParameter("apikey", _configurationService.Configuration.TextMessageApiKey);

            var result = client.Execute(request);
        }
    }
}
