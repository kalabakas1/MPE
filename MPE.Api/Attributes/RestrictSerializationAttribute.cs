using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RestrictSerializationAttribute : Attribute
    {
    }
}
