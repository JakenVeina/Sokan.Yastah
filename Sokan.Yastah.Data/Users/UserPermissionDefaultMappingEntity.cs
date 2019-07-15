using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Authorization;

namespace Sokan.Yastah.Data.Users
{
    internal class UserPermissionDefaultMappingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

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
    }

    [ModelCreatingHandler]
    public class UserPermissionDefaultMappingEntityModelCreatingHandler
        : IModelCreatingHandler<YastahDbContext>
    {
        public void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<UserPermissionDefaultMappingEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.CreatedById)
                    .HasConversion<long>();

                entityBuilder
                    .Property(x => x.DeletedById)
                    .HasConversion<long?>();
            });
    }
}
