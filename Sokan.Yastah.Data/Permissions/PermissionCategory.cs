using System;
using System.ComponentModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Permissions
{
    public enum PermissionCategory
    {
        [Description("Permissions related to administration of the application")]
        Administration = 1,
        
        [Description("Permissions related to administration of game characters")]
        CharacterAdministration = 2,
    }

    internal class PermissionCategoryDataConfiguration
        : IEntityTypeConfiguration<PermissionCategoryEntity>
    {
        public void Configure(
            EntityTypeBuilder<PermissionCategoryEntity> entityBuilder)
        {
            foreach (var (value, description) in EnumEx.EnumerateValuesWithDescriptions<PermissionCategory>())
                entityBuilder.HasData(new PermissionCategoryEntity(
                    id:             (int)value,
                    name:           value.ToString(),
                    description:    description
                ));
        }
    }
}
