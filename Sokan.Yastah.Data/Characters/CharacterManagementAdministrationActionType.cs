using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;

namespace Sokan.Yastah.Data.Characters
{
    public enum CharacterManagementAdministrationActionType
    {
        GuildCreated                = AuditingActionCategory.CharacterManagement + 0x010100,
        GuildModified               = AuditingActionCategory.CharacterManagement + 0x010200,
        GuildDeleted                = AuditingActionCategory.CharacterManagement + 0x010300,
        GuildRestored               = AuditingActionCategory.CharacterManagement + 0x010400,
        DivisionCreated             = AuditingActionCategory.CharacterManagement + 0x020100,
        DivisionModified            = AuditingActionCategory.CharacterManagement + 0x020200,
        DivisionDeleted             = AuditingActionCategory.CharacterManagement + 0x020300,
        DivisionRestored            = AuditingActionCategory.CharacterManagement + 0x020400,
        LevelDefinitionsInitialized = AuditingActionCategory.CharacterManagement + 0x030100,
        LevelDefinitionsUpdated     = AuditingActionCategory.CharacterManagement + 0x030200,
        CharacterCreated            = AuditingActionCategory.CharacterManagement + 0x040100,
        CharacterModified           = AuditingActionCategory.CharacterManagement + 0x040200,
        CharacterDeleted            = AuditingActionCategory.CharacterManagement + 0x040300,
        CharacterRestored           = AuditingActionCategory.CharacterManagement + 0x040400,
    }

    internal class CharacterManagementAdministrationActionTypeDataConfiguration
        : IEntityTypeConfiguration<AuditableActionTypeEntity>
    {
        public void Configure(
            EntityTypeBuilder<AuditableActionTypeEntity> entityBuilder)
        {
            var types = Enum.GetValues(typeof(CharacterManagementAdministrationActionType))
                .Cast<CharacterManagementAdministrationActionType>();

            foreach (var type in types)
                entityBuilder.HasData(new AuditableActionTypeEntity(
                    id:         (int)type,
                    categoryId: (int)AuditingActionCategory.CharacterManagement,
                    name:       type.ToString()));
        }
    }
}
