using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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

        public RegtimeConsole(
            string configurationPath)
        {
            _configurationService = new ConfigurationService(configurationPath);
            _textMessageService = new TextMessageService(_configurationService);
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
                foreach (var registration in validations.Select(x => x.Object).ToList())
                {
                    var fogbugzClient = new FogbugzClient(_configurationService.Configuration.Username, _configurationService.Configuration.FogbugzPassword, registration.Fogbugz);
                    if (!string.IsNullOrEmpty(registration.CaseNumber))
                    {
                        fogbugzClient.SetEstimateIfNone(int.Parse(registration.CaseNumber)
                            , validations.Where(x => x.Object.CaseNumber == registration.CaseNumber)
                                .Sum(x => x.Object.Hours));
                    }

                    _client.RegisterHours(registration);
                    Console.WriteLine("Request sent for: " + registration.ToString());
                }

                Console.WriteLine("DONE...");

                _textMessageService.Send(_configurationService.Configuration.Mobile,
                    $"B-) Regtime synch done... B-) {DateTime.Now:yyyy-MM-dd HH:mm}");
            }
            else
            {
                _textMessageService.Send(_configurationService.Configuration.Mobile,
                    $"Entries at {date:yyyy-MM-dd} is not valid - execute manually");
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
                Console.WriteLine("Some entries are not valid:");
                foreach (var validationResult in validations.Where(x => !x.IsValid).ToList())
                {
                    Console.WriteLine(validationResult.Message);
                }
            }
            else
            {
                Console.WriteLine("CONGRATS - You are now ready for the registration of time...");
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

        private Customer GetCustomerFromConsole()
        {
            var customerAlias = "";
            Customer customer;
            do
            {
                Print("Customer: ");
                customerAlias = Console.ReadLine();
                customer = _configurationService.Configuration.GetByAlias(customerAlias);

                if (customer == null)
                {
                    Console.WriteLine("Not valid customer - try again");
                }
            } while (customer == null);

            return customer;
        }

        private Project GetProjectFromConsole(Customer customer)
        {
            Project project;
            do
            {
                Print("Project: ");
                var projectAlias = Console.ReadLine();
                project = customer.GetProject(projectAlias);

                if (project == null)
                {
                    project = customer.Projects.FirstOrDefault();
                    Console.WriteLine("Defaults => " + (project != null ? project.Name : "N/A"));
                }
            } while (project == null && customer.Projects.Any());

            return project;
        }

        private decimal GetHoursFromConsole()
        {
            decimal hours = 0;
            do
            {
                Print("Hours: ");
                if (!decimal.TryParse(Console.ReadLine(), out hours))
                {
                    Console.WriteLine("Not valid hours - try again");
                }
            } while (hours == 0);

            return hours;
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
