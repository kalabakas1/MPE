using System;

namespace MPE.Regtime.Outlook.App.Models
{
    internal class RegtimeRegistration
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string CaseNumber { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public decimal Hours { get; set; }
        public string Note { get; set; }
        public RegistrationType Type { get; set; }

        public string Fogbugz { get; set; }

        public override string ToString()
        {
            return $"{Hours,-5} {CaseNumber,-6} {Customer,-20} {Project,-20} {Note}";
        }
    }
}
