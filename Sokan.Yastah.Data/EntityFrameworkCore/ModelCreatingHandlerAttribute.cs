using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ModelCreatingHandlerAttribute
        : Attribute
    {
        public static IEnumerable<Type> EnumerateAttachedTypes<TContext>(Assembly assembly)
            => assembly
                .DefinedTypes
                .Where(type => type.ImplementedInterfaces.Contains(typeof(IModelCreatingHandler<TContext>)));
    }
}
