using System;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Administration
{
    public enum AdministrationActionCategory
    {
        RoleManagement = 1,
        UserManagement = 2,
    }

    public static class AdministrationActionCategoryModelBuilder
    {
        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AdministrationActionCategoryEntity>(entityBuilder =>
            {
                foreach (var category in EnumEx.EnumerateValues<AdministrationActionCategory>())
                    entityBuilder.HasData(new AdministrationActionCategoryEntity()
                    {
                        Id = (int)category,
                        Name = category.ToString()
                    });
            });
    }
}
