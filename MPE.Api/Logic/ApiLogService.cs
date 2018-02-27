using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Api.Interfaces;
using MPE.Api.Models;
using NPoco;

namespace MPE.Api.Logic
{
    internal class ApiLogService : IApiLogService
    {
        public ApiLog Create()
        {
            return new ApiLog
            {
                CreatedOn = DateTime.Now,
                Deleted = false
            };
        }

        public void Save(ApiLog log)
        {
            Task.Run(() =>
            {
                using (var client = new Database(ApiConstants.ConnectionStringName))
                {
                    client.Save(log);
                }
            });
        }
    }
}
