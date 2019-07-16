using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Authorization;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Roles
{
    internal class RolePermissionMappingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; set; }

        public RoleEntity Role { get; set; }

        [ForeignKey(nameof(Permission))]
        public long PermissionId { get; set; }

        public PermissionEntity Permission { get; set; }

        public DateTimeOffset Created { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public ulong CreatedById { get; set; }

        public UserEntity CreatedBy { get; set; }

        [ForeignKey(nameof(DeletedBy))]
        public ulong? DeletedById { get; set; }

        public UserEntity DeletedBy { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<RolePermissionMappingEntity>(entityBuilder =>
            {
                entityBuilder
                    .ToTable("RolePermissionMappings");

                entityBuilder
                    .Property(x => x.CreatedById)
                    .HasConversion<long>();

                entityBuilder
                    .Property(x => x.DeletedById)
                    .HasConversion<long?>();
            });
    }
}
