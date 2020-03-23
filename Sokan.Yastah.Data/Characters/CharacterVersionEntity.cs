using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterVersions", Schema = "Characters")]
    internal class CharacterVersionEntity
    {
        public CharacterVersionEntity(
            long id,
            long characterId,
            string name,
            long divisionId,
            int experiencePoints,
            int goldAmount,
            int insanityValue,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            Id = id;
            CharacterId = characterId;
            Name = name;
            DivisionId = divisionId;
            ExperiencePoints = experiencePoints;
            GoldAmount = goldAmount;
            InsanityValue = insanityValue;
            IsDeleted = isDeleted;
            CreationId = creationId;
            PreviousVersionId = previousVersionId;
            NextVersionId = nextVersionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Character))]
        public long CharacterId { get; }

        public CharacterEntity Character { get; internal set; }
            = null!;

        [Required]
        public string Name { get; }

        [ForeignKey(nameof(Division))]
        public long DivisionId { get; }

        public CharacterGuildDivisionEntity Division { get; internal set; }
            = null!;

        public int ExperiencePoints { get; }

        public int GoldAmount { get; }

        public int InsanityValue { get; }

        public bool IsDeleted { get; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AuditableActionEntity Creation { get; internal set; }
            = null!;

        public long? PreviousVersionId { get; set; }

        public CharacterVersionEntity? PreviousVersion { get; set; }

        public long? NextVersionId { get; set; }

        public CharacterVersionEntity? NextVersion { get; set; }
    }

    internal class CharacterVersionEntityTypeConfiguration
        : IEntityTypeConfiguration<CharacterVersionEntity>
    {
        public void Configure(
            EntityTypeBuilder<CharacterVersionEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Name);

            entityBuilder
                .Property(x => x.DivisionId);

            entityBuilder
                .Property(x => x.ExperiencePoints);

            entityBuilder
                .Property(x => x.GoldAmount);

            entityBuilder
                .Property(x => x.InsanityValue);

            entityBuilder
                .Property(x => x.IsDeleted);

            entityBuilder
                .HasOne(x => x.PreviousVersion)
                .WithOne()
                .HasForeignKey<CharacterVersionEntity>(x => x.PreviousVersionId);

            entityBuilder
                .HasOne(x => x.NextVersion)
                .WithOne()
                .HasForeignKey<CharacterVersionEntity>(x => x.NextVersionId);
        }
    }
}
