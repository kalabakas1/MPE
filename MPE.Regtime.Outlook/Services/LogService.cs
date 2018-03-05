using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Regtime.Outlook.App.Services
{
    internal class LogService
    {
        private List<string> _messages;

        public LogService()
        {
            _messages = new List<string>();
        }

        public void Log(string messageTemplate, params object[] fields)
        {
            _messages.Add(string.Format(messageTemplate, fields));
        }

        public override string ToString()
        {
            return string.Join("\n", _messages);
        }
    }
}
