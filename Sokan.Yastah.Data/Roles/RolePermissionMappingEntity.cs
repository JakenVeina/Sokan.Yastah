using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Roles
{
    [Table("RolePermissionMappings", Schema = "Roles")]
    internal class RolePermissionMappingEntity
    {
        public RolePermissionMappingEntity(
            long id,
            long roleId,
            int permissionId,
            long creationId,
            long? deletionId)
        {
            Id = id;
            RoleId = roleId;
            PermissionId = permissionId;
            CreationId = creationId;
            DeletionId = deletionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; }

        public RoleEntity Role { get; internal set; }
            = null!;

        [ForeignKey(nameof(Permission))]
        public int PermissionId { get; }

        public PermissionEntity Permission { get; internal set; }
            = null!;

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AuditableActionEntity Creation { get; internal set; }
            = null!;

        [ForeignKey(nameof(Deletion))]
        public long? DeletionId { get; set; }

        public AuditableActionEntity? Deletion { get; set; }
    }

    internal class RolePermissionMappingEntityTypeConfiguration
        : IEntityTypeConfiguration<RolePermissionMappingEntity>
    {
        public void Configure(
            EntityTypeBuilder<RolePermissionMappingEntity> entityBuilder) { }
    }
}
