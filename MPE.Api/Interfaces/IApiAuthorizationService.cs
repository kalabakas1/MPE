using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Api.Interfaces
{
    internal interface IApiAuthorizationService
    {
        bool ShouldFieldBeSerialized(string key, Type type, string fieldName);
    }
}
