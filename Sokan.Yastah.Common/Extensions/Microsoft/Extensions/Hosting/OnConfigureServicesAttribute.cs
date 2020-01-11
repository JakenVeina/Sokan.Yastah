using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnConfigureServicesAttribute
        : Attribute
    {
        public static IEnumerable<Delegate> EnumerateAttachedMethods(Assembly assembly)
            => assembly
                .DefinedTypes
                .SelectMany(x => x.DeclaredMethods)
                .Where(x => x.CustomAttributes
                    .Any(y => y.AttributeType == typeof(OnConfigureServicesAttribute)))
                .Select(x =>
                {
                    var parameterInfos = x.GetParameters();

                    switch(parameterInfos.Length)
                    {
                        case 1:
                            if (parameterInfos[0].ParameterType == typeof(IServiceCollection))
                                return x.CreateDelegate(typeof(ConfigureServicesHandler));
                            break;

                        case 2:
                            if (parameterInfos[0].ParameterType == typeof(IServiceCollection)
                                    && parameterInfos[1].ParameterType == typeof(IConfiguration))
                                return x.CreateDelegate(typeof(ConfigureServicesWithConfigurationHandler));
                            break;
                    }

                    throw new ArgumentException($"Method {x.DeclaringType!.FullName}.{x.Name} is tagged with {typeof(OnConfigureServicesAttribute).FullName} but does have a compatible signature. The signature must match either {typeof(ConfigureServicesHandler).FullName} or {typeof(ConfigureServicesWithConfigurationHandler).FullName}", nameof(assembly));
                });
    }
}
