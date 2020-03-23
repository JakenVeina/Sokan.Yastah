using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;

namespace Sokan.Yastah.Data.Users
{
    public enum UserManagementAdministrationActionType
    {
        UserCreated         = AuditingActionCategory.UserManagement + 0x010000,
        UserModified        = AuditingActionCategory.UserManagement + 0x020000,
        DefaultsModified    = AuditingActionCategory.UserManagement + 0x030000
    }

    internal class UserManagementAdministrationActionTypeDataConfiguration
        : IEntityTypeConfiguration<AuditableActionTypeEntity>
    {
        public void Configure(
            EntityTypeBuilder<AuditableActionTypeEntity> entityBuilder)
        {
            var types = Enum.GetValues(typeof(UserManagementAdministrationActionType))
                .Cast<UserManagementAdministrationActionType>();

            foreach (var type in types)
                entityBuilder.HasData(new AuditableActionTypeEntity(
                    id:         (int)type,
                    categoryId: (int)AuditingActionCategory.UserManagement,
                    name:       type.ToString()));
        }
    }
}
