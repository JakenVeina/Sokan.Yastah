using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Users
{
    public enum UserManagementAdministrationActionType
    {
        UserCreated = 20,
        UserModified = 21,
        DefaultsModified = 22
    }

    internal class UserManagementAdministrationActionTypeDataConfiguration
        : IEntityTypeConfiguration<AdministrationActionTypeEntity>
    {
        public void Configure(
            EntityTypeBuilder<AdministrationActionTypeEntity> entityBuilder)
        {
            var types = Enum.GetValues(typeof(UserManagementAdministrationActionType))
                .Cast<UserManagementAdministrationActionType>();

            foreach (var type in types)
                entityBuilder.HasData(new AdministrationActionTypeEntity(
                    id:         (int)type,
                    categoryId: (int)AdministrationActionCategory.UserManagement,
                    name:       type.ToString()));
        }
    }
}
