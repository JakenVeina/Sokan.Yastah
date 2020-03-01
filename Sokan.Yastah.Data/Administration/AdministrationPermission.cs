using System;
using System.ComponentModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Administration
{
    public enum AdministrationPermission
    {
        [Description("Allows management of application permissions")]
        ManagePermissions = 1,

        [Description("Allows management of application roles")]
        ManageRoles = 2,

        [Description("Allows management of application users")]
        ManageUsers = 3
    }

    internal class AdministrationPermissionDataConfiguration
        : IEntityTypeConfiguration<PermissionEntity>
    {
        public void Configure(
            EntityTypeBuilder<PermissionEntity> entityBuilder)
        {
            foreach (var (value, description) in EnumEx.EnumerateValuesWithDescriptions<AdministrationPermission>())
                entityBuilder.HasData(new PermissionEntity(
                    categoryId:     (int)PermissionCategory.Administration,
                    permissionId:   (int)value,
                    name:           value.ToString(),
                    description:    description));
        }
    }
}
