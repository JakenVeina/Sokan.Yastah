using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnModelCreatingAttribute
        : Attribute
    {
        public static IEnumerable<Action<ModelBuilder>> EnumerateAttachedMethods(Assembly assembly)
            => assembly
                .DefinedTypes
                .SelectMany(x => x.DeclaredMethods)
                .Where(x => x.CustomAttributes
                    .Any(y => y.AttributeType == typeof(OnModelCreatingAttribute)))
                .Select(x => x.CreateDelegate(typeof(Action<ModelBuilder>)))
                .Cast<Action<ModelBuilder>>();
    }
}
