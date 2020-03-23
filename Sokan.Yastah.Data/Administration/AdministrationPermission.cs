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
        ManagePermissions   = PermissionCategory.Administration + 0x010000,

        [Description("Allows management of application roles")]
        ManageRoles         = PermissionCategory.Administration + 0x020000,

        [Description("Allows management of application users")]
        ManageUsers         = PermissionCategory.Administration + 0x030000
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
