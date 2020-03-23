using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;

namespace Sokan.Yastah.Data.Roles
{
    public enum RoleManagementAdministrationActionType
    {
        RoleCreated     = AuditingActionCategory.RoleManagement + 0x010000,
        RoleModified    = AuditingActionCategory.RoleManagement + 0x020000,
        RoleDeleted     = AuditingActionCategory.RoleManagement + 0x030000,
        RoleRestored    = AuditingActionCategory.RoleManagement + 0x040000
    }

    internal class RoleManagementAdministrationActionTypeDataConfiguration
        : IEntityTypeConfiguration<AuditableActionTypeEntity>
    {
        public void Configure(
            EntityTypeBuilder<AuditableActionTypeEntity> entityBuilder)
        {
            var types = Enum.GetValues(typeof(RoleManagementAdministrationActionType))
                .Cast<RoleManagementAdministrationActionType>();

            foreach (var type in types)
                entityBuilder.HasData(new AuditableActionTypeEntity(
                    id:         (int)type,
                    categoryId: (int)AuditingActionCategory.RoleManagement,
                    name:       type.ToString()));
        }
    }
}
