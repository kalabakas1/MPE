using System;
using System.Linq;
using System.Net.Http;
using MPE.Pinger.Enums;
using MPE.Pinger.Interfaces;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;

namespace MPE.Pinger.Logic.Testers
{
    internal class WebTester : IConnectionTester
    {
        private static HttpClient _client;

        private HttpClient GetClient()
        {
            if (_client == null)
            {
                _client = new HttpClient();
            }

            return _client;
        }

        public bool CanTest(Connection connection)
        {
            return connection.Type.Equals(ConnectionType.Web.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        public void Test(Connection connection)
        {
            try
            {
                HttpResponseMessage response = GetClient().GetAsync(connection.Target).Result;
                if (!connection.Response.Contains((int) response.StatusCode))
                {
                    throw new Exception(
                        $"{connection.Alias} - Not returning one of the responsecodes [{string.Join(",", connection.Response)}]");
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}