using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Users
{
    [Table("UserPermissionMappings", Schema = "Users")]
    internal class UserPermissionMappingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public ulong UserId { get; set; }

        public UserEntity User { get; set; }

        [ForeignKey(nameof(Permission))]
        public int PermissionId { get; set; }

        public PermissionEntity Permission { get; set; }

        public bool IsDenied { get; set; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; set; }

        public AdministrationActionEntity Creation { get; set; }

        [ForeignKey(nameof(Deletion))]
        public long? DeletionId { get; set; }

        public AdministrationActionEntity Deletion { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<UserPermissionMappingEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.UserId)
                    .HasConversion<long>();
            });
    }
}
