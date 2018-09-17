using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Enums;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models.Configurations;

namespace MPE.Pinger.Logic.Testers
{
    internal class SslTester : ITester
    {
        private const int DefaultPort = 443;

        public bool CanTest(Connection connection)
        {
            return connection.Type.Equals(ConnectionType.Ssl.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        public void Test(Connection connection)
        {
            try
            {
                TcpClient client = new TcpClient(connection.Target, DefaultPort);

                X509Certificate2 certificate = null;
                SslStream sslStream = new SslStream(client.GetStream(), false,
                    delegate (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslError)
                    {
                        if (sslError != SslPolicyErrors.None)
                        {
                            throw new Exception(sslError.ToString());    
                        }

                        certificate = new X509Certificate2(cert);
                        return true;
                    });
                sslStream.AuthenticateAsClient(connection.Target);
                client.Close();

                var threshold = DateTime.Now.AddDays(connection.DaysLeft);
                if (threshold > certificate.NotAfter)
                {
                    throw new Exception();
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
