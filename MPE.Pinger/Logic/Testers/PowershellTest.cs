using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using MPE.Logging;
using MPE.Pinger.Enums;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models.Configurations;

namespace MPE.Pinger.Logic.Testers
{
    internal class PowershellTest : ITester
    {
        public bool CanTest(Connection connection)
        {
            return connection.Type.Equals(ConnectionType.Powershell.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        public void Test(Connection connection)
        {
            if (string.IsNullOrEmpty(connection.Script))
            {
                LoggerFactory.Instance.Debug($"No script for: {connection.Alias}");
                return;
            }

            try
            {
                using (PowerShell instance = PowerShell.Create())
                {
                    instance.AddScript("Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted -Force");

                    instance.Invoke();

                    instance.AddScript(connection.Script);
                    var result = instance.Invoke();

                    if (instance.HadErrors)
                    {
                        throw new Exception("Error in script");
                    }

                    if (result == null || !result.Any())
                    {
                        throw new Exception("No result from script");
                    }

                    var resultItem = result.FirstOrDefault();
                    var passedTest = false;
                    if (bool.TryParse(resultItem?.ToString(), out passedTest))
                    {
                        if (!passedTest)
                        {
                            throw new Exception("Did not pass test");
                        }
                    }
                    else
                    {
                        throw new Exception("Could not convert to bool");
                    }
                }
            }
            catch (Exception e)
            {
                var msg = $"{connection.Alias} - {connection.Target} - {e.Message}";
                throw new Exception(msg);
            }
        }
    }
}
