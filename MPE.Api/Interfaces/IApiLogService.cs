using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Api.Models;

namespace MPE.Api.Interfaces
{
    internal interface IApiLogService
    {
        ApiLog Create();
        void Save(ApiLog log);
    }
}
