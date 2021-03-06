﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MPE.Library.Slack;
using MPE.Regtime.Outlook.App.Clients;
using MPE.Regtime.Outlook.App.Converters;
using MPE.Regtime.Outlook.App.Extensions;
using MPE.Regtime.Outlook.App.Models;
using MPE.Regtime.Outlook.App.Models.Configurations;
using MPE.Regtime.Outlook.App.Services;
using RestSharp;

namespace MPE.Regtime.Outlook.App
{
    internal class RegtimeConsole
    {
        private RegtimeClient _client;
        private readonly ConfigurationService _configurationService;
        private readonly TextMessageService _textMessageService;
        private readonly SlackService _slackService;

        public RegtimeConsole(
            string configurationPath)
        {
            _configurationService = new ConfigurationService(configurationPath);
            _textMessageService = new TextMessageService(_configurationService);

            var slackConfiguration = _configurationService.Configuration.Slack;
            _slackService = new SlackService(slackConfiguration.Token, slackConfiguration.Username, slackConfiguration.Channel, slackConfiguration.Account);
        }

        public void Run(string cmd = null, DateTime date = default(DateTime))
        {
            _client = new RegtimeClient(_configurationService);
            if (string.IsNullOrEmpty(_configurationService.Configuration.Password))
            {
                Print("Password: ");
                _client.SetPassword(GetPassword());
            }
            else
            {
                _client.SetPassword(_configurationService.Configuration.Password);
            }

            StartConsole(cmd, date);
        }

        private void StartConsole(string cmd = "", DateTime date = default(DateTime))
        {
            var consoleMode = string.IsNullOrEmpty(cmd);
            do
            {
                while (cmd?.Trim() == string.Empty || cmd == null)
                {
                    Console.Write("> ");
                    cmd = Console.ReadLine();
                }

                try
                {
                    switch (cmd)
                    {
                        case "config":
                            PrintConfig(_configurationService.Configuration);
                            break;
                        case "clear":
                            Console.Clear();
                            break;
                        case "validate":
                            ExecuteValidateDate();
                            break;
                        case "register":
                            ExecuteRegistrationOfDate(date);
                            break;
                        case "register-all":
                            ExecuteRangeRegistration();
                            break;
                        default:
                            Console.WriteLine("No command named: " + cmd);
                            break;
                    }
                }
                catch (Exception e)
                {
                    e.Message.Print();
                }

                cmd = "";
            } while (consoleMode);
        }

        private void ExecuteRegistrationOfDate(DateTime date = default(DateTime))
        {
            if (date == default(DateTime))
            {
                date = GetDateFromInput();
            }

            var validations = ExecuteValidateDate(date);
            if (validations.All(x => x.IsValid))
            {
                var logService = new LogService();
                foreach (var registration in validations.Select(x => x.Object).ToList())
                {
                    if (_client.HaveRegistration(registration))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(registration.Fogbugz))
                    {
                        var fogbugzClient = new FogbugzClient(_configurationService.Configuration.Username, _configurationService.Configuration.FogbugzPassword, registration.Fogbugz, logService);
                        if (!string.IsNullOrEmpty(registration.CaseNumber))
                        {
                            fogbugzClient.SetEstimateIfNone(int.Parse(registration.CaseNumber)
                                , validations.Where(x => x.Object.CaseNumber == registration.CaseNumber && !_client.HaveRegistration(x.Object))
                                    .Sum(x => x.Object.Hours));

                            int caseNumber = 0;
                            if (int.TryParse(registration.CaseNumber, out caseNumber))
                            {
                                var fbcase = fogbugzClient.GetCase(caseNumber);
                                if (fbcase != null)
                                {
                                    registration.Note = fbcase.Title + ": " + registration.Note;
                                }
                            }
                        }
                    }

                    _client.RegisterHours(registration);
                    Console.WriteLine("Request sent for: " + registration);
                }

                Console.WriteLine("DONE...");

                if (validations.Any())
                {
                    var syncSuccessfullMessage = $"B-) Regtime synch done... B-) {DateTime.Now:yyyy-MM-dd HH:mm}";
                    //_textMessageService.Send(_configurationService.Configuration.Mobile, syncSuccessfullMessage);
                    _slackService.SendMessage(syncSuccessfullMessage);

                    if (!string.IsNullOrEmpty(logService.ToString()))
                    {
                        var caseUpdatesMessage = $"Case updates: \n {logService}";
                        //_textMessageService.Send(_configurationService.Configuration.Mobile, caseUpdatesMessage);
                        _slackService.SendMessage(caseUpdatesMessage);
                    }
                }
            }
            else
            {
                var failureRegistrations = $"Entries at {date:yyyy-MM-dd} is not valid - execute manually";
                //_textMessageService.Send(_configurationService.Configuration.Mobile, failureRegistrations);
                _slackService.SendMessage(failureRegistrations);
            }
        }

        private void ExecuteRangeRegistration()
        {
            var startDate = _client.GetLatestRegistrationDate().AddDays(1);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            while (startDate < endDate)
            {
                ExecuteRegistrationOfDate(startDate);
                startDate = startDate.AddDays(1);
            }
        }

        private List<ValidationResult<RegtimeRegistration>> ExecuteValidateDate(DateTime date = default(DateTime))
        {
            if (date == default(DateTime))
            {
                date = GetDateFromInput();
            }

            var outlookClient = new OutlookClient(_configurationService.Configuration.Username, _configurationService.Configuration.Password);
            var converter = new CalendarEventConverter(_configurationService.Configuration);

            var events = outlookClient.GetEvents(_configurationService.Configuration.Calendar, date, date.AddDays(1));
            var validations = converter.ConvertMultipleToRegtimeRegistrations(events);

            if (validations.Any(x => !x.IsValid))
            {
                Console.WriteLine($"Some entries are not valid {date.ToShortDateString()}:");
                foreach (var validationResult in validations.Where(x => !x.IsValid).ToList())
                {
                    Console.WriteLine(validationResult.Message);
                }
            }
            else
            {
                Console.WriteLine("CONGRATS - You are now ready for the registration of time... " + date.ToShortDateString());
                PrintRegistrations(validations.Select(x => x.Object).ToList());
            }
            return validations;
        }

        private string GetPassword()
        {
            var pwd = "";
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Substring(0, pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd += i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }

        private void PrintConfig(Configuration configuration)
        {
            foreach (var customer in configuration.Customers)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{customer.Alias} - {customer.Name}");
                Console.ForegroundColor = ConsoleColor.Green;

                foreach (var project in customer.Projects)
                {
                    Console.WriteLine($"  {project.Alias} - {project.Name}");
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void PrintRegistrations(List<RegtimeRegistration> registrations)
        {
            if (!registrations.Any())
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"{"Hours",-5} {"Case",-6} {"Customer",-20} {"Project",-20} Note");
            foreach (var registration in registrations)
            {
                Console.WriteLine(registration.ToString());
            }
            Console.WriteLine();
        }

        private DateTime GetDateFromInput()
        {
            var date = DateTime.Now;
            do
            {
                try
                {
                    SendKeys.SendWait(date.ToString("yyyy-MM-dd"));
                    var inputDate = Console.ReadLine();
                    date = DateTime.Parse(inputDate);
                }
                catch
                {
                    Console.WriteLine("Not valid date - try again");
                }
            } while (date == default(DateTime));
            return date;
        }

        private void Print(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{input}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
