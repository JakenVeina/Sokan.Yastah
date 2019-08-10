using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Roles
{
    public enum RoleManagementAdministrationActionType
    {
        RoleCreated = 1,
        RoleModified = 2,
        RoleDeleted = 3,
        RoleRestored = 4
    }

    public static class RoleManagementAdministrationActionTypeModelBuilder
    {
        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AdministrationActionTypeEntity>(entityBuilder =>
            {
                var types = Enum.GetValues(typeof(RoleManagementAdministrationActionType))
                    .Cast<RoleManagementAdministrationActionType>();

                foreach (var type in types)
                    entityBuilder.HasData(new AdministrationActionTypeEntity()
                    {
                        Id = (int)type,
                        CategoryId = (int)AdministrationActionCategory.RoleManagement,
                        Name = type.ToString()
                    });
            });
    }
}
