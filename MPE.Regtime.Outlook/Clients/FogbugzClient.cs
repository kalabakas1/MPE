using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MPE.Regtime.Outlook.App.Models.FogBugz;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;

namespace MPE.Regtime.Outlook.App.Clients
{
    internal class FogbugzClient
    {
        private const string Endpoint = "https://{0}.fogbugz.com/f/api/0/jsonapi";

        private readonly string _username;
        private readonly string _password;
        private readonly RestClient _client;

        public FogbugzClient(
            string username, 
            string password,
            string fogbugz)
        {
            _username = username;
            _password = password;
            _client = new RestClient(string.Format(Endpoint, fogbugz));
        }

        public Case GetCase(int caseNumber)
        {
            var token = GetToken();
            var request = SetupRequest();
            request.AddJsonBody(new SearchRequest
            {
                Cmd = "search",
                Columns = new[] { "sProject", "sTitle", "hrsOrigEst", "hrsCurrEst", "hrsElapsed" },
                Max = 1,
                Query = caseNumber.ToString(),
                Token = token
            });

            var response = JsonConvert.DeserializeObject<SearchResponse>(_client.Execute(request).Content);

            if (response.Data != null 
                && response.Data != null 
                && response.Data.Cases != null
                && response.Data.Cases.Any())
            {
                return response.Data.Cases[0];
            }

            return null;
        }

        public void SetEstimateIfNone(int caseNumber, decimal estimate)
        {
            var fbCase = GetCase(caseNumber);
            if (fbCase != null && fbCase.CurrentEstimate == 0)
            {
                var token = GetToken();
                var request = SetupRequest();
                request.AddJsonBody(new
                {
                    cmd = "edit",
                    token = token,
                    ixBug = caseNumber,
                    hrsCurrEst = estimate
                });

                _client.Execute(request);
            }
        }

        private string GetToken()
        {
            var request = SetupRequest();
            request.AddJsonBody(new LogonRequest
            {
                Cmd = "logon",
                Email = _username,
                Password = _password
            });

            var response = _client.Execute(request);
            var data = JsonConvert.DeserializeObject<LogonResponse>(response.Content);
            return data.Data.Token;
        }

        private RestRequest SetupRequest()
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftSerializer();
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");

            return request;
        }

        private class NewtonsoftSerializer : ISerializer
        {
            public NewtonsoftSerializer()
            {
                ContentType = "application/json";
            }

            public string Serialize(object obj)
            {
                return JsonConvert.SerializeObject(obj);
            }

            public string RootElement { get; set; }

            public string Namespace { get; set; }

            public string DateFormat { get; set; }

            public string ContentType { get; set; }

        }
    }
}
