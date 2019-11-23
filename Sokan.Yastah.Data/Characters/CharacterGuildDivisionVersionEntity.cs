using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterGuildDivisionVersions", Schema = "Characters")]
    internal class CharacterGuildDivisionVersionEntity
    {
        public CharacterGuildDivisionVersionEntity(
            long id,
            long divisionId,
            string name,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            Id = id;
            DivisionId = divisionId;
            Name = name;
            IsDeleted = isDeleted;
            CreationId = creationId;
            PreviousVersionId = previousVersionId;
            NextVersionId = nextVersionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Division))]
        public long DivisionId { get; }

        public CharacterGuildDivisionEntity Division { get; set; }
            = null!;

        [Required]
        public string Name { get; }

        public bool IsDeleted { get; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AdministrationActionEntity Creation { get; internal set; }
            = null!;

        public long? PreviousVersionId { get; set; }

        public CharacterGuildDivisionVersionEntity? PreviousVersion { get; set; }

        public long? NextVersionId { get; set; }

        public CharacterGuildDivisionVersionEntity? NextVersion { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterGuildDivisionVersionEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.Name);

                entityBuilder
                    .Property(x => x.IsDeleted);

                entityBuilder
                    .HasOne(x => x.PreviousVersion)
                    .WithOne()
                    .HasForeignKey<CharacterGuildDivisionVersionEntity>(x => x.PreviousVersionId)
                    .HasConstraintName("FK_CharacterGuildDivisionVersions_PreviousVersion"); // Auto-generated name hits max length, and collides

                entityBuilder
                    .HasOne(x => x.NextVersion)
                    .WithOne()
                    .HasForeignKey<CharacterGuildDivisionVersionEntity>(x => x.NextVersionId)
                    .HasConstraintName("FK_CharacterGuildDivisionVersions_NextVersion"); // Auto-generated name hits max length, and collides
            });
    }
}
