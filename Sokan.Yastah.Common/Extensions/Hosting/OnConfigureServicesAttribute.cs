using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Hosting
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnConfigureServicesAttribute
        : Attribute
    {
        public static IEnumerable<ConfigureServicesHandler> EnumeratedAttachedMethods(Assembly assembly)
            => assembly
                .DefinedTypes
                .SelectMany(x => x.DeclaredMethods)
                .Where(x => x.CustomAttributes
                    .Any(y => y.AttributeType == typeof(OnConfigureServicesAttribute)))
                .Select(x => x.CreateDelegate(typeof(ConfigureServicesHandler)))
                .Cast<ConfigureServicesHandler>();
    }
}
