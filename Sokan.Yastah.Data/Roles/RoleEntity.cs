using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Roles
{
    internal class RoleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public ulong CreatedById { get; set; }

        public UserEntity CreatedBy { get; set; }

        [ForeignKey(nameof(DeletedBy))]
        public ulong? DeletedById { get; set; }

        public UserEntity DeletedBy { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<RoleEntity>(entityBuilder =>
            {
                entityBuilder
                    .ToTable("Roles");

                entityBuilder
                    .Property(x => x.CreatedById)
                    .HasConversion<long>();

                entityBuilder
                    .Property(x => x.DeletedById)
                    .HasConversion<long?>();
            });
    }
}
