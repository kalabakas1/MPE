using MPE.Pinger.Models;

namespace MPE.Pinger.Interfaces
{
    internal interface IConnectionTester
    {
        bool CanTest(Connection connection);
        void Test(Connection connection);
    }
}