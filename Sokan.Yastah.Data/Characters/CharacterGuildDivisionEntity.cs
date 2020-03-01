using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterGuildDivisions", Schema = "Characters")]
    internal class CharacterGuildDivisionEntity
    {
        public CharacterGuildDivisionEntity(
            long id,
            long guildId)
        {
            Id = id;
            GuildId = guildId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Guild))]
        public long GuildId { get; }

        public CharacterGuildEntity Guild { get; internal set; }
            = null!;

        public ICollection<CharacterGuildDivisionVersionEntity> Versions { get; internal set; }
            = null!;
    }

    internal class CharacterGuildDivisionEntityTypeConfiguration
        : IEntityTypeConfiguration<CharacterGuildDivisionEntity>
    {
        public void Configure(
            EntityTypeBuilder<CharacterGuildDivisionEntity> entityBuilder) { }
    }
}
