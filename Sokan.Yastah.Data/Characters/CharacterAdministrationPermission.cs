using System;
using System.ComponentModel;

using Microsoft.EntityFrameworkCore;

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

    public static class CharacterAdministrationPermissionModelBuilder
    {
        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<PermissionEntity>(entityBuilder =>
            {
                foreach (var (value, description) in EnumEx.EnumerateValuesWithDescriptions<CharacterAdministrationPermission>())
                    entityBuilder.HasData(new PermissionEntity(
                        categoryId:     (int)PermissionCategory.CharacterAdministration,
                        permissionId:   (int)value,
                        name:           value.ToString(),
                        description:    description));
            });
    }
}
