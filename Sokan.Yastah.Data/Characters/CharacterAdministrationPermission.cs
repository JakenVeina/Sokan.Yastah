using System;
using System.ComponentModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Characters
{
    public enum CharacterAdministrationPermission
    {
        [Description("Allows management of character guilds")]
        ManageGuilds    = PermissionCategory.CharacterManagement + 0x010000,

        [Description("Allows management of character level definitions")]
        ManageLevels    = PermissionCategory.CharacterManagement + 0x020000,
    }

    internal class CharacterAdministrationPermissionDataConfiguration
        : IEntityTypeConfiguration<PermissionEntity>
    {
        public void Configure(
            EntityTypeBuilder<PermissionEntity> entityBuilder)
        {
            foreach (var (value, description) in EnumEx.EnumerateValuesWithDescriptions<CharacterAdministrationPermission>())
                entityBuilder.HasData(new PermissionEntity(
                    categoryId:     (int)PermissionCategory.CharacterManagement,
                    permissionId:   (int)value,
                    name:           value.ToString(),
                    description:    description));
        }
    }
}
