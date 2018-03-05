using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Library.Slack
{
    public class SlackService
    {
        private const string UriTemplate = "https://slack.com/api/chat.postMessage";

        private string Token { get; }
        private string Username { get; }
        private string Channel { get; }
        private string Account { get; }

        public SlackService(
            string token,
            string username,
            string channel,
            string account)
        {
            Token = token;
            Username = username;
            Channel = channel;
            Account = account;
        }

        public void SendMessage(string message)
        {
            using (var client = new WebClient())
            {
                var data = new NameValueCollection();
                data["channel"] = Channel;
                data["token"] = Token;
                data["text"] = message;
                data["username"] = Username;
                var response = client.UploadValues(UriTemplate, "POST", data);
                string responseText = Encoding.UTF8.GetString(response);
            }
        }
    }
}
