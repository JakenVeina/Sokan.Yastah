using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Administration
{
    public enum AdministrationActionCategory
    {
        RoleManagement = 1,
        UserManagement = 2,
        CharacterManagement = 3
    }

    internal class AdministrationActionCategoryDataConfiguration
        : IEntityTypeConfiguration<AdministrationActionCategoryEntity>
    {
        public void Configure(
            EntityTypeBuilder<AdministrationActionCategoryEntity> entityBuilder)
        {
            foreach (var category in EnumEx.EnumerateValues<AdministrationActionCategory>())
                entityBuilder.HasData(new AdministrationActionCategoryEntity(
                    id:     (int)category,
                    name:   category.ToString()));
        }
    }
}
