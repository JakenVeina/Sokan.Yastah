﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Users
{
    [Table("DefaultPermissionMappings", Schema = "Users")]
    internal class DefaultPermissionMappingEntity
    {
        public DefaultPermissionMappingEntity(
            long id,
            int permissionId,
            long creationId,
            long? deletionId)
        {
            Id = id;
            PermissionId = permissionId;
            CreationId = creationId;
            DeletionId = deletionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

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

    internal class DefaultPermissionMappingEntityTypeConfiguration
        : IEntityTypeConfiguration<DefaultPermissionMappingEntity>
    {
        public void Configure(
            EntityTypeBuilder<DefaultPermissionMappingEntity> entityBuilder) { }
    }
}
