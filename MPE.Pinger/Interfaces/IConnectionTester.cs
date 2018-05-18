using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;

namespace MPE.Pinger.Interfaces
{
    internal interface IConnectionTester
    {
        bool CanTest(Connection connection);
        void Test(Connection connection);
    }
}