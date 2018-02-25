using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using MPE.Api.Logic;

namespace MPE.Api.Attributes
{
    public class RestrictSerializationControllerAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new ApiLimitedFieldContractResolver();
        }
    }
}
