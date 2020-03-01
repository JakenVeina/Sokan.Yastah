using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

    internal class RoleManagementAdministrationActionTypeDataConfiguration
        : IEntityTypeConfiguration<AdministrationActionTypeEntity>
    {
        public void Configure(
            EntityTypeBuilder<AdministrationActionTypeEntity> entityBuilder)
        {
            var types = Enum.GetValues(typeof(RoleManagementAdministrationActionType))
                .Cast<RoleManagementAdministrationActionType>();

            foreach (var type in types)
                entityBuilder.HasData(new AdministrationActionTypeEntity(
                    id:         (int)type,
                    categoryId: (int)AdministrationActionCategory.RoleManagement,
                    name:       type.ToString()));
        }
    }
}
