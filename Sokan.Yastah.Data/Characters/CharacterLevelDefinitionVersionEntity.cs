using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterLevelDefinitionVersions", Schema = "Characters")]
    internal class CharacterLevelDefinitionVersionEntity
    {
        public CharacterLevelDefinitionVersionEntity(
            long id,
            int level,
            int experienceThreshold,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            Id = id;
            Level = level;
            ExperienceThreshold = experienceThreshold;
            IsDeleted = isDeleted;
            CreationId = creationId;
            PreviousVersionId = previousVersionId;
            NextVersionId = nextVersionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Definition))]
        public int Level { get; }

        public CharacterLevelDefinitionEntity Definition { get; internal set; }
            = null!;

        public int ExperienceThreshold { get; }

        public bool IsDeleted { get; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AdministrationActionEntity Creation { get; internal set; }
            = null!;

        public long? PreviousVersionId { get; set; }

        public CharacterLevelDefinitionVersionEntity? PreviousVersion { get; set; }

        public long? NextVersionId { get; set; }

        public CharacterLevelDefinitionVersionEntity? NextVersion { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterLevelDefinitionVersionEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.ExperienceThreshold);

                entityBuilder
                    .Property(x => x.IsDeleted);

                entityBuilder
                    .HasOne(x => x.PreviousVersion)
                    .WithOne()
                    .HasForeignKey<CharacterLevelDefinitionVersionEntity>(x => x.PreviousVersionId)
                    .HasConstraintName("FK_CharacterLevelDefinitionVersions_PreviousVersion"); // Auto-generated name hits max length, and collides

                entityBuilder
                    .HasOne(x => x.NextVersion)
                    .WithOne()
                    .HasForeignKey<CharacterLevelDefinitionVersionEntity>(x => x.NextVersionId)
                    .HasConstraintName("FK_CharacterLevelDefinitionVersions_NextVersion"); // Auto-generated name hits max length, and collides
            });
    }
}
