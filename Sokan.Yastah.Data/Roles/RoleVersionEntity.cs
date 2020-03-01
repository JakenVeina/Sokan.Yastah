using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Roles
{
    [Table("RoleVersions", Schema = "Roles")]
    internal class RoleVersionEntity
    {
        public RoleVersionEntity(
            long id,
            long roleId,
            string name,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            Id = id;
            RoleId = roleId;
            Name = name;
            IsDeleted = isDeleted;
            CreationId = creationId;
            PreviousVersionId = previousVersionId;
            NextVersionId = nextVersionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; }

        public RoleEntity Role { get; internal set; }
            = null!;

        [Required]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AdministrationActionEntity Creation { get; internal set; }
            = null!;

        public long? PreviousVersionId { get; set; }

        public RoleVersionEntity? PreviousVersion { get; set; }

        public long? NextVersionId { get; set; }

        public RoleVersionEntity? NextVersion { get; set; }
    }

    internal class RoleVersionEntityTypeConfiguration
        : IEntityTypeConfiguration<RoleVersionEntity>
    {
        public void Configure(
            EntityTypeBuilder<RoleVersionEntity> entityBuilder)
        {
            entityBuilder
                .HasOne(x => x.PreviousVersion)
                .WithOne()
                .HasForeignKey<RoleVersionEntity>(x => x.PreviousVersionId);

            entityBuilder
                .HasOne(x => x.NextVersion)
                .WithOne()
                .HasForeignKey<RoleVersionEntity>(x => x.NextVersionId);
        }
    }
}
