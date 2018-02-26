using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Api.Helpers
{
    internal static class ReflectionHelper
    {
        public static HashSet<Type> GetTypesDecoratedByAttribute(Type attributeType)
        {
            var types = new HashSet<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = type.GetCustomAttribute(attributeType);
                    if (attribute != null)
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }
    }
}
