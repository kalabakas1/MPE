using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.WebServices.Data;
using CalendarEvent = MPE.Regtime.Outlook.App.Models.CalendarEvent;

namespace MPE.Regtime.Outlook.App.Clients
{
    internal class OutlookClient
    {
        private string _username;
        private string _password;
        private string _exchangeUrl = "https://outlook.office365.com/ews/exchange.asmx";
        public OutlookClient(
            string username, 
            string password)
        {
            _username = username;
            _password = password;
        }

        public List<CalendarEvent> GetEvents(string calendarName, DateTime fromDate, DateTime toDate)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
            service.Credentials = new WebCredentials(_username, _password);
            service.Url = new Uri(_exchangeUrl);

            var folderView = new FolderView(100);
            folderView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            folderView.PropertySet.Add(FolderSchema.DisplayName);
            folderView.Traversal = FolderTraversal.Deep;
            var searchFilter = new SearchFilter.IsEqualTo(FolderSchema.FolderClass, "IPF.Appointment");
            var foldersResults = service.FindFolders(WellKnownFolderName.Root, searchFilter, folderView);
            var calendar = foldersResults.Where(f => f.DisplayName == calendarName).Cast<CalendarFolder>().FirstOrDefault();

            if (calendar == null)
            {
                return new List<CalendarEvent>();
            }

            var cView = new CalendarView(fromDate, toDate, 50);
            cView.PropertySet = new PropertySet(ItemSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End, ItemSchema.Id);
            var appointments = calendar.FindAppointments(cView).ToList();

            return appointments.Select(x => new CalendarEvent
            {
                Id = x.Id.UniqueId,
                DateFrom = x.Start,
                DateTo = x.End,
                Duration = Convert.ToDecimal((x.End - x.Start).TotalHours),
                Subject = x.Subject
            }).ToList();
        }

        public void SetExchangeUrl(string url)
        {
            _exchangeUrl = url;
        }
    }
}
