using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ApplyConfiguration(this ModelBuilder modelBuilder, Assembly assembly)
        {
            var handlers = OnModelCreatingAttribute
                .EnumeratedAttachedMethods(assembly);

            foreach (var handler in handlers)
                handler.Invoke(modelBuilder);

            return modelBuilder;
        }
    }
}
