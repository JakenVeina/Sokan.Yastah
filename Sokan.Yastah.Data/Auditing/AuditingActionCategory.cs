using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Auditing
{
    public enum AuditingActionCategory
    {
        RoleManagement      = 0x01000000,
        UserManagement      = 0x02000000,
        CharacterManagement = 0x03000000
    }

    internal class AdministrationActionCategoryDataConfiguration
        : IEntityTypeConfiguration<AuditableActionCategoryEntity>
    {
        public void Configure(
            EntityTypeBuilder<AuditableActionCategoryEntity> entityBuilder)
        {
            foreach (var category in EnumEx.EnumerateValues<AuditingActionCategory>())
                entityBuilder.HasData(new AuditableActionCategoryEntity(
                    id:     (int)category,
                    name:   category.ToString()));
        }
    }
}
