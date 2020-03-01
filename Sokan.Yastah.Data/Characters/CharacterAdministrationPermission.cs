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
        ManageGuilds = 100,

        [Description("Allows management of character level definitions")]
        ManageLevels = 101,
    }

    internal class CharacterAdministrationPermissionDataConfiguration
        : IEntityTypeConfiguration<PermissionEntity>
    {
        public void Configure(
            EntityTypeBuilder<PermissionEntity> entityBuilder)
        {
            foreach (var (value, description) in EnumEx.EnumerateValuesWithDescriptions<CharacterAdministrationPermission>())
                entityBuilder.HasData(new PermissionEntity(
                    categoryId:     (int)PermissionCategory.CharacterAdministration,
                    permissionId:   (int)value,
                    name:           value.ToString(),
                    description:    description));
        }
    }
}
