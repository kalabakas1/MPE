﻿using System;
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
        internal bool CanHeal(Connection connection)
        {
            return !string.IsNullOrEmpty(connection.Healing?.Script);
        }

        internal void Heal(Connection connection)
        {
            if (!CanHeal(connection))
            {
                return;
            }

            LoggerFactory.Instance.Debug($"Healing of {connection.Alias} started...");

            using (PowerShell instance = PowerShell.Create())
            {
                instance.AddScript(connection.Healing.Script);
                instance.Invoke();

                LoggerFactory.Instance.Debug($"Healing of {connection.Alias} finished... with errors: {instance.HadErrors}");

                if (instance.HadErrors)
                {
                    foreach (ErrorRecord record in instance.Streams.Error)
                    {
                        Console.WriteLine(record.ToString());
                    }
                }
            }
        }
    }
}
