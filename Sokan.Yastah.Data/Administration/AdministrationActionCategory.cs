using System;
using System.Linq;

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
                var types = Enum.GetValues(typeof(AdministrationActionCategory))
                    .Cast<AdministrationActionCategory>();

                foreach (var type in types)
                    entityBuilder.HasData(new AdministrationActionCategoryEntity()
                    {
                        Id = (int)type,
                        Name = type.ToString()
                    });
            });
    }
}
