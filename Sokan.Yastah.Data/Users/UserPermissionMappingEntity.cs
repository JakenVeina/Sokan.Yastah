﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Users
{
    [Table("UserPermissionMappings", Schema = "Users")]
    internal class UserPermissionMappingEntity
    {
        public UserPermissionMappingEntity(
            long id,
            ulong userId,
            int permissionId,
            bool isDenied,
            long creationId,
            long? deletionId)
        {
            Id = id;
            UserId = userId;
            PermissionId = permissionId;
            IsDenied = isDenied;
            CreationId = creationId;
            DeletionId = deletionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(User))]
        public ulong UserId { get; }

        public UserEntity User { get; internal set; }
            = null!;

        [ForeignKey(nameof(Permission))]
        public int PermissionId { get; }

        public PermissionEntity Permission { get; internal set; }
            = null!;

        public bool IsDenied { get; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AuditableActionEntity Creation { get; internal set; }
            = null!;

        [ForeignKey(nameof(Deletion))]
        public long? DeletionId { get; set; }

        public AuditableActionEntity? Deletion { get; set; }
    }

    internal class UserPermissionMappingEntityTypeConfiguration
        : IEntityTypeConfiguration<UserPermissionMappingEntity>
    {
        public void Configure(
            EntityTypeBuilder<UserPermissionMappingEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.UserId)
                .HasConversion<long>();

            entityBuilder
                .Property(x => x.IsDenied);
        }
    }
}
