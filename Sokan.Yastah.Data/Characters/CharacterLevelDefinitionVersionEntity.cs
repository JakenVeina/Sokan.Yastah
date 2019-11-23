using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Characters
{
    internal class CharacterLevelDefinitionVersionEntity
    {
        public CharacterLevelDefinitionVersionEntity(
            long id,
            int level,
            decimal experienceThreshold,
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

        public decimal ExperienceThreshold { get; }

        public bool IsDeleted { get; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AdministrationActionEntity Creation { get; internal set; }
            = null!;

        [ForeignKey(nameof(PreviousVersion))]
        public long? PreviousVersionId { get; set; }

        public CharacterLevelDefinitionVersionEntity? PreviousVersion { get; set; }

        [ForeignKey(nameof(NextVersion))]
        public long? NextVersionId { get; set; }

        public CharacterLevelDefinitionVersionEntity? NextVersion { get; set; }
    }
}
