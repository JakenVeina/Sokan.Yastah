using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterGuildVersions", Schema = "Characters")]
    internal class CharacterGuildVersionEntity
    {
        public CharacterGuildVersionEntity(
            long id,
            long guildId,
            string name,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            Id = id;
            GuildId = guildId;
            Name = name;
            IsDeleted = isDeleted;
            CreationId = creationId;
            PreviousVersionId = previousVersionId;
            NextVersionId = nextVersionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Guild))]
        public long GuildId { get; }

        public CharacterGuildEntity Guild { get; internal set; }
            = null!;

        [Required]
        public string Name { get; }

        public bool IsDeleted { get; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AuditableActionEntity Creation { get; internal set; }
            = null!;

        public long? PreviousVersionId { get; set; }

        public CharacterGuildVersionEntity? PreviousVersion { get; set; }

        public long? NextVersionId { get; set; }

        public CharacterGuildVersionEntity? NextVersion { get; set; }
    }

    internal class CharacterGuildVersionEntityTypeConfiguration
        : IEntityTypeConfiguration<CharacterGuildVersionEntity>
    {
        public void Configure(
            EntityTypeBuilder<CharacterGuildVersionEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Name);

            entityBuilder
                .Property(x => x.IsDeleted);

            entityBuilder
                .HasOne(x => x.PreviousVersion)
                .WithOne()
                .HasForeignKey<CharacterGuildVersionEntity>(x => x.PreviousVersionId);

            entityBuilder
                .HasOne(x => x.NextVersion)
                .WithOne()
                .HasForeignKey<CharacterGuildVersionEntity>(x => x.NextVersionId);
        }
    }
}
