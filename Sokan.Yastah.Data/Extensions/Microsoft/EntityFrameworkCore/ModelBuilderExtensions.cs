using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ApplyAssembly(this ModelBuilder modelBuilder, Assembly assembly)
        {
            var handlers = OnModelCreatingAttribute
                .EnumerateAttachedMethods(assembly);

            foreach (var handler in handlers)
                handler.Invoke(modelBuilder);

            return modelBuilder;
        }
    }
}
