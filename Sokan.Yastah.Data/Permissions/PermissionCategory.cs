using System;
using System.ComponentModel;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Permissions
{
    public enum PermissionCategory
    {
        [Description("Permissions related to administration of the application")]
        Administration = 1
    }

    public static class PermissionCategoryModelBuilder
    {
        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<PermissionCategoryEntity>(entityBuilder =>
            {
                foreach (var (value, description) in EnumEx.EnumerateValuesWithDescriptions<PermissionCategory>())
                    entityBuilder.HasData(new PermissionCategoryEntity(
                        id:             (int)value,
                        name:           value.ToString(),
                        description:    description
                    ));
            });
    }
}
