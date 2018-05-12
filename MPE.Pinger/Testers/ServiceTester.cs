using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Enums;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;

namespace MPE.Pinger.Testers
{
    internal class ServiceTester : IConnectionTester
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
