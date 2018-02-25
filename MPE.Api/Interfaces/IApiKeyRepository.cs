using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functional.Maybe;
using MPE.Api.Models;

namespace MPE.Api.Interfaces
{
    internal interface IApiKeyRepository
    {
        Maybe<ApiKey> Get(string key);
    }
}
