using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Users
{
    internal class UserRoleDefaultMappingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; set; }

        public RoleEntity Role { get; set; }

        public DateTimeOffset Created { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public ulong CreatedById { get; set; }

        public UserEntity CreatedBy { get; set; }

        [ForeignKey(nameof(DeletedBy))]
        public ulong? DeletedById { get; set; }

        public UserEntity DeletedBy { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<UserRoleDefaultMappingEntity>(entityBuilder =>
            {
                entityBuilder
                    .ToTable("UserRoleDefaultMappings");

                entityBuilder
                    .Property(x => x.CreatedById)
                    .HasConversion<long>();

                entityBuilder
                    .Property(x => x.DeletedById)
                    .HasConversion<long?>();
            });
    }
}
