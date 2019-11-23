using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Characters
{
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

        [ForeignKey(nameof(PreviousVersion))]
        public long? PreviousVersionId { get; set; }

        public CharacterGuildDivisionVersionEntity? PreviousVersion { get; set; }

        [ForeignKey(nameof(NextVersion))]
        public long? NextVersionId { get; set; }

        public CharacterGuildDivisionVersionEntity? NextVersion { get; set; }
    }
}
