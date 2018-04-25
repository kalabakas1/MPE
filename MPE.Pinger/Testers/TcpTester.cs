using System;
using System.Net;
using System.Net.Sockets;
using MPE.Pinger.Enums;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;

namespace MPE.Pinger.Testers
{
    internal class TcpTester : IConnectionTester
    {
        public bool CanTest(Connection connection)
        {
            return connection.Type == ConnectionType.Tcp.ToString();
        }

        public void Test(Connection connection)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Parse(connection.Target), connection.Port);
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