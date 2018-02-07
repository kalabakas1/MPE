using System.Collections.Generic;
using System.Linq;
using MPE.Regtime.Outlook.App.Clients;
using MPE.Regtime.Outlook.App.Models;
using MPE.Regtime.Outlook.App.Models.Configurations;

namespace MPE.Regtime.Outlook.App.Converters
{
    internal class CalendarEventConverter
    {
        private const string EntryFormat = "[Customer - Project - Case - Note] or [SICK - Note] or [FLEX - Note]";
        private readonly Configuration _configuration;
        public CalendarEventConverter(
            Configuration configuration)
        {
            _configuration = configuration;
        }

        private ValidationResult<RegtimeRegistration> ConvertToRegtimeRegistration(CalendarEvent calendarEvent)
        {
            var validationResult = new ValidationResult<RegtimeRegistration>();

            var datetime = calendarEvent.DateFrom;
            var dataFields = calendarEvent.Subject.Split('-');
            for (int i = 0; i < dataFields.Length; i++)
            {
                dataFields[i] = dataFields[i].Trim();
            }

            if (dataFields.Length > 0 
                && dataFields[0].Trim() == "IGN")
            {
                return null;
            }

            if (dataFields.Length != 4)
            {
                int caseNumber;
                if (dataFields.Length == 2 
                    && (dataFields[0] == "SICK" || dataFields[0] == "FLEX"))
                {
                    validationResult.IsValid = true;
                    validationResult.Object = new RegtimeRegistration
                    {
                        Type = dataFields[0] == "SICK" ? RegistrationType.HoursS : RegistrationType.HoursF,
                        Id = calendarEvent.Id,
                        Date = datetime,
                        Hours = calendarEvent.Duration,
                        Note = dataFields[1]
                    };
                }
                else
                {
                    validationResult.Message =
                        $"Missing fields: [{calendarEvent.Subject}] please use format: {EntryFormat} - no dashes in note field!";
                }
                return validationResult;
            }
            else
            {
                var customerAlias = dataFields[0];
                var customer = _configuration.Customers.FirstOrDefault(x => x.Alias == customerAlias);

                if (customer == null)
                {
                    validationResult.Message = $"No customer for entry: [{calendarEvent.Subject}]";
                    return validationResult;
                }

                var projectAlias = dataFields[1];
                var project = customer.GetProject(projectAlias);
                if (project == null)
                {
                    validationResult.Message = $"No project for entry: [{calendarEvent.Subject}]";
                    return validationResult;
                }

                var caseNumberString = dataFields[2];
                int caseNumber = 0;
                if (!string.IsNullOrEmpty(caseNumberString)
                    && !int.TryParse(caseNumberString, out caseNumber))
                {
                    validationResult.Message = $"Not valid casenumber for entry: [{calendarEvent.Subject}] please be int or empty";
                    return validationResult;
                }

                if (calendarEvent.Duration == 0)
                {
                    validationResult.Message = $"Duration must be over zero for entry: [{calendarEvent.Subject}]";
                    return validationResult;
                }

                validationResult.IsValid = true;
                validationResult.Object = new RegtimeRegistration
                {
                    Type = RegistrationType.HoursA,
                    Id = calendarEvent.Id,
                    Date = datetime,
                    CaseNumber = caseNumberString,
                    Customer = customer.Name,
                    Project = project.Name,
                    Fogbugz = customer.Fogbugz,
                    Hours = calendarEvent.Duration,
                    Note = dataFields[3]
                };
            }

            return validationResult;
        }

        public List<ValidationResult<RegtimeRegistration>> ConvertMultipleToRegtimeRegistrations(List<CalendarEvent> calendarEvents)
        {
            var result = new List<ValidationResult<RegtimeRegistration>>();
            foreach (var calendarEvent in calendarEvents)
            {
                var registration = ConvertToRegtimeRegistration(calendarEvent);
                if (registration != null)
                {
                    result.Add(registration);
                }
            }

            return result;
        }
    }
}
