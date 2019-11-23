using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;

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
            decimal experiencePoints,
            decimal goldAmount,
            decimal sanityValue,
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
            SanityValue = sanityValue;
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

        public CharacterGuildDivisionEntity Division { get; }
            = null!;

        public decimal ExperiencePoints { get; }

        public decimal GoldAmount { get; }

        public decimal SanityValue { get; }

        public bool IsDeleted { get; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AdministrationActionEntity Creation { get; internal set; }
            = null!;

        [ForeignKey(nameof(PreviousVersion))]
        public long? PreviousVersionId { get; set; }

        public CharacterVersionEntity? PreviousVersion { get; set; }

        [ForeignKey(nameof(NextVersion))]
        public long? NextVersionId { get; set; }

        public CharacterVersionEntity? NextVersion { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterVersionEntity>(entityBuilder =>
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
                    .Property(x => x.SanityValue);

                entityBuilder
                    .Property(x => x.IsDeleted);
            });
    }
}
