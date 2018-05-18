using System;
using System.Net;
using System.Net.Sockets;
using MPE.Pinger.Enums;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;

namespace MPE.Pinger.Logic.Testers
{
    internal class TcpTester : IConnectionTester
    {
        public bool CanTest(Connection connection)
        {
            return connection.Type.Equals(ConnectionType.Tcp.ToString(),StringComparison.CurrentCultureIgnoreCase);
        }

        public void Test(Connection connection)
        {
            try
            {
                var addresses = Dns.GetHostAddresses(connection.Target);
                foreach (var ipAddress in addresses)
                {
                    using (var client = new TcpClient())
                    {
                        client.Connect(ipAddress, connection.Port);
                    }
                }
            }
            catch (Exception e)
            {
                var msg = $"{connection.Alias} - {connection.Target}:{connection.Port} - {e.Message}";
                throw new Exception(msg);
            }
        }
    }
}