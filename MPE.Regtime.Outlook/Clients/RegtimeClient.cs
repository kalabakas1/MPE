using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MPE.Regtime.Outlook.App.Models;
using MPE.Regtime.Outlook.App.Services;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace MPE.Regtime.Outlook.App.Clients
{
    internal class RegtimeClient
    {
        private string _urlEndpoint;
        private const string PriorRegistrationFileName = "Registrations.json";

        private readonly ConfigurationService _configurationService;

        private HashSet<RegtimeRegistration> _priorRegistrations;

        private readonly RestClient _client;

        public RegtimeClient(
            ConfigurationService configurationService)
        {
            _configurationService = configurationService;

            _urlEndpoint = _configurationService.Configuration.RegtimeEndpoint;

            _client = new RestClient(_urlEndpoint);
            _client.Authenticator = new NtlmAuthenticator(_configurationService.Configuration.Username, _configurationService.Configuration.Password);

            ReadPriorRegistrations();
        }

        public void SetPassword(string password)
        {
            _configurationService.SetPassword(password);
            _client.Authenticator = new NtlmAuthenticator(_configurationService.Configuration.Username, _configurationService.Configuration.Password);
        }

        public void RegisterHours(RegtimeRegistration registration)
        {
            if (_priorRegistrations.Any(x => x.Id == registration.Id))
            {
                return;
            }

            var request = new RestRequest("/TimeEntry/UpdateTimeSheet", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Referer", "https://www.google.dk");

            var date = registration.Date.ToString("dd-MM-yyyy");
            request.AddParameter("entryDate", date, ParameterType.GetOrPost);

            switch (registration.Type)
            {

                case RegistrationType.HoursA:
                    request.AddParameter("AFBCaseId", registration.CaseNumber, ParameterType.GetOrPost);
                    request.AddParameter("AFromFB", "false", ParameterType.GetOrPost);
                    request.AddParameter("Acustomer", registration.Customer, ParameterType.GetOrPost);
                    request.AddParameter("Aid", "", ParameterType.GetOrPost);
                    request.AddParameter("Anote", registration.Note, ParameterType.GetOrPost);
                    request.AddParameter("Aproject", registration.Project, ParameterType.GetOrPost);
                    request.AddParameter("Astatus", "default", ParameterType.GetOrPost);
                    request.AddParameter("Atime", registration.Hours, ParameterType.GetOrPost);
                    break;
                case RegistrationType.HoursF:
                    request.AddParameter("FFromFB", "false", ParameterType.GetOrPost);
                    request.AddParameter("Fid", "", ParameterType.GetOrPost);
                    request.AddParameter("Fnote", registration.Note, ParameterType.GetOrPost);
                    request.AddParameter("Fstatus", "default", ParameterType.GetOrPost);
                    request.AddParameter("Ftime", registration.Hours, ParameterType.GetOrPost);
                    break;
                case RegistrationType.HoursS:
                    request.AddParameter("SFromFB", "false", ParameterType.GetOrPost);
                    request.AddParameter("Sid", "", ParameterType.GetOrPost);
                    request.AddParameter("Snote", registration.Note, ParameterType.GetOrPost);
                    request.AddParameter("Sstatus", "default", ParameterType.GetOrPost);
                    request.AddParameter("Stime", registration.Hours, ParameterType.GetOrPost);
                    break;
            }

            var response = _client.Execute(request);

            _priorRegistrations.Add(registration);
            PersistPriorRegistrations();
        }

        public DateTime GetLatestRegistrationDate()
        {
            return _priorRegistrations.Max(x => x.Date);
        }

        private void ReadPriorRegistrations()
        {
            if (File.Exists(PriorRegistrationFileName))
            {
                _priorRegistrations = new HashSet<RegtimeRegistration>(
                    JsonConvert.DeserializeObject<List<RegtimeRegistration>>(
                        File.ReadAllText(PriorRegistrationFileName)));
            }
            else
            {
                _priorRegistrations = new HashSet<RegtimeRegistration>();
            }
        }

        private void PersistPriorRegistrations()
        {
            File.WriteAllText(PriorRegistrationFileName, JsonConvert.SerializeObject(_priorRegistrations.ToList(), Formatting.Indented));
        }
    }
}
