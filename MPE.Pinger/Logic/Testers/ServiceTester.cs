using System;
using System.ServiceProcess;
using MPE.Pinger.Enums;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;

namespace MPE.Pinger.Logic.Testers
{
    internal class ServiceTester : ITester
    {
        public bool CanTest(Connection connection)
        {
            return connection.Type.Equals(ConnectionType.Service.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        public void Test(Connection connection)
        {
            try
            {
                using (ServiceController sc = new ServiceController(connection.Target))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        throw new Exception("Service not running");
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
