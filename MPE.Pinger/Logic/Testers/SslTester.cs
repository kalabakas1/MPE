using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                X509Certificate2 certificate = null;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + connection.Target);
                request.ServerCertificateValidationCallback = (sender, x509Certificate, chain, errors) =>
                {
                    if (errors != SslPolicyErrors.None)
                    {
                        throw new Exception(errors.ToString());
                    }

                    certificate = new X509Certificate2(x509Certificate);
                    return true;
                };

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();

                var threshold = DateTime.Now.AddDays(connection.DaysLeft);
                if (threshold > certificate.NotAfter)
                {
                    throw new Exception("Expired");
                }
            }
            catch (Exception e)
            {
                var msg = $"{connection.Alias} - {connection.Target}:{connection.Port} - {(e.InnerException != null ? e.InnerException.Message : e.Message)}";
                throw new Exception(msg);
            }
        }
    }
}
