using MPE.Pinger.Models;
using MPE.Pinger.Models.Configurations;

namespace MPE.Pinger.Interfaces
{
    internal interface ITester
    {
        bool CanTest(Connection connection);
        void Test(Connection connection);
    }
}