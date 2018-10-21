using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Models.Configurations;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using MPE.Logging;

namespace MPE.Pinger.Logic
{
    public class HealingExecutor
    {
        internal void ExecuteHealing(Connection connection)
        {
            if (string.IsNullOrEmpty(connection.Healing?.Script))
            {
                return;
            }

            LoggerFactory.Instance.Debug($"Healing of {connection.Alias} started...");

            using (PowerShell instance = PowerShell.Create())
            {
                instance.AddScript(connection.Healing.Script);
                instance.Invoke();

                LoggerFactory.Instance.Debug($"Healing of {connection.Alias} finished... with errors: {instance.HadErrors}");
            }
        }
    }
}
