using System;

namespace MPE.Regtime.Outlook.App.Models
{
    internal class CalendarEvent
    {
        public string Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Subject { get; set; }
        public decimal Duration { get; set; }
    }
}
